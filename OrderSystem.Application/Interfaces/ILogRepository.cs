namespace OrderSystem.Application.Interfaces
{
    public interface ILogRepository
    {
        Task CreatedException(Exception exception, string traceId);
    }
}