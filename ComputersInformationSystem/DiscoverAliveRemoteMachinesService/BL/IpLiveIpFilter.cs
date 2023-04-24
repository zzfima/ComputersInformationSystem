internal class IpLiveIpFilter : IIpLiveFilter
{
    public async Task<IList<IPAddress>> FilterLiveOnly(IList<IPAddress> allIp)
    {
        var liveIpAddresses = new List<IPAddress>();

        await Parallel.ForEachAsync(allIp, async (ip, ct) =>
        {
            var pingSender = new Ping();
            var reply = await pingSender.SendPingAsync(ip, 100);
            if (reply.Status == IPStatus.Success)
                liveIpAddresses.Add(ip);
        });

        return liveIpAddresses;
    }
}