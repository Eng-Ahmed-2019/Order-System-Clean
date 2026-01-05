namespace OrderSystem.Application.DTOs
{
    public class CreateOrderRequestDto
    {
        public string OrderNumber { get; set; } = null!;
        public decimal TotalAmount { get; set; }
    }
}