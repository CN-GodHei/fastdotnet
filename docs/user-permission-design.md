# Fastdotnet 框架用户与权限模块设计方案 (V2 - 分离用户体系)

## 1. 概述

本方案旨在设计一个灵活、可扩展的用户管理和权限控制系统，支持主框架和插件的双重使用场景。系统需要支持后台管理模块和用户使用模块的权限分离，并确保插件可以方便地集成和使用权限系统。

**核心变更 (V2)**: 本版本采纳了后台管理员 (`AdminUser`) 与前台应用用户 (`AppUser`) 在数据层面物理分离的设计，以获得更彻底的职责隔离和安全性。

## 2. 设计目标

1.  **模块化设计**：用户和权限模块作为核心功能，可以被主框架和插件共同使用。
2.  **物理分离**: 后台管理与前台用户的 **数据、接口、服务** 完全分离。
3.  **灵活的权限模型**：支持基于角色的访问控制（RBAC）和细粒度权限控制。
4.  **插件友好**：插件可以方便地注册和使用权限系统，并明确其权限归属（管理端/用户端）。
5.  **可扩展性**：易于添加新的权限类型和控制策略。

## 3. 核心概念

### 3.1 用户（User）
- **后台管理员 (AdminUser)**: 系统的操作和维护人员，在 `FdAdminUser` 表中管理。
- **前台应用用户 (AppUser)**: 应用的最终用户，在 `FdAppUser` 表中管理。

### 3.2 角色（Role）
- 权限的集合体，分为“后台角色”和“前台角色”。
- 可以分配给对应的用户类型。
- 支持角色继承和层级关系。

### 3.3 权限（Permission）
- 系统中最小的权限单位，同样需要明确归属（管理端/用户端）。
- 可以是API访问权限、菜单权限、数据权限等。

### 3.4 菜单（Menu）
- 系统功能菜单项，分为“后台菜单”和“前台菜单”。
- 与前端路由和权限关联。

### 3.5 权限代码规范 (Permission Code Convention)
为保证权限代码的清晰、唯一和可维护性，强烈建议遵循统一的命名规范。
- **命名格式**: 推荐采用 `模块.资源.操作` 的三段式格式。
  - `admin.user.read`
  - `app.profile.update`
- **插件权限**: 插件的权限代码应以插件ID为前缀，确保全局唯一。例如：`PluginA.settings.update`。
- **避免魔法字符串**: 建议为每个模块的核心权限创建一个静态常量类，避免在代码中硬编码权限字符串。

## 4. 数据模型设计

### 4.1 后台管理员表 (FdAdminUser)
为系统操作员设计，注重管理和安全属性。
```csharp
/// <summary>
/// 后台管理员表
/// </summary>
public class FdAdminUser : BaseEntity, ISoftDelete
{
    /// <summary>
    /// 登录用户名 (必须唯一)
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// 哈希后的密码
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// 真实姓名
    /// </summary>
    public string FullName { get; set; }

    /// <summary>
    /// 管理员邮箱
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    public string Phone { get; set; }

    /// <summary>
    /// 账户是否激活
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 最后登录时间
    /// </summary>
    public DateTime? LastLoginTime { get; set; }

    /// <summary>
    /// 最后登录IP
    /// </summary>
    public string? LastLoginIp { get; set; }
}
```

### 4.2 前台应用用户表 (FdAppUser)
为应用的终端用户设计，注重个人资料和状态。
```csharp
/// <summary>
/// 前台应用用户表
/// </summary>
public class FdAppUser : BaseEntity, ISoftDelete
{
    /// <summary>
    /// 登录用户名 (可选, 可能使用手机或邮箱登录)
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// 哈希后的密码
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// 邮箱 (可用于登录, 必须唯一)
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 手机号 (可用于登录, 必须唯一)
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// 用户昵称
    /// </summary>
    public string Nickname { get; set; }

    /// <summary>
    /// 用户头像URL
    /// </summary>
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// 账户状态 (0:正常, 1:待验证, 2:已禁用)
    /// </summary>
    public int Status { get; set; } = 0;

    /// <summary>
    /// 注册时间
    /// </summary>
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 最后登录时间
    /// </summary>
    public DateTime? LastLoginTime { get; set; }
}
```

### 4.3 角色表 (FdRole)
```csharp
public class FdRole : BaseEntity, ISoftDelete
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public string Category { get; set; } // 角色类别: "Admin" 或 "User"
    public long? ParentId { get; set; } // 父级角色ID，用于支持角色层级
    public bool IsSystem { get; set; } // 是否系统角色
}
```

### 4.4 权限表 (FdPermission)
```csharp
public class FdPermission : BaseEntity, ISoftDelete
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public string Module { get; set; } // 所属模块（主框架/插件标识）
    public PermissionType Type { get; set; } // 权限类型（菜单/API/数据等）
    public string Category { get; set; } // 权限分类: "Admin" 或 "User"
}
```

### 4.5 用户角色关联表
由于用户表分离，关联表也必须分离。

**后台管理员角色关联表 (FdAdminUserRole)**
```csharp
public class FdAdminUserRole : BaseEntity
{
    public long AdminUserId { get; set; }
    public long RoleId { get; set; }
}
```

**前台用户角色关联表 (FdAppUserRole)**
```csharp
public class FdAppUserRole : BaseEntity
{
    public long AppUserId { get; set; }
    public long RoleId { get; set; }
}
```

### 4.6 角色权限关联表 (FdRolePermission)
```csharp
public class FdRolePermission : BaseEntity
{
    public long RoleId { get; set; }
    public long PermissionId { get; set; }
}
```

### 4.7 角色层级与权限继承
通过在 `FdRole` 中增加 `ParentId` 字段，系统可以支持角色层级或角色组。权限继承规则需要明确：
- **继承规则**: 子角色自动继承其所有父级角色的权限。
- **权限检查**: 在进行权限验证时，系统需要递归检查用户所拥有的角色及其所有父级角色链的权限总和。

## 5. 权限类型设计

### 5.1 API权限
- 控制对API接口的访问
- 基于路由路径和HTTP方法进行控制

### 5.2 菜单权限
- 控制前端菜单的显示
- 与前端路由相关联

### 5.3 数据权限
- 控制用户可以访问的数据范围，是权限系统中的一个高级但关键的功能。
- 支持行级权限（过滤数据行）和列级权限（隐藏特定字段）。

#### 实现思路
1.  **规则定义与存储**:
    - 在 `FdPermission` 表中增加一个 `Rule` 字段（如 `TEXT` 或 `JSON` 类型），用于存储数据权限的具体规则。
    - **行级规则示例**: `{"Table": "Orders", "Condition": "CreatorId == ${user.id} OR DepartmentId IN ${user.departmentIds}"}`
    - **列级规则示例**: `{"Table": "Users", "HiddenColumns": ["Salary", "PhoneNumber"]}`
2.  **规则自动应用**:
    - **与ORM集成**: 数据权限的实现需要与ORM框架（如SqlSugar）深度集成。通过实现一个全局查询筛选器（Global Query Filter），在每次数据库查询时，自动解析当前用户的权限规则。
    - **动态拼接SQL**: 该筛选器会根据规则动态地将条件注入到SQL的 `WHERE` 子句（行级权限）或从 `SELECT` 子句中移除字段（列级权限）。

### 5.4 操作权限
- 控制特定按钮或操作的可见性
- 用于前端元素的显示控制

## 7. 鉴权方式设计

### 7.1 统一认证，差异授权
- 使用统一的认证机制（如JWT），但为Admin和User颁发包含不同标识的Token。
- 通过JWT中的 `category` 声明来区分用户类型，调用不同的授权逻辑。

### 7.2 JWT Token 设计
```json
// Admin Token 示例
{
  "sub": "admin_user_id",
  "name": "admin_username",
  "roles": ["super_admin", "user_manager"],
  "category": "Admin", // 关键区分字段
  "exp": 1234567890
}

// User Token 示例
{
  "sub": "app_user_id",
  "name": "user_nickname",
  "roles": ["vip_member"],
  "category": "User", // 关键区分字段
  "exp": 1234567890
}
```
#### 优化建议：减小Token体积
- **简化Token**: 只在JWT中存放关键信息，如 `userId`, `roles`, `category`。
- **后端查询**: 当请求到达后端时，后端服务根据 `userId` 和 `category` 从高速缓存（如Redis）中获取完整的权限列表。

### 7.3 鉴权中间件实现
鉴权中间件需要读取 `category` 声明，并委托给对应的服务进行权限检查。

### 7.4 角色和权限分类

#### 管理员角色
- **系统管理员 (SuperAdmin)**: 一个内置的、拥有所有管理权限的特殊角色。应在系统初始化时创建。
- **用户管理员**: 拥有用户管理相关权限。

#### 用户角色
- **普通用户**: 拥有基本User权限。
- **VIP用户**: 拥有额外User权限。

## 8. 代码结构设计

物理分离的设计与分离的用户表天然契合。

### 8.1 物理分离（推荐）

#### 控制器示例
```csharp
// 后台管理控制器
namespace Fastdotnet.WebApi.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [Authorize] // 管理端授权策略
    public class UsersController : ControllerBase // 注意: 此控制器管理 FdAdminUser
    {
        private readonly IAdminUserService _adminUserService;
        // ...
    }
}

// 用户端控制器
namespace Fastdotnet.WebApi.Controllers.Api
{
    [Route("api/[controller]")]
    [Authorize] // 用户端授权策略
    public class ProfileController : ControllerBase // 注意: 此控制器管理 FdAppUser
    {
        private readonly IAppUserService _appUserService;
        // ...
    }
}
```
**注**: 上述代码中的 `UserDto` 将映射自 `FdAdminUser`，而 `UserProfileDto` 将映射自 `FdAppUser`。

## 9. 接口设计策略

接口的分离策略与用户表分离的设计完全对应。

### 9.1 后台管理接口（Admin API）
- **说明**: 此组API用于管理 `FdAdminUser` 实体及相关的后台资源。
- **示例**: `/api/admin/users`, `/api/admin/roles`

### 9.2 用户使用接口（User API）
- **说明**: 此组API用于管理当前登录的 `FdAppUser` 实体及其相关资源。
- **示例**: `/api/profile`, `/api/settings`

## 10. 插件权限集成方案

插件在注册权限和菜单时，必须明确指定其 `Category` 是 `Admin` 还是 `User`，以确保集成到正确的管理体系中。

## 13. 安全考虑

### 13.1 密码安全
- 对 `FdAdminUser` 和 `FdAppUser` 的密码都必须使用强哈希算法存储。

### 13.2 审计日志
- 记录关键的、与安全相关的操作日志，用于问题追踪和安全审计。
- **建议记录的关键事件**: 认证与会话、权限与角色变更、用户与资源操作、插件管理。

### 13.3 用户模拟 (Impersonation)
用户模拟是一个高级管理功能，允许管理员临时以其他用户的身份访问系统。
- **权限控制**: 需要一个独立的、高权限的权限点来控制此功能，例如 `system.admin.impersonate`。
- **安全实现**: 必须有明确的界面标识、严格的审计日志和安全的会话隔离。

## 14. API设计

### 14.1 用户管理API（后台）
- **说明**: 此组API用于管理 `FdAdminUser` 实体。
```
GET    /api/admin/users              # 获取管理员列表
POST   /api/admin/users              # 创建管理员
```

### 14.2 用户个人API（用户端）
- **说明**: 此组API用于管理当前登录的 `FdAppUser` 实体。
```
GET    /api/profile            # 获取当前用户资料
PUT    /api/profile            # 更新当前用户资料
```

(其他API设计章节内容与原文档类似，此处省略)

## 17. 总结

本方案（V2）采纳了后台管理员和前台用户在数据层、服务层和接口层完全分离的设计。这种设计最大化了系统的安全性、隔离性和职责清晰度，但也对开发和维护提出了更高的要求。

**核心优势**:
- **高安全性**: 后台和前台用户数据完全隔离。
- **职责清晰**: 每个模块只处理一种类型的用户。

**需要注意的复杂性**:
- **身份统一问题**: 如果同一个自然人需要两种身份，必须注册两个独立账户。
- **代码量增加**: 认证、授权、管理等逻辑都需要实现两套。

该方案为需要高度安全隔离的复杂系统提供了一个坚实的设计基础。