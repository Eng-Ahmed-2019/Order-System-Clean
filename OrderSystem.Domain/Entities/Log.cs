namespace OrderSystem.Domain.Entities
{
    public class Log
    {
        public int Id { get; set; }
        public string? TraceId { set; get; }
        public string? Message { get; set; }
        public string? Level { get; set; }
        public string? Exception { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}