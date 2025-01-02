using CartAPI.DTO;
using CartAPI.Interfaces;
using Newtonsoft.Json;

namespace CartAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductService(IHttpClientFactory clientFactory)
        {
            _httpClientFactory = clientFactory;
        }
        public async Task<ProductResponseDto> GetProduct(string id)
        {
            var client = _httpClientFactory.CreateClient("Product");
            var response = await client.GetAsync($"/api/Product/{id}");

            var apiContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ProductResponseDto>(apiContent);
        }
    }
}