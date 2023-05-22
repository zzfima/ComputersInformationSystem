public class RemoteMachineRepository : IRemoteMachineRepository
{
    private RemoteMachineDB _dbContext { get; }

    public RemoteMachineRepository(RemoteMachineDB hdb)
    {
        _dbContext = hdb;
    }

    public async Task<IList<RemoteMachine>> GetRemoteMachinesAsync() => await _dbContext.RemoteMachines.ToListAsync();

    public async Task<RemoteMachine> GetRemoteMachineAsync(int id) => await _dbContext.RemoteMachines.FirstAsync(h => h.Id == id);

    public async Task<RemoteMachine> GetRemoteMachineAsync(string ip) => await _dbContext.RemoteMachines.FirstAsync(h => h.IPAddress == ip);

    public async Task InsertRemoteMachineAsync(RemoteMachine newRemoteMachine)
    {
        await _dbContext.AddAsync(newRemoteMachine);
        await SaveAsync();
    }

    public async Task UpdateRemoteMachineAsync(RemoteMachine newRemoteMachine)
    {
        var foundRemoteMachine = await _dbContext.RemoteMachines.FindAsync(new object[] { newRemoteMachine.Id });
        if (foundRemoteMachine == null)
            return;

        foundRemoteMachine.IPAddress = newRemoteMachine.IPAddress;
        foundRemoteMachine.HostName = newRemoteMachine.HostName;
        foundRemoteMachine.PhoenixVersion = newRemoteMachine.PhoenixVersion;
        foundRemoteMachine.FWVersion = newRemoteMachine.FWVersion;
        foundRemoteMachine.LastUpdate = newRemoteMachine.LastUpdate;

        _dbContext.Update(foundRemoteMachine);
        await SaveAsync();
    }

    public async Task DeleteRemoteMachineAsync(int id)
    {
        var remoteMachineToDelete = await _dbContext.RemoteMachines.FindAsync(new object[] { id });
        if (remoteMachineToDelete == null)
            return;
        _dbContext.Remove(remoteMachineToDelete);
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