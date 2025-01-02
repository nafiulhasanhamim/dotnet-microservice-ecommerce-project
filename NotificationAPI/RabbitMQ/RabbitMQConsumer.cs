using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using NotificationAPI.DTOs;
using NotificationAPI.Interfaces;
using NotificationAPI.SignalRHub;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NotificationAPI.RabbitMQ
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private IConnection _connection;
        private IModel _channel;
        private readonly IHubContext<ChatHub> _chatHubContext;


        public RabbitMQConsumer(IServiceProvider serviceProvider, IConfiguration configuration, IHubContext<ChatHub> chatHubContext)
        {
            _serviceProvider = serviceProvider;
            _chatHubContext = chatHubContext;
            var factory = new ConnectionFactory
            {
                HostName = configuration.GetSection("RabbitMQ")["HostName"],
                UserName = configuration.GetSection("RabbitMQ")["UserName"],
                Password = configuration.GetSection("RabbitMQ")["Password"],
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare("sentNotification", false, false, false, null);
            _channel.QueueDeclare("chatReceiverNotification", false, false, false, null);
            _channel.QueueDeclare("chatSenderNotification", false, false, false, null);
            // _channel.ExchangeDeclare(exchange: "OrderExchange", type: ExchangeType.Fanout);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var queueName = _channel.QueueDeclare().QueueName;
            // _channel.QueueBind(queue: queueName, exchange: "OrderExchange", routingKey: "");

            // CreateConsumer(queueName, async (message) =>
            // {
            //     var eventMessage = JsonSerializer.Deserialize<EventDTO>(message);
            //     if (eventMessage == null)
            //     {
            //         return;
            //     }
            //     // using (var scope = _serviceProvider.CreateScope())
            //     // {
            //     //     var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            //     //     emailService.SendEmail(messages);
            //     // }
            // });

            CreateConsumer("chatReceiverNotification", async (message) =>
            {

                var eventMessage = JsonSerializer.Deserialize<ChatEventDto>(message);
                if (eventMessage == null)
                {
                    return;
                }
                foreach (var userId in eventMessage.UserId!)
                {
                    await _chatHubContext.Clients.Group($"user:{userId}").SendAsync("ChatReceiverMessage", eventMessage.Message);
                }
            });

            CreateConsumer("chatSenderNotification", async (message) =>
            {
                var eventMessage = JsonSerializer.Deserialize<ChatEventDto>(message);
                if (eventMessage == null)
                {
                    return;
                }
                foreach (var userId in eventMessage.UserId!)
                {
                    await _chatHubContext.Clients.Group($"user:{userId}").SendAsync("ChatSenderMessage", eventMessage.Message);
                }
            });

            CreateConsumer("sentNotification", async (message) =>
            {

                var eventMessage = JsonSerializer.Deserialize<EventDto>(message);
                if (eventMessage == null)
                {
                    return;
                }
                using (var scope = _serviceProvider.CreateScope())
                {
                    var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                    await notificationService.CreateNotification(eventMessage);
                }
            });

            return Task.CompletedTask;
        }



        private void CreateConsumer(string queueName, Func<string, Task> processMessage)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (ch, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                await processMessage(message);

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
