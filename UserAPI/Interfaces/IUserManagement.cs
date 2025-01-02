using api.Controllers;
using User.Management.Service.Models;
using User.Management.Service.Models.Authentication.SignUp;
using UserAPI.DTOs;
using UserAPI.Models;
using UserAPI.Models.Authentication.Login;
using UserAPI.Models.Authentication.User;

namespace UserAPI.Interfaces
{
    public interface IUserManagement
    {
        Task<ApiResponseUser<CreateUserResponse>> CreateUserWithTokenAsync(RegisterUser registerUser);
        Task<ApiResponseUser<LoginOtpResponse>> GetOtpByLoginAsync(LoginModel loginModel);
        Task<ApiResponseUser<LoginResponse>> LoginUserWithJWTokenAsync(string otp, string userName);
        Task<ApiResponseUser<LoginResponse>> GetJwtTokenAsync(ApplicationUser user);
        Task<ApiResponseUser<LoginResponse>> RenewAccessTokenAsync(LoginResponse tokens);
        Task<IEnumerable<AdminDto>> GetAdmins();
        Task<UserDto> GetUser(string id);

    }
}
