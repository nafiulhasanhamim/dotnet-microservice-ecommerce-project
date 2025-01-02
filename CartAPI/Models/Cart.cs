using System.ComponentModel.DataAnnotations;

namespace CartAPI.Models
{
    public class CartItem
    {
        [Key]
        public string CartId { get; set; }
        public string UserId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime AddedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
