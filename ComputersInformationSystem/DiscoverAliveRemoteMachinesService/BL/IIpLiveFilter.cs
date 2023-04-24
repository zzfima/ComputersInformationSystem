
public interface IIpLiveFilter
{
    Task<IList<IPAddress>> FilterLiveOnly(IList<IPAddress> allIp);
}