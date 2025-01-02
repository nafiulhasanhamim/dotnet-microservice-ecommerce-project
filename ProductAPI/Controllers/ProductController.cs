using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Controllers;
using ProductAPI.DTOs;
using ProductAPI.Interfaces;
using ProductAPI.Services.Caching;
namespace Services.ProductAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IRedisCacheService _cache;
        public ProductController(IProductService productService, IRedisCacheService cache)
        {
            _productService = productService;
            _cache = cache;

        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int PageNumber = 1, [FromQuery] int PageSize = 5, [FromQuery] string? search = null, [FromQuery] string? sortOrder = null)
        {
            var products = await _cache.GetDataAsync<IEnumerable<ProductReadDto>>("products");
            if (products is not null)
            {
                return ApiResponse.Success(products);
            }
            var productList = await _productService.GetAllAsync(PageNumber, PageSize, search, sortOrder);
            _cache.SetData("products", productList);
            return Ok(productList);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return ApiResponse.BadRequest("Product with this id is not found");
            return ApiResponse.Success(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateDto productDto)
        {
            var product = await _productService.CreateAsync(productDto);
            _cache.RemoveData("products");
            return ApiResponse.Success(product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] ProductUpdateDto productDto)
        {
            var product = await _productService.UpdateAsync(id, productDto);
            _cache.RemoveData("products");
            return ApiResponse.Success(product);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleteProduct = await _productService.DeleteAsync(id);
            if (deleteProduct == true)
            {
                _cache.RemoveData("products");
                return ApiResponse.Success("Deleted Successfully");
            }
            else
            {
                return ApiResponse.BadRequest("Product not found");
            }
        }
    }
}