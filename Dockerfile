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

# 复制包管理文件
COPY ["backend/Directory.Build.props", "backend/"]
COPY ["backend/Directory.Packages.props", "backend/"]

# 复制解决方案文件
COPY ["backend/Fastdotnet.sln", "./"]

# 复制所有项目文件（只复制必要的项目）
COPY ["backend/Fastdotnet.WebApi/Fastdotnet.WebApi.csproj", "Fastdotnet.WebApi/"]
COPY ["backend/Fastdotnet.Core/Fastdotnet.Core.csproj", "Fastdotnet.Core/"]
COPY ["backend/Fastdotnet.Orm/Fastdotnet.Orm.csproj", "Fastdotnet.Orm/"]
COPY ["backend/Fastdotnet.Service/Fastdotnet.Service.csproj", "Fastdotnet.Service/"]
COPY ["backend/Fastdotnet.Plugin.Contracts/Fastdotnet.Plugin.Contracts.csproj", "Fastdotnet.Plugin.Contracts/"]
COPY ["backend/Fastdotnet.Plugin.Shared/Fastdotnet.Plugin.Shared.csproj", "Fastdotnet.Plugin.Shared/"]

# 创建正确的项目目录结构并移动项目文件
RUN mkdir -p backend && \
    # 将包管理文件移动到根目录
    mv backend/Directory.Build.props ./ && \
    mv backend/Directory.Packages.props ./

# 还原 NuGet 包（直接还原各项目，避免解决方案中包含 Plugins）
WORKDIR /src
RUN dotnet restore "Fastdotnet.WebApi/Fastdotnet.WebApi.csproj" && \
    dotnet restore "Fastdotnet.Core/Fastdotnet.Core.csproj" && \
    dotnet restore "Fastdotnet.Service/Fastdotnet.Service.csproj"

# 有选择地复制源代码（排除不必要的文件和目录）
COPY backend/Fastdotnet.WebApi/ Fastdotnet.WebApi/
COPY backend/Fastdotnet.Core/ Fastdotnet.Core/
COPY backend/Fastdotnet.Orm/ Fastdotnet.Orm/
COPY backend/Fastdotnet.Service/ Fastdotnet.Service/
COPY backend/Fastdotnet.Plugin.Contracts/ Fastdotnet.Plugin.Contracts/
COPY backend/Fastdotnet.Plugin.Shared/ Fastdotnet.Plugin.Shared/

# 构建应用
WORKDIR "/src/Fastdotnet.WebApi"
RUN dotnet build "Fastdotnet.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
# 启用发布优化：禁用调试符号（代码裁剪需要自包含部署，这里先不启用）
RUN dotnet publish "Fastdotnet.WebApi.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseHost=false \
    /p:DebugType=None \
    /p:DebugSymbols=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# 创建必要目录（包括密钥目录）
RUN mkdir -p /app/secrets && \
    mkdir -p /app/data && \
    mkdir -p /app/logs

# 设置入口点
ENTRYPOINT ["dotnet", "Fastdotnet.WebApi.dll"]