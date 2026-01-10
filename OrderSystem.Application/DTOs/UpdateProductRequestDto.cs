namespace OrderSystem.Application.DTOs
{
    public class UpdateProductRequestDto
    {
        public int Id { get; set; }
        public int SubCategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}