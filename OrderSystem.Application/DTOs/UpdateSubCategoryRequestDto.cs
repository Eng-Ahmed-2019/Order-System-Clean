namespace OrderSystem.Application.DTOs
{
    public class UpdateSubCategoryRequestDto
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}