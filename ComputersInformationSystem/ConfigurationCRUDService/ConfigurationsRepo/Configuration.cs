public class Configuration
{
    public int Id { get; set; }
    public string FromIPAddress { get; set; } = string.Empty;
    public string ToIPAddress { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FWFileName { get; set; } = string.Empty;
    public string PhoenixFileName { get; set; } = string.Empty;
    public int DiscoverFrequencyMinutes { get; set; }
    public int UpdateFrequencyMinutes { get; set; }
}