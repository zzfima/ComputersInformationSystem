internal class IpIterator : IIpIterator
{
    public IList<IPAddress> CreateIpList(IPAddress startIp, IPAddress endIp)
    {
        List<IPAddress> ipAddressList = new List<IPAddress>();
        IPAddress currentIp = startIp;

        while (currentIp.Address.CompareTo(endIp.Address) <= 0)
        {
            ipAddressList.Add(currentIp);
            byte[] bytes = currentIp.GetAddressBytes();

            for (int i = bytes.Length - 1; i >= 0; i--)
            {
                if (bytes[i] == 255)
                {
                    bytes[i] = 0;
                }
                else
                {
                    bytes[i]++;
                    break;
                }
            }

            currentIp = new IPAddress(bytes);
        }

        return ipAddressList;
    }
}