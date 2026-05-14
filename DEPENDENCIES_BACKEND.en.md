# Backend Dependencies

This document lists all major third-party dependencies used in the Fastdotnet framework backend along with their license types. This serves both as acknowledgments to the open-source community and helps users understand the third-party libraries used in the project, enabling them to assess whether these dependencies meet their requirements, licensing needs, or compliance requirements.

> **Last Updated**: 2026-05-14  
> **Fastdotnet Version**: 1.0.x

## Table of Contents

- [Core Framework Dependencies](#core-framework-dependencies)
- [ORM & Database](#orm--database)
- [Authentication & Authorization](#authentication--authorization)
- [Dependency Injection](#dependency-injection)
- [Caching System](#caching-system)
- [API Documentation](#api-documentation)
- [Utility Libraries](#utility-libraries)
- [Security & Encryption](#security--encryption)
- [Other Dependencies](#other-dependencies)

---

## Core Framework Dependencies

| Package | Version | License | Purpose | Official Link |
|---------|---------|---------|---------|---------------|
| **.NET Runtime** | 10.0 | MIT | .NET runtime environment | [dotnet.github.io](https://dotnet.github.io/) |
| **Microsoft.AspNetCore** | 10.0.0 | Apache-2.0 | ASP.NET Core Web Framework | [asp.net](https://asp.net/) |
| **Microsoft.Extensions** | 10.x | Apache-2.0 | .NET extensions library collection | [github.com/dotnet/extensions](https://github.com/dotnet/extensions) |

---

## ORM & Database

| Package | Version | License | Purpose | Official Link |
|---------|---------|---------|---------|---------------|
| **SqlSugarCore** | 5.1.4.214 | Apache-2.0 | High-performance ORM framework supporting multiple databases | [sqlsugarframework.com](https://www.sqlsugarframework.com/) |
| **System.Linq.Dynamic.Core** | 1.7.1 | Apache-2.0 | Dynamic LINQ query support | [github.com/zzzprojects/System.Linq.Dynamic.Core](https://github.com/zzzprojects/System.Linq.Dynamic.Core) |

**Notes**:
- SqlSugar is the core data access layer of Fastdotnet, supporting SQLite, MySQL, PostgreSQL, SQL Server, Dameng, and other databases
- System.Linq.Dynamic.Core enables dynamic querying and filtering capabilities

---

## Authentication & Authorization

| Package | Version | License | Purpose | Official Link |
|---------|---------|---------|---------|---------------|
| **OpenIddict.AspNetCore** | 7.4.0 | Apache-2.0 | OIDC/OAuth2 single sign-on framework | [openiddict.com](https://openiddict.com/) |
| **OpenIddict.Abstractions** | 7.4.0 | Apache-2.0 | OpenIddict abstraction layer | [openiddict.com](https://openiddict.com/) |
| **OpenIddict.Core** | 7.4.0 | Apache-2.0 | OpenIddict core library | [openiddict.com](https://openiddict.com/) |
| **Microsoft.AspNetCore.Authentication.JwtBearer** | 10.0.0 | Apache-2.0 | JWT Bearer authentication middleware | [docs.microsoft.com](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/) |
| **System.IdentityModel.Tokens.Jwt** | 8.16.0 | MIT | JWT token processing library | [github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet](https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet) |
| **Microsoft.IdentityModel.Tokens** | 8.16.0 | MIT | Identity model token library | [github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet](https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet) |

**Notes**:
- OpenIddict provides complete OIDC/OAuth2 protocol support for enterprise-grade SSO
- JWT-related libraries handle token generation, validation, and management

---

## Dependency Injection

| Package | Version | License | Purpose | Official Link |
|---------|---------|---------|---------|---------------|
| **Autofac** | 8.4.0 | MIT | High-performance IoC container | [autofac.org](https://autofac.org/) |
| **Autofac.Extensions.DependencyInjection** | 10.0.0 | MIT | Autofac integration with .NET DI | [autofac.org](https://autofac.org/) |
| **Scrutor** | 7.0.0 | MIT | Assembly scanning and service registration | [github.com/khellang/Scrutor](https://github.com/khellang/Scrutor) |
| **Microsoft.Extensions.DependencyInjection** | 10.0.3 | Apache-2.0 | Built-in .NET dependency injection | [docs.microsoft.com](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection) |

**Notes**:
- Autofac serves as an enhanced main container providing more flexible dependency management
- Scrutor enables automatic service scanning and registration, reducing manual configuration

---

## Caching System

| Package | Version | License | Purpose | Official Link |
|---------|---------|---------|---------|---------------|
| **Microsoft.Extensions.Caching.Hybrid** | 10.4.0 | Apache-2.0 | Hybrid cache (local + distributed) | [docs.microsoft.com](https://docs.microsoft.com/en-us/dotnet/core/extensions/caching) |
| **Microsoft.Extensions.Caching.StackExchangeRedis** | 9.0.8 | Apache-2.0 | Redis distributed cache support | [stackexchange.github.io/StackExchange.Redis](https://stackexchange.github.io/StackExchange.Redis/) |

**Notes**:
- HybridCache provides dual-layer caching: local memory cache + Redis distributed cache
- Significantly improves performance while ensuring data consistency across multiple instances

---

## API Documentation

| Package | Version | License | Purpose | Official Link |
|---------|---------|---------|---------|---------------|
| **Swashbuckle.AspNetCore** | 6.5.0 | MIT | Swagger/OpenAPI documentation generator | [github.com/domaindrivendev/Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) |
| **Swashbuckle.AspNetCore.Newtonsoft** | 6.5.0 | MIT | Newtonsoft.Json support | [github.com/domaindrivendev/Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) |
| **Microsoft.AspNetCore.OpenApi** | 8.0.1 | Apache-2.0 | OpenAPI specification support | [docs.microsoft.com](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/openapi) |

**Notes**:
- Swashbuckle automatically generates Swagger UI and OpenAPI specification documents
- Supports online API interface testing

---

## Utility Libraries

| Package | Version | License | Purpose | Official Link |
|---------|---------|---------|---------|---------------|
| **Mapster** | 10.0.7 | Apache-2.0 | High-performance object mapping library | [github.com/MapsterMapper/Mapster](https://github.com/MapsterMapper/Mapster) |
| **Mapster.DependencyInjection** | 10.0.7 | Apache-2.0 | Mapster DI integration | [github.com/MapsterMapper/Mapster](https://github.com/MapsterMapper/Mapster) |
| **Newtonsoft.Json** | 13.0.4 | MIT | JSON serialization and deserialization | [newtonsoft.com/json](https://www.newtonsoft.com/json) |
| **Microsoft.AspNetCore.Mvc.NewtonsoftJson** | 9.0.0 | Apache-2.0 | MVC Newtonsoft.Json integration | [docs.microsoft.com](https://docs.microsoft.com/en-us/aspnet/core/mvc/) |
| **Yitter.IdGenerator** | 1.0.14 | MIT | Distributed ID generator | [github.com/yitter/idgenerator](https://github.com/yitter/idgenerator) |
| **Lazy.Captcha.Core** | 2.2.2 | MIT | CAPTCHA generation library | [github.com/LazyArchitect/Lazy.Captcha](https://github.com/LazyArchitect/Lazy.Captcha) |
| **MailKit** | 4.16.0 | MIT | Email sending library | [github.com/jstedfast/MailKit](https://github.com/jstedfast/MailKit) |

**Notes**:
- Mapster offers better performance than AutoMapper for DTO and entity conversion
- Yitter.IdGenerator provides high-performance snowflake algorithm ID generation
- MailKit is used for sending email notifications

---

## Security & Encryption

| Package | Version | License | Purpose | Official Link |
|---------|---------|---------|---------|---------------|
| **BouncyCastle.Cryptography** | 2.6.2 | MIT | Cryptographic algorithm library | [bouncycastle.org](https://www.bouncycastle.org/) |
| **Portable.BouncyCastle** | 1.9.0 | MIT | Portable version of BouncyCastle | [bouncycastle.org](https://www.bouncycastle.org/) |
| **Obfuscar** | 2.2.47 | MIT | .NET code obfuscation tool | [github.com/obfuscar/obfuscar](https://github.com/obfuscar/obfuscar) |
| **Obfuscar.MsBuild** | 2.2.47 | MIT | Obfuscar MSBuild integration | [github.com/obfuscar/obfuscar](https://github.com/obfuscar/obfuscar) |

**Notes**:
- BouncyCastle provides various encryption algorithm support (AES, RSA, etc.)
- Obfuscar is used for code obfuscation during release to protect intellectual property

---

## Other Dependencies

| Package | Version | License | Purpose | Official Link |
|---------|---------|---------|---------|---------------|
| **Yarp.ReverseProxy** | 2.3.0 | Apache-2.0 | Reverse proxy server | [microsoft.github.io/reverse-proxy](https://microsoft.github.io/reverse-proxy/) |
| **Microsoft.VisualStudio.Azure.Containers.Tools.Targets** | 1.23.0 | MIT | Visual Studio Docker tools | [visualstudio.microsoft.com](https://visualstudio.microsoft.com/) |
| **MinVer** | 6.0.0 | Apache-2.0 | Git tag-based version management | [github.com/adamralph/minver](https://github.com/adamralph/minver) |
| **System.ComponentModel.Composition** | 9.0.2 | MIT | MEF component composition framework | [docs.microsoft.com](https://docs.microsoft.com/en-us/dotnet/framework/mef/) |
| **System.Runtime.InteropServices** | 4.3.0 | MIT | Runtime interop services | [docs.microsoft.com](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices) |
| **System.IO.FileSystem.Primitives** | 4.3.0 | MIT | File system primitive types | [docs.microsoft.com](https://docs.microsoft.com/en-us/dotnet/api/system.io) |
| **System.Diagnostics.Debug** | 4.3.0 | MIT | Debugging support | [docs.microsoft.com](https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics) |

**Notes**:
- YARP is used for plugin reverse proxy escape hatch functionality
- MinVer automatically generates version numbers from Git tags

---

## License Summary

### By License Type

#### MIT License (Most Permissive)
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
- And many Microsoft system libraries

#### Apache-2.0 License (Permissive, requires attribution)
- .NET Runtime and most Microsoft.Extensions libraries
- SqlSugarCore
- OpenIddict
- Mapster
- Yarp.ReverseProxy
- MinVer

### Compatibility Notes

✅ **Commercial Friendly**: All dependencies use MIT or Apache-2.0 licenses, allowing commercial use  
✅ **No Open Source Required**: Using these libraries does not require your project to be open source  
⚠️ **Apache-2.0 Note**: If using Apache-2.0 licensed libraries, it's recommended to retain copyright notices in your project

---

## Internal Dependencies

| Package | Version | License | Purpose |
|---------|---------|---------|---------|
| **Fastdotnet.Plugin.Core** | 1.1.4 | MIT | Fastdotnet plugin core contracts |

**Notes**:
- This is Fastdotnet framework's own NuGet package
- Uses MIT license, consistent with the framework body

---

## How to Verify Dependency Licenses

If you need to verify a package's license information, you can use the following methods:

1. **NuGet Website**: Visit [nuget.org](https://www.nuget.org/) and search for the package name
2. **GitHub Repository**: Check the project's LICENSE file
3. **Command Line Tool**:
   ```bash
   dotnet list package --include-transitive
   ```

---

## Changelog

- **2026-05-14**: Initial version, documenting Fastdotnet 1.0.x backend dependencies

---

## Feedback & Contributions

If you find incorrect dependency information or have new dependencies to add, please contact us through:

- 📧 Email: [yunnanzuyuankeji@163.com](mailto:yunnanzuyuankeji@163.com)
- 💬 QQ Group: 779454817
- 🐛 GitHub Issues: [https://github.com/CN-GodHei/fastdotnet/issues](https://github.com/CN-GodHei/fastdotnet/issues)

---

**Thank you to all open-source project contributors! It is thanks to these excellent open-source libraries that Fastdotnet can be so powerful and easy to use.**