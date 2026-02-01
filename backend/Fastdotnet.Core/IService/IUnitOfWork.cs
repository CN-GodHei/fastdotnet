
namespace Fastdotnet.Core.IService
{
    // 1. 定义抽象
    public interface IUnitOfWork
    {
        Task BeginTransactionAsync(CancellationToken ct = default);
        Task CommitAsync(CancellationToken ct = default);
        Task RollbackAsync(CancellationToken ct = default);

        // 可选：暴露通用查询方法（如 QuerySingleOrDefaultAsync）
        //Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? parameters = null, CancellationToken ct = default) where T : class, new();
    }
}
