# Fastdotnet 项目缓存功能说明

## 1. 概述

Fastdotnet项目集成了Microsoft HybridCache，提供了高性能的缓存解决方案。该缓存系统具有以下特点：

- **双层缓存架构**：本地内存缓存 + 分布式缓存
- **灵活配置**：支持MemoryCache和Redis两种后端
- **多层级使用**：控制器、服务层、仓储层均可使用
- **安全保证**：与JWT认证无缝集成

## 2. 核心组件

### 2.1 HybridCacheService
核心缓存服务实现，提供基本的缓存操作方法。

### 2.2 CacheResultAttribute
控制器方法缓存特性，使用简单便捷。

### 2.3 CacheTagAttribute
缓存标签特性，用于对缓存项进行分类管理。

## 3. 使用方式

### 3.1 控制器层缓存

```csharp
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    [HttpGet("{id}")]
    [CacheResult(ExpirationSeconds = 600)]
    [CacheTag("user")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        // 实际业务逻辑
    }
}
```

### 3.2 服务层缓存

```csharp
public class UserService : IUserService
{
    private readonly IHybridCacheService _cacheService;
    
    public async Task<UserDto> GetUserAsync(int userId)
    {
        var key = $"user:{userId}";
        return await _cacheService.GetOrCreateAsync(key, async () =>
        {
            // 从数据库获取数据
        });
    }
}
```

### 3.3 仓储层缓存

```csharp
public class UserRepository : Repository<User>
{
    private readonly IHybridCacheService _cacheService;
    
    public async Task<User> GetByIdAsync(int id)
    {
        var key = $"db:user:{id}";
        return await _cacheService.GetOrCreateAsync(key, async () =>
        {
            // 数据库查询
        });
    }
}
```

## 4. 配置说明

在`appsettings.json`中配置：

```json
{
  "CacheSettings": {
    "CacheType": "MemoryCache",
    "RedisConnectionString": "localhost:6379",
    "LocalCacheExpirationMinutes": 5,
    "DistributedCacheExpirationMinutes": 30
  }
}
```

## 5. 安全性

缓存机制与JWT认证完全兼容，认证检查始终在缓存检查之前执行，确保安全性。

## 6. 最佳实践

1. 根据数据重要性和访问频率设置合适的过期时间
2. 使用标签对缓存进行分类管理
3. 在多层同时使用缓存以获得最佳性能
4. 监控缓存命中率以优化性能