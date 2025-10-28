# 生成 API 客户端脚本
# 使用 Refitter 生成 API 客户端和 DTO

Write-Host "Generating API client and DTOs..." -ForegroundColor Green

# 导航到 GenerateAPI 项目目录
Set-Location -Path "$PSScriptRoot\GenerateAPI"

# 构建 GenerateAPI 项目（这将触发 Refitter 生成代码）
dotnet build

# 检查生成是否成功
if ($LASTEXITCODE -eq 0) {
    Write-Host "API client generated successfully!" -ForegroundColor Green
    
    # 如果使用多文件生成模式，可以重命名 Contracts.cs 文件
    $contractsFile = "..\Fastdotnet.Desktop\Api\Models\Contracts.cs"
    $newContractsFile = "..\Fastdotnet.Desktop\Api\Models\ApiModels.cs"
    
    if (Test-Path $contractsFile) {
        Write-Host "Renaming Contracts.cs to ApiModels.cs..." -ForegroundColor Yellow
        Move-Item -Path $contractsFile -Destination $newContractsFile -Force
        Write-Host "File renamed successfully!" -ForegroundColor Green
    }
    
    # 如果使用单文件模式，文件名将根据配置或默认规则生成
} else {
    Write-Host "Failed to generate API client!" -ForegroundColor Red
    exit 1
}

Write-Host "Generation complete!" -ForegroundColor Green