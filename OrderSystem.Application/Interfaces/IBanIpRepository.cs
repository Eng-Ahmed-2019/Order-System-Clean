namespace OrderSystem.Application.Interfaces
{
    public interface IBanIpRepository
    {
        Task<bool> IsBanned(string ip);
        Task BanAsync(string ip, TimeSpan duration, string reason);
    }
}