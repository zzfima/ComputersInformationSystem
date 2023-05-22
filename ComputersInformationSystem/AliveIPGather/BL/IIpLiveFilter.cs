
public interface IIpLiveFilter
{
    Task<IList<IPAddress>> FilterLiveOnly(IList<IPAddress> allIp);
    Task<bool> IsIPAlive(IPAddress ip);
}