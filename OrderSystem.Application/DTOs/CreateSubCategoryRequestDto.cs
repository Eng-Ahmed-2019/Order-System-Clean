namespace OrderSystem.Application.DTOs
{
    public class CreateSubCategoryRequestDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}