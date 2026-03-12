#!/usr/bin/env pwsh
# Fastdotnet Docker Image Build Script
# Usage: .\build.ps1 [-NoCache] [-Clean] [-Push] [-RegistryHost <host>] [-Namespace <namespace>] [-Tag <tag>]

param(
    [switch]$NoCache,
    [switch]$Clean,
    [switch]$Push,
    [string]$RegistryHost,
    [string]$Namespace,
    [string]$Tag = "latest"
)

$ErrorActionPreference = "Stop"
Set-Location "$PSScriptRoot"

Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  Fastdotnet Docker Build" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Clean if requested
if ($Clean) {
    Write-Host "Cleaning old images..." -ForegroundColor Yellow
    $container = docker ps -a --filter "name=fastdotnet-api" --format "{{.ID}}"
    if ($container) { docker rm -f $container | Out-Null }
    $image = docker images fastdotnet-api --format "{{.ID}}"
    if ($image) { docker rmi -f $image | Out-Null }
    docker image prune -f | Out-Null
}

# Build
Write-Host "Building Docker image..." -ForegroundColor Yellow
$startTime = Get-Date

$contextPath = Join-Path $PSScriptRoot "."
$dockerfilePath = Join-Path $PSScriptRoot "Dockerfile"

if ($NoCache) {
    & docker build --no-cache -t "fastdotnet-api:$Tag" -f $dockerfilePath $contextPath
} else {
    & docker build -t "fastdotnet-api:$Tag" -f $dockerfilePath $contextPath
}

if ($LASTEXITCODE -ne 0) { 
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1 
}
$duration = (New-TimeSpan -Start $startTime -End (Get-Date)).TotalSeconds
Write-Host "Build successful! Duration: $([math]::Round($duration, 2))s" -ForegroundColor Green
docker images fastdotnet-api:$Tag

# Push if requested
if ($Push) {
    Write-Host "`nPushing image to registry..." -ForegroundColor Yellow
    if ([string]::IsNullOrWhiteSpace($RegistryHost) -or [string]::IsNullOrWhiteSpace($Namespace)) {
        Write-Host "Please provide -RegistryHost and -Namespace parameters" -ForegroundColor Yellow
        Write-Host "Example: .\build.ps1 -Push -RegistryHost `"registry.cn-hangzhou.aliyuncs.com`" -Namespace `"your-namespace`"" -ForegroundColor Gray
    } else {
        $fullName = "$RegistryHost/$Namespace/fastdotnet-api:$Tag"
        docker tag "fastdotnet-api:$Tag" $fullName
        if ($LASTEXITCODE -eq 0) {
            docker push $fullName
            if ($LASTEXITCODE -eq 0) { 
                Write-Host "Push successful!" -ForegroundColor Green 
            } else { 
                Write-Host "Push failed!" -ForegroundColor Red 
            }
        }
    }
}

Write-Host "`nDone!" -ForegroundColor Green
Write-Host "Start: cd docker; docker-compose up -d" -ForegroundColor Cyan
Write-Host "Test: curl http://localhost:18889/health" -ForegroundColor Cyan
Write-Host ""
