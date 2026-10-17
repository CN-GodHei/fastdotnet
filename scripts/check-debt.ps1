#!/usr/bin/env pwsh
<#
.SYNOPSIS
    技术债守门（架构治理 Phase 1.1 的 debt-guard）

.DESCRIPTION
    核心原则：**不让债务继续增长**比清理历史债务更重要。
    本脚本统计两类隐形债，超过基线即失败：

      1. <Compile Remove> —— 源码里存在但不参与编译的文件，
         读代码会得到错误的心智模型（本次勘察时 30 处）。
      2. null! —— 让编译器闭嘴的手法。DTO 属性上的属于可接受惯例，
         但总数增长意味着有人在用同一手法掩盖真实的可空性问题（本次勘察时 57 处）。

    基线只应单调递减：每清理一处，请同步下调下方基线值。

.NOTES
    在 CI 与本地均可运行：pwsh -File scripts/check-debt.ps1
#>
param(
    [switch]$Detailed
)

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
$backendDir = Join-Path $repoRoot 'backend'

# ===== 基线（只允许下调）=====
$compileRemoveBaseline = 30
$nullForgivingBaseline = 54
# ==========================

$sourceFiles = Get-ChildItem $backendDir -Recurse -Filter *.cs -File |
    Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' }
$csprojFiles = Get-ChildItem $backendDir -Recurse -Filter *.csproj -File |
    Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' }

$compileRemoves = @()
foreach ($proj in $csprojFiles) {
    $matches = Select-String -Path $proj.FullName -Pattern '<Compile Remove'
    foreach ($m in $matches) {
        $compileRemoves += "$($proj.FullName.Substring($repoRoot.Length).TrimStart('\','/')) :: $($m.Line.Trim())"
    }
}

$nullForgiving = @()
foreach ($file in $sourceFiles) {
    $matches = Select-String -Path $file.FullName -Pattern 'null!'
    foreach ($m in $matches) {
        # 排除注释中的提及（否则「在注释里解释 null! 的危害」会被误判为技术债）
        $line = $m.Line
        $idx = $line.IndexOf('null!')
        if ($idx -lt 0) { continue }
        $before = $line.Substring(0, $idx)
        if ($before.Contains('//')) { continue }

        $nullForgiving += "$($file.FullName.Substring($repoRoot.Length).TrimStart('\','/')):$($m.LineNumber)"
    }
}

Write-Host "技术债统计：<Compile Remove> = $($compileRemoves.Count)（基线 $compileRemoveBaseline）｜ null! = $($nullForgiving.Count)（基线 $nullForgivingBaseline）"

$failed = $false

if ($compileRemoves.Count -gt $compileRemoveBaseline) {
    Write-Host ""
    Write-Host "❌ <Compile Remove> 增长：$($compileRemoves.Count) > $compileRemoveBaseline" -ForegroundColor Red
    Write-Host "   新增的隐形债：请删除废弃文件，或恢复编译并修好，不要用 Compile Remove 掩盖。" -ForegroundColor Red
    if ($Detailed) { $compileRemoves | ForEach-Object { Write-Host "   · $_" -ForegroundColor DarkRed } }
    $failed = $true
}

if ($nullForgiving.Count -gt $nullForgivingBaseline) {
    Write-Host ""
    Write-Host "❌ null! 增长：$($nullForgiving.Count) > $nullForgivingBaseline" -ForegroundColor Red
    Write-Host "   注意：注入字段上的 null! 会把编译期错误推迟到运行时（本次勘察发现的 CurrentUser 缺陷即为此类）。" -ForegroundColor Red
    if ($Detailed) { $nullForgiving | ForEach-Object { Write-Host "   · $_" -ForegroundColor DarkRed } }
    $failed = $true
}

if ($compileRemoves.Count -lt $compileRemoveBaseline) {
    Write-Host "👍 <Compile Remove> 已下降 $($compileRemoveBaseline - $compileRemoves.Count) 处，请把基线下调到 $($compileRemoves.Count)。" -ForegroundColor Green
}
if ($nullForgiving.Count -lt $nullForgivingBaseline) {
    Write-Host "👍 null! 已下降 $($nullForgivingBaseline - $nullForgiving.Count) 处，请把基线下调到 $($nullForgiving.Count)。" -ForegroundColor Green
}

if ($failed) { exit 1 }

Write-Host "技术债未增长 ✅" -ForegroundColor Green
exit 0
