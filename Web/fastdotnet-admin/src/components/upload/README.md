# GlobalFileUploader 组件使用指南

## 概述

`GlobalFileUploader` 是一个通用的文件上传组件，支持多种存储后端（本地存储、阿里云OSS、腾讯云COS、AWS S3、MinIO、七牛云等），并根据系统当前配置自动选择上传方式。

## 功能特性

- **统一接口**：无论后端使用何种存储服务，前端都使用相同的接口
- **智能切换**：根据系统配置自动选择后端代理上传或前端直传
- **插件化支持**：无缝集成各种OSS插件
- **微应用兼容**：可在微应用中使用主应用的上传服务
- **响应格式兼容**：自动适配前后端不同的响应格式
- **类型校验增强**：支持通配符类型的文件校验（如image/*）

## Props

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| bucketName | string | - | 存储桶名称 |
| maxSize | number | 10 | 最大文件大小(MB) |
| accept | string | '' | 允许的文件类型，如 `.jpg,.png,image/*` |
| showStorageInfo | boolean | true | 是否显示当前存储信息区域 |
| showStorageTypeLabel | boolean | true | 是否显示当前存储类型标签（需同时设置showStorageInfo为true） |
| showProgress | boolean | true | 是否显示上传进度 |
| customParams | Record<string, any> | {} | 自定义上传参数 |

## 使用示例

### 基本用法

```vue
<template>
  <GlobalFileUploader
    :max-size="5"
    accept=".jpg,.png,.pdf"
    @success="onSuccess"
    @error="onError"
  >
    <el-button type="primary">点击上传</el-button>
    <template #tip>
      <div class="el-upload__tip">
        只能上传jpg/png/pdf文件，且不超过5MB
      </div>
    </template>
  </GlobalFileUploader>
</template>

<script setup lang="ts">
import GlobalFileUploader from '@/components/upload/GlobalFileUploader.vue';

const onSuccess = (response: any, file: any, fileList: any) => {
  console.log('上传成功:', response);
};

const onError = (error: any, file: any, fileList: any) => {
  console.error('上传失败:', error);
};
</script>
```

### 图片上传

```vue
<GlobalFileUploader
  :max-size="2"
  accept="image/*"
  list-type="picture-card"
>
  <el-icon><Plus /></el-icon>
</GlobalFileUploader>
```

### 隐藏存储类型标签

```vue
<GlobalFileUploader
  :show-storage-type-label="false"  // 隐藏存储类型标签
  :max-size="5"
  accept=".jpg,.png,.pdf"
>
  <el-button type="primary">点击上传</el-button>
</GlobalFileUploader>
```

### 高级用法

```vue
<GlobalFileUploader
  bucket-name="user-avatars"
  :max-size="10"
  accept="*"
  :custom-params="{ userId: '12345', category: 'avatar' }"
>
  <el-button type="success">上传头像</el-button>
</GlobalFileUploader>
```

## 工具函数

### upload.ts 提供的工具函数

```ts
import { uploadFile, getCurrentStorageConfig, getUploadCredential } from '@/utils/upload';

// 上传单个文件
await uploadFile({
  file: myFile,
  bucketName: 'my-bucket',
  onProgress: (percent) => console.log(percent)
});

// 获取当前存储配置
const config = await getCurrentStorageConfig();

// 获取上传凭证
const credential = await getUploadCredential({
  fileName: 'test.jpg',
  fileSize: 102400,
  contentType: 'image/jpeg',
  bucketName: 'my-bucket'
});
```

### 微应用上传工具

在微应用中使用主应用的上传服务：

```ts
import { 
  microAppUploadFile, 
  microAppUploadFiles, 
  microAppGetCurrentStorageConfig,
  setMicroAppProps 
} from '@/utils/microAppUpload';

// 设置微应用props
setMicroAppProps(propsFromMainApp);

// 上传文件
await microAppUploadFile(myFile, { bucketName: 'my-bucket' });
```

## 服务类

### UploadService

提供了单例的服务类，可全局使用：

```ts
import uploadService from '@/services/uploadService';

// 上传文件
await uploadService.uploadFile(file);

// 批量上传
await uploadService.uploadFiles([file1, file2]);

// 验证文件
const { valid, errors } = uploadService.validateFile(file, {
  maxSize: 10,
  accept: 'image/*'
});
```

## API 接口

组件依赖以下后端API：

- `GET /api/storage/config` - 获取当前存储配置
- `POST /api/storage/get-upload-credential` - 获取上传凭证
- `POST /api/storage/upload` - 文件上传接口

## 注意事项

1. 组件会自动检测当前系统使用的存储服务，并相应调整上传方式
2. 对于不支持前端直传的存储服务（如本地存储），会自动使用后端代理上传
3. 在微应用中使用时，需要确保主应用已正确初始化上传服务
4. 文件验证会在上传前进行，包括大小和类型检查
5. 组件会根据后端返回的配置信息智能选择上传方式（直传或代理）
6. 支持多种文件类型校验格式，包括通配符（如image/*）和具体扩展名（如.jpg）
7. 内部自动处理不同后端响应格式的兼容性问题