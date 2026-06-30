# Fastdotnet: The Enterprise Dynamic Plugin Base for the AI Agent Era

**[🇨🇳 中文文档](README.md)** | **[🇺🇸 English Documentation](README.en.md)**

> 🏪 **Plugin Marketplace is Now Live!** Massive ready-to-use plugins to accelerate outsourcing project delivery by 70%+ → [Browse Now](https://fastdotnet.top/marketplace)

##  Why Choose Fastdotnet?

Struggling with complex business module decoupling? Worried about how to achieve uninterrupted updates?

Fastdotnet provides an out-of-the-box **dynamic plugin loading solution** based on .NET 10 and AssemblyLoadContext technology, achieving true runtime hot-swapping. It's not just a development framework; it's an **efficient base for modular orchestration of AI Agent applications**.

### 💡 Core Value: Build Enterprise Applications Like Building Blocks

<div align="center">
  <img src="docs/.vitepress/public/images/plugin-marketplace.png" alt="Fastdotnet Plugin Management Interface" width="800" />
  <p><em>Visual plugin management interface supporting online purchase, one-click installation, and hot updates</em></p>
</div>

#### 🏪 Plugin Marketplace Ecosystem
- **Official Plugin Library**: Payment, workflow, social login, object storage and other common features out of the box
- **Third-Party Developers**: Professional plugins contributed by the community, covering various industry scenarios
- **Rapid Project Delivery**: Outsourcing projects can be delivered in "weeks" instead of "months" through plugin combinations
- **💰 Developer Value Reuse**: Develop once, sell multiple times - Package common functionalities as plugins for the marketplace, transforming from "project-based" to "productized"

### What Problems Does It Solve?
- **Enterprise SaaS Multi-Tenancy**: Different customers need different functional modules, traditional monolithic applications are difficult to configure flexibly
- **Dynamic Feature Expansion**: Dynamically load/unload feature modules without restarting services during rapid business iterations
- **Team Collaboration & Isolation**: Multiple teams independently develop functional modules, avoiding dependency conflicts and mutual interference
- **Lightweight Microservices Alternative**: Provides microservice-like modularity without complex distributed infrastructure
- **🚀 Rapid Project Delivery**: Massive ready-made plugins from the marketplace enable quick assembly and deployment, shortening delivery cycles by 70%+

### How Does It Differ from ABP / Orchard Core?

| Feature | Fastdotnet | ABP Framework | Orchard Core |
|---------|-----------|---------------|-------------|
| **Plugin Isolation** | ✅ True isolation via AssemblyLoadContext, no DLL version conflicts | ❌ Shared application domain, risk of dependency conflicts | ⚠️ Tenant-based isolation, higher complexity |
| **Hot-Swapping** | ✅ Runtime dynamic loading/unloading, no restart required | ⚠️ Requires application restart to load new modules | ✅ Supports modularity but configuration is cumbersome |
| **Frontend Integration** | ✅ Deep qiankun micro-frontend integration, synchronized frontend-backend delivery | ❌ Need to implement frontend modularization separately | ❌ Primarily focuses on backend CMS capabilities |
| **Architecture Complexity** | 🟢 Lightweight layered architecture, low learning curve | 🔴 Heavy DDD architecture, steep learning curve | 🟡 Medium complexity, suitable for CMS scenarios |
| **AI Integration Potential** | 🚀 Designed for AI Agent orchestration, easy to mount agent plugins | ⚠️ Requires additional adaptation | ❌ No native support |
| **Use Cases** | SME applications, SaaS platforms, rapid prototyping, AI app base | Large-scale enterprise complex business systems | Content Management Systems (CMS) |

---

## ⚡ 3-Minute Quick Start

Experience the complete Fastdotnet ecosystem in under 1 minute via Docker without complex environment configuration.

### Method 1: One-Click Docker Startup (Recommended)

```
# Clone repository
git clone https://github.com/CN-GodHei/fastdotnet.git
cd fastdotnet/docker

# Start all services (Backend + Frontend + Database)
docker-compose up -d
```

Once started, visit:
- **Admin Panel**: http://localhost:8080 (`superadmin` / `123456`)
- **Application**: http://localhost:8081 (`admintest` / `123456`)

### Method 2: Minimal Code Example

If you want to experience plugin loading directly in code, it only takes a few lines:

``csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddFastdotnet(); // Register framework core services

var app = builder.Build();
app.UseFastdotnetPlugins(); // Automatically scan and load all plugins in the plugins directory

app.Run();
```

> 💡 **Tip**: For more details on plugin development, please check the [Plugin Development Guide](https://docs.fastdotnet.top).

---

## 💬 Community & Support

**👥 QQ Group: [779454817](https://qm.qq.com/cgi-bin/qm/qr?k=779454817&jump_from=webapi)**

**🗣️ GitHub Discussions**: [Join Now](https://github.com/CN-GodHei/fastdotnet/discussions) - Public Q&A, feature suggestions, plugin discussions (Recommended)

> Click the link to join the QQ group for technical support, development experience sharing, and feedback

---

## 🌐 Official Websites

- **Official Site**: [https://fastdotnet.top](https://fastdotnet.top) - Framework introduction and feature showcase
- **🏪 Plugin Marketplace**: [https://fastdotnet.top/marketplace](https://fastdotnet.top/marketplace) - Browse massive plugins, quickly assemble projects
- **Official Documentation**: [https://docs.fastdotnet.top](https://docs.fastdotnet.top) - Complete development documentation

## 🎮 Live Demo

Experience Fastdotnet instantly without installation:

- **Admin Panel Demo**: http://admin.demo.fastdotnet.top/
  - Username: `superadmin` / Password: `123456`
  - Features: System management, user management, permission configuration, etc.
  - 💡 Tip:  use `mktest` / `123456` to experience plugin marketplace download and authorization features

- **Application Demo**: http://app.demo.fastdotnet.top/
  - Username: `admintest` / Password: `123456`
  - Features: Business operations, data display, etc.

> 💡 Note: Demo environment data is reset periodically. Do not store important data.

## 🤝 Sponsors

Thanks to the following sponsors for supporting the Fastdotnet project:

<div align="center">
  <a href="https://www.netzyun.com/" target="_blank">
    <img src="sponsorship/网臻云_logo.png" alt="NetZyun" width="200" />
  </a>
  <p><strong>NetZyun</strong> - Our demo server sponsor</p>
</div>

---

## 🔒 Security Testing Partnership

**We are looking for professional cybersecurity companies to collaborate!**

As an enterprise-grade development framework, Fastdotnet places high importance on system security. We invite professional security testing teams to conduct comprehensive security audits and penetration tests.

### Collaboration Scope
- ✅ Framework core code security audit
- ✅ Plugin system isolation mechanism verification
- ✅ Authentication & authorization system (OIDC/JWT/RBAC) security testing
- ✅ API interface security assessment
- ✅ Data encryption and transmission security inspection
- ✅ Common vulnerability scanning (SQL injection, XSS, CSRF, etc.)

### Collaboration Model
- **Free Testing**: Provide free testing environment and technical support for security companies
- **Brand Exposure**: Display your company logo and links in README, official website, and documentation
- **Report Publication**: Publicly release test reports to enhance industry influence for both parties
- **Long-term Partnership**: Establish continuous security collaboration mechanisms

### Contact Us
📧 Email: yunnanzuyuankeji@163.com  
💬 QQ Group: 779454817

> 💡 If you are a professional company or team in the security field, please contact us to jointly enhance Fastdotnet's security!

---

## 📚 Official Documentation

Visit [https://docs.fastdotnet.top](https://docs.fastdotnet.top) for complete documentation including:
- Quick Start Guide
- Architecture Design
- Backend Development Tutorial
- Frontend Development Tutorial
- Plugin Development Guide
- API Reference

---

## Project Overview

Fastdotnet is a modular development framework based on **.NET 10**, featuring a plugin-based architecture with hot-swapping capabilities, offering high scalability and flexibility. The framework base adopts a layered architecture design, allowing free choice of architectural patterns (such as DDD, Clean Architecture, etc.) for plugin development.

### ✨ Core Features

#### 📦 Implemented Features in Core Framework

> 💡 **All features below are built into the framework base, ready to use out of the box!**

##### 🔐 Authentication & Authorization System
- ✅ **OIDC/SSO Single Sign-On**: Based on OpenIddict, supports cross-application unified authentication
- ✅ **JWT Token Authentication**: Standard JSON Web Token mechanism
- ✅ **RBAC Permission Management**: Role-based access control with button-level permissions
- ✅ **Multi-Tenancy Isolation**: Comprehensive tenant data isolation mechanism
- ✅ **CAPTCHA System**: Graphical CAPTCHA, slider verification and other validation methods
- ✅ **Blacklist Management**: IP blacklist, user blacklist
- ✅ **Rate Limiting**: API request frequency limiting

##### 👥 User Management System
- ✅ **Administrator Management**: Complete CRUD + role assignment
- ✅ **Application User Management**: B2C user management with social account binding support
- ✅ **User Layout Configuration**: Personalized interface layout saving
- ✅ **Password Reset**: Secure password recovery and reset process
- ✅ **User Todo Tasks**: Task assignment and approval workflows

##### 📋 System Management Features
- ✅ **Menu Management**: Dynamic menu configuration with multi-level nesting support
- ✅ **Role Management**: Role creation and permission assignment
- ✅ **Dictionary Management**: Data dictionary maintenance with tree structure support
- ✅ **System Configuration**: Global parameter configuration and management
- ✅ **Notifications**: In-app message notification system
- ✅ **Workbench**: Personalized dashboard

##### 💾 Data Storage & Caching
- ✅ **SqlSugar ORM**: High-performance ORM supporting CodeFirst/DbFirst
- ✅ **SQL Execution Logging**: Auto-captures every SQL execution (with parameter values) via SqlSugar AOP hooks, stored in `log_sql_execution` daily-partitioned table for slow-query analysis and request tracing. (Configurable via `SqlSugar.EnableSqlExecutionLogging` setting)
- ✅ **Multi-Database Support**: SQLite, MySQL, PostgreSQL, SQL Server, Dameng
- ✅ **Hybrid Cache System**: Local memory + Redis dual-layer cache architecture
- ✅ **Repository Pattern**: Standardized data access abstraction layer
- ✅ **Transaction Management**: Complete transaction support

##### 🎨 Frontend Capabilities
- ✅ **Vue 3 + TypeScript**: Modern frontend technology stack
- ✅ **Element Plus UI**: Enterprise-grade component library integration
- ✅ **qiankun Micro-Frontends**: Dynamic sub-application loading and independent deployment
- ✅ **Dynamic Route Generation**: Automatically generate frontend routes from backend configuration
- ✅ **Responsive Layout**: Perfect support for desktop and mobile
- ✅ **Theme Switching**: Light/dark themes, custom theme colors
- ✅ **Internationalization**: Multi-language support (i18n)
- ✅ **Tab Management**: Multi-tab browsing with KeepAlive caching

##### 🔌 Plugin System Core
- ✅ **AssemblyLoadContext Isolation**: True plugin runtime isolation
- ✅ **Dynamic Hot-Swapping**: Runtime loading/unloading without restart
- ✅ **Plugin Lifecycle Management**: Initialize, start, stop, unload
- ✅ **Independent Dependency Context**: Independent DLL version management per plugin
- ✅ **Inter-Plugin EventBus**: Low-coupling cross-plugin communication → [Details](docs/05-插件开发/事件总线.md)
- ✅ **Plugin Branch Gateway**: Private request processing pipeline for plugins
- ✅ **Reverse Proxy Escape Hatch**: Support for plugins with their own web servers
- ✅ **Static Resource Mapping**: Automatic handling of plugin wwwroot files
- ✅ **Containerized DI**: Independent LifetimeScope based on Autofac
- ✅ **File System Monitoring**: Automatic detection of plugin changes

##### 🛡️ Security & Protection
- ✅ **Sensitive Data Masking**: Automatic identification and masking of sensitive information
- ✅ **Replay Attack Prevention**: Request replay detection and protection
- ✅ **Encrypted Transmission**: AES+RSA encryption for requests/responses
- ✅ **Global Exception Handling**: Unified exception capture - BusinessException returns 422, uncontrolled exceptions persist to DB + return 500
- ✅ **Business Operation Logs**: Complete operation audit trail (operator, IP, headers/body, status code, elapsed time), auto-partitioned by day
- ✅ **Exception Log Persistence**: Uncontrolled exceptions auto-write to `log_exception` table with exception type, stack trace, request path
- ✅ **Debug Logs**: Business tracing logs for development, support Key identifier and RequestId correlation
- ✅ **Graceful Shutdown**: Elegant shutdown mechanism

##### 📡 Real-Time Communication
- ✅ **SignalR Hub**: WebSocket real-time communication support
- ✅ **Universal Hub**: UniversalHub supporting multiple message types
- ✅ **Plugin SignalR**: Plugin-level real-time communication endpoints
- ✅ **Connection State Management**: Auto-reconnect and state monitoring
- ✅ **Framework Push Notifications**: Reliable event push based on Outbox pattern, supporting SignalR real-time push and Webhook callbacks → [Details](docs/05-插件开发/对外推送.md)

##### 🔧 Development Tools
- ✅ **Swagger/OpenAPI**: Auto-generated API documentation
- ✅ **Code Generator**: One-click CRUD code generation
- ✅ **Plugin CLI Tool**: Quick plugin template creation
- ✅ **Hot Reload Support**: Automatic code change detection during development
- ✅ **Unified Response Format**: Standardized API response structure
- ✅ **Model Validation**: Automatic parameter validation and error messages

##### 🚀 Performance Optimization
- ✅ **HybridCache**: Local + distributed dual-layer caching
- ✅ **Cache Attribute Annotation**: @CacheResult decorator simplifies cache usage
- ✅ **Batch Operation Optimization**: Support for batch updates, deletions, etc.
- ✅ **Paginated Queries**: Efficient paginated data queries

#### 🏪 Plugin Marketplace Ecosystem (⭐ Core Advantage)

> 💡 **Framework Built-in Capabilities + Plugin Marketplace Extensions = Complete Enterprise Solution**

| Category | Framework Built-in (✅ Implemented) | Plugin Marketplace (🔌 Extensible) |
|----------|-------------------------------------|------------------------------------|
| **Authentication** | OIDC/SSO, JWT, RBAC, Multi-tenancy | Social Login (WeChat/QQ/DingTalk), LDAP Integration |
| **User Management** | Admin, App Users, Role Permissions | Membership Levels, Points System, Real-name Verification |
| **Data Storage** | SqlSugar ORM, Multi-database, Cache | Data Sync, ETL Tools, Data Backup |
| **Payment System** | - | Alipay, WeChat Pay, UnionPay |
| **Workflow** | Basic Todo Tasks | Elsa Workflow Engine, Visual Designer |
| **Object Storage** | - | Aliyun OSS, Tencent COS, MinIO, AWS S3 |
| **Notifications** | In-app Messages | Email, SMS, WeCom, DingTalk Bot |
| **Rich Text** | - | TinyMCE, Quill, WangEditor and more |
| **Reporting** | - | Data Reports, Chart Analysis, Print Templates |
| **Industry Plugins** | - | CRM, ERP, OA, E-commerce, Education solutions |

- **Official精选 Plugins**: Payment systems, workflow engines, social login, object storage and other common features, ready to use
- **Third-Party Developer Ecosystem**: Professional plugins contributed by the community, covering e-commerce, CRM, ERP, OA and other industry scenarios
- **Rapid Project Delivery**: Outsourcing projects can be delivered in "weeks" instead of "months" through plugin combinations, efficiency improved by 70%+
- **💰 Developer Value Reuse**: Develop once, sell multiple times - Extract common functionalities from projects, package as plugins for the marketplace to achieve continuous revenue
- **From Project-Based to Productized**: Break free from "writing code from scratch for each project", create reusable product assets
- **One-Click Installation**: Visual plugin management interface supporting online purchase, automatic download, one-click installation
- **Version Management**: Comprehensive plugin version control supporting smooth upgrades and rollbacks

#### 🔌 Plugin System
- **True Runtime Hot-Swapping**: Plugin isolation based on AssemblyLoadContext, supporting dynamic loading/unloading
- **Independent Dependency Management**: Each plugin has its own dependency context, completely solving DLL version conflicts
- **Micro-Frontend Integration**: Backend plugins seamlessly integrate with qiankun micro-frontends for synchronized delivery
- **Inter-Plugin Communication**: Built-in EventBus for low-coupling cross-plugin联动
- **Flexible Routing**: Support for plugin branch gateways, reverse proxy escape hatches, automatic static resource mapping

#### 🔐 Authentication & Authorization
- **OIDC/SSO Support**: Open Identity Connect protocol based on OpenIddict, supporting single sign-on
- **JWT Authentication**: Standard JSON Web Token authentication mechanism
- **Fine-Grained Permissions**: Role-based access control (RBAC) with button-level permissions
- **Multi-Tenancy**: Comprehensive tenant isolation mechanism

#### 💾 Data Access
- **SqlSugar ORM**: High-performance ORM framework supporting CodeFirst/DbFirst
- **Multi-Database Adaptation**: SQLite, MySQL, PostgreSQL, SQL Server, Dameng, etc.
- **Hybrid Cache System**: Local memory + distributed cache (Redis) dual-layer architecture
- **Repository Pattern**: Standardized data access abstraction layer

#### 🎨 Frontend Architecture
- **Vue 3 + TypeScript**: Modern frontend technology stack
- **Element Plus UI**: Enterprise-grade component library
- **qiankun Micro-Frontends**: Support for dynamic sub-application loading and independent deployment
- **Dynamic Routing**: Automatically generate frontend routes from backend configuration
- **Responsive Design**: Perfect support for desktop and mobile

#### 🛠️ Developer Experience
- **Code Generator**: One-click CRUD code generation to improve development efficiency
- **Swagger/OpenAPI**: Auto-generated API documentation
- **Plugin CLI Tool**: Quick plugin template creation
- **Hot Reload Support**: Automatic code change detection during development

#### 📊 Business Features
- **Workflow Engine**: Integrated Elsa Workflow with visual process design
- **Payment System**: Multi-channel payment support including Alipay, WeChat Pay
- **Social Login**: Third-party login via WeChat, QQ, DingTalk, etc.
- **Object Storage**: Aliyun OSS, Tencent COS, MinIO, AWS S3, etc.
- **Rich Text Editor**: Multiple rich text editing solutions integrated
- **Notifications**: Multi-channel notifications including in-app messages, email, SMS

### Core Technology Stack

**Backend:**
- 🚀 **.NET 10** - Latest version of .NET runtime
- 🗄️ **SqlSugar ORM** - High-performance ORM framework
- 🔐 **OpenIddict / OIDC** - Open Identity Connect protocol supporting SSO
- 🔑 **JWT** - JSON Web Token authentication
- 💉 **Autofac** - Dependency injection container
- 📝 **Swagger/OpenAPI** - API documentation

**Frontend:**
- ⚡ **Vue 3 + TypeScript** - Modern frontend framework
- 🎨 **Element Plus** - Enterprise-grade UI component library
- 🏗️ **Vite** - Ultra-fast build tool
- 🔌 **qiankun** - Micro-frontend framework

## Project Structure
```
├── backend/
│   ├── Fastdotnet.Core/           # Core domain layer
│   │   ├── Attributes/            # Attribute definitions
│   │   ├── Constants/             # Constant definitions
│   │   ├── Controllers/           # Controller base classes
│   │   ├── Dtos/                  # Data transfer objects
│   │   ├── Entities/              # Entity models
│   │   ├── Enum/                  # Enum definitions
│   │   ├── Exceptions/            # Exception handling
│   │   ├── Extensions/            # Extension methods
│   │   ├── Hubs/                  # SignalR Hubs
│   │   ├── IService/              # Service interfaces
│   │   ├── Middleware/            # Middleware
│   │   ├── Plugin/                # Plugin system core interfaces
│   │   ├── Service/               # Service implementations
│   │   ├── Settings/              # Configuration settings
│   │   ├── Utils/                 # Utility classes
│   │   └── Version/               # Version information
│   ├── Fastdotnet.Orm/            # ORM data access layer
│   ├── Fastdotnet.Plugin.Contracts/ # Plugin contract layer
│   ├── Fastdotnet.Plugin.Shared/  # Plugin shared components
│   ├── Fastdotnet.Service/        # Application service layer
│   │   ├── IService/              # Service interfaces
│   │   └── Service/               # Service implementations
│   ├── Fastdotnet.WebApi/         # Web API layer
│   │   ├── Controllers/           # API controllers
│   │   ├── Middleware/            # Middleware
│   │   └── plugins/               # Plugin deployment directory
│   └── Plugins/                   # Plugin source code directory
│       ├── PluginA/               # Sample plugin
│       └── README.md              # Plugin system documentation
├── Web/                           # Frontend projects
│   ├── fastdotnet-admin/          # Admin panel frontend
│   ├── fastdotnet-app/            # Application frontend
│   └── plugin-a-admin/            # Plugin A admin frontend
```

## Architecture

### Layered Architecture
1. **Fastdotnet.Core**
   - Core foundation layer providing common infrastructure and底层 support
   - Defines system-level constants, enums, exception handling and utility classes
   - Houses core contracts and extension point interfaces for the plugin system
   - Maintains zero external dependencies ensuring framework stability and purity

2. **Fastdotnet.Orm**
   - Data access infrastructure layer based on SqlSugar
   - Provides generic repository pattern and database connection management
   - Supports multi-database adaptation (SQLite, MySQL, PostgreSQL, Dameng, etc.)

3. **Fastdotnet.Plugin.Contracts**
   - Plugin communication contract layer defining standard interfaces for host-plugin interaction
   - Contains core definitions including plugin lifecycle (IPlugin), permission extensions and storage contexts
   - Serves as the sole dependency for plugin development ensuring loose coupling with the base

4. **Fastdotnet.Plugin.Shared**
   - Plugin runtime shared components providing AOT adapters and common utilities
   - Assists plugins in safely accessing system resources within isolated contexts

5. **Fastdotnet.Service**
   - Business logic implementation layer housing core functional modules of the base
   - Encapsulates common business services like user management, permission control, system configuration
   - Coordinates ORM data operations with upper-layer API requests

6. **Fastdotnet.WebApi**
   - Host startup layer responsible for application assembly and execution
   - Integrates authentication/authorization, global middleware, Swagger and plugin loading engine
   - Provides unified HTTP entry point and dynamic route distribution capabilities

### Dependencies
- Fastdotnet.Core: No dependencies on other projects
- Fastdotnet.Orm: Depends on Core
- Fastdotnet.Plugin.Contracts: Depends on Core
- Fastdotnet.Plugin.Shared: Depends on Contracts
- Fastdotnet.Service: Depends on Core, Orm
- Fastdotnet.WebApi: Depends on Core, Orm, Plugin.Contracts, Plugin.Shared, Service

## Plugin System

### 📊 Architecture Visualization

We used [Graphify](https://github.com/graphify-dev/graphify) to generate a complete code knowledge graph to help you better understand the system architecture:

- **Interactive Graph**: View [graph.html](graphify-out/graph.html) - Zoomable, searchable complete relationship diagram
- **Tree Structure**: View [GRAPH_TREE.html](graphify-out/GRAPH_TREE.html) - Hierarchical module organization view
- **Detailed Report**: View [GRAPH_REPORT.md](graphify-out/GRAPH_REPORT.md) - Includes community analysis, core node statistics, etc.

> 💡 Tip: The knowledge graph displays 4,349 nodes and 10,146 relationship edges, divided into 294 functional communities - the best tool for understanding project structure.

### Core Advantages
Fastdotnet's plugin architecture is designed for high scalability, dynamism and enterprise SaaS scenarios, with the following significant advantages:

1. **Strong Isolation**
   - Uses `AssemblyLoadContext` (ALC) technology to create independent runtime contexts for each plugin
   - Completely solves DLL version conflict issues - different plugins can use different versions of the same library without interference
   - Plugin crashes don't affect host stability, achieving true fault isolation

2. **Dynamic Hot-Swapping**
   - Supports dynamic loading, starting, stopping and unloading of plugins at runtime without server restart
   - Combined with file system monitoring (FileSystemWatcher) for automatic plugin file detection and hot updates
   - Real-time plugin state updates with graceful shutdown and resource cleanup support

3. **Micro-Frontends Integration**
   - Backend plugins seamlessly integrate with qiankun micro-frontends
   - Main frontend dynamically loads remote plugin Vue/React sub-applications based on metadata from backend
   - Achieves complete decoupling and synchronized delivery of frontend-backend functional modules

4. **Flexible Gateway & Routing**
   - **Plugin Branch Gateway**: Allows plugins to register private request processing pipelines for "sub-application"-like independent logic
   - **Reverse Proxy Escape Hatch**: Supports plugins with their own mini web servers (e.g., Kestrel), transparent forwarding via YARP for heterogeneous tech stack compatibility
   - **Static Resource Mapping**: Automatically handles file serving under plugin `wwwroot`, perfect support for SPA History mode

5. **Scoped DI Container**
   - Allocates independent `LifetimeScope` for each plugin based on Autofac
   - Highly autonomous service registration within plugins while safely interacting with host or other plugins via interfaces
   - Avoids global container pollution, improving system maintainability

6. **Event-Driven Communication**
   - Built-in powerful `IEventBus` supporting publish/subscribe模式 across plugins
   - Low-coupling联动 between plugins via events, facilitating complex business flow systems

### Technology Stack
The plugin system is built on the following core technologies:

- .NET Core dynamic loading mechanism (AssemblyLoadContext)
- Dependency Injection (Microsoft.Extensions.DependencyInjection and Autofac)
- ASP.NET Core MVC (for plugin controller registration and routing)
- File System Monitoring (FileSystemWatcher for hot loading)
- Reflection (for type discovery and service registration)

### Plugin Directory Structure
```
backend/Plugins/
├── PluginA/                # Plugin A project
│   ├── Controllers/        # Controllers
│   ├── IService/           # Service interfaces
│   ├── Services/           # Service implementations
│   ├── PluginA.cs          # Plugin main class
│   ├── PluginA.csproj      # Project file
│   └── plugin.json         # Plugin configuration file
└── README.md               # Plugin system documentation
```

### Plugin Interface
Each plugin must implement the `IPlugin` interface, which defines basic plugin lifecycle methods:

```csharp
public interface IPlugin
{
    string PluginId { get; }     // Unique plugin identifier
    string Name { get; }         // Plugin name
    string Version { get; }      // Plugin version
    PluginLifecycleState LifecycleState { get; } // Plugin lifecycle state
    
    Task InitializeAsync(IServiceProvider serviceProvider); // Plugin initialization
    Task StartAsync();          // Plugin start
    Task StopAsync();           // Plugin stop
    Task UnloadAsync(IServiceProvider serviceProvider); // Cleanup before plugin unload
    void ConfigureServices(ContainerBuilder builder); // Configure plugin services
}
```

### Plugin Configuration File
Each plugin needs a `plugin.json` configuration file describing basic plugin information:

```json
{
  "id": "PluginA",
  "name": "Fastdotnet Demo Plugin",
  "description": "Demo plugin showcasing basic plugin system functionality",
  "version": "1.0.0",
  "enabled": true,
  "author": "Fastdotnet Team",
  "dependencies": [],
  "tags": ["demo", "example"]
}
```

### Plugin Development Guidelines
1. Plugin project naming: Use meaningful names
2. Each plugin must implement `IPlugin` interface or inherit `PluginBase` class (recommended)
   - `PluginBase` provides default lifecycle state management, simplifying plugin development
   - After inheriting `PluginBase`, only override hook methods like `OnInitializeAsync`, `OnStartAsync`
3. Plugins must configure unique `<PluginId>` property in .csproj file
4. Plugins should remain independent, avoiding mutual dependencies
5. Plugins can include their own controllers, services and models
6. Plugin controllers should inherit from `GenericDtoControllerBase` or `AppGenericDtoControllerBase`

### Hot-Swapping Mechanism
- Plugin manager handles plugin loading, unloading and lifecycle management
- Supports runtime dynamic loading and unloading of plugins
- File system monitoring automatically detects plugin changes
- Real-time plugin state updates with graceful shutdown support

## Cache System

### Technology Stack
The cache system is built on Microsoft HybridCache with the following features:

- **Dual-Layer Cache Architecture**: Local memory cache + distributed cache (Redis/MemoryCache)
- **High Performance**: Local cache provides fastest access speed
- **High Availability**: Distributed cache ensures data consistency across multiple instances
- **Flexible Configuration**: Supports both MemoryCache and Redis backends

### Core Components
1. **IHybridCacheService** - Core cache service interface
2. **CacheResultAttribute** - Controller method cache attribute
3. **CacheTagAttribute** - Cache tag attribute

### Usage

#### Controller Level Usage
``csharp
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    [HttpGet("{id}")]
    [CacheResult(ExpirationSeconds = 600)]
    [CacheTag("user")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        // Actual business logic
    }
}
```

#### Service Layer Usage
``csharp
public class UserService : IUserService
{
    private readonly IHybridCacheService _cacheService;
    
    public async Task<UserDto> GetUserAsync(int userId)
    {
        var key = $"user:{userId}";
        return await _cacheService.GetOrCreateAsync(key, async () =>
        {
            // Fetch data from database
        });
    }
}
```

### Security Notes
The caching mechanism is fully compatible with JWT authentication - authentication checks always execute before cache checks, ensuring security.

For more details, please refer to [Cache Usage Guide](docs/HybridCache使用指南.md)

## Design Principles
1. **Dependency Inversion Principle (DIP)**
   - High-level modules should not depend on low-level modules; both should depend on abstractions
   - Abstractions should not depend on details; details should depend on abstractions

2. **Single Responsibility Principle (SRP)**
   - Each class should have a single responsibility
   - Each module should have high cohesion and low coupling

3. **Open-Closed Principle (OCP)**
   - Open for extension, closed for modification
   - Support feature expansion through plugin system

4. **Interface Segregation Principle (ISP)**
   - Clients should not depend on interfaces they don't need
   - Interfaces should be small and precise

## Quick Start

### 🏪 Method 1: Quick Assembly via Plugin Marketplace (Recommended)

Suitable for outsourcing projects and rapid prototyping without coding from scratch:

#### 📋 Feature Requirements Checklist

**What features does your project need?**

| Feature Requirement | Framework Built-in | Requires Plugin |
|--------------------|-------------------|-----------------|
| User Login, Permission Management | ✅ Built-in | - |
| Menu, Role, Dictionary Management | ✅ Built-in | - |
| Multi-Tenancy Data Isolation | ✅ Built-in | - |
| API Documentation, Code Generation | ✅ Built-in | - |
| WeChat/Alipay Payment | - | 🔌 Payment Plugin |
| Workflow Approval | Basic Todo | 🔌 Elsa Workflow Plugin |
| Social Account Login | - | 🔌 Social Login Plugin |
| File Upload & Storage | - | 🔌 OSS Plugin (Aliyun/Tencent/etc.) |
| Email/SMS Notifications | In-app Messages | 🔌 Notification Plugin |
| Rich Text Editing | - | 🔌 Rich Text Plugin |
| Data Reporting | - | 🔌 Report Plugin |

**Steps:**
1. **Visit Plugin Marketplace**: [https://fastdotnet.top/marketplace](https://fastdotnet.top/marketplace)
2. **Browse/Search Plugins**: Select required feature plugins based on the table above
   - 💼 Payment Systems: Alipay, WeChat Pay integration
   - 📊 Workflow Engine: Visual process design
   - 🔐 Social Login: WeChat, QQ, DingTalk, etc.
   - ☁️ Object Storage: Aliyun OSS, Tencent COS, etc.
3. **One-Click Installation**: Purchase and install plugins directly from admin panel
4. **Configuration**: Simple configuration according to plugin documentation
5. **Rapid Delivery**: Combine multiple plugins for quick project completion

> 💡 **Case Study**: An outsourcing company used Fastdotnet + Plugin Marketplace to shorten a CRM project that would have taken 2 months down to just 2 weeks!

> 💰 **Developer Opportunity**: You can also extract common functionalities from your projects, package them as plugins for the marketplace, achieving "develop once, sell multiple times" value reuse!

> ✨ **Framework Completeness**: Fastdotnet is not an "empty shell" framework - it already includes complete enterprise application foundational capabilities (authentication, permissions, user management, system management, etc.). You can directly build business logic on top of these features. For additional industry-specific features, extend via the plugin marketplace.

### Environment Requirements
- .NET 10.0 SDK or higher
- Supported Operating Systems: Windows, Linux, macOS

### Build Project
```
# Clone repository
git clone https://github.com/yourusername/fastdotnet.git
cd fastdotnet/backend

# Build solution
dotnet build

# Run WebApi project
cd Fastdotnet.WebApi
dotnet run
```

### Develop Plugins
1. Create new plugin project under `backend/Plugins` directory
2. Implement `IPlugin` interface or inherit `PluginBase` class (recommended), and configure `<PluginId>` in .csproj
3. Build plugin and copy output to `backend/Fastdotnet.WebApi/plugins` directory

#### 💰 Plugin Development & Publishing - Achieve Value Reuse

**Traditional Development vs Fastdotnet Plugin Model:**

| Dimension | Traditional Project-Based | Fastdotnet Plugin Model |
|-----------|--------------------------|------------------------|
| **Code Reuse** | ❌ Redevelop for each project | ✅ Develop once, reuse multiple times |
| **Revenue Model** | 💸 One-time project income | 💰 Continuous sales commissions |
| **Asset Accumulation** | 📉 Code闲置 after project ends | 📈 Plugins become appreciating product assets |
| **Market Reach** | 🎯 Single client | 🌍 All platform users |
| **Maintenance Cost** | 🔧 Maintain multiple projects separately | ⚡ Unified version, centralized maintenance |

**Publishing Process:**
1. **Develop Plugin**: Extract common functionalities from projects, develop plugins according to specifications
2. **Test & Validate**: Thoroughly test plugin functionality and compatibility in local environment
3. **Submit for Review**: Submit plugin to official marketplace for quality and security review
4. **List for Sale**: List after approval, set your own pricing and licensing model
5. **Continuous Revenue**: Earn commissions on each sale, achieving "passive income"
6. **Iterate & Optimize**: Continuously optimize based on user feedback to improve sales and reputation

👉 **Success Story**: A developer packaged the "WeChat Login" functionality from a project as a plugin, sold 500+ copies after listing, generating over ¥100,000 in passive income!

👉 Detailed Guide: [Plugin Development Docs](https://docs.fastdotnet.top) | [Marketplace Seller Guide](https://fastdotnet.top/marketplace/seller)

#### Example: Inheriting PluginBase Class
``csharp
public class MyPlugin : PluginBase
{
    public override string PluginId => "your-plugin-id";
    public override string Name => "My Plugin";
    public override string Version => "1.0.0";
    
    protected override Task OnInitializeAsync(IServiceProvider serviceProvider)
    {
        // Initialization logic
        return Task.CompletedTask;
    }
    
    protected override Task OnStartAsync()
    {
        // Start logic
        return Task.CompletedTask;
    }
    
    public override void ConfigureServices(ContainerBuilder builder)
    {
        // Register services
    }
}
```

### Frontend Development
The project includes multiple frontend applications:
- `Web/fastdotnet-admin/`: Admin panel frontend application
- `Web/fastdotnet-app/`: User application frontend
- `Web/plugin-a-admin/`: Plugin A admin frontend

## Contributing
Welcome to contribute code, report issues or suggest improvements. Please follow these steps:

1. Fork the project
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Create a Pull Request

Before contributing, please read:
- 📜 [Code of Conduct](CODE_OF_CONDUCT.en.md) - Understand our community standards and expectations
- 🔒 [Security Policy](SECURITY.en.md) - How to responsibly report security vulnerabilities

## Related Links

- 🌐 **Official Website**: [https://fastdotnet.top](https://fastdotnet.top)
- 🏪 **🔥 Plugin Marketplace**: [https://fastdotnet.top/marketplace](https://fastdotnet.top/marketplace) - Browse massive plugins, quickly assemble projects
  - 💼 **Official Plugins**: Payment, workflow, social login, OSS and other common features
  - 👨‍💻 **Third-Party Plugins**: Professional plugins contributed by community developers
  - 🚀 **Rapid Delivery**: Outsourcing projects shorten delivery cycles by 70%+ through plugin combinations
  - 💰 **Developer Value Reuse**: Develop once, sell multiple times, transform "project code" into "product assets"
- 📚 **Official Documentation**: [https://docs.fastdotnet.top](https://docs.fastdotnet.top)
- 🎮 **Live Demos**:
  - Admin Panel: http://admin.demo.fastdotnet.top/ (`superadmin` / `123456`, or use `mktest` / `123456` for plugin features)
  - Application: http://app.demo.fastdotnet.top/ (`admintest` / `123456`)
- 📦 **Dependency Lists**:
  - [Backend Dependencies](DEPENDENCIES_BACKEND.en.md) - View third-party libraries and licenses used in backend
  - [Frontend Dependencies](DEPENDENCIES_FRONTEND.en.md) - View third-party libraries and licenses used in frontend
- 🐛 **Issue Tracking**: [GitHub Issues](https://github.com/CN-GodHei/fastdotnet/issues)
- 📧 **Official Email**: yunnanzuyuankeji@163.com
- 💬 **QQ Group**: 779454817

---

## Contact

- 📧 **Official Email**: yunnanzuyuankeji@163.com
- 💬 **QQ Group**: 779454817

## License
This project is licensed under the MIT License - see the LICENSE file for details

---

## 🏆 Why Choose Fastdotnet?

### Is It Right for Your Scenario?

✅ **If You Are:**
- SaaS platform developer needing customized features for different customers
- Enterprise internal system development team requiring modular collaborative development
- **Outsourcing Company/Independent Developer**: Need rapid project delivery to reduce development costs
- Startup requiring rapid iteration and flexible scaling
- Want to dynamically update features without service restart
- **💰 Tech Entrepreneur/Developer**: Want to achieve value reuse via plugin marketplace, transforming "project code" into "product assets" for continuous revenue

✅ **You Will Get:**
- 🚀 **50%+ Development Efficiency Improvement**: Code generator + plugin templates + comprehensive documentation
- 🔧 **60% Maintenance Cost Reduction**: True module isolation makes problem localization simpler
- 💰 **Infrastructure Cost Savings**: Monolithic architecture performance with microservice flexibility
- 🎯 **Faster Business Response**: New feature deployment shortened from "days" to "hours"
- 🏪 **70% Shorter Project Delivery Cycle**: Quick assembly via plugin marketplace pre-built features
- 💵 **Developer Value Reuse**: Develop once, sell multiple times, transform "project code" into "product assets" for continuous passive income

### Technical Highlights Summary

1. **True Hot-Swapping**: Not simple module loading, but true isolation based on AssemblyLoadContext
2. **Frontend-Backend Integration**: Backend plugins automatically connect to frontend micro-apps without manual configuration
3. **Lightweight Architecture**: No forced DDD conventions, free to choose architecture patterns within plugins
4. **Enterprise Features**: OIDC/SSO, multi-tenancy, workflow, payment systems out of the box
5. **Active Community**: QQ Group 779454817 with continuous updates and maintenance
6. **🏪 Plugin Marketplace Ecosystem**: Official + third-party plugin library for quick project assembly
7. **💰 Developer Value Reuse**: Develop once, sell multiple times, from "project-based" to "productized" for continuous revenue

### Comparison with Other Frameworks

| Scenario | Recommended Solution |
|----------|---------------------|
| Large enterprise complex business, team size 100+ | ABP Framework |
| **SME, SaaS Platform, Rapid Iteration** | **Fastdotnet** ⭐ |
| Simple CRUD applications | ASP.NET Core Minimal API |
| Ultra-large-scale distributed systems | Microservices Architecture |

---

## 💰 Developer Value Reuse - From "Project-Based" to "Productized"

### Pain Points of Traditional Development Model

😩 **"Write Code from Scratch for Each Project"**
- Redevelop similar features (login, payment, permissions, etc.) for every project
- Code cannot be reused, repetitive labor, low efficiency
- Code becomes闲置 after project completion, unable to generate continuous value
- Income depends on new projects, lacking passive income sources

### Advantages of Fastdotnet Plugin Model

✨ **"Develop Once, Sell Multiple Times"**

```
During Project Development
    ↓
Extract Common Functional Modules
    ↓
Package as Fastdotnet Plugin
    ↓
List on Plugin Marketplace
    ↓
Continuous Sales + Earn Commissions 💰💰💰
```

#### Real Cases

**Case 1: WeChat Login Plugin**
- Developer A implemented WeChat login functionality in a project
- Packaged it as a plugin, listed on marketplace at ¥199
- Sold 500+ copies cumulatively, total revenue exceeding ¥100,000
- Low subsequent maintenance cost, almost pure profit

**Case 2: Data Export Plugin**
- Developer B created universal Excel/PDF export functionality
- Supports multiple formats and custom templates
- Became a popular plugin after listing, averaging 30+ sales per month
- Achieved stable passive income

**Case 3: Industry Solution Plugin Bundle**
- A team split core CRM system functionalities into multiple plugins
- Including: customer management, order processing, data analysis, etc.
- Bundle sales increased average order value to ¥2000+
- Also provided customization services, forming diversified revenue streams

#### How to Get Started?

1. **Identify Common Functionalities**: Look for reusable functional modules in your projects
   - Authentication/authorization, payment integration, notifications
   - Data import/export, report generation
   - Third-party API integrations (WeChat, Alipay, DingTalk, etc.)

2. **Package as Plugin**: Develop according to Fastdotnet plugin specifications
   - Refer to official documentation and sample plugins
   - Ensure good compatibility and stability

3. **List for Sale**: Submit to plugin marketplace for review
   - Write clear documentation
   - Set reasonable pricing and licensing model

4. **Continuously Optimize**: Iterate and upgrade based on user feedback
   - Fix bugs, add new features
   - Improve user experience, accumulate positive reviews

👉 **Get Started Now**: [Plugin Development Guide](https://docs.fastdotnet.top/05-%E6%8F%92%E4%BB%B6%E5%BC%80%E5%8F%91/%E5%88%9B%E5%BB%BA%E6%8F%92%E4%BB%B6.html) | [Marketplace Seller](https://fastdotnet.top/account/badges)

---

**🌟 If Fastdotnet has helped you, please Star to show your support!**

```
