# Fastdotnet Agent 技能包

本目录是随仓库分发的 **agent 技能包（skills）**，让任何支持技能/规则文件的编码 agent 拉取仓库后即可按框架约定开发，而不是靠猜。

## 包里有什么

```
skills/
├── fastdotnet/                    # 框架开发技能（宿主 / 后端 / 前端 / 构建 / 排错）
│   ├── SKILL.md                   # 入口：30 秒上手 + 硬规则 + 导航
│   └── references/                # 架构、后端、前端、构建测试、约定、排错
└── fastdotnet-plugin-dev/         # 插件开发技能（全流程 + 模板 + 验收清单）
    ├── SKILL.md                   # 入口：决策树 + 七步流程 + 铁律
    ├── references/                # 契约生命周期、后端、前端、扩展点、打包调试、清单
    └── assets/
        ├── templates/             # 可直接复制的插件骨架文件
        │                          # （Plugin.csproj.template 故意加后缀，避免被 MSBuild 当成项目自动发现）
        └── new-plugin.ps1         # 一键生成插件骨架（替换 PluginId/命名）
```

根目录的 [`AGENTS.md`](../AGENTS.md) 是通用入口：大多数 agent（Codex、opencode、Cursor、DSH 等）会自动读取，再由它指向这两个技能。

## 技能格式

每个 `SKILL.md` 使用 YAML frontmatter + Markdown，兼容主流 agent 的技能约定：

```markdown
---
name: fastdotnet
description: Use when ...（触发场景，写清楚"什么时候该加载"）
---
```

正文只放"必须立刻知道的"；细节放 `references/` 按需读取，避免一次性塞满上下文。

## 怎么让 agent 用上

### 支持技能目录的 agent（DSH / Claude Code 等）

把技能目录复制或链接到该 agent 的技能根目录，目录名即技能名：

```bash
# DSH：复制到当前预设的 skills 目录（或在 profile 的 agent preset 里引用本仓库路径）
cp -r skills/fastdotnet        <dsh-preset>/skills/
cp -r skills/fastdotnet-plugin-dev <dsh-preset>/skills/

# Claude Code：项目级技能目录
mkdir -p .claude/skills
cp -r skills/fastdotnet .claude/skills/
cp -r skills/fastdotnet-plugin-dev .claude/skills/

# Windows 上不想复制，可用目录联接（改仓库文件即时生效）
cmd /c mklink /J .claude\skills\fastdotnet ..\..\skills\fastdotnet
```

> 技能目录内的相对路径以 `SKILL.md` 所在目录为基准解析；`references/` 与 `assets/` 都是相对路径，复制时需要整体复制。

### 只读 `AGENTS.md` 的 agent（Codex / opencode / 通用 CLI）

无需安装。agent 进入仓库会自动读取根目录 `AGENTS.md`，其中已列出技能路径与硬规则。任务明确要求时，让它先读：

```
先读 skills/fastdotnet-plugin-dev/SKILL.md 和它的 references，再开始改代码。
```

### 纯人工/对话式使用

直接把技能文件内容贴给任意模型即可；`fastdotnet-plugin-dev/assets/templates/` 就是一套可编译的骨架。

## 验证状态（2026-11，本仓库实测）

| 内容 | 结果 |
| --- | --- |
| `assets/new-plugin.ps1` 生成骨架 | 通过 |
| 生成的后端项目 `dotnet build` | 通过（0 error） |
| 生成的管理端 / 应用端 `pnpm install && pnpm build` | 通过（Vite 7 + `vite-plugin-qiankun`，产出 UMD + CSS + index.html） |
| 文档假设的关键行为（路由重写、`[ApiUsageScope]` 必填、影子复制、许可校验、初始化器执行） | 已对照 `Fastdotnet.Plugin.Core 1.1.4` 反编译结果核对 |

已知差异：仓库内 `PluginA` 是历史写法（Vite 5、external 了 Vue/Element Plus、两个 `PluginBase` 实现、`/Plugins/` 大写路径）。
模板有意采用更稳妥的写法（Vite 7、默认不 external、单一入口、小写路径、显式引用 CSS）；照模板走，不要照 PluginA 抄。

## 维护约定

- 技能内容以**仓库当前代码**为准。文档与代码冲突时，以代码为准，并顺手修文档或技能。
- 插件加载器在 NuGet 包 `Fastdotnet.Plugin.Core`（版本见 `backend/Directory.Packages.props`）中；包行为若变化，先反编译核对再改技能。
- 新增扩展点（契约接口、宿主约定）时，同步更新 `fastdotnet-plugin-dev/references/extension-points.md` 的表格。
- 技能文件不放在 `docs/` 下，避免被 `scripts/verify-docs.ps1` 当作站点文档做链接/API 校验；但其中引用的 API 仍应真实存在。
