public class RemoteMachineRepository : IRemoteMachineRepository
{
    private RemoteMachineDB _dbContext { get; }

    public RemoteMachineRepository(RemoteMachineDB hdb)
    {
        _dbContext = hdb;
    }

    public async Task<IList<RemoteMachine>> GetRemoteMachinesAsync() => await _dbContext.RemoteMachines.ToListAsync();

    public async Task<RemoteMachine> GetRemoteMachineAsync(int id) => await _dbContext.RemoteMachines.FirstAsync(h => h.Id == id);

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
        foundRemoteMachine.PhoenixVersion = newRemoteMachine.PhoenixVersion;
        foundRemoteMachine.FWVersion = newRemoteMachine.FWVersion;
        foundRemoteMachine.LastUpdate = newRemoteMachine.LastUpdate;

        _dbContext.Update(foundRemoteMachine);
        await SaveAsync();
    }

    public async Task DeleteRemoteMachineAsync(int id)
    {
        var foundRemoteMachine = await _dbContext.RemoteMachines.FindAsync(new object[] { id });
        if (foundRemoteMachine == null)
            return;
        _dbContext.Remove(foundRemoteMachine);
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