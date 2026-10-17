#!/usr/bin/env pwsh
<#
.SYNOPSIS
    文档守门脚本（架构治理 Phase 0.5）

.DESCRIPTION
    把「文档不许说谎」变成可执行的检查：
      1. 相对链接必须真实存在
      2. 文档里出现的 API 名必须在代码中真实存在（防止再写出 AddFastdotnet() 这类不存在的接口）
      3. Markdown 代码围栏必须配平

    设计原则：宁可漏报，不可误报——否则 CI 会被绕过。

.NOTES
    外部 API（框架/第三方库）请登记到 scripts/doc-api-allowlist.txt
#>
param(
    [switch]$Detailed
)

$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent $PSScriptRoot
Set-Location $repoRoot

$rootDocs = @('README.md', 'README.en.md', 'CONTRIBUTING.md', 'SECURITY.md')
$docsDir = Join-Path $repoRoot 'docs'

$docFiles = [System.Collections.Generic.List[string]]::new()
foreach ($f in $rootDocs) {
    if (Test-Path $f) { $docFiles.Add((Resolve-Path $f).Path) }
}
if (Test-Path $docsDir) {
    Get-ChildItem $docsDir -Recurse -Filter *.md -File |
        Where-Object { $_.FullName -notmatch '[\\/]node_modules[\\/]' } |
        ForEach-Object { $docFiles.Add($_.FullName) }
}

# 代码索引：所有 C# 源文件内容（用于 API 名存在性检查）
$csFiles = Get-ChildItem (Join-Path $repoRoot 'backend') -Recurse -Filter *.cs -File |
    Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' }
$csText = ($csFiles | ForEach-Object { Get-Content $_.FullName -Raw }) -join "`n"

# API 允许清单（外部依赖提供的 API）
$allowList = @()
$allowListPath = Join-Path $PSScriptRoot 'doc-api-allowlist.txt'
if (Test-Path $allowListPath) {
    $allowList = Get-Content $allowListPath |
        Where-Object { $_ -and -not $_.TrimStart().StartsWith('#') } |
        ForEach-Object { $_.Trim() }
}

$errors = [System.Collections.Generic.List[string]]::new()
$linkCount = 0
$apiCount = 0

foreach ($file in $docFiles) {
    $relative = $file.Substring($repoRoot.Length).TrimStart('\', '/')
    $text = Get-Content $file -Raw

    # ---------- 1. 相对链接 ----------
    $targets = @()
    $targets += [regex]::Matches($text, '\]\(([^)]+)\)') | ForEach-Object { $_.Groups[1].Value }
    $targets += [regex]::Matches($text, 'src="([^"]+)"') | ForEach-Object { $_.Groups[1].Value }

    foreach ($target in ($targets | Sort-Object -Unique)) {
        if ($target -match '^(https?:|mailto:|#|data:)') { continue }

        $path = ($target -split '#')[0]
        if ([string]::IsNullOrWhiteSpace($path)) { continue }

        $linkCount++
        $full = Join-Path (Split-Path -Parent $file) $path

        # VitePress 风格链接不带 .md 后缀（如 ./安装部署、../05-插件开发/插件概述），
        # 需要依次尝试：原路径 → path.md → path/index.md
        $candidates = @($full)
        if ([string]::IsNullOrEmpty([System.IO.Path]::GetExtension($path))) {
            $candidates += "$full.md"
            $candidates += (Join-Path $full 'index.md')
        }

        $exists = $false
        foreach ($candidate in $candidates) {
            if (Test-Path $candidate) { $exists = $true; break }
        }

        if (-not $exists) {
            $errors.Add("[$relative] 断链：$target")
        }
    }

    # ---------- 3. 代码围栏配平 ----------
    $fences = ([regex]::Matches($text, '(?m)^```')).Count
    if ($fences % 2 -ne 0) {
        $errors.Add("[$relative] Markdown 代码围栏不配平（共 $fences 个 ```）")
    }

    # ---------- 2. API 名存在性 ----------
    # 只检查代码块内的调用，避免把正文里的普通英文词误判为 API
    $codeBlocks = [regex]::Matches($text, '(?s)```[a-zA-Z]*\r?\n(.*?)```') |
        ForEach-Object { $_.Groups[1].Value }

    foreach ($block in $codeBlocks) {
        foreach ($m in [regex]::Matches($block, '\b((?:Add|Use|Map|With)[A-Z][A-Za-z0-9_]*)\s*\(')) {
            $api = $m.Groups[1].Value
            if ($allowList -contains $api) { continue }

            $apiCount++
            if ($csText -notmatch [regex]::Escape("$api(")) {
                $errors.Add("[$relative] 文档中使用的 API 在代码中不存在：$api()")
            }
        }
    }
}

Write-Host "文档守门检查：$($docFiles.Count) 个文档，$linkCount 个相对链接，$apiCount 个 API 引用"

# ---------- 基线机制 ----------
# 历史遗留问题登记在 scripts/doc-baseline.txt：允许存在，但不允许新增。
# 每修掉一条请从基线中删除一行——基线只应单调递减。
$baselinePath = Join-Path $PSScriptRoot 'doc-baseline.txt'
$baseline = @()
if (Test-Path $baselinePath) {
    $baseline = Get-Content $baselinePath |
        Where-Object { $_ -and -not $_.TrimStart().StartsWith('#') } |
        ForEach-Object { $_.Trim() }
}

$unique = $errors | Sort-Object -Unique
$known = $unique | Where-Object { $baseline -contains $_ }
$new = $unique | Where-Object { $baseline -notcontains $_ }

if ($known.Count -gt 0) {
    Write-Host ""
    Write-Host "已登记的历史问题（$($known.Count) 条，不阻断构建；请逐步清理并从基线删除）：" -ForegroundColor Yellow
    if ($Detailed) { $known | ForEach-Object { Write-Host "  · $_" -ForegroundColor DarkYellow } }
    else { Write-Host "  （使用 -Detailed 查看明细）" -ForegroundColor DarkYellow }
}

# 基线里已不存在的问题 → 提示可以删除该行（避免基线虚高）
$stale = $baseline | Where-Object { $_ -and $unique -notcontains $_ }
if ($stale.Count -gt 0) {
    Write-Host ""
    Write-Host "基线中有 $($stale.Count) 条问题已不存在，请从 doc-baseline.txt 删除：" -ForegroundColor Cyan
    $stale | ForEach-Object { Write-Host "  ✓ $_" -ForegroundColor Cyan }
}

if ($new.Count -gt 0) {
    Write-Host ""
    Write-Host "发现 $($new.Count) 个【新增】问题：" -ForegroundColor Red
    $new | ForEach-Object { Write-Host "  ✗ $_" -ForegroundColor Red }
    exit 1
}

Write-Host "无新增问题 ✅" -ForegroundColor Green
exit 0
