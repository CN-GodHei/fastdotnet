# Fastdotnet HybridCache 使用指南

## 1. 概述

Fastdotnet集成了Microsoft的HybridCache，这是一个结合了本地内存缓存和分布式缓存的高性能缓存解决方案。它提供了两级缓存机制：
- **本地缓存**：使用MemoryCache，提供最快的访问速度
- **分布式缓存**：支持Redis或MemoryCache，确保多实例间的数据一致性

## 2. 配置

在 `appsettings.json` 中配置缓存设置：

```json
{
  "CacheSettings": {
    "CacheType": "MemoryCache", // 或 "Redis"
    "RedisConnectionString": "localhost:6379",
    "RedisInstanceName": "Fastdotnet:",
    "LocalCacheExpirationMinutes": 5,
    "DistributedCacheExpirationMinutes": 30
  }
}
```

## 3. 核心组件

### 3.1 IHybridCacheService接口

```csharp
public interface IHybridCacheService
{
    /// <summary>
    /// 获取或创建缓存项
    /// </summary>
    Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, HybridCacheEntryOptions options = null, string[] tags = null);

    /// <summary>
    /// 设置缓存项
    /// </summary>
    Task SetAsync<T>(string key, T value, HybridCacheEntryOptions options = null, string[] tags = null);

    /// <summary>
    /// 获取缓存项
    /// </summary>
    Task<T> GetAsync<T>(string key);

    /// <summary>
    /// 移除缓存项
    /// </summary>
    Task RemoveAsync(string key);

    /// <summary>
    /// 根据标签移除缓存项
    /// </summary>
    Task RemoveByTagAsync(string tag);
}
```

### 3.2 CacheResultAttribute特性

用于控制器方法的缓存特性：

```csharp
[HttpGet("{id}")]
[CacheResult(ExpirationSeconds = 600)]
[CacheTag("user")]
[CacheTag("profile")]
public async Task<ActionResult<UserDto>> GetUser(int id)
{
    var user = await _userService.GetUserAsync(id);
    return Ok(user);
}
```

## 4. 使用方式

### 4.1 控制器层使用（特性方式）

```csharp
[ApiController]
[Route("api/[controller]")]
[CacheTag("products")]
public class ProductController : ControllerBase
{
    [HttpGet]
    [CacheResult(ExpirationSeconds = 300)]
    [CacheTag("product-list")]
    public async Task<ActionResult<List<ProductDto>>> GetProducts(
        [FromQuery] int categoryId, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10)
    {
        var products = await _productService.GetProductsAsync(categoryId, page, pageSize);
        return Ok(products);
    }
}
```

### 4.2 服务层使用（直接注入）

```csharp
public class UserService : IUserService
{
    private readonly IHybridCacheService _cacheService;
    private readonly IUserRepository _userRepository;

    public UserService(IHybridCacheService cacheService, IUserRepository userRepository)
    {
        _cacheService = cacheService;
        _userRepository = userRepository;
    }

    public async Task<UserDto> GetUserAsync(int userId)
    {
        var key = $"user:{userId}";
        var tags = new[] { "user", "data" };

        return await _cacheService.GetOrCreateAsync(key, async () =>
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user?.ToDto();
        }, null, tags);
    }
}
```

### 4.3 仓储层使用（直接注入）

```csharp
public class UserRepository : Repository<User>, IUserRepository
{
    private readonly IHybridCacheService _cacheService;

    public UserRepository(ISqlSugarClient db, IHybridCacheService cacheService) : base(db)
    {
        _cacheService = cacheService;
    }

    public async Task<User> GetByIdAsync(int id)
    {
        var key = $"db:user:{id}";
        var tags = new[] { "user", "database" };

        return await _cacheService.GetOrCreateAsync(key, async () =>
        {
            return await _db.Queryable<User>().InSingleAsync(id);
        }, null, tags);
    }
}
```

## 5. 缓存键生成机制

缓存键会根据以下规则自动生成：
1. 控制器类的全名
2. 方法名
3. 参数的哈希值（确保相同参数生成相同键，不同参数生成不同键）

例如：
- `Fastdotnet.WebApi.Controllers.Admin.UserController.GetUser(id=123)` 生成键类似于：`Fastdotnet.WebApi.Controllers.Admin.UserController:GetUser?abcd1234`
- `Fastdotnet.WebApi.Controllers.Admin.UserController.GetUser(id=456)` 生成键类似于：`Fastdotnet.WebApi.Controllers.Admin.UserController:GetUser?ef567890`

## 6. 缓存标签管理

使用标签可以方便地批量管理缓存：

```csharp
// 移除所有标记为"user"的缓存项
await _cacheService.RemoveByTagAsync("user");
```

## 7. HybridCacheEntryOptions 说明

```csharp
var options = new HybridCacheEntryOptions
{
    // 分布式缓存过期时间（保存在Redis等分布式缓存中）
    Expiration = TimeSpan.FromMinutes(30),
    
    // 本地缓存过期时间（保存在MemoryCache中）
    LocalCacheExpiration = TimeSpan.FromMinutes(5),
    
    // 缓存标志
    Flags = HybridCacheEntryFlags.None
};
```

## 8. 使用范围和限制

### 8.1 控制器层
- **可以使用**：`CacheResultAttribute`特性
- **执行时机**：在Action Filter阶段执行
- **安全性**：JWT鉴权等认证机制会在缓存检查之前执行

### 8.2 服务层
- **不能使用**：`CacheResultAttribute`特性
- **推荐方式**：直接注入`IHybridCacheService`使用

### 8.3 仓储层
- **不能使用**：`CacheResultAttribute`特性
- **推荐方式**：直接注入`IHybridCacheService`使用

## 9. 安全性说明

使用`CacheResultAttribute`时，认证和授权机制会正常工作：

1. **JWT认证在Middleware层执行**，早于任何MVC过滤器
2. **Authorize属性在Authorization Filter阶段执行**，早于Action Filter
3. **即使从缓存返回数据，用户也已经通过了认证**

请求处理管道顺序：
```
1. Middleware管道（JWT认证等）
2. MVC过滤器管道
   ├── Authorization Filters（授权过滤器）← JWT鉴权在这里执行
   ├── Resource Filters（资源过滤器）
   ├── Action Filters（动作过滤器）← CacheResultAttribute在这里执行
   ├── Exception Filters（异常过滤器）
   └── Result Filters（结果过滤器）
```

## 10. 最佳实践

### 10.1 分层缓存策略
```
┌─────────────────┐
│   控制器层缓存   │ ← API响应缓存
├─────────────────┤
│   服务层缓存     │ ← 业务逻辑结果缓存
├─────────────────┤
│   仓储层缓存     │ ← 数据库查询结果缓存
└─────────────────┘
```

### 10.2 标签管理
- 使用有意义的标签名称
- 按业务领域分组标签
- 便于批量清理相关缓存

### 10.3 过期时间设置
- 热点数据：较短的过期时间
- 冷数据：较长的过期时间
- 根据业务需求调整

### 10.4 错误处理
- 缓存失败时回退到正常流程
- 记录缓存相关日志
- 监控缓存命中率

## 11. 注意事项

1. 默认使用MemoryCache，如需使用Redis，请修改配置文件中的CacheType为"Redis"并提供连接字符串
2. 缓存键是自动生成的，基于控制器全名、方法名和参数，确保唯一性
3. 标签用于分类管理缓存，便于批量操作
4. 在插件中也可以直接使用IHybridCacheService服务
5. 缓存过期时间可以单独为每个方法设置
6. GetAsync方法在缓存不存在时会返回类型的默认值