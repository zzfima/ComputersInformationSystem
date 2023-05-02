namespace Utilities
{
    public static class RequestsWarpper
    {
        public static async Task<IList<Configuration>> GetConfiguration()
        {
            IHttpResponseWrapper<IList<Configuration>> httpResponseConfiguration = new HttpResponseWrapper<IList<Configuration>>();
            return await httpResponseConfiguration.Get("http://localhost:5223/configurations");
        }

        public static async Task<IList<string>> GetAliveMachines()
        {
            var configurations = await GetConfiguration();
            var configuration = configurations.FirstOrDefault();
            IHttpResponseWrapper<IList<string>> httpResponseAliveRemoteMachines = new HttpResponseWrapper<IList<string>>();
            return await httpResponseAliveRemoteMachines.Get($"http://localhost:5084/remoteMachines/{configuration.FromIPAddress}/{configuration.ToIPAddress}");
        }

        public static async Task<string> GetPhoenixVersion(string remoteConfiguredIpMachine)
        {
            IHttpResponseWrapper<string> remoteMachineVersionInfoService = new HttpResponseWrapper<string>();
            var phoenixVersionRestApi = $"http://localhost:5218/remoteMachinePhoenixVersion/{remoteConfiguredIpMachine}";
            return await remoteMachineVersionInfoService.Get(phoenixVersionRestApi);
        }

        public static async Task<string> GetFWVersion(string remoteConfiguredIpMachine)
        {
            IHttpResponseWrapper<string> remoteMachineVersionInfoService = new HttpResponseWrapper<string>();
            var phoenixVersionRestApi = $"http://localhost:5218/remoteMachineFWVersion/{remoteConfiguredIpMachine}";
            return await remoteMachineVersionInfoService.Get(phoenixVersionRestApi);
        }

        public static async Task<List<RemoteMachine>> GetExistingCRUDRemoteMachines()
        {
            IHttpResponseWrapper<List<RemoteMachine>> toolsInformationCRUDServiceRead = new HttpResponseWrapper<List<RemoteMachine>>();
            return await toolsInformationCRUDServiceRead.Get("http://localhost:5271/remoteMachines");
        }

        public static async Task<string> AddRemoteMachine(RemoteMachine newRemoteMachine)
        {
            IHttpResponseWrapper<RemoteMachine> toolsInformationCRUDServiceWrite = new HttpResponseWrapper<RemoteMachine>();
            return await toolsInformationCRUDServiceWrite.Post("http://localhost:5271/remoteMachines", newRemoteMachine);
        }

        public static async Task<string> RemoveRemoteMachine(RemoteMachine remoteMachineToDelete)
        {
            IHttpResponseWrapper<RemoteMachine> toolsInformationCRUDServiceWrite = new HttpResponseWrapper<RemoteMachine>();
            return await toolsInformationCRUDServiceWrite.Delete($"http://localhost:5271/remoteMachines/{remoteMachineToDelete.Id}");
        }

        public static async Task<string> ChangeRemoteMachine(RemoteMachine newRemoteMachine)
        {
            IHttpResponseWrapper<RemoteMachine> toolsInformationCRUDServiceWrite = new HttpResponseWrapper<RemoteMachine>();
            return await toolsInformationCRUDServiceWrite.Put("http://localhost:5271/remoteMachines", newRemoteMachine);
        }
    }
}
