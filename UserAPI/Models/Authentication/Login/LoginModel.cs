using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UserAPI.Models.Authentication.Login
{
    public class LoginModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}