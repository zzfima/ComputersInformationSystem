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
    public bool IsToDeleteDeathRemoteMachine { get; set; }

    public string ConfigurationSqliteCRUDServiceURL { get; set; } = string.Empty;
    public string RemoteMachinesSqliteCRUDServiceURL { get; set; } = string.Empty;
    public string ToolsInformationSystemSchedulerServiceURL { get; set; } = string.Empty;
    public string LoggingServiceURL { get; set; } = string.Empty;
    public string RemoteMachinesNeo4jCRUDServiceURL { get; set; } = string.Empty;

    public string IPsSqliteCRUDServiceURL { get; set; } = string.Empty;
    public string CacheServiceURL { get; set; } = string.Empty;
    public string RedisServerHostName { get; set; } = string.Empty;
    public double CacheServiceTTLAbsoluteExpirationMinutes { get; set; }

    public string MQVersionGatherProducerServiceURL { get; set; } = string.Empty;
    public string MQAliveIPGatherProducerServiceURL { get; set; } = string.Empty;
    public string MQHostName { get; set; } = string.Empty;
    public string MQPassword { get; set; } = string.Empty;
    public string MQUserName { get; set; } = string.Empty;
    public string MQAliveIPGatherRoutingKey { get; set; } = string.Empty;
    public string MQVersionGatherRoutingKey { get; set; } = string.Empty;

    public string Neo4jHostName { get; set; } = string.Empty;
    public string Neo4jUserName { get; set; } = string.Empty;
    public string Neo4jPassword { get; set; } = string.Empty;

    public string ElasticHostName { get; set; } = string.Empty;
    public string ElasticIndexName { get; set; } = string.Empty;
}