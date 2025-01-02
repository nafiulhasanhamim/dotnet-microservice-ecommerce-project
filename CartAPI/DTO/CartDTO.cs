namespace CartAPI.DTOs
{
    public class AddToCartDto
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
    }
    public class UpdateCartItemDto
    {
        public int Quantity { get; set; }
    }
    public class CartItemDto
    {
        public string CartId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime AddedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    public class CartDto
    {
        public List<CartItemDto> Items { get; set; }
        public decimal TotalPrice => Items.Sum(item => item.Price * item.Quantity);
    }
}
