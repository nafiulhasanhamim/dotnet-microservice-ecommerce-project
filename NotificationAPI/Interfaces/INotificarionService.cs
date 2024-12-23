using NotificationAPI.DTOs;
namespace NotificationAPI.Interfaces
{
    public interface INotificationService
    {
        Task<bool> CreateNotification(EventDto categoryData);
    }
}