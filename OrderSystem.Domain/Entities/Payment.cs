namespace OrderSystem.Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Provider { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string TransactionId { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}