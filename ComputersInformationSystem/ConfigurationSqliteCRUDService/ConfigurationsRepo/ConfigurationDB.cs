public class ConfigurationDB : DbContext
{
    public ConfigurationDB(DbContextOptions<ConfigurationDB> options) : base(options) { }
    public DbSet<Configuration> Configurations => Set<Configuration>();
}