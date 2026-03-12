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

# 复制解决方案文件（如果存在）
COPY ["backend/*.sln", "./"]

# 复制所有项目文件
COPY ["backend/Fastdotnet.WebApi/Fastdotnet.WebApi.csproj", "Fastdotnet.WebApi/"]
COPY ["backend/Fastdotnet.Core/Fastdotnet.Core.csproj", "Fastdotnet.Core/"]
COPY ["backend/Fastdotnet.Orm/Fastdotnet.Orm.csproj", "Fastdotnet.Orm/"]
COPY ["backend/Fastdotnet.Service/Fastdotnet.Service.csproj", "Fastdotnet.Service/"]
COPY ["backend/Fastdotnet.Plugin.Contracts/Fastdotnet.Plugin.Contracts.csproj", "Fastdotnet.Plugin.Contracts/"]
COPY ["backend/Fastdotnet.Plugin.Shared/Fastdotnet.Plugin.Shared.csproj", "Fastdotnet.Plugin.Shared/"]

# 创建正确的项目目录结构并移动项目文件
# 这确保所有项目文件都在正确的位置，便于解决方案还原
RUN mkdir -p backend && \
    # 将包管理文件移动到 backend 目录
    mv backend/Directory.Build.props ./ 2>/dev/null || true && \
    mv backend/Directory.Packages.props ./ 2>/dev/null || true && \
    # 检查是否有解决方案文件
    if [ -f "backend/*.sln" ]; then \
        mv backend/*.sln ./ 2>/dev/null || true; \
    fi

# 还原 NuGet 包（使用解决方案文件或直接还原项目）
# 优先使用解决方案文件还原
RUN if [ -f "*.sln" ]; then \
        dotnet restore; \
    else \
        dotnet restore "Fastdotnet.WebApi/Fastdotnet.WebApi.csproj"; \
    fi

# 复制所有源代码
COPY backend/ .

# 构建应用
WORKDIR "/src/Fastdotnet.WebApi"
RUN dotnet build "Fastdotnet.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "Fastdotnet.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# 创建必要目录（包括密钥目录）
RUN mkdir -p /app/secrets && \
    mkdir -p /app/data && \
    mkdir -p /app/logs

# 设置入口点
ENTRYPOINT ["dotnet", "Fastdotnet.WebApi.dll"]