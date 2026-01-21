namespace OrderSystem.Domain.Entities
{
    public class LoginLockout
    {
        public int Id { get; set; }
        public string KeyValue { get; set; } = null!;
        public int FailedAttempts { get; set; }
        public DateTime? LockedUntil { get; set; }
    }
}