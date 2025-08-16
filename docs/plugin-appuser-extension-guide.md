# 插件化用户 (FdAppUser) 扩展指南

## 1. 概述

本指南旨在为插件开发者提供一套标准的、高性能的模式，用于扩展核心的 `FdAppUser` 用户实体。在插件化架构中，直接修改主框架的 `FdAppUser` 表是绝对禁止的。本指南将解决以下两个核心问题：

1.  **如何为用户增加自定义数据**：在不修改主表的前提下，为用户附加插件专属的字段。
2.  **如何查询组合后的数据**：高效地查询并返回同时包含主用户数据和插件自定义数据的信息。

推荐的黄金组合方案是： **扩展表 (Extension Tables) + 事件通知 (Event Notifications) + 插件独立API (Plugin-Specific API)**。

---

## 2. 数据存储与逻辑触发

此部分解决“如何存数据”和“如何响应主框架事件”的问题。

### 2.1 模式核心：一对一扩展表

插件不应修改 `FdAppUser` 表，而是应创建自己的扩展表，并与 `FdAppUser` 建立一对一的关联关系。

**工作原理:**

1.  当插件（例如 `PluginA`）需要为用户添加自定义字段（比如 `Level` 和 `Points`）时，它会创建一个属于自己的新表，例如 `PluginA_UserData`。
2.  这个新表的主键 `Id` **同时也是一个外键**，指向 `FdAppUser` 表的 `Id`。
3.  这样，`FdAppUser` 的每一行记录，都可以在 `PluginA_UserData` 表中拥有唯一的一行与之对应的扩展数据。

**示例 - PluginA 的数据模型 (`PluginA_UserData.cs`):**

```csharp
/// <summary>
/// 插件A的用户数据扩展表
/// </summary>
public class PluginA_UserData
{
    /// <summary>
    /// 主键，同时也是指向 FdAppUser.Id 的外键
    /// </summary>
    [Key] // 使用 [Key] 特性标记为主键
    public string Id { get; set; }

    /// <summary>
    /// 插件A自定义的字段：等级
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// 插件A自定义的字段：积分
    /// </summary>
    public decimal Points { get; set; }
}
```

### 2.2 模式核心：事件通知

当主框架创建了一个新用户时，插件需要一种机制来获知这一事件，以便在自己的扩展表中创建对应的记录。这通过发布和订阅事件来实现（推荐使用 `MediatR` 库）。

**工作流程:**

**1. 主框架定义并发布事件**

在主框架中定义一个事件，并在核心业务（如创建用户）完成后发布它。

```csharp
// 在主框架中定义事件
public class AppUserCreatedEvent : INotification
{
    public string UserId { get; }
    public string Nickname { get; }

    public AppUserCreatedEvent(string userId, string nickname)
    {
        UserId = userId;
        Nickname = nickname;
    }
}

// 在主框架的用户服务 (AppUserService.cs) 中发布事件
public class AppUserService
{
    private readonly IMediator _mediator;
    private readonly IRepository<FdAppUser> _userRepository;

    public async Task<FdAppUser> CreateUserAsync(CreateUserDto dto)
    {
        var newUser = new FdAppUser { Nickname = dto.Nickname };
        await _userRepository.InsertAsync(newUser);

        // 发布用户创建成功事件
        await _mediator.Publish(new AppUserCreatedEvent(newUser.Id, newUser.Nickname));

        return newUser;
    }
}
```

**2. 插件订阅并处理事件**

插件可以创建一个或多个“事件处理器”来订阅它感兴趣的事件。

```csharp
// 在 PluginA 的代码中创建一个事件处理器
public class AppUserCreatedHandler : INotificationHandler<AppUserCreatedEvent>
{
    private readonly IRepository<PluginA_UserData> _pluginDataRepository;

    public AppUserCreatedHandler(IRepository<PluginA_UserData> pluginDataRepository)
    {
        _pluginDataRepository = pluginDataRepository;
    }

    // 当 AppUserCreatedEvent 事件发生时，此方法会被自动调用
    public async Task Handle(AppUserCreatedEvent notification, CancellationToken cancellationToken)
    {
        // 在插件自己的表中创建一条对应的扩展数据记录
        var pluginData = new PluginA_UserData
        {
            Id = notification.UserId, // 使用主表用户的ID
            Level = 1,                // 设置初始等级
            Points = 0                // 设置初始积分
        };
        await _pluginDataRepository.InsertAsync(pluginData);
    }
}
```

通过以上两个模式的结合，插件便拥有了与主框架用户数据同步的、自己专属的、类型安全的数据存储。

---

## 3. 组合数据查询

此部分解决“如何高效查询组合数据”的问题。推荐的模式是 **由插件提供自己的查询API，并在后端实现中高效地组合数据**。

### 3.1 模式核心：插件API + 服务端高效组合

**工作流程:**

1.  **插件定义组合DTO**: 插件定义一个包含“主用户信息”和“插件扩展信息”的组合DTO。
2.  **插件创建独立API**: 插件创建自己的API控制器来提供组合查询接口。
3.  **高效实现**: 在接口的内部实现中，通过一次性批量查询来避免性能陷阱（如N+1查询）。

**示例代码:**

**1. 插件定义组合DTO (`PluginA_UserDto.cs`)**

```csharp
/// <summary>
/// 插件A的组合用户DTO
/// </summary>
public class PluginA_UserDto
{
    // --- 来自主框架 FdAppUser 的信息 ---
    public string Id { get; set; }
    public string Nickname { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime RegistrationDate { get; set; }

    // --- 来自插件扩展表 PluginA_UserData 的信息 ---
    public int Level { get; set; }
    public decimal Points { get; set; }
}
```

**2. 插件创建自己的API控制器 (`PluginAUsersController.cs`)**

```csharp
[Route("api/plugin-a/users")]
[ApiController]
public class PluginAUsersController : ControllerBase
{
    private readonly IAppUserService _appUserService; // 注入主框架的用户服务
    private readonly IRepository<PluginA_UserData> _pluginDataRepository; // 注入插件的仓储

    public PluginAUsersController(
        IAppUserService appUserService, 
        IRepository<PluginA_UserData> pluginDataRepository)
    {
        _appUserService = appUserService;
        _pluginDataRepository = pluginDataRepository;
    }

    [HttpGet]
    public async Task<PageResult<PluginA_UserDto>> GetUsersWithPluginData([FromQuery] PageQueryDto query)
    {
        // 1. 调用主框架服务，获取基础分页数据
        var baseUserPageResult = await _appUserService.GetUsersAsync(query);
        
        if (baseUserPageResult.Items.Count == 0) return new PageResult<PluginA_UserDto>();

        // 2. 从基础数据中提取用户ID列表
        var userIds = baseUserPageResult.Items.Select(u => u.Id).ToList();

        // 3. 使用 WHERE IN 一次性查询所有相关的插件扩展数据
        var pluginDataList = await _pluginDataRepository.GetListAsync(p => userIds.Contains(p.Id));

        // 4. 将扩展数据放入字典，方便O(1)复杂度快速查找
        var pluginDataDict = pluginDataList.ToDictionary(p => p.Id);

        // 5. 组合数据
        var combinedItems = baseUserPageResult.Items.Select(baseUser => 
        {
            pluginDataDict.TryGetValue(baseUser.Id, out var pluginData);
            return new PluginA_UserDto
            {
                Id = baseUser.Id,
                Nickname = baseUser.Nickname,
                AvatarUrl = baseUser.AvatarUrl,
                RegistrationDate = baseUser.RegistrationDate,
                Level = pluginData?.Level ?? 0,
                Points = pluginData?.Points ?? 0
            };
        }).ToList();

        // 6. 返回最终的、包含了完整信息的分页结果
        return new PageResult<PluginA_UserDto>(combinedItems, baseUserPageResult.Total);
    }
}
```

## 4. 总结

通过遵循本指南提出的模式，插件开发者可以实现：

- **高内聚，低耦合**: 插件的扩展逻辑和数据存储与主框架完全分离。
- **高性能**: 服务端组合查询避免了多次API调用和数据库N+1查询问题。
- **强类型与可维护性**: 插件数据存储在自己的规范化表中，保证了数据的完整性和可维护性。
- **强大的可扩展性**: 可以轻松地围绕用户创建、更新、删除等事件，构建出复杂的业务流程。
