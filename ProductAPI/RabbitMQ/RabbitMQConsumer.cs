using System.Text;
using System.Text.Json;
using ProductAPI.DTO;
using ProductAPI.DTOs;
using ProductAPI.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ProductAPI.Messaging
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private IConnection _connection;
        private IModel _channel;

        public RabbitMQConsumer(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;

            var factory = new ConnectionFactory
            {
                HostName = configuration.GetSection("RabbitMQ")["HostName"],
                UserName = configuration.GetSection("RabbitMQ")["UserName"],
                Password = configuration.GetSection("RabbitMQ")["Password"]
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // _channel.QueueDeclare("demo", false, false, false, null);

            // Declare the fanout exchange
            _channel.ExchangeDeclare(exchange: "OrderExchange", type: ExchangeType.Fanout);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            // Declare a unique queue for StockService and bind it to the fanout exchange
            var queueName = _channel.QueueDeclare().QueueName; // Generate unique queue
            _channel.QueueBind(queue: queueName, exchange: "OrderExchange", routingKey: "");

            // Start consuming messages from the bound queue
            CreateConsumer(queueName, async (message) =>
            {
                var eventMessage = JsonSerializer.Deserialize<OrderDTO>(message);
                using (var scope = _serviceProvider.CreateScope())
                {
                    var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
                    // await ProcessUpdateStockEvent(eventMessage, productService);
                }
            });

            // CreateConsumer("demo", async (message) =>
            // {
            // var eventMessage = JsonSerializer.Deserialize<EventDTO>(message);
            // if (eventMessage == null)
            // {
            //     return;
            // }
            // // using (var scope = _serviceProvider.CreateScope())
            // // {
            // //     var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            // //     emailService.SendEmail(messages);
            // // }
            // });
            return Task.CompletedTask;
        }

        private void CreateConsumer(string queueName, Func<string, Task> processMessage)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (ch, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine($"Received event from queue {queueName}: {message}");
                await processMessage(message);

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
        }

        private async Task ProcessUpdateStockEvent(OrderDTO eventMessage, IProductService productService)
        {
            Console.WriteLine($"Processing stock update for Product ID: {eventMessage.ProductId}");
            // await productService.UpdateStockAsync(eventMessage);
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
