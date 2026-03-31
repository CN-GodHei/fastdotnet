# 请参阅 https://aka.ms/customizecontainer 以了解如何自定义调试容器，以及 Visual Studio 如何使用此 Dockerfile 生成映像以更快地进行调试。

# 此阶段用于在快速模式(默认为调试配置)下从 VS 运行时
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 18889


# 此阶段用于生成服务项目
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["nuget.config", "."]
COPY ["Fastdotnet.WebApi/Fastdotnet.WebApi.csproj", "Fastdotnet.WebApi/"]
COPY ["Fastdotnet.Core/Fastdotnet.Core.csproj", "Fastdotnet.Core/"]
COPY ["Fastdotnet.Plugin.Contracts/Fastdotnet.Plugin.Contracts.csproj", "Fastdotnet.Plugin.Contracts/"]
COPY ["Fastdotnet.Orm/Fastdotnet.Orm.csproj", "Fastdotnet.Orm/"]
COPY ["Fastdotnet.Plugin.Shared/Fastdotnet.Plugin.Shared.csproj", "Fastdotnet.Plugin.Shared/"]
COPY ["Fastdotnet.Service/Fastdotnet.Service.csproj", "Fastdotnet.Service/"]
RUN dotnet restore "./Fastdotnet.WebApi/Fastdotnet.WebApi.csproj"
COPY . .
WORKDIR "/src/Fastdotnet.WebApi"
RUN dotnet build "./Fastdotnet.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# 此阶段用于发布要复制到最终阶段的服务项目
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Fastdotnet.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false /p:DebugType=None /p:EmbedAllSources=false
# 删除开发环境配置文件
RUN rm -f /app/publish/appsettings.Development.json

# 此阶段在生产中使用，或在常规模式下从 VS 运行时使用(在不使用调试配置时为默认值)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# 创建必要目录（包括密钥目录、插件目录等）
RUN mkdir -p /app/secrets && \
    mkdir -p /app/data && \
    mkdir -p /app/logs && \
    mkdir -p /app/plugins
ENTRYPOINT ["dotnet", "Fastdotnet.WebApi.dll"]