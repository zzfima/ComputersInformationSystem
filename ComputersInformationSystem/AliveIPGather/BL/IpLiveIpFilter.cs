public class IpLiveIpFilter : IIpLiveFilter
{
    public async Task<IList<IPAddress>> FilterLiveOnly(IList<IPAddress> allIp)
    {
        var liveIpAddresses = new List<IPAddress>();

        await Parallel.ForEachAsync(allIp, async (ip, ct) =>
        {
            var res = await IsIPAlive(ip);
            if (res)
                liveIpAddresses.Add(ip);
        });

        return liveIpAddresses;
    }

    public async Task<bool> IsIPAlive(IPAddress ip)
    {
        var pingSender = new Ping();
        var reply = await pingSender.SendPingAsync(ip, 100);
        return reply.Status == IPStatus.Success;
    }
}