namespace Utilities
{
    public static class ConfigurationWarpper
    {
        public static async Task<IList<Configuration>> GetConfiguration()
        {
            IHttpResponseWrapper<IList<Configuration>> httpResponseConfiguration = new HttpResponseWrapper<IList<Configuration>>();
            return await httpResponseConfiguration.Get("http://localhost:5213/configurations");
        }
    }
}