public interface IIPRepository : IDisposable
{
    Task<IList<IP>> GetIPsAsync();
    Task<IP> GetIPAsync(int id);
    Task<IP> GetIPAsync(string ip);
    Task InsertIPAsync(IP ip);
    Task UpdateIPAsync(IP ip);
    Task DeleteIPAsync(int id);
    Task SaveAsync();
}