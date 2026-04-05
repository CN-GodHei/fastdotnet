namespace Fastdotnet.Core.Extensibility.Users
{
    public interface IFdAppUserExtensionHandler<TData> where TData : class
    {
        Task SaveAsync(string userId, TData data, IStorageContext context, CancellationToken ct = default);
        Task<TData?> LoadAsync(string userId, IStorageContext context, CancellationToken ct = default);
    }
}