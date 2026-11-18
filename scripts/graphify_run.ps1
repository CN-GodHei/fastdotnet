<#
.SYNOPSIS
  用 opencode 订阅里的模型跑 graphify 全量抽取（AST 免费 + 文档语义用网关模型）。

.DESCRIPTION
  graphify 的 openai 后端把 base_url 写死在 api.openai.com，没有环境变量可改。
  本脚本从 opencode 的 auth.json 取出网关 key，再交给 scripts/graphify_extract.py
  改写 base_url，从而把语义抽取打到自己的 OpenAI 兼容网关（默认 dy）。

.EXAMPLE
  pwsh -File scripts\graphify_run.ps1                      # 全量，默认 dy / glm-5.3-flash
.EXAMPLE
  pwsh -File scripts\graphify_run.ps1 -Model kimi-k3 -MaxConcurrency 4
.EXAMPLE
  pwsh -File scripts\graphify_run.ps1 -Target backend -DryRun   # 只看扫描范围，不抽取
.EXAMPLE
  pwsh -File scripts\graphify_run.ps1 -Clean -Model kimi-k2.8-code  # 改了 .graphifyignore 后必须全量重建
#>
[CmdletBinding()]
param(
    [string]$Target = ".",
    [string]$BaseUrl = "",   # 留空则依次取 $env:GX_BASE_URL → opencode.jsonc 里该 provider 的 baseURL
    [string]$Model = "glm-5.3-flash",
    [string]$Provider = "dy",
    [int]$MaxConcurrency = 2,
    [int]$TokenBudget = 60000,
    [int]$MaxWorkers = 0,
    [switch]$DryRun,
    [switch]$Clean
)

$ErrorActionPreference = 'Stop'
$repo = Split-Path -Parent $PSScriptRoot
$authFile = Join-Path $env:USERPROFILE '.local\share\opencode\auth.json'

if (-not $env:OPENAI_API_KEY) {
    if (-not (Test-Path $authFile)) { throw "找不到 opencode 凭据：$authFile，请先 opencode auth login" }
    $key = (Get-Content $authFile -Raw | ConvertFrom-Json).$Provider.key
    if (-not $key) { throw "auth.json 里没有 provider '$Provider' 的 key（现有：$(((Get-Content $authFile -Raw | ConvertFrom-Json).PSObject.Properties.Name) -join ', ')）" }
    $env:OPENAI_API_KEY = $key
}

# base_url 解析顺序：-BaseUrl 参数 → $env:GX_BASE_URL → opencode.jsonc 里 provider 的 options.baseURL
if (-not $BaseUrl -and $env:GX_BASE_URL) { $BaseUrl = $env:GX_BASE_URL }
if (-not $BaseUrl) {
    $cfg = Join-Path $env:USERPROFILE '.config\opencode\opencode.jsonc'
    if (Test-Path $cfg) {
        $json = (Get-Content $cfg -Raw) -replace '(?m)^\s*//.*$', ''
        try {
            $prov = ($json | ConvertFrom-Json).provider.$Provider
            if ($prov.options.baseURL) { $BaseUrl = $prov.options.baseURL }
        } catch { }
    }
}
if (-not $BaseUrl) { throw "没有 base_url：请用 -BaseUrl 指定，或设置 GX_BASE_URL，或在 opencode.jsonc 里配置 provider '$Provider'" }

$env:GX_BASE_URL = $BaseUrl
$env:GX_MODEL = $Model

python -c "import graphify" 2>&1
if ($LASTEXITCODE -ne 0) { throw "当前 python 里没有 graphify，请先 pip install graphifyy" }

Push-Location $repo
try {
    if ($DryRun) {
        python -c @"
import json
from pathlib import Path
from graphify.detect import detect
r = detect(Path('$Target'))
print(json.dumps({k: v for k, v in r.items() if k in ('total_files','total_words','skipped_sensitive')}, ensure_ascii=False, indent=2))
for k, v in r.get('files', {}).items():
    print(f'{k}: {len(v)}')
"@
        return
    }

    # --Clean：删掉增量基线，强制全量重扫。
    # 改了 .graphifyignore 之后必须这样跑 —— 否则 graphify 走增量模式，
    # 会把「现在已被忽略、但文件还在磁盘上」的旧节点原样保留，看起来像规则没生效。
    if ($Clean) {
        $bk = Join-Path $env:TEMP ("gx-backup-" + (Get-Date -Format 'yyyyMMdd-HHmmss'))
        New-Item -ItemType Directory -Path $bk -Force | Out-Null
        foreach ($f in @('graph.json', 'manifest.json', '.graphify_labels.json')) {
            $src = Join-Path $repo "graphify-out\$f"
            if (Test-Path $src) {
                Copy-Item $src $bk -Force
                Remove-Item $src -Force
            }
        }
        Write-Host "== 已移除增量基线，备份在 $bk" -ForegroundColor Yellow
    }

    $argv = @('scripts/graphify_extract.py', $Target, '--max-concurrency', $MaxConcurrency, '--token-budget', $TokenBudget)
    if ($MaxWorkers -gt 0) { $argv += @('--max-workers', $MaxWorkers) }
    Write-Host "== graphify 全量抽取：$Target  model=$Model  endpoint=$BaseUrl" -ForegroundColor Cyan
    python @argv
    if ($LASTEXITCODE -ne 0) { throw "graphify 抽取失败（exit $LASTEXITCODE）" }
}
finally {
    Pop-Location
}
