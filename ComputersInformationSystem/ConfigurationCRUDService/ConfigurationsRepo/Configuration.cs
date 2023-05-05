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
    public string ConfigurationCRUDServiceURL { get; set; } = string.Empty;
    public string DiscoverAliveRemoteMachinesServiceURL { get; set; } = string.Empty;
    public string RemoteMachineVersionInfoServiceURL { get; set; } = string.Empty;
    public string ToolsInformationCRUDServiceURL { get; set; } = string.Empty;
    public string ToolsInformationSystemSchedulerServiceURL { get; set; } = string.Empty;
    public string MessageQueueHostName { get; set; } = string.Empty;
    public string MessageQueuePassword { get; set; } = string.Empty;
    public string MessageQueueUserName { get; set; } = string.Empty;
    public string MessageQueueRoutingKey { get; set; } = string.Empty;
    public string MessageQueueServiceURL { get; set; } = string.Empty;
}