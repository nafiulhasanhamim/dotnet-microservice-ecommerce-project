namespace CategoryAPI.Models
{
    public class Category
    {
        public string? CategoryId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ParentId { get; set; }
        public Category? ParentCategory { get; set; }
        public ICollection<Category>? SubCategories { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}