#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Fastdotnet Docker 管理脚本
.DESCRIPTION
    用于构建、启动、停止和管理 Fastdotnet Docker 容器
.EXAMPLE
    .\start.ps1           - 正常启动
    .\start.ps1 -Build    - 重新构建镜像后启动
    .\start.ps1 -Logs     - 启动并查看日志
    .\start.ps1 -Stop     - 停止服务
    .\start.ps1 -Restart  - 重启服务
#>

param(
    [switch]$Build,
    [switch]$Logs,
    [switch]$Stop,
    [switch]$Restart,
    [switch]$Clean
)

$ComposeFile = "docker-compose.yml"
$ProjectDir = Split-Path -Parent $PSScriptRoot

Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  Fastdotnet Docker 管理脚本" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# 检查 docker-compose 文件是否存在
if (-not (Test-Path $ComposeFile)) {
    Write-Host "❌ 错误：找不到 $ComposeFile" -ForegroundColor Red
    Write-Host "请确保在 docker 目录下运行此脚本" -ForegroundColor Yellow
    exit 1
}

# 检查密钥文件是否存在
$secretFile = "secrets/marketplace_private_key.txt"
if (-not (Test-Path $secretFile)) {
    Write-Host "⚠️  警告：密钥文件不存在：$secretFile" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "请先创建密钥文件：" -ForegroundColor Cyan
    Write-Host "  1. 从 user-secrets 导出私钥" -ForegroundColor Gray
    Write-Host "  2. 保存到 secrets/marketplace_private_key.txt" -ForegroundColor Gray
    Write-Host ""
    
    $continue = Read-Host "是否继续？(y/n)"
    if ($continue -ne 'y') {
        exit 0
    }
}

if ($Stop) {
    Write-Host "🛑 停止服务..." -ForegroundColor Yellow
    docker-compose -f $ComposeFile down
    Write-Host "✅ 服务已停止" -ForegroundColor Green
    return
}

if ($Clean) {
    Write-Host "🧹 清理所有容器、网络和卷..." -ForegroundColor Yellow
    docker-compose -f $ComposeFile down -v --remove-orphans
    Write-Host "✅ 清理完成" -ForegroundColor Green
    return
}

if ($Restart) {
    Write-Host "🔄 重启服务..." -ForegroundColor Yellow
    docker-compose -f $ComposeFile restart
    Write-Host "✅ 服务已重启" -ForegroundColor Green
    return
}

if ($Build) {
    Write-Host "🔨 构建 Docker 镜像（无缓存）..." -ForegroundColor Yellow
    docker-compose -f $ComposeFile build --no-cache
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ 构建失败！" -ForegroundColor Red
        exit 1
    }
    
    Write-Host "✅ 构建成功" -ForegroundColor Green
} else {
    # 如果没有镜像，先尝试构建
    $imageExists = docker images fastdotnet-api:latest --format "{{.Repository}}"
    if (-not $imageExists) {
        Write-Host "📦 镜像不存在，开始构建..." -ForegroundColor Yellow
        docker-compose -f $ComposeFile build
        
        if ($LASTEXITCODE -ne 0) {
            Write-Host "❌ 构建失败！" -ForegroundColor Red
            exit 1
        }
        
        Write-Host "✅ 构建成功" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "🚀 启动服务..." -ForegroundColor Green
docker-compose -f $ComposeFile up -d

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ 启动失败！" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "📊 查看服务状态:" -ForegroundColor Cyan
Start-Sleep -Seconds 2
docker-compose -f $ComposeFile ps

Write-Host ""
Write-Host "📝 查看日志:" -ForegroundColor Cyan

if ($Logs) {
    Write-Host "(按 Ctrl+C 退出实时日志)" -ForegroundColor Gray
    docker-compose -f $ComposeFile logs -f
} else {
    Write-Host "(最近 50 行日志)" -ForegroundColor Gray
    Start-Sleep -Seconds 3
    docker-compose -f $ComposeFile logs --tail=50
}

Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  常用命令:" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  查看状态：   docker-compose ps" -ForegroundColor Gray
Write-Host "  查看日志：   docker-compose logs -f" -ForegroundColor Gray
Write-Host "  停止服务：   .\start.ps1 -Stop" -ForegroundColor Gray
Write-Host "  重启服务：   .\start.ps1 -Restart" -ForegroundColor Gray
Write-Host "  完全清理：   .\start.ps1 -Clean" -ForegroundColor Gray
Write-Host "  进入容器：   docker exec -it fastdotnet-api /bin/bash" -ForegroundColor Gray
Write-Host ""
