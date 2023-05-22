using System.Net;

namespace VersionRetrieverService
{
    public interface IVersionRetriver
    {
        Task<string> GetVersionByIp(IPAddress iPAddress, string userName, string password);
    }
}