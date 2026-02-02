
namespace Fastdotnet.Core.Services.App;

public class FdAppUserService : IFdAppUserService
{
    private readonly IServiceProvider _serviceProvider; // 保留，用于动态解析泛型 handler
    private readonly IRepository<FdAppUser> _fdAppUserRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStorageContext _storageContext; // 添加对IStorageContext的直接依赖

    public FdAppUserService(
        IServiceProvider serviceProvider,
        IRepository<FdAppUser> fdAppUserRepo,
        IUnitOfWork unitOfWork,
        IStorageContext storageContext) // 添加IStorageContext参数
    {
        _serviceProvider = serviceProvider;
        _fdAppUserRepo = fdAppUserRepo;
        _unitOfWork = unitOfWork;
        _storageContext = storageContext;
    }

    public async Task<FdAppUser?> GetFdAppUserAsync(string fdAppUserId, CancellationToken ct = default)
    {
        return await _fdAppUserRepo.GetByIdAsync(fdAppUserId, ct);
    }

    public async Task<(FdAppUser Base, TExtension? Extension)> GetFdAppUserWithExtensionAsync<TExtension>(
        string fdAppUserId,
        CancellationToken ct = default) where TExtension : class
    {
        var fdAppUser = await _fdAppUserRepo.GetByIdAsync(fdAppUserId, ct);
        if (fdAppUser == null) return (null!, default);

        TExtension? extension = default;

        // 从当前作用域解析 handler（不需要 CreateScope！）
        var handlerType = typeof(IFdAppUserExtensionHandler<>).MakeGenericType(typeof(TExtension)); // 使用插件系统定义的接口
        if (_serviceProvider.GetService(handlerType) is IFdAppUserExtensionHandler<TExtension> handler)
        {
            extension = await handler.LoadAsync(fdAppUserId, _storageContext, ct); // 使用_storageContext
        }

        return (fdAppUser, extension);
    }

    public async Task UpdateFdAppUserAsync(
        string fdAppUserId,
        Action<FdAppUser> updateAction,
        CancellationToken ct = default)
    {
        _unitOfWork.BeginTransactionAsync(ct); // 开始事务，不使用await

        var fdAppUser = await _fdAppUserRepo.GetByIdAsync(fdAppUserId, ct)
            ?? throw new KeyNotFoundException($"FdAppUser with ID {fdAppUserId} not found.");

        updateAction(fdAppUser);
        await _fdAppUserRepo.UpdateAsync(fdAppUser); // 不需要CancellationToken参数

        await _unitOfWork.CommitAsync(ct); // 提交事务
    }

    public async Task UpdateFdAppUserWithExtensionAsync<TExtension>(
        string fdAppUserId,
        Action<FdAppUser> updateBaseAction,
        TExtension extensionData,
        CancellationToken ct = default) where TExtension : class
    {
        if (extensionData == null)
            throw new ArgumentNullException(nameof(extensionData));

        _unitOfWork.BeginTransactionAsync(ct); // 开始事务，不使用await

        // 1. 更新主表
        var fdAppUser = await _fdAppUserRepo.GetByIdAsync(fdAppUserId, ct)
            ?? throw new KeyNotFoundException($"FdAppUser with ID {fdAppUserId} not found.");

        updateBaseAction(fdAppUser);
        await _fdAppUserRepo.UpdateAsync(fdAppUser); // 不需要CancellationToken参数

        // 2. 更新扩展表
        var handlerType = typeof(IFdAppUserExtensionHandler<>).MakeGenericType(typeof(TExtension)); // 使用插件系统定义的接口
        if (_serviceProvider.GetService(handlerType) is IFdAppUserExtensionHandler<TExtension> handler)
        {
            await handler.SaveAsync(fdAppUserId, extensionData, _storageContext, ct); // 使用正确的参数顺序
        }
        else
        {
            throw new InvalidOperationException(
                $"No extension handler registered for type '{typeof(TExtension).Name}'. " +
                $"Did you forget to register IFdAppUserExtensionHandler<{typeof(TExtension).Name}>?");
        }

        await _unitOfWork.CommitAsync(ct); // 提交事务
    }

    public async Task<string> CreateFdAppUserWithExtensionAsync<TExtension>(
        Action<FdAppUser> initBaseAction,
        TExtension extensionData,
        CancellationToken ct = default) where TExtension : class
    {
        if (extensionData == null)
            throw new ArgumentNullException(nameof(extensionData));

        _unitOfWork.BeginTransactionAsync(ct); // 开始事务，不使用await

        // 1. 创建主表
        var fdAppUser = new FdAppUser();
        initBaseAction(fdAppUser);
        await _fdAppUserRepo.InsertAsync(fdAppUser);
        var fdAppUserId = fdAppUser.Id; // SqlSugar 会回填 ID

        // 2. 创建扩展
        var handlerType = typeof(IFdAppUserExtensionHandler<>).MakeGenericType(typeof(TExtension)); // 使用插件系统定义的接口
        if (_serviceProvider.GetService(handlerType) is IFdAppUserExtensionHandler<TExtension> handler)
        {
            await handler.SaveAsync(fdAppUserId, extensionData, _storageContext, ct); // 使用正确的参数顺序
        }
        else
        {
            throw new InvalidOperationException(
                $"No extension handler registered for type '{typeof(TExtension).Name}'.");
        }

        await _unitOfWork.CommitAsync(ct); // 提交事务
        return fdAppUserId;
    }
}