namespace RemoteMachineLiveIPService
{
    public interface IRemoteMachineLiveIP
    {
        Task<bool> IsIPAlive(string ip);
        Task<string> GetHostName(string ip);
    }
}