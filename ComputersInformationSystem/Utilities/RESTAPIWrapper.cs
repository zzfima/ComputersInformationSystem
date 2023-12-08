namespace Utilities
{
    public static class RESTAPIWrapper
    {
        //private const string CONFIGURATION_SERVICE_URL = "http://configuration-sqlite-crud-service-container:80/configurations";
        private const string CONFIGURATION_SERVICE_URL = "http://172.22.148.100:5200/configurations";

        #region ConfigurationSqliteCRUDService REST API
        public static async Task<IList<Configuration>> GetConfigurationAsync()
        {
            IHttpResponseWrapper<IList<Configuration>> httpResponseConfiguration = new HttpResponseWrapper<IList<Configuration>>();
            return await httpResponseConfiguration.Get(CONFIGURATION_SERVICE_URL);
        }
        #endregion

        #region IPsSQLiteCRUDService REST API
        public static async Task<List<IP>> GetIPsFromDBAsync(Configuration configuration)
        {
            IHttpResponseWrapper<List<IP>> ipsCRUDServiceRead = new HttpResponseWrapper<List<IP>>();
            return await ipsCRUDServiceRead.Get($"{configuration.IPsSqliteCRUDServiceURL}IPs");
        }

        public static async Task<string> SentIPToDBAsync(Configuration configuration, IP newIP)
        {
            IHttpResponseWrapper<IP> ipCacheServiceWrite = new HttpResponseWrapper<IP>();
            return await ipCacheServiceWrite.Post($"{configuration.IPsSqliteCRUDServiceURL}IP", newIP);
        }
        #endregion

        #region RemoteMachinesSQLiteCRUDService REST API
        public static async Task<List<RemoteMachine>> GetExistingDBRemoteMachinesAsync(Configuration configuration)
        {
            IHttpResponseWrapper<List<RemoteMachine>> toolsInformationCRUDServiceRead = new HttpResponseWrapper<List<RemoteMachine>>();
            return await toolsInformationCRUDServiceRead.Get($"{configuration.RemoteMachinesSqliteCRUDServiceURL}remoteMachines");
        }
        public static async Task<RemoteMachine> GetExistingDBRemoteMachineByIPAsync(Configuration configuration, string ip)
        {
            IHttpResponseWrapper<RemoteMachine> toolsInformationCRUDServiceRead = new HttpResponseWrapper<RemoteMachine>();
            return await toolsInformationCRUDServiceRead.Get($"{configuration?.RemoteMachinesSqliteCRUDServiceURL}remoteMachineByIP/{ip}");
        }

        public static async Task<string> AddNewRemoteMachineToSqliteDBAsync(Configuration configuration, RemoteMachine newRemoteMachine)
        {
            IHttpResponseWrapper<RemoteMachine> toolsInformationCRUDServiceWrite = new HttpResponseWrapper<RemoteMachine>();
            return await toolsInformationCRUDServiceWrite.Post($"{configuration?.RemoteMachinesSqliteCRUDServiceURL}remoteMachine", newRemoteMachine);
        }

        public static async Task<string> RemoveRemoteMachineFromDBAsync(Configuration configuration, RemoteMachine remoteMachineToDelete)
        {
            IHttpResponseWrapper<RemoteMachine> toolsInformationCRUDServiceWrite = new HttpResponseWrapper<RemoteMachine>();
            return await toolsInformationCRUDServiceWrite.Delete($"{configuration?.RemoteMachinesSqliteCRUDServiceURL}remoteMachine/{remoteMachineToDelete.Id}");
        }

        public static async Task<string> ChangeRemoteMachineOnSqliteDBAsync(Configuration configuration, RemoteMachine remoteMachine)
        {
            IHttpResponseWrapper<RemoteMachine> toolsInformationCRUDServiceWrite = new HttpResponseWrapper<RemoteMachine>();
            return await toolsInformationCRUDServiceWrite.Put($"{configuration?.RemoteMachinesSqliteCRUDServiceURL}remoteMachine", remoteMachine);
        }
        #endregion

        #region MQAliveIPGatherProducerService REST API
        public static async Task<string> PublishIPToIPGatherAsync(Configuration configuration, string ip)
        {
            IHttpResponseWrapper<string> messageQueueService = new HttpResponseWrapper<string>();
            return await messageQueueService.Post($"{configuration.MQAliveIPGatherProducerServiceURL}Publish/{ip}", ip);
        }
        #endregion

        #region MQVersionGatherProducerService REST API
        public static async Task<string> PublishIPToMQVersionGatherProducerServiceAsync(Configuration configuration, string ip)
        {
            IHttpResponseWrapper<string> messageQueueService = new HttpResponseWrapper<string>();
            return await messageQueueService.Post($"{configuration.MQVersionGatherProducerServiceURL}Publish/{ip}", ip);
        }
        #endregion

        #region RemoteMachinesNeo4jCRUDService REST API
        public static async Task<string> AddNewRemoteMachineToNeo4jDBAsync(Configuration configuration, RemoteMachine newRemoteMachine)
        {
            IHttpResponseWrapper<RemoteMachine> toolsInformationCRUDServiceWrite = new HttpResponseWrapper<RemoteMachine>();
            return await toolsInformationCRUDServiceWrite.Post($"{configuration?.RemoteMachinesNeo4jCRUDServiceURL}remoteMachine", newRemoteMachine);
        }
        #endregion

        #region CacheService REST API
        public static async Task<IP> GetIPFromCacheAsync(Configuration configuration, string address)
        {
            IHttpResponseWrapper<IP> ipCacheServiceRead = new HttpResponseWrapper<IP>();
            return await ipCacheServiceRead.Get($"{configuration?.CacheServiceURL}ip/{address}");
        }

        public static async Task<string> SentIPToCacheAsync(Configuration configuration, IP ip)
        {
            IHttpResponseWrapper<IP> ipCacheServiceWrite = new HttpResponseWrapper<IP>();
            return await ipCacheServiceWrite.Post($"{configuration?.CacheServiceURL}ip/{ip.Address}/{ip.IsAlive}", ip);
        }
        #endregion

        public static async Task<string> SentLogAsync(Configuration configuration, string sender, string message)
        {
            IHttpResponseWrapper<string> logServiceWrite = new HttpResponseWrapper<string>();
            return await logServiceWrite.Post($"{configuration.LoggingServiceURL}log/{sender}/{message}", sender);
        }
    }
}
