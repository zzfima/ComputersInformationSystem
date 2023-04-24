namespace Utilities
{
    public interface IHttpResponseWrapper<T>
    {
        Task<T> GetResponse(string restApiAddress);
        Task<string> Post(string restApiAddress, T content);
    }
}