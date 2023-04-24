namespace GetFileVersion.BL
{
    public interface IVersionRetriver
    {
        Task<string> GetVersionByIp(IPAddress iPAddress, string userName, string password);
    }
}