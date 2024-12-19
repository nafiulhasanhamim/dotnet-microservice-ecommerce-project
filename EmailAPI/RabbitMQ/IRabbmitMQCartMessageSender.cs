
namespace EmailAPI.RabbitMQ
{
    public interface IRabbmitMQCartMessageSender
    {
        void SendMessage(object message, string name, string type);
    }
}