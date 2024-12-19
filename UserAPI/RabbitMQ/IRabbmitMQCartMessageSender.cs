
namespace UserAPI.RabbitMQ
{
    public interface IRabbmitMQCartMessageSender
    {
        void SendMessage(object message, string name, string type);
    }
}