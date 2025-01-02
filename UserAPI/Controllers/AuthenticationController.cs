using System.ComponentModel.DataAnnotations;
using api.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using User.Management.Service.Models.Authentication.SignUp;
using UserAPI.DTOs;
using UserAPI.Interfaces;
using UserAPI.Models;
using UserAPI.Models.Authentication.Login;
using UserAPI.Models.Authentication.User;
using UserAPI.RabbitMQ;

namespace UserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserManagement _user;
        private readonly IRabbmitMQCartMessageSender _messageBus;

        public AuthenticationController(UserManager<ApplicationUser> userManager, IUserManagement user,
            IRabbmitMQCartMessageSender messageBus
            )
        {
            _userManager = userManager;
            _user = user;
            _messageBus = messageBus;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterUser registerUser)
        {
            var tokenResponse = await _user.CreateUserWithTokenAsync(registerUser);
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Authentication", new { tokenResponse.Response!.Token, email = registerUser.Email }, Request.Scheme);
            _messageBus.SendMessage(new { To = registerUser.Email, Subject = "Confirmation email link", Content = confirmationLink! }, "sentEmail", "queue");
            return ApiResponse.Success($"User created & Email Sent to {registerUser.Email} SuccessFully");
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return ApiResponse.Success("Email Verified Successfully");
                }
            }
            return ApiResponse.BadRequest("This User Doesnot exist!");
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var loginOtpResponse = await _user.GetOtpByLoginAsync(loginModel);
            if (loginOtpResponse.Response != null)
            {
                var user = loginOtpResponse.Response.User;
                if (user.TwoFactorEnabled)
                {
                    var token = loginOtpResponse.Response.Token;
                    _messageBus.SendMessage(new { To = user.Email, Subject = "OTP Confirmation", Content = token }, "sentEmail", "queue");
                    return ApiResponse.Success($"We have sent an OTP to your Email {user.Email}");
                }
                if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password!))
                {
                    var serviceResponse = await _user.GetJwtTokenAsync(user);
                    return ApiResponse.Success(serviceResponse);
                }
            }

            return Unauthorized();
        }

        [HttpPost]
        [Route("login-2FA")]
        public async Task<IActionResult> LoginWithOTP(LoginWithOTPDTO loginWithOTP)
        {
            var jwt = await _user.LoginUserWithJWTokenAsync(loginWithOTP.Code, loginWithOTP.Username);
            if (jwt.IsSuccess)
            {
                return ApiResponse.Success(jwt);

            }
            return ApiResponse.BadRequest("Invalid code");
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword([Required] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var forgotPasswordLink = Url.Action(nameof(ResetPassword), "Authentication", new { token, email = user.Email }, Request.Scheme);
                _messageBus.SendMessage(new { To = user.Email, Subject = "Forgot password link", Content = forgotPasswordLink! }, "sentEmail", "queue");
                return ApiResponse.Success($"Password changed request is sent on email: ${user.Email}");
            }
            return ApiResponse.BadRequest("Something wrong..Try Again");
        }

        [HttpGet("reset-password")]

        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            var model = new ResetPassword { Token = token, Email = email };
            return Ok(new
            {
                model
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        {
            var user = await _userManager.FindByEmailAsync(resetPassword.Email);
            if (user != null)
            {
                var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);
                if (!resetPassResult.Succeeded)
                {
                    foreach (var error in resetPassResult.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return Ok(ModelState);
                }
                return ApiResponse.Success($"Password has been changed");
            }
            return ApiResponse.BadRequest("Something wrong..Try Again");
        }

        [HttpPost]
        [Route("Refresh-Token")]
        public async Task<IActionResult> RefreshToken(LoginResponse tokens)
        {
            var jwt = await _user.RenewAccessTokenAsync(tokens);
            if (jwt.IsSuccess)
            {
                return ApiResponse.Success(jwt);
            }
            return ApiResponse.BadRequest("Invalid Code");
        }

        [HttpGet]
        [Route("admin")]
        public async Task<IActionResult> GetAllAdmins()
        {
            var admins = await _user.GetAdmins();
            return Ok(admins);
        }

        [HttpGet]
        [Route("user")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _user.GetUser(id);
            return Ok(user);
        }

    }
}