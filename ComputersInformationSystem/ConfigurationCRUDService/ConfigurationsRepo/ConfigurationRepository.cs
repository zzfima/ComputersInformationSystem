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

    public async void Dispose()
    {
        await _dbContext.DisposeAsync();
    }
}