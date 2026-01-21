namespace OrderSystem.Domain.Entities
{
    public class BannedIp
    {
        public int Id { get; set; }
        public string IpAddress { get; set; } = null!;
        public DateTime BannedUntil { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}