using EmailAPI.Models;

namespace EmailAPI.Interfaces
{
    public interface IEmailService
    {
        void SendEmail(Message message);
    }
}