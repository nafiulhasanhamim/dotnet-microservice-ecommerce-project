using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace dotnet_mvc.Extensions
{
    public static class ClaimsExtensions
    {
        public static string GetUserName(this ClaimsPrincipal user) {
            return user.Claims.SingleOrDefault(x => x.Type.Equals("https://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname")).Value;
        }

        public static string? GetUserId(this ClaimsPrincipal user)
        {
            Console.WriteLine("claimprincipal");
            Console.WriteLine(user.Claims.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value);
            return user.Claims.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }

    }
}