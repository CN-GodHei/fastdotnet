#!/usr/bin/env pwsh
# Fastdotnet Docker Management Script
# Usage: .\start.ps1 [-Build] [-Logs] [-Stop] [-Restart] [-Clean]
# docker-compose -f docker-compose.yml -f docker-compose.external-db.yml up -d

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
Write-Host "  Fastdotnet Docker Manager" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Check if docker-compose file exists
if (-not (Test-Path $ComposeFile)) {
    Write-Host "[ERROR] Cannot find $ComposeFile" -ForegroundColor Red
    Write-Host "Please make sure you are running this script from the docker directory" -ForegroundColor Yellow
    exit 1
}

# Check if secret key file exists
$secretFile = "secrets/marketplace_private_key.txt"
if (-not (Test-Path $secretFile)) {
    Write-Host "[WARNING] Secret key file not found: $secretFile" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Please create the secret key file first:" -ForegroundColor Cyan
    Write-Host "  1. Export private key from user-secrets" -ForegroundColor Gray
    Write-Host "  2. Save to secrets/marketplace_private_key.txt" -ForegroundColor Gray
    Write-Host ""
    
    $continue = Read-Host "Continue anyway? (y/n)"
    if ($continue -ne 'y') {
        exit 0
    }
}

if ($Stop) {
    Write-Host "[STOP] Stopping services..." -ForegroundColor Yellow
    docker-compose -f $ComposeFile down
    Write-Host "[DONE] Services stopped" -ForegroundColor Green
    return
}

if ($Clean) {
    Write-Host "[CLEAN] Removing all containers, networks and volumes..." -ForegroundColor Yellow
    docker-compose -f $ComposeFile down -v --remove-orphans
    Write-Host "[DONE] Cleanup completed" -ForegroundColor Green
    return
}

if ($Restart) {
    Write-Host "[RESTART] Restarting services..." -ForegroundColor Yellow
    docker-compose -f $ComposeFile restart
    Write-Host "[DONE] Services restarted" -ForegroundColor Green
    return
}

if ($Build) {
    Write-Host "[BUILD] Building Docker image (no cache)..." -ForegroundColor Yellow
    docker-compose -f $ComposeFile build --no-cache
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "[ERROR] Build failed!" -ForegroundColor Red
        exit 1
    }
    
    Write-Host "[SUCCESS] Build completed" -ForegroundColor Green
} else {
    # If no image exists, try to build it
    $imageExists = docker images fastdotnetwebapi:latest --format "{{.Repository}}"
    if (-not $imageExists) {
        Write-Host "[IMAGE] Image does not exist, starting build..." -ForegroundColor Yellow
        docker-compose -f $ComposeFile build
        
        if ($LASTEXITCODE -ne 0) {
            Write-Host "[ERROR] Build failed!" -ForegroundColor Red
            exit 1
        }
        
        Write-Host "[SUCCESS] Build completed" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "[START] Starting services..." -ForegroundColor Green
docker-compose -f $ComposeFile up -d

if ($LASTEXITCODE -ne 0) {
    Write-Host "[ERROR] Failed to start!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "[STATUS] Checking service status:" -ForegroundColor Cyan
Start-Sleep -Seconds 2
docker-compose -f $ComposeFile ps

Write-Host ""
Write-Host "[LOGS] Viewing logs:" -ForegroundColor Cyan

if ($Logs) {
    Write-Host "(Press Ctrl+C to exit live logs)" -ForegroundColor Gray
    docker-compose -f $ComposeFile logs -f
} else {
    Write-Host "(Last 50 lines of logs)" -ForegroundColor Gray
    Start-Sleep -Seconds 3
    docker-compose -f $ComposeFile logs --tail=50
}

Write-Host ""
Write-Host "[INFO] Common commands:" -ForegroundColor Cyan
Write-Host "  Check status:   docker-compose ps" -ForegroundColor Gray
Write-Host "  View logs:      docker-compose logs -f" -ForegroundColor Gray
Write-Host "  Stop service:   .\start.ps1 -Stop" -ForegroundColor Gray
Write-Host "  Restart:        .\start.ps1 -Restart" -ForegroundColor Gray
Write-Host "  Full cleanup:   .\start.ps1 -Clean" -ForegroundColor Gray
Write-Host "  Enter container: docker exec -it fastdotnetwebapi /bin/bash" -ForegroundColor Gray
Write-Host ""
