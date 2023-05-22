namespace RemoteMachineLiveIPService
{
    public class RemoteMachineLiveIP : IRemoteMachineLiveIP
    {
        IIpIterator _ipIterator;
        IIpLiveFilter _ipLiveFilter;

        public RemoteMachineLiveIP()
        {
            _ipIterator = new IpIterator();
            _ipLiveFilter = new IpLiveIpFilter();
        }

        public async Task<string[]> GetAliveIPs(string fromIP, string toIP)
        {
            var IPs = _ipIterator.CreateIpList(from: IPAddress.Parse(fromIP), to: IPAddress.Parse(toIP));
            var liveIPs = await _ipLiveFilter.FilterLiveOnly(IPs);
            return liveIPs.Select(ip => ip.ToString()).ToArray();
        }

        public async Task<bool> IsIPAlive(string ip) => await _ipLiveFilter.IsIPAlive(IPAddress.Parse(ip));

        public async Task<string> GetHostName(string ip)
        {
            var hostEntry = await Dns.GetHostEntryAsync(ip);
            return hostEntry.HostName;
        }
    }
}
