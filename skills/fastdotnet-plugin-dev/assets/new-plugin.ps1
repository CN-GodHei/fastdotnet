<#
.SYNOPSIS
    从技能包模板生成一个 Fastdotnet 插件骨架（后端 + 可选双前端）。

.EXAMPLE
    pwsh -File skills/fastdotnet-plugin-dev/assets/new-plugin.ps1 `
      -Name MyPlugin -PluginId 12345678901234567 -DisplayName "我的插件"

.NOTES
    - 生成后仍需：从商城获取 plugin.lic、把 publish/{PluginId} 部署到宿主运行目录 plugins/、启用插件。
    - 不会覆盖已存在目录，除非显式 -Force（只允许删除 OutputRoot 下的目标目录）。
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidatePattern('^[A-Za-z][A-Za-z0-9]{1,63}$')]
    [string]$Name,

    [Parameter(Mandatory = $true)]
    [ValidatePattern('^[A-Za-z0-9_-]{3,64}$')]
    [string]$PluginId,

    [string]$DisplayName,
    [string]$Description = "基于 Fastdotnet 的插件",
    [string]$Author = "Your Name",
    [string]$OutputRoot,
    [switch]$IncludeFrontend,
    [switch]$Force
)

$ErrorActionPreference = 'Stop'

$scriptDir = $PSScriptRoot
$templateDir = Join-Path $scriptDir 'templates'
$repoRoot = (Resolve-Path (Join-Path $scriptDir '..\..\..')).Path
if (-not $OutputRoot) { $OutputRoot = Join-Path $repoRoot 'backend\Plugins' }
$OutputRoot = [System.IO.Path]::GetFullPath($OutputRoot)

if (-not $DisplayName) { $DisplayName = $Name }
$slug = ($Name.ToLowerInvariant() -replace '[^a-z0-9]', '')
if (-not $slug) { $slug = 'plugin' }

$pluginDir = [System.IO.Path]::GetFullPath((Join-Path $OutputRoot $Name))
if (-not $pluginDir.StartsWith($OutputRoot + [System.IO.Path]::DirectorySeparatorChar, [System.StringComparison]::OrdinalIgnoreCase)) {
    throw "目标路径不在 OutputRoot 下，拒绝执行：$pluginDir"
}
if (Test-Path $pluginDir) {
    if (-not $Force) { throw "目录已存在：$pluginDir（如需重建请加 -Force）" }
    Write-Host "删除已存在目录：$pluginDir" -ForegroundColor Yellow
    Remove-Item -LiteralPath $pluginDir -Recurse -Force
}

if (-not (Test-Path $templateDir)) { throw "模板目录不存在：$templateDir" }

$tokens = [ordered]@{
    '__PLUGIN_NAME__'  = $Name
    '__PLUGIN_ID__'    = $PluginId
    '__PLUGIN_SLUG__'  = $slug
    '__DISPLAY_NAME__' = $DisplayName
    '__DESCRIPTION__'  = $Description
    '__AUTHOR__'       = $Author
    '__YEAR__'         = (Get-Date).Year
}

function New-FromTemplate {
    param(
        [string]$TemplateFile,
        [string]$Destination,
        [hashtable]$ExtraTokens = @{}
    )
    $source = Join-Path $templateDir $TemplateFile
    if (-not (Test-Path $source)) { throw "缺少模板：$source" }
    $content = Get-Content -LiteralPath $source -Raw
    foreach ($key in $tokens.Keys) { $content = $content.Replace($key, [string]$tokens[$key]) }
    foreach ($key in $ExtraTokens.Keys) { $content = $content.Replace($key, [string]$ExtraTokens[$key]) }
    $destDir = Split-Path -Parent $Destination
    if (-not (Test-Path $destDir)) { New-Item -ItemType Directory -Force -Path $destDir | Out-Null }
    Set-Content -LiteralPath $Destination -Value $content -Encoding utf8
}

# ---------------- 后端 ----------------
$backend = Join-Path $pluginDir 'Backend'
$dirs = @(
    $backend,
    (Join-Path $backend 'Controllers'),
    (Join-Path $backend 'Services'),
    (Join-Path $backend 'Entities'),
    (Join-Path $backend 'Dto'),
    (Join-Path $backend 'Initializers'),
    (Join-Path $backend 'Mappings'),
    (Join-Path $backend 'Providers'),
    (Join-Path $backend 'EventHandlers')
)
foreach ($d in $dirs) { New-Item -ItemType Directory -Force -Path $d | Out-Null }

# 固定名文件（无需改文件名）
$backendSimple = @{
    'GlobalUsings.cs'    = 'GlobalUsings.cs'
    'MyThing.cs'         = 'Entities\MyThing.cs'
    'MyThingDtos.cs'     = 'Dto\MyThingDtos.cs'
    'MyThingService.cs'  = 'Services\MyThingService.cs'
    'MyThingController.cs' = 'Controllers\MyThingController.cs'
    'MappingRegistry.cs' = 'Mappings\MappingRegistry.cs'
}
foreach ($pair in $backendSimple.GetEnumerator()) {
    New-FromTemplate -TemplateFile $pair.Key -Destination (Join-Path $backend $pair.Value)
}

# 需要按插件名改名的文件
$backendRenamed = @{
    'Plugin.csproj.template'   = "$Name.csproj"
    'Plugin.cs'                = "$($Name)Plugin.cs"
    'Permissions.cs'           = "$($Name)Permissions.cs"
    'PermissionProvider.cs'    = "$($Name)PermissionProvider.cs"
    'ToolProvider.cs'          = "$($Name)ToolProvider.cs"
    'MenuInitializer.cs'       = "Initializers\$($Name)MenuInitializer.cs"
    'plugin.json'              = 'plugin.json'
}
foreach ($pair in $backendRenamed.GetEnumerator()) {
    New-FromTemplate -TemplateFile $pair.Key -Destination (Join-Path $backend $pair.Value)
}

# ---------------- 前端 ----------------
if ($IncludeFrontend) {
    $frontends = @(
        @{ Segment = 'admin'; Port = 8099; Suffix = 'admin' },
        @{ Segment = 'app';   Port = 8100; Suffix = 'app' }
    )

    foreach ($fe in $frontends) {
        $proj = Join-Path $pluginDir ("Frontend\{0}.{1}" -f $Name, $fe.Suffix)
        $extra = @{
            '__FRONTEND_SEGMENT__' = $fe.Segment
            '__FRONTEND_PORT__'    = $fe.Port
        }

        New-FromTemplate -TemplateFile 'index.html'           -Destination (Join-Path $proj 'index.html') -ExtraTokens $extra
        New-FromTemplate -TemplateFile 'micro-index.html'     -Destination (Join-Path $proj 'micro-index.html') -ExtraTokens $extra
        New-FromTemplate -TemplateFile 'vite.config.ts'       -Destination (Join-Path $proj 'vite.config.ts') -ExtraTokens $extra
        New-FromTemplate -TemplateFile 'package.json'         -Destination (Join-Path $proj 'package.json') -ExtraTokens $extra
        New-FromTemplate -TemplateFile 'pnpm-workspace.yaml'  -Destination (Join-Path $proj 'pnpm-workspace.yaml') -ExtraTokens $extra
        New-FromTemplate -TemplateFile 'tsconfig.json'        -Destination (Join-Path $proj 'tsconfig.json') -ExtraTokens $extra
        New-FromTemplate -TemplateFile 'openapi2ts.config.ts' -Destination (Join-Path $proj 'openapi2ts.config.ts') -ExtraTokens $extra
        New-FromTemplate -TemplateFile 'frontend-plugin.json' -Destination (Join-Path $proj 'plugin.json') -ExtraTokens $extra
        New-FromTemplate -TemplateFile 'main.ts'              -Destination (Join-Path $proj 'src\main.ts') -ExtraTokens $extra
        New-FromTemplate -TemplateFile 'micro-main.ts'        -Destination (Join-Path $proj 'src\micro-main.ts') -ExtraTokens $extra
        New-FromTemplate -TemplateFile 'App.vue'              -Destination (Join-Path $proj 'src\App.vue') -ExtraTokens $extra
        New-FromTemplate -TemplateFile 'Home.vue'             -Destination (Join-Path $proj 'src\views\Home.vue') -ExtraTokens $extra
        New-FromTemplate -TemplateFile 'router.ts'            -Destination (Join-Path $proj 'src\router\index.ts') -ExtraTokens $extra
        New-FromTemplate -TemplateFile 'fdRequestAdapter.ts'  -Destination (Join-Path $proj 'src\utils\fdRequestAdapter.ts') -ExtraTokens $extra

        $envDts = @"
/// <reference types="vite/client" />

declare module '*.vue' {
  import type { DefineComponent } from 'vue'
  const component: DefineComponent<{}, {}, any>
  export default component
}
"@
        Set-Content -LiteralPath (Join-Path $proj 'src\env.d.ts') -Value $envDts -Encoding utf8
    }
}

# ---------------- .gitignore ----------------
$gitignore = @"
bin/
obj/
publish/
node_modules/
dist/
.buildtmp/
*.user
"@
Set-Content -LiteralPath (Join-Path $pluginDir '.gitignore') -Value $gitignore -Encoding utf8

# ---------------- README ----------------
$readme = @"
# $DisplayName

Fastdotnet 插件（PluginId: ``$PluginId``）。

## 构建

``````powershell
dotnet build Backend/$Name.csproj -c Release
``````

产物在 ``publish/$PluginId/``。

## 部署与启用

``````powershell
`$dst = "backend/Fastdotnet.WebApi/bin/Debug/net10.0/plugins/$PluginId"
New-Item -ItemType Directory -Force `$dst | Out-Null
Copy-Item "publish/$PluginId/*" `$dst -Recurse -Force
# 把从商城获取的 plugin.lic 放进 `$dst
Invoke-RestMethod -Method Post "http://localhost:18889/api/Plugin/enable/$PluginId" -Headers @{ Authorization = "Bearer <token>" }
``````

## 接口

宿主会把控制器路由重写为 ``/api/plugins/{admin|app|shared}/p$PluginId/...``。
示例：``GET /api/plugins/admin/p$PluginId/MyThing``。

## 前端（如已生成）

``````powershell
cd Frontend/$Name.admin
pnpm install
pnpm build      # 输出到 ../../publish/$PluginId/wwwroot/admin
``````
"@
Set-Content -LiteralPath (Join-Path $pluginDir 'README.md') -Value $readme -Encoding utf8

Write-Host ""
Write-Host "已生成插件骨架：" -ForegroundColor Green
Write-Host "  $pluginDir"
Write-Host ""
Write-Host "下一步：" -ForegroundColor Cyan
Write-Host "  1) 修改 Backend/plugin.json 与 csproj 的说明/作者（PluginId 已填好）"
Write-Host "  2) dotnet build Backend/$Name.csproj -c Release"
Write-Host "  3) 把 publish/$PluginId 复制到宿主运行目录 plugins/$PluginId，并放入 plugin.lic"
Write-Host "  4) 启用插件：POST /api/Plugin/enable/$PluginId"
if ($IncludeFrontend) {
    Write-Host "  5) 前端：cd Frontend/$Name.admin && pnpm install && pnpm build"
}
Write-Host ""
