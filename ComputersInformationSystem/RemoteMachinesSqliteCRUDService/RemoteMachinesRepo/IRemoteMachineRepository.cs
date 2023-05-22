public interface IRemoteMachineRepository : IDisposable
{
    Task<IList<RemoteMachine>> GetRemoteMachinesAsync();
    Task<RemoteMachine> GetRemoteMachineAsync(int id);
    Task<RemoteMachine> GetRemoteMachineAsync(string ip);
    Task InsertRemoteMachineAsync(RemoteMachine remoteMachine);
    Task UpdateRemoteMachineAsync(RemoteMachine remoteMachine);
    Task DeleteRemoteMachineAsync(int id);
    Task SaveAsync();
}