public class RemoteMachine
{
    public int Id { get; set; }
    public string IPAddress { get; set; } = string.Empty;
    public string HostName { get; set; } = string.Empty;
    public string PhoenixVersion { get; set; } = string.Empty;
    public string FWVersion { get; set; } = string.Empty;
    public DateTime LastUpdate { get; set; }
}