using Newtonsoft.Json;

namespace Utilities
{
    internal class HttpResponseWrapper<T> : IHttpResponseWrapper<T>
    {
        public async Task<T> Get(string restApiAddress)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync(restApiAddress))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<T>(apiResponse);
                    }
                }
            }
            return default(T);
        }

        public async Task<string> Post(string restApiAddress, T content)
        {
            var serializedContent = JsonConvert.SerializeObject(content);
            StringContent httpContent = new StringContent(serializedContent, System.Text.Encoding.UTF8, "application/json");
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.PostAsync(restApiAddress, httpContent))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        return "Posted";
                    }
                }
            }
            return "Not Posted";
        }

        public async Task<string> Put(string restApiAddress, T content)
        {
            var serializedContent = JsonConvert.SerializeObject(content);
            StringContent httpContent = new StringContent(serializedContent, System.Text.Encoding.UTF8, "application/json");
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.PutAsync(restApiAddress, httpContent))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        return "Put";
                    }
                }
            }
            return "Not Put";
        }

        public async Task<string> Delete(string restApiAddress)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.DeleteAsync(restApiAddress))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return "Deleted";
                    }
                }
            }
            return "Not Deleted";
        }
    }
}