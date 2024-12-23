using Newtonsoft.Json;
using NotificationAPI.DTOs;
using NotificationAPI.Interfaces;

namespace NotificationAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public UserService(IHttpClientFactory clientFactory)
        {
            _httpClientFactory = clientFactory;
        }
        public async Task<IEnumerable<AdminDto>> GetAdmins()
        {
            var client = _httpClientFactory.CreateClient("User");
            var response = await client.GetAsync($"/api/Authentication/admin");

            var apiContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<AdminDto>>(apiContent);
        }
    }
}