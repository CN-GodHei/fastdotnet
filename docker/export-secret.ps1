#!/usr/bin/env pwsh
<#
.SYNOPSIS
    从 user-secrets 导出私钥到 Docker 密钥文件
.DESCRIPTION
    自动从开发环境的 user-secrets 中读取 Marketplace:PrivateKey
    并保存到 docker/secrets/marketplace_private_key.txt
.EXAMPLE
    .\export-secret.ps1
#>

$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  导出私钥到 Docker" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# 设置路径
$ProjectRoot = Split-Path -Parent $PSScriptRoot | Split-Path -Parent
$WebApiProject = Join-Path $ProjectRoot "backend\Fastdotnet.WebApi"
$SecretFile = Join-Path $PSScriptRoot "secrets\marketplace_private_key.txt"

Write-Host "📁 项目路径：$WebApiProject" -ForegroundColor Gray
Write-Host "📄 目标文件：$SecretFile" -ForegroundColor Gray
Write-Host ""

# 检查项目路径是否存在
if (-not (Test-Path $WebApiProject)) {
    Write-Host "❌ 错误：找不到项目路径 $WebApiProject" -ForegroundColor Red
    exit 1
}

# 检查 user-secrets 是否初始化
try {
    Write-Host "🔍 检查 user-secrets 配置..." -ForegroundColor Yellow
    $secretsList = dotnet user-secrets list --project $WebApiProject 2>&1
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ user-secrets 未初始化或发生错误" -ForegroundColor Red
        Write-Host "请先运行：dotnet user-secrets init --project $WebApiProject" -ForegroundColor Yellow
        exit 1
    }
    
    # 查找私钥
    $privateKeyLine = $secretsList | Where-Object { $_ -match "Marketplace:PrivateKey\s*=" }
    
    if (-not $privateKeyLine) {
        Write-Host "❌ 未找到 Marketplace:PrivateKey 配置" -ForegroundColor Red
        Write-Host ""
        Write-Host "可用的密钥：" -ForegroundColor Yellow
        $secretsList | ForEach-Object { Write-Host "  $_" -ForegroundColor Gray }
        Write-Host ""
        Write-Host "请先设置私钥：" -ForegroundColor Cyan
        Write-Host '  dotnet user-secrets set "Marketplace:PrivateKey" "你的私钥 Base64" --project $WebApiProject' -ForegroundColor Gray
        exit 1
    }
    
    # 提取私钥值
    $privateKey = $privateKeyLine -replace "^Marketplace:PrivateKey\s*=\s*", ""
    
    if ([string]::IsNullOrWhiteSpace($privateKey)) {
        Write-Host "❌ 私钥值为空！" -ForegroundColor Red
        exit 1
    }
    
    Write-Host "✅ 找到私钥配置" -ForegroundColor Green
    Write-Host "   长度：$($privateKey.Length) 字符" -ForegroundColor Gray
    
} catch {
    Write-Host "❌ 读取 user-secrets 失败：$_" -ForegroundColor Red
    exit 1
}

# 确保 secrets 目录存在
$secretsDir = Split-Path -Parent $SecretFile
if (-not (Test-Path $secretsDir)) {
    Write-Host "📂 创建 secrets 目录..." -ForegroundColor Yellow
    New-Item -ItemType Directory -Force -Path $secretsDir | Out-Null
}

# 保存私钥到文件
try {
    Write-Host "💾 保存私钥到文件..." -ForegroundColor Yellow
    
    # 使用 UTF8 编码（无 BOM）保存
    [System.IO.File]::WriteAllText(
        $SecretFile, 
        $privateKey, 
        [System.Text.UTF8Encoding]::new($false)
    )
    
    Write-Host "✅ 私钥已成功导出！" -ForegroundColor Green
    Write-Host ""
    Write-Host "📄 文件位置：$SecretFile" -ForegroundColor Cyan
    Write-Host "📏 文件大小：$((Get-Item $SecretFile).Length) 字节" -ForegroundColor Gray
    Write-Host ""
    
    # 提示设置权限
    Write-Host "🔒 安全建议：设置文件访问权限" -ForegroundColor Yellow
    Write-Host "   运行以下命令（需要管理员权限）：" -ForegroundColor Gray
    Write-Host ""
    Write-Host '   $acl = Get-Acl "'$SecretFile'"' -ForegroundColor Gray
    Write-Host '   $acl.SetAccessRuleProtection($true, $false)' -ForegroundColor Gray
    Write-Host '   $rule = New-Object System.Security.AccessControl.FileSystemAccessRule($env:USERNAME, "Read", "Allow")' -ForegroundColor Gray
    Write-Host '   $acl.AddAccessRule($rule)' -ForegroundColor Gray
    Write-Host '   Set-Acl "'$SecretFile'" $acl' -ForegroundColor Gray
    Write-Host ""
    
} catch {
    Write-Host "❌ 保存私钥失败：$_" -ForegroundColor Red
    exit 1
}

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  下一步操作:" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "1️⃣  启动 Docker 容器：" -ForegroundColor White
Write-Host "   cd docker" -ForegroundColor Gray
Write-Host "   .\start.ps1 -Build" -ForegroundColor Gray
Write-Host ""
Write-Host "2️⃣  验证部署：" -ForegroundColor White
Write-Host "   docker-compose ps" -ForegroundColor Gray
Write-Host "   docker-compose logs -f" -ForegroundColor Gray
Write-Host ""
Write-Host "3️⃣  测试 API：" -ForegroundColor White
Write-Host "   curl http://localhost:18889/health" -ForegroundColor Gray
Write-Host ""
