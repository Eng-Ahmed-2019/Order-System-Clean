namespace OrderSystem.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { set; get; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}