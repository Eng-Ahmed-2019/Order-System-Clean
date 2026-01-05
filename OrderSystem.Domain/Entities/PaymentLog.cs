namespace OrderSystem.Domain.Entities
{
    public class PaymentLog
    {
        public int Id { get; set; }
        public string RequestJson { get; set; } = null!;
        public string ResponseJson { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}