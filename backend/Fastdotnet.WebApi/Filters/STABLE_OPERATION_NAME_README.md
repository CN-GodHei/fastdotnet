# 稳定 API 操作名称功能

## 功能说明

为了解决插件 API 因 `ApiUsageScope` 动态路由前缀变化导致的前端生成函数名不稳定问题，引入了稳定操作名称机制。

## 问题背景

插件 API 的路由会根据作用域动态变化：
- `AdminOnly`: `/api/plugins/admin/p{pluginId}/...`
- `AppOnly`: `/api/plugins/app/p{pluginId}/...`
- `Both`: `/api/plugins/shared/p{pluginId}/...`

这导致 Swagger 文档中的路径和生成的前端函数名会随作用域变更而变化，例如：
```typescript
// 旧名称（不稳定，包含动态路由前缀）
postPluginsSharedP11365281228127823UnifiedPayPay
postPluginsAdminP11365281228127823UnifiedPayPay

// 新名称（稳定，kebab-case 格式）
unified-pay-pay
```

## 实现方案

### 后端实现

1. **StableOperationNameFilter** (`Fastdotnet.WebApi/Filters/StableOperationNameFilter.cs`)
   - 为每个 API 操作生成稳定的操作 ID
   - 格式：`{controller}-{method}`（kebab-case，全小写+连字符）
   - 示例：`unified-pay-pay`, `certificate-tools-convert-private-key`
   - **关键设计**：
     1. 不包含 HTTP 方法（GET→POST 变更不影响）
     2. 移除 Async 后缀（C# 实现细节不暴露）
     3. 统一 kebab-case（大小写变化不影响）

2. **注册过滤器** (`SwaggerExtensions.cs`)
   ```csharp
   c.OperationFilter<StableOperationNameFilter>();
   ```

### 前端实现

1. **serviceGenerator.ts** 
   - 读取 `x-stable-operation-id` 扩展字段
   - 使用稳定 ID 作为主函数名
   - 保留原函数名作为 `legacyFunctionName`

2. **serviceController.njk 模板**
   - 生成新函数（使用稳定名称）
   - 添加 `@deprecated` 注释说明旧名称
   - 导出旧名称作为别名（指向新函数）

## 生成效果示例

```typescript
/** 此处后端没有提供注释 POST /api/plugins/shared/p11365281228127823/UnifiedPay/pay */
/** @deprecated 请使用 unified-pay-pay，原函数名: postPluginsSharedP11365281228127823UnifiedPayPay */
export async function unifiedPayPay(
  body: APIModel.UnifiedPayDto,
  options?: { [key: string]: any }
) {
  return request<any>("/api/plugins/shared/p11365281228127823/UnifiedPay/pay", {
    method: "POST",
    headers: {
      "Content-Type": "application/json-patch+json",
    },
    data: body,
    ...(options || {}),
  });
}

/** @deprecated 此函数名已变更，请使用 unified-pay-pay */
export const postPluginsSharedP11365281228127823UnifiedPayPay = unifiedPayPay;
```

## 迁移指南

### 对于新项目
直接使用新生成的稳定函数名即可。

### 对于现有项目

1. **重新生成 API 代码**
   ```bash
   npm run openapi2ts
   ```

2. **查看生成的文件**
   - 新函数名是主要导出
   - 旧函数名作为别名保留，标记为 `@deprecated`
   - TypeScript 会显示废弃警告，提示使用新函数名

3. **逐步迁移**
   ```typescript
   // 旧代码
   import { postPluginsSharedP11365281228127823UnifiedPayPay } from '@/api/Pay/UnifiedPay';
   
   // IDE 会显示警告，提示使用 unified-pay-pay
   
   // 新代码（推荐）
   import { unifiedPayPay } from '@/api/Pay/UnifiedPay';
   ```

4. **批量替换（可选）**
   可以使用 IDE 的全局搜索替换功能，将所有旧函数名替换为新函数名。

## 优势

✅ **稳定性**：函数名不再受路由前缀变化影响  
✅ **向后兼容**：旧函数名作为别名保留，不会立即破坏现有代码  
✅ **平滑迁移**：通过 `@deprecated` 标记引导开发者逐步迁移  
✅ **可追溯性**：注释中保留了原函数名，方便查找和替换  

## 注意事项

1. 非插件 API 不受影响，保持原有行为
2. 旧函数名别名会在未来的主要版本中移除，建议尽快迁移
3. 如果自定义了 `customFunctionName` hook，稳定操作 ID 仍会优先生效
