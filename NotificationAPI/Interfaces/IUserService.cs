using NotificationAPI.DTOs;

namespace NotificationAPI.Interfaces
{
    public interface IUserService
    {

        Task<IEnumerable<AdminDto>> GetAdmins();

    }
}