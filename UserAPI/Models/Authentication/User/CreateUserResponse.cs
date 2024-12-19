using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAPI.Models.Authentication.User
{
    public class CreateUserResponse
    {
       public string Token { get; set; }=null!; 
    }
}