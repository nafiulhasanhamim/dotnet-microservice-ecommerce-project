using System.Security.Claims;

namespace CouponAPI.Extensions
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