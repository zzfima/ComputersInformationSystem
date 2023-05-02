namespace Utilities
{
    public interface IHttpResponseWrapper<T>
    {
        Task<T> Get(string restApiAddress);
        Task<string> Post(string restApiAddress, T content);
        Task<string> Put(string restApiAddress, T content);
        Task<string> Delete(string restApiAddress);
    }
}