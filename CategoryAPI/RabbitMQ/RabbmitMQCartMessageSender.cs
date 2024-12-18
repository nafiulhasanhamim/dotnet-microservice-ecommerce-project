using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace CategoryAPI.RabbitMQ
{
    public class RabbmitMQCartMessageSender : IRabbmitMQCartMessageSender
    {
        private readonly string _hostName;
        private readonly string _username;
        private readonly string _password;
        private IConnection _connection;

        public RabbmitMQCartMessageSender(IConfiguration configuration)
        {
            var rabbitMqConfig = configuration.GetSection("RabbitMQ");
            _hostName = rabbitMqConfig["HostName"];
            _username = rabbitMqConfig["UserName"];
            _password = rabbitMqConfig["Password"];
        }
        public void SendMessage(object message, string exchangeName = "DefaultExchange")
        {
            if (string.IsNullOrEmpty(exchangeName))
            {
                throw new ArgumentException("Exchange name cannot be null or empty.", nameof(exchangeName));
            }

            if (ConnectionExists())
            {
                using var channel = _connection.CreateModel();

                channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);

                var json = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(json);

                channel.BasicPublish(
                    exchange: exchangeName,
                    routingKey: "",
                    basicProperties: null,
                    body: body
                );

            }
        }

        private void CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _hostName,
                    Password = _password,
                    UserName = _username
                };

                _connection = factory.CreateConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not create connection: {ex.Message}");
            }
        }

        private bool ConnectionExists()
        {
            if (_connection != null)
            {
                return true;
            }

            CreateConnection();
            return _connection != null;
        }
    }
}
