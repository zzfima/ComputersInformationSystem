public interface IConfigurationRepository : IDisposable
{
    Task<IList<Configuration>> GetConfigurationsAsync();
    Task UpdateConfigurationAsync(Configuration newConfiguration);
    Task InsertConfigurationAsync(Configuration newConfiguration);
    Task SaveAsync();
}