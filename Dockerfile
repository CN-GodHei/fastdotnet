# 使用 .NET 运行时基础镜像
# 版本会自动匹配项目中定义的 TargetFramework
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER app
WORKDIR /app
EXPOSE 18889

# 使用 .NET SDK 镜像进行构建
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 复制解决方案和包管理文件
COPY ["backend/Directory.Build.props", "backend/"]
COPY ["backend/Directory.Packages.props", "backend/"]
COPY ["backend/Fastdotnet.WebApi/Fastdotnet.WebApi.csproj", "Fastdotnet.WebApi/"]
COPY ["backend/Fastdotnet.Core/Fastdotnet.Core.csproj", "Fastdotnet.Core/"]
COPY ["backend/Fastdotnet.Orm/Fastdotnet.Orm.csproj", "Fastdotnet.Orm/"]
COPY ["backend/Fastdotnet.Service/Fastdotnet.Service.csproj", "Fastdotnet.Service/"]
COPY ["backend/Fastdotnet.Plugin.Contracts/Fastdotnet.Plugin.Contracts.csproj", "Fastdotnet.Plugin.Contracts/"]
COPY ["backend/Fastdotnet.Plugin.Shared/Fastdotnet.Plugin.Shared.csproj", "Fastdotnet.Plugin.Shared/"]

# 还原 NuGet 包
RUN dotnet restore "Fastdotnet.WebApi/Fastdotnet.WebApi.csproj"

# 复制所有源代码
COPY backend/ .

# 构建应用
WORKDIR "/src/Fastdotnet.WebApi"
RUN dotnet build "Fastdotnet.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "Fastdotnet.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# 创建必要目录（包括密钥目录）
RUN mkdir -p /app/secrets && \
    mkdir -p /app/data && \
    mkdir -p /app/logs

# 设置入口点
ENTRYPOINT ["dotnet", "Fastdotnet.WebApi.dll"]
