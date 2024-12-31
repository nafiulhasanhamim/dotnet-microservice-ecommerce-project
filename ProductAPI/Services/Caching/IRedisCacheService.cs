namespace ProductAPI.Services.Caching
{
    public interface IRedisCacheService
    {
        Task<T?> GetDataAsync<T>(string key);
        void SetData<T>(string key, T data);
        void RemoveData(string key);
    }
}