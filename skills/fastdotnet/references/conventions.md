# 约定与守门（必须遵守）

## 1. C# 约定

- 命名：类型/方法/属性 `PascalCase`，局部变量/参数 `camelCase`，私有字段 `_camelCase`；异步方法 `XxxAsync`。
- 公共 API 补 XML 注释（`GenerateDocumentationFile` 已开，核心项目把 `CS1591` 设为 NoWarn，但对外契约仍要写清"为什么/约束"）。
- 注释解释**为什么**，不复述代码。涉及坑的地方写清楚触发条件（参考 `PluginManager` 里两次 GC 的注释）。
- **可空性**：项目开启 `<Nullable>enable</Nullable>`，`CS8604`/`CS8625` 是 error。
  - 用可空标注表达真实语义（`string?`）、`required`、`ArgumentNullException.ThrowIfNull`。
  - 禁止用 `null!` 让编译器闭嘴；`scripts/check-debt.ps1` 有基线，超过即 CI 失败。
- **禁止新增 `<Compile Remove>`**：它会让"存在的文件"不参与编译，读代码的人会得到错误结论。同样有基线约束。
- 分层：Controller 只做参数校验与编排，业务在 Service；Service 才注入 `IRepository<T>` / `ISqlSugarClient`。
  （`CONTRIBUTING.md` 明确写了这条，来自提交 `d70ea99` 的约定。）
- 异常：业务失败抛 `BusinessException`；不要吞异常后返回 `null`；日志带业务标识。
- 序列化：Web API 用 Newtonsoft（`AddNewtonsoftJsonWithCustomSettings`），不要混用 `System.Text.Json` 的特性假设。

## 2. TypeScript / Vue 约定

- `<script setup lang="ts">` + Composition API；组件文件 PascalCase，页面目录小写。
- 类型优先于 `any`；确实未知时用 `unknown` 再收窄。生成的 API 客户端（`src/api/`）不要手改。
- 公共代码进 `@fastdotnet/shared`，用子路径导入；公共层禁止应用别名 `@/`。
- 样式优先 `<style scoped>` 与 CSS 变量；不要全局覆盖 Element Plus。
- 提交前跑对应应用的 `pnpm build` 与 ESLint（`pnpm lint-fix`，管理端有配置）。

## 3. 文档约定（`docs/` 是站点，不是草稿箱）

- VitePress 链接**不带 `.md` 后缀**：`./安装部署`、`../05-插件开发/插件概述`；图片放 `docs/.vitepress/public/`。
- 代码围栏必须配平；代码块里出现的 `AddXxx(` / `UseXxx(` / `MapXxx(` / `WithXxx(` 会被守门脚本拿去 C# 源码里找，
  **找不到就失败**。外部库 API 登记到 `scripts/doc-api-allowlist.txt`。
- 断链会失败：改文件名/移动文件时同步改所有引用。
- 历史问题登记在 `scripts/doc-baseline.txt`，只允许减少。
- 技能包（`skills/`）**不放在 `docs/` 下**，避免被站点守门当作文档扫描；但里面写的 API 同样必须真实存在。

## 4. 测试约定

- 纯逻辑（工具、算法、缓存键、权限码生成）→ `backend/Fastdotnet.Core.Tests`。
- 宿主行为（过滤器、中间件、序列化、鉴权、端点契约）→ `backend/Fastdotnet.WebApi.Tests`（`WebApplicationFactory<Program>`）。
- 修 Bug 先补能复现的用例；插件改动至少手工验证「加载 → 启用 → 停用 → 卸载」全流程。
- 不为通过测试而放宽断言/跳过用例；跳过要写明原因与跟踪项。

## 5. Git 与 PR

- 从 `develop` 切分支：`feat/xxx`、`fix/xxx`、`docs/xxx`、`refactor/xxx`。
- Conventional Commits：`feat: 新增插件热更新事件通知`、`fix: 修复插件卸载后静态资源 404`、`docs: 补充事件总线说明`。
- 一个 PR 只做一件事；不要混入无关格式化。
- PR 描述写清：**改了什么、为什么改、如何验证**。
- 不提交：`bin/`、`obj/`、`node_modules/`、`.history/`、运行时 `plugins/`、`*.db`、真实密钥/证书/授权文件。

## 6. 安全与数据

- 权限码使用小写点分：`system.user.view`、`pluginA.data.view`；新增权限必须能被 `IPermissionProvider` 提供，
  否则运行时授权一定失败。
- 敏感出参用 `.MaskSensitiveData()`；需要加密响应用 `[EncryptResponse]`，不要自己发明加密协议。
- 多租户/数据权限查询必须在 SQL 层带隔离条件，不要先查全量再在内存过滤。
- 文件上传用 `IStorageService`，不要直接写 `wwwroot` 任意路径。

## 7. Agent 工作约定

- 改代码前先读同目录既有实现，按现有模式写；新增模式要说明理由。
- 每轮改动至少构建一次；涉及公共层/契约再跑测试。
- 结论、排错过程、方案要落成仓库文件（`docs/`、模块 README 或本技能），不要只留在对话里。
- 不确定的事实（包内行为、线上配置）先标注"未验证"，不要编造 API 或命令。
