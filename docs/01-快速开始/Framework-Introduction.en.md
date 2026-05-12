# Framework Introduction

**[🇨🇳 中文文档](框架介绍.md)** | **[🇺🇸 English Documentation](Framework-Introduction.en.md)**

## 🎯 Framework Overview

**Fastdotnet** is an enterprise-grade modular development framework based on **.NET 10**, featuring a true plugin-based architecture with runtime hot-swapping capabilities.

### 💡 Core Value Proposition

> **Build Enterprise Applications Like Building Blocks** - Select functional plugins from the marketplace and quickly assemble a complete system

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

### How Does It Differ from ABP and Other Frameworks?

| Feature | Fastdotnet | ABP Framework |
|---------|-----------|---------------|
| **Plugin Isolation** | ✅ True isolation via AssemblyLoadContext, no DLL version conflicts | ❌ Shared application domain, risk of dependency conflicts |
| **Hot-Swapping** | ✅ Runtime dynamic loading/unloading, no restart required | ⚠️ Requires application restart to load new modules |
| **Frontend Integration** | ✅ Deep qiankun micro-frontend integration, synchronized frontend-backend delivery | ❌ Need to implement frontend modularization separately |
| **Architecture Complexity** | 🟢 Lightweight layered architecture, low learning curve | 🔴 Heavy DDD architecture, steep learning curve |
| **Use Cases** | SME applications, SaaS platforms, rapid prototyping | Large-scale enterprise complex business systems |
| **Customization** | ✅ Free to choose architecture patterns within plugins (DDD/Clean/etc.) | ⚠️ Must follow ABP conventions |

---

## ✨ Implemented Features List

> 💡 **All features below are built into the framework base, ready to use out of the box!**

### 🔐 Authentication & Authorization System
- ✅ **OIDC/SSO Single Sign-On**: Based on OpenIddict, supports cross-application unified authentication
- ✅ **JWT Token Authentication**: Standard JSON Web Token mechanism
- ✅ **RBAC Permission Management**: Role-based access control with button-level permissions
- ✅ **Multi-Tenancy Isolation**: Comprehensive tenant data isolation mechanism
- ✅ **CAPTCHA System**: Graphical CAPTCHA, slider verification and other validation methods
- ✅ **Blacklist Management**: IP blacklist, user blacklist
- ✅ **Rate Limiting**: API request frequency limiting

### 👥 User Management System
- ✅ **Administrator Management**: Complete CRUD + role assignment
- ✅ **Application User Management**: B2C user management with social account binding support
- ✅ **User Layout Configuration**: Personalized interface layout saving
- ✅ **Password Reset**: Secure password recovery and reset process
- ✅ **User Todo Tasks**: Task assignment and approval workflows

### 📋 System Management Features
- ✅ **Menu Management**: Dynamic menu configuration with multi-level nesting support
- ✅ **Role Management**: Role creation and permission assignment
- ✅ **Dictionary Management**: Data dictionary maintenance with tree structure support
- ✅ **System Configuration**: Global parameter configuration and management
- ✅ **Notifications**: In-app message notification system
- ✅ **Workbench**: Personalized dashboard

### 💾 Data Storage & Caching
- ✅ **SqlSugar ORM**: High-performance ORM supporting CodeFirst/DbFirst
- ✅ **Multi-Database Support**: SQLite, MySQL, PostgreSQL, SQL Server, Dameng
- ✅ **Hybrid Cache System**: Local memory + Redis dual-layer cache architecture
- ✅ **Repository Pattern**: Standardized data access abstraction layer
- ✅ **Transaction Management**: Complete transaction support

### 🎨 Frontend Capabilities
- ✅ **Vue 3 + TypeScript**: Modern frontend technology stack
- ✅ **Element Plus UI**: Enterprise-grade component library integration
- ✅ **qiankun Micro-Frontends**: Dynamic sub-application loading and independent deployment
- ✅ **Dynamic Route Generation**: Automatically generate frontend routes from backend configuration
- ✅ **Responsive Layout**: Perfect support for desktop and mobile
- ✅ **Theme Switching**: Light/dark themes, custom theme colors
- ✅ **Internationalization**: Multi-language support (i18n)
- ✅ **Tab Management**: Multi-tab browsing with KeepAlive caching

### 🔌 Plugin System Core
- ✅ **AssemblyLoadContext Isolation**: True plugin runtime isolation
- ✅ **Dynamic Hot-Swapping**: Runtime loading/unloading without restart
- ✅ **Plugin Lifecycle Management**: Initialize, start, stop, unload
- ✅ **Independent Dependency Context**: Independent DLL version management per plugin
- ✅ **Inter-Plugin EventBus**: Low-coupling cross-plugin communication
- ✅ **Plugin Branch Gateway**: Private request processing pipeline for plugins
- ✅ **Reverse Proxy Escape Hatch**: Support for plugins with their own web servers
- ✅ **Static Resource Mapping**: Automatic handling of plugin wwwroot files
- ✅ **Containerized DI**: Independent LifetimeScope based on Autofac
- ✅ **File System Monitoring**: Automatic detection of plugin changes

### 🛡️ Security & Protection
- ✅ **Sensitive Data Masking**: Automatic identification and masking of sensitive information
- ✅ **Replay Attack Prevention**: Request replay detection and protection
- ✅ **Encrypted Transmission**: AES+RSA encryption for requests/responses
- ✅ **Global Exception Handling**: Unified exception capture and logging
- ✅ **Business Operation Logs**: Complete operation audit trail
- ✅ **Graceful Shutdown**: Elegant shutdown mechanism

### 📡 Real-Time Communication
- ✅ **SignalR Hub**: WebSocket real-time communication support
- ✅ **Universal Hub**: UniversalHub supporting multiple message types
- ✅ **Plugin SignalR**: Plugin-level real-time communication endpoints
- ✅ **Connection State Management**: Auto-reconnect and state monitoring

### 🔧 Development Tools
- ✅ **Swagger/OpenAPI**: Auto-generated API documentation
- ✅ **Code Generator**: One-click CRUD code generation
- ✅ **Plugin CLI Tool**: Quick plugin template creation
- ✅ **Hot Reload Support**: Automatic code change detection during development
- ✅ **Unified Response Format**: Standardized API response structure
- ✅ **Model Validation**: Automatic parameter validation and error messages

### 🚀 Performance Optimization
- ✅ **HybridCache**: Local + distributed dual-layer caching
- ✅ **Cache Attribute Annotation**: @CacheResult decorator simplifies cache usage
- ✅ **Batch Operation Optimization**: Support for batch updates, deletions, etc.
- ✅ **Paginated Queries**: Efficient paginated data queries

---

## 🏪 Plugin Marketplace Ecosystem

> 💡 **Framework Built-in Capabilities + Plugin Marketplace Extensions = Complete Enterprise Solution**

### Framework Built-in vs Plugin Marketplace Comparison

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

### Core Advantages

- **Official精选 Plugins**: Payment systems, workflow engines, social login, object storage and other common features, ready to use
- **Third-Party Developer Ecosystem**: Professional plugins contributed by the community, covering e-commerce, CRM, ERP, OA and other industry scenarios
- **Rapid Project Delivery**: Outsourcing projects can be delivered in "weeks" instead of "months" through plugin combinations, efficiency improved by 70%+
- **💰 Developer Value Reuse**: Develop once, sell multiple times - Extract common functionalities from projects, package as plugins for the marketplace to achieve continuous revenue
- **From Project-Based to Productized**: Break free from "writing code from scratch for each project", create reusable product assets
- **One-Click Installation**: Visual plugin management interface supporting online purchase, automatic download, one-click installation
- **Version Management**: Comprehensive plugin version control supporting smooth upgrades and rollbacks

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

### Real Cases

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

### How to Get Started?

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

👉 **Get Started Now**: [Plugin Development Guide](../05-插件开发/创建插件.md) | [Marketplace Seller](https://fastdotnet.top/account/badges)

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

### What You Will Get:

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

## 📊 Architecture Visualization

We used [Graphify](https://github.com/graphify-dev/graphify) to generate a complete code knowledge graph to help you better understand the system architecture:

- **Interactive Graph**: View [graph.html](../../graphify-out/graph.html) - Zoomable, searchable complete relationship diagram
- **Tree Structure**: View [GRAPH_TREE.html](../../graphify-out/GRAPH_TREE.html) - Hierarchical module organization view
- **Detailed Report**: View [GRAPH_REPORT.md](../../graphify-out/GRAPH_REPORT.md) - Includes community analysis, core node statistics, etc.

> 💡 Tip: The knowledge graph displays 4,349 nodes and 10,146 relationship edges, divided into 294 functional communities - the best tool for understanding project structure.

---

## 📞 Related Links

- 🌐 **Official Website**: [https://fastdotnet.top](https://fastdotnet.top)
- 🏪 **🔥 Plugin Marketplace**: [https://fastdotnet.top/marketplace](https://fastdotnet.top/marketplace)
  - 💼 **Official Plugins**: Payment, workflow, social login, OSS and other common features
  - 👨‍💻 **Third-Party Plugins**: Professional plugins contributed by community developers
  - 🚀 **Rapid Delivery**: Outsourcing projects shorten delivery cycles by 70%+ through plugin combinations
  - 💰 **Developer Value Reuse**: Develop once, sell multiple times, transform "project code" into "product assets"
- 📚 **Official Documentation**: [https://docs.fastdotnet.top](https://docs.fastdotnet.top)
- 🎮 **Live Demos**:
  - Admin Panel: http://admin.demo.fastdotnet.top/ (`superadmin` / `123456`)
  - Application: http://app.demo.fastdotnet.top/ (`admintest` / `123456`)
- 💬 **QQ Group**: 779454817

---

**🌟 If Fastdotnet has helped you, please Star to show your support!**
