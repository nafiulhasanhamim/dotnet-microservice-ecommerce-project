namespace CartAPI.DTO;

public class ProductResponseDto
{
    public bool Success { get; set; }
    public ProductDto Data { get; set; }
    public string Message { get; set; }
    public int StatusCode { get; set; }
}

public class ProductDto
{
    public string ProductId { get; set; }
    public string Name { get; set; }
    public string ProductImageUrl { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
    public int Quantity { get; set; }
    public string Status { get; set; }
    public int ThreshholdQuantity { get; set; }
    public bool CategoryVerify { get; set; }
    public string CategoryId { get; set; }
    public DateTime CreatedAt { get; set; }
}