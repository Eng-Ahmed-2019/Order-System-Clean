namespace OrderSystem.Domain.Entities
{
    public class UserSession
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public DateTime ExpiresAt {  get; set; }
        public DateTime CreatedAt {  get; set; }
    }
}