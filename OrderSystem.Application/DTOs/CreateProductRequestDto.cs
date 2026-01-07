namespace OrderSystem.Application.DTOs
{
    public class CreateProductRequestDto
    {
        public int SubCategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}