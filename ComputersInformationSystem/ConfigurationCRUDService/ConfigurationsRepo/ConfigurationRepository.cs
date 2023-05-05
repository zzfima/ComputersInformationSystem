public class ConfigurationRepository : IConfigurationRepository
{
    private ConfigurationDB _dbContext { get; }

    public ConfigurationRepository(ConfigurationDB hdb)
    {
        _dbContext = hdb;
    }

    public async Task<IList<Configuration>> GetConfigurationsAsync() => await _dbContext.Configurations.ToListAsync();

    public async Task UpdateConfigurationAsync(Configuration newConfiguration)
    {
        var foundConfiguration = await _dbContext.Configurations.FindAsync(new object[] { newConfiguration.Id });
        if (foundConfiguration == null)
            return;

        foundConfiguration.FromIPAddress = newConfiguration.FromIPAddress;
        foundConfiguration.ToIPAddress = newConfiguration.ToIPAddress;
        foundConfiguration.DiscoverFrequencyMinutes = newConfiguration.DiscoverFrequencyMinutes;
        foundConfiguration.UpdateFrequencyMinutes = newConfiguration.UpdateFrequencyMinutes;
        foundConfiguration.PhoenixFileName = newConfiguration.PhoenixFileName;
        foundConfiguration.FWFileName = newConfiguration.FWFileName;
        foundConfiguration.UserName = newConfiguration.UserName;
        foundConfiguration.Password = newConfiguration.Password;
        foundConfiguration.ConfigurationCRUDServiceURL = newConfiguration.ConfigurationCRUDServiceURL;
        foundConfiguration.DiscoverAliveRemoteMachinesServiceURL = newConfiguration.DiscoverAliveRemoteMachinesServiceURL;
        foundConfiguration.RemoteMachineVersionInfoServiceURL = newConfiguration.RemoteMachineVersionInfoServiceURL;
        foundConfiguration.ToolsInformationCRUDServiceURL = newConfiguration.ToolsInformationCRUDServiceURL;
        foundConfiguration.MessageQueueServiceURL = newConfiguration.MessageQueueServiceURL;
        foundConfiguration.MessageQueueHostName = newConfiguration.MessageQueueHostName;
        foundConfiguration.MessageQueuePassword = newConfiguration.MessageQueuePassword;
        foundConfiguration.MessageQueueUserName = newConfiguration.MessageQueueUserName;
        foundConfiguration.MessageQueueServiceURL = newConfiguration.MessageQueueServiceURL;
        foundConfiguration.MessageQueueRoutingKey = newConfiguration.MessageQueueRoutingKey;

        _dbContext.Update(foundConfiguration);
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