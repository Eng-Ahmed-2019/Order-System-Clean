namespace OrderSystem.Application.DTOs
{
    public class UpdateCategoryRequestDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}