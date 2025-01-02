
using ChatAPI.DTO;

namespace ChatAPI.Interfaces
{
    public interface IUserService
    {

        Task<UserDto> GetUser(string id);

    }
}