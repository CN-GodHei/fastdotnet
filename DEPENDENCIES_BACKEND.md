# 后端依赖清单

本文档列出了 Fastdotnet 框架后端使用的所有主要第三方依赖库及其许可证类型。这既是对开源社区的鸣谢，也帮助使用者了解项目中使用的第三方库，以便评估是否符合您的需求、许可要求或合规性要求。

> **最后更新时间**：2026-05-14  
> **Fastdotnet 版本**：1.0.x

## 目录

- [核心框架依赖](#核心框架依赖)
- [ORM 与数据库](#orm-与数据库)
- [认证与授权](#认证与授权)
- [依赖注入](#依赖注入)
- [缓存系统](#缓存系统)
- [API 文档](#api-文档)
- [工具库](#工具库)
- [安全与加密](#安全与加密)
- [其他依赖](#其他依赖)

---

## 核心框架依赖

| 包名 | 版本 | 许可证 | 用途 | 官方链接 |
|------|------|--------|------|----------|
| **.NET Runtime** | 10.0 | MIT | .NET 运行时环境 | [dotnet.github.io](https://dotnet.github.io/) |
| **Microsoft.AspNetCore** | 10.0.0 | Apache-2.0 | ASP.NET Core Web 框架 | [asp.net](https://asp.net/) |
| **Microsoft.Extensions** | 10.x | Apache-2.0 | .NET 扩展库集合 | [github.com/dotnet/extensions](https://github.com/dotnet/extensions) |

---

## ORM 与数据库

| 包名 | 版本 | 许可证 | 用途 | 官方链接 |
|------|------|--------|------|----------|
| **SqlSugarCore** | 5.1.4.214 | Apache-2.0 | 高性能 ORM 框架，支持多数据库 | [sqlsugarframework.com](https://www.sqlsugarframework.com/) |
| **System.Linq.Dynamic.Core** | 1.7.1 | Apache-2.0 | 动态 LINQ 查询支持 | [github.com/zzzprojects/System.Linq.Dynamic.Core](https://github.com/zzzprojects/System.Linq.Dynamic.Core) |

**说明**：
- SqlSugar 是 Fastdotnet 的核心数据访问层，支持 SQLite、MySQL、PostgreSQL、SQL Server、达梦等多种数据库
- System.Linq.Dynamic.Core 用于支持动态查询和过滤功能

---

## 认证与授权

| 包名 | 版本 | 许可证 | 用途 | 官方链接 |
|------|------|--------|------|----------|
| **OpenIddict.AspNetCore** | 7.4.0 | Apache-2.0 | OIDC/OAuth2 单点登录框架 | [openiddict.com](https://openiddict.com/) |
| **OpenIddict.Abstractions** | 7.4.0 | Apache-2.0 | OpenIddict 抽象层 | [openiddict.com](https://openiddict.com/) |
| **OpenIddict.Core** | 7.4.0 | Apache-2.0 | OpenIddict 核心库 | [openiddict.com](https://openiddict.com/) |
| **Microsoft.AspNetCore.Authentication.JwtBearer** | 10.0.0 | Apache-2.0 | JWT Bearer 认证中间件 | [docs.microsoft.com](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/) |
| **System.IdentityModel.Tokens.Jwt** | 8.16.0 | MIT | JWT Token 处理库 | [github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet](https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet) |
| **Microsoft.IdentityModel.Tokens** | 8.16.0 | MIT | 身份模型令牌库 | [github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet](https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet) |

**说明**：
- OpenIddict 提供完整的 OIDC/OAuth2 协议支持，实现企业级 SSO 单点登录
- JWT 相关库用于 Token 的生成、验证和管理

---

## 依赖注入

| 包名 | 版本 | 许可证 | 用途 | 官方链接 |
|------|------|--------|------|----------|
| **Autofac** | 8.4.0 | MIT | 高性能 IoC 容器 | [autofac.org](https://autofac.org/) |
| **Autofac.Extensions.DependencyInjection** | 10.0.0 | MIT | Autofac 与 .NET DI 集成 | [autofac.org](https://autofac.org/) |
| **Scrutor** | 7.0.0 | MIT | 程序集扫描和服务注册 | [github.com/khellang/Scrutor](https://github.com/khellang/Scrutor) |
| **Microsoft.Extensions.DependencyInjection** | 10.0.3 | Apache-2.0 | .NET 内置依赖注入 | [docs.microsoft.com](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection) |

**说明**：
- Autofac 作为主容器的增强，提供更灵活的依赖管理能力
- Scrutor 用于自动扫描和注册服务，减少手动配置

---

## 缓存系统

| 包名 | 版本 | 许可证 | 用途 | 官方链接 |
|------|------|--------|------|----------|
| **Microsoft.Extensions.Caching.Hybrid** | 10.4.0 | Apache-2.0 | 混合缓存（本地+分布式） | [docs.microsoft.com](https://docs.microsoft.com/en-us/dotnet/core/extensions/caching) |
| **Microsoft.Extensions.Caching.StackExchangeRedis** | 9.0.8 | Apache-2.0 | Redis 分布式缓存支持 | [stackexchange.github.io/StackExchange.Redis](https://stackexchange.github.io/StackExchange.Redis/) |

**说明**：
- HybridCache 提供双层缓存架构：本地内存缓存 + Redis 分布式缓存
- 显著提升性能并保证多实例间的数据一致性

---

## API 文档

| 包名 | 版本 | 许可证 | 用途 | 官方链接 |
|------|------|--------|------|----------|
| **Swashbuckle.AspNetCore** | 6.5.0 | MIT | Swagger/OpenAPI 文档生成 | [github.com/domaindrivendev/Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) |
| **Swashbuckle.AspNetCore.Newtonsoft** | 6.5.0 | MIT | Newtonsoft.Json 支持 | [github.com/domaindrivendev/Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) |
| **Microsoft.AspNetCore.OpenApi** | 8.0.1 | Apache-2.0 | OpenAPI 规范支持 | [docs.microsoft.com](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/openapi) |

**说明**：
- Swashbuckle 自动生成 Swagger UI 和 OpenAPI 规范文档
- 支持在线测试 API 接口

---

## 工具库

| 包名 | 版本 | 许可证 | 用途 | 官方链接 |
|------|------|--------|------|----------|
| **Mapster** | 10.0.7 | Apache-2.0 | 高性能对象映射库 | [github.com/MapsterMapper/Mapster](https://github.com/MapsterMapper/Mapster) |
| **Mapster.DependencyInjection** | 10.0.7 | Apache-2.0 | Mapster DI 集成 | [github.com/MapsterMapper/Mapster](https://github.com/MapsterMapper/Mapster) |
| **Newtonsoft.Json** | 13.0.4 | MIT | JSON 序列化和反序列化 | [newtonsoft.com/json](https://www.newtonsoft.com/json) |
| **Microsoft.AspNetCore.Mvc.NewtonsoftJson** | 9.0.0 | Apache-2.0 | MVC Newtonsoft.Json 集成 | [docs.microsoft.com](https://docs.microsoft.com/en-us/aspnet/core/mvc/) |
| **Yitter.IdGenerator** | 1.0.14 | MIT | 分布式 ID 生成器 | [github.com/yitter/idgenerator](https://github.com/yitter/idgenerator) |
| **Lazy.Captcha.Core** | 2.2.2 | MIT | 验证码生成库 | [github.com/LazyArchitect/Lazy.Captcha](https://github.com/LazyArchitect/Lazy.Captcha) |
| **MailKit** | 4.16.0 | MIT | 邮件发送库 | [github.com/jstedfast/MailKit](https://github.com/jstedfast/MailKit) |

**说明**：
- Mapster 比 AutoMapper 性能更优，用于 DTO 和实体之间的转换
- Yitter.IdGenerator 提供高性能的雪花算法 ID 生成
- MailKit 用于发送邮件通知

---

## 安全与加密

| 包名 | 版本 | 许可证 | 用途 | 官方链接 |
|------|------|--------|------|----------|
| **BouncyCastle.Cryptography** | 2.6.2 | MIT | 加密算法库 | [bouncycastle.org](https://www.bouncycastle.org/) |
| **Portable.BouncyCastle** | 1.9.0 | MIT | BouncyCastle 便携版 | [bouncycastle.org](https://www.bouncycastle.org/) |
| **Obfuscar** | 2.2.47 | MIT | .NET 代码混淆工具 | [github.com/obfuscar/obfuscar](https://github.com/obfuscar/obfuscar) |
| **Obfuscar.MsBuild** | 2.2.47 | MIT | Obfuscar MSBuild 集成 | [github.com/obfuscar/obfuscar](https://github.com/obfuscar/obfuscar) |

**说明**：
- BouncyCastle 提供多种加密算法支持（AES、RSA 等）
- Obfuscar 用于发布时的代码混淆，保护知识产权

---

## 其他依赖

| 包名 | 版本 | 许可证 | 用途 | 官方链接 |
|------|------|--------|------|----------|
| **Yarp.ReverseProxy** | 2.3.0 | Apache-2.0 | 反向代理服务器 | [microsoft.github.io/reverse-proxy](https://microsoft.github.io/reverse-proxy/) |
| **Microsoft.VisualStudio.Azure.Containers.Tools.Targets** | 1.23.0 | MIT | Visual Studio Docker 工具 | [visualstudio.microsoft.com](https://visualstudio.microsoft.com/) |
| **MinVer** | 6.0.0 | Apache-2.0 | 基于 Git 标签的版本管理 | [github.com/adamralph/minver](https://github.com/adamralph/minver) |
| **System.ComponentModel.Composition** | 9.0.2 | MIT | MEF 组件组合框架 | [docs.microsoft.com](https://docs.microsoft.com/en-us/dotnet/framework/mef/) |
| **System.Runtime.InteropServices** | 4.3.0 | MIT | 运行时互操作服务 | [docs.microsoft.com](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices) |
| **System.IO.FileSystem.Primitives** | 4.3.0 | MIT | 文件系统基础类型 | [docs.microsoft.com](https://docs.microsoft.com/en-us/dotnet/api/system.io) |
| **System.Diagnostics.Debug** | 4.3.0 | MIT | 调试支持 | [docs.microsoft.com](https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics) |

**说明**：
- YARP 用于插件的反向代理逃生舱功能
- MinVer 自动从 Git 标签生成版本号

---

## 许可证总结

### 按许可证类型分类

#### MIT 许可证（最宽松）
- Autofac
- Newtonsoft.Json
- Yitter.IdGenerator
- Lazy.Captcha.Core
- MailKit
- BouncyCastle
- Obfuscar
- Swashbuckle.AspNetCore
- Microsoft.IdentityModel.Tokens
- System.IdentityModel.Tokens.Jwt
- 以及多个 Microsoft 系统库

#### Apache-2.0 许可证（宽松，需声明变更）
- .NET Runtime 及大部分 Microsoft.Extensions 库
- SqlSugarCore
- OpenIddict
- Mapster
- Yarp.ReverseProxy
- MinVer

### 兼容性说明

✅ **商业友好**：所有依赖均使用 MIT 或 Apache-2.0 许可证，允许商业用途  
✅ **无需开源**：使用这些库不需要将您的项目开源  
⚠️ **Apache-2.0 注意**：如果使用 Apache-2.0 许可证的库，建议在项目中保留版权声明

---

## 内部依赖

| 包名 | 版本 | 许可证 | 用途 |
|------|------|--------|------|
| **Fastdotnet.Plugin.Core** | 1.1.4 | MIT | Fastdotnet 插件核心契约 |

**说明**：
- 这是 Fastdotnet 框架自身的 NuGet 包
- 采用 MIT 许可证，与框架主体保持一致

---

## 如何验证依赖许可证

如果您需要验证某个包的许可证信息，可以使用以下方法：

1. **NuGet 官网**：访问 [nuget.org](https://www.nuget.org/) 搜索包名
2. **GitHub 仓库**：查看项目的 LICENSE 文件
3. **命令行工具**：
   ```bash
   dotnet list package --include-transitive
   ```

---

## 更新日志

- **2026-05-14**：初始版本，记录 Fastdotnet 1.0.x 的后端依赖

---

## 反馈与贡献

如果您发现依赖信息有误或有新的依赖需要添加，请通过以下方式联系我们：

- 📧 邮箱：[yunnanzuyuankeji@163.com](mailto:yunnanzuyuankeji@163.com)
- 💬 QQ 群：779454817
- 🐛 GitHub Issues：[https://github.com/CN-GodHei/fastdotnet/issues](https://github.com/CN-GodHei/fastdotnet/issues)

---

**感谢所有开源项目的贡献者！正是有了这些优秀的开源库，Fastdotnet 才能如此强大和易用。**