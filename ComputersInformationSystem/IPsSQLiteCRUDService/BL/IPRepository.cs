public class IPRepository : IIPRepository
{
    private IPDB _dbContext { get; }

    public IPRepository(IPDB hdb)
    {
        _dbContext = hdb;
    }

    public async Task<IList<IP>> GetIPsAsync() => await _dbContext.RemoteMachines.ToListAsync();

    public async Task<IP> GetIPAsync(int id) => await _dbContext.RemoteMachines.FirstAsync(h => h.Id == id);

    public async Task<IP> GetIPAsync(string ipAddress) => await _dbContext.RemoteMachines.FirstAsync(h => h.Address == ipAddress);

    public async Task InsertIPAsync(IP newIPAddress)
    {
        await _dbContext.AddAsync(newIPAddress);
        await SaveAsync();
    }

    public async Task UpdateIPAsync(IP newIP)
    {
        var foundIP = await _dbContext.RemoteMachines.FindAsync(new object[] { newIP.Id });
        if (foundIP == null)
            return;

        foundIP.Address = newIP.Address;
        foundIP.IsAlive = newIP.IsAlive;
        foundIP.LastUpdate = newIP.LastUpdate;

        _dbContext.Update(foundIP);
        await SaveAsync();
    }

    public async Task DeleteIPAsync(int id)
    {
        var IPToDelete = await _dbContext.RemoteMachines.FindAsync(new object[] { id });
        if (IPToDelete == null)
            return;
        _dbContext.Remove(IPToDelete);
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