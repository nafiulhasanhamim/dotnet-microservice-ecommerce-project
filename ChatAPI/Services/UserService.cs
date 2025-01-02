using ChatAPI.DTO;
using ChatAPI.Interfaces;
using Newtonsoft.Json;

namespace ChatAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public UserService(IHttpClientFactory clientFactory)
        {
            _httpClientFactory = clientFactory;
        }
        public async Task<UserDto> GetUser(string id)
        {
            var client = _httpClientFactory.CreateClient("User");
            var response = await client.GetAsync($"/api/Authentication/user?id={id}");

            var apiContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserDto>(apiContent);
        }
    }
}