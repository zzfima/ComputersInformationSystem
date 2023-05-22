public class ConfigurationRepository : IConfigurationRepository
{
    private ConfigurationDB _dbContext { get; }

    public ConfigurationRepository(ConfigurationDB hdb)
    {
        _dbContext = hdb;
    }

    public async Task<IList<Configuration>> GetConfigurationsAsync() => await _dbContext.Configurations.ToListAsync();

    public async Task UpdateConfigurationAsync(Configuration newCnfg)
    {
        var oldCnfg = await _dbContext.Configurations.FindAsync(new object[] { newCnfg.Id });
        if (oldCnfg == null)
            return;

        oldCnfg.FromIPAddress = newCnfg.FromIPAddress;
        oldCnfg.ToIPAddress = newCnfg.ToIPAddress;
        oldCnfg.DiscoverFrequencyMinutes = newCnfg.DiscoverFrequencyMinutes;
        oldCnfg.UpdateFrequencyMinutes = newCnfg.UpdateFrequencyMinutes;
        oldCnfg.PhoenixFileName = newCnfg.PhoenixFileName;
        oldCnfg.FWFileName = newCnfg.FWFileName;
        oldCnfg.UserName = newCnfg.UserName;
        oldCnfg.Password = newCnfg.Password;
        oldCnfg.ConfigurationSqliteCRUDServiceURL = newCnfg.ConfigurationSqliteCRUDServiceURL;
        oldCnfg.RemoteMachinesSqliteCRUDServiceURL = newCnfg.RemoteMachinesSqliteCRUDServiceURL;
        oldCnfg.RemoteMachinesNeo4jCRUDServiceURL = newCnfg.RemoteMachinesNeo4jCRUDServiceURL;
        oldCnfg.IsToDeleteDeathRemoteMachine = newCnfg.IsToDeleteDeathRemoteMachine;

        oldCnfg.MQHostName = newCnfg.MQHostName;
        oldCnfg.MQPassword = newCnfg.MQPassword;
        oldCnfg.MQUserName = newCnfg.MQUserName;
        oldCnfg.MQVersionGatherProducerServiceURL = newCnfg.MQVersionGatherProducerServiceURL;
        oldCnfg.MQAliveIPGatherProducerServiceURL = newCnfg.MQAliveIPGatherProducerServiceURL;
        oldCnfg.MQAliveIPGatherRoutingKey = newCnfg.MQAliveIPGatherRoutingKey;
        oldCnfg.MQVersionGatherRoutingKey = newCnfg.MQVersionGatherRoutingKey;

        oldCnfg.ToolsInformationSystemSchedulerServiceURL = newCnfg.ToolsInformationSystemSchedulerServiceURL;
        oldCnfg.LoggingServiceURL = newCnfg.LoggingServiceURL;
        oldCnfg.Neo4jHostName = newCnfg.Neo4jHostName;
        oldCnfg.Neo4jUserName = newCnfg.Neo4jUserName;
        oldCnfg.Neo4jPassword = newCnfg.Neo4jPassword;

        oldCnfg.IPsSqliteCRUDServiceURL = newCnfg.IPsSqliteCRUDServiceURL;
        oldCnfg.CacheServiceURL = newCnfg.CacheServiceURL;
        oldCnfg.CacheServiceTTLAbsoluteExpirationMinutes = newCnfg.CacheServiceTTLAbsoluteExpirationMinutes;
        oldCnfg.RedisServerHostName = newCnfg.RedisServerHostName;

        oldCnfg.ElasticHostName = newCnfg.ElasticHostName;
        oldCnfg.ElasticIndexName = newCnfg.ElasticIndexName;

        _dbContext.Update(oldCnfg);
        await SaveAsync();
    }

    public async Task InsertConfigurationAsync(Configuration newConfiguration)
    {
        await _dbContext.AddAsync(newConfiguration);
        await SaveAsync();
    }

    public async Task SaveAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteConfigurationAsync(int id)
    {
        var configurationToDelete = await _dbContext.Configurations.FindAsync(new object[] { id });
        if (configurationToDelete == null)
            return;
        _dbContext.Remove(configurationToDelete);
        await SaveAsync();
    }

    public async void Dispose()
    {
        await _dbContext.DisposeAsync();
    }
}