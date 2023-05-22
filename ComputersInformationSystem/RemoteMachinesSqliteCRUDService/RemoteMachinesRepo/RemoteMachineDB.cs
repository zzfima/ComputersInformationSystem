public class RemoteMachineDB : DbContext
{
    public RemoteMachineDB(DbContextOptions<RemoteMachineDB> options) : base(options) { }
    public DbSet<RemoteMachine> RemoteMachines => Set<RemoteMachine>();
}