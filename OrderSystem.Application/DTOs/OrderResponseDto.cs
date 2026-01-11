namespace OrderSystem.Application.DTOs
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; } = 0;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}