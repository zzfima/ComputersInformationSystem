
public class IPDB : DbContext
{
    public IPDB(DbContextOptions<IPDB> options) : base(options) { }
    public DbSet<IP> RemoteMachines => Set<IP>();
}