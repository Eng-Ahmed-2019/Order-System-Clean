namespace OrderSystem.Application.DTOs
{
    public class AddToCartRequestDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}