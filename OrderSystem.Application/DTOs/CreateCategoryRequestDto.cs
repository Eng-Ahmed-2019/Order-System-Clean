namespace OrderSystem.Application.DTOs
{
    public class CreateCategoryRequestDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}