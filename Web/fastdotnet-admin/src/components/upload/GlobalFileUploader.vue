<template>
  <div class="global-file-uploader">
    <!-- 显示当前存储类型 -->
    <div v-if="showStorageInfo && showStorageTypeLabel" class="storage-info">
      <el-tag size="small" :type="storageTypeTagType">{{ storageTypeName }}</el-tag>
    </div>
    
    <!-- 根据文件类型和配置决定是否使用图片预览裁剪组件 -->
    <template v-if="shouldUseImagePreview">
      <ImagePreviewCropper
        :bucket-name="bucketName"
        :max-size="maxSize"
        :accept="accept"
        :show-storage-info="showStorageInfo"
        :show-progress="showProgress"
        :custom-params="customParams"
        :show-storage-type-label="showStorageTypeLabel"
        :enable-preview="enableImagePreview"
        :enable-crop="enableImageCrop"
        :crop-aspect-ratio="cropAspectRatio"
        :list-type="listType"
        :limit="limit"
        :convert-to-web-p="convertToWebP"
        @success="emit('success', $event)"
        @error="emit('error', $event)"
        @progress="emit('progress', $event)"
        @change="emit('change', $event)"
      >
        <slot></slot>
        <template #tip v-if="$slots.tip">
          <slot name="tip"></slot>
        </template>
      </ImagePreviewCropper>
    </template>
    <template v-else>
      <!-- 上传组件 -->
      <el-upload
        v-bind="$attrs"
        :action="uploadAction"
        :headers="uploadHeaders"
        :data="uploadData"
        :before-upload="handleBeforeUpload"
        :http-request="currentStorageConfig.supportDirectUpload ? customUpload : defaultUpload"
        :on-success="handleSuccess"
        :on-error="handleError"
        :on-progress="handleProgress"
        :disabled="isUploading"
        :list-type="listType"
        :limit="limit"
      >
        <slot>
          <el-button type="primary">
            <el-icon><Plus /></el-icon>
            <span>点击上传</span>
          </el-button>
        </slot>
        <slot name="tip" v-if="$slots.tip"></slot>
      </el-upload>
    </template>
    
    <!-- 上传进度条 -->
    <el-progress 
      v-if="showProgress && isUploading" 
      :percentage="uploadProgress" 
      :status="uploadProgressStatus"
      :stroke-width="2"
      style="margin-top: 10px;"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, reactive } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { getApiStorageConfig, postApiStorageGetUploadCredential, postApiStorageUpload } from '@/api/fd-system-api-admin/Storage';
import { Plus } from '@element-plus/icons-vue';

// 按需引入图片预览裁剪组件
import ImagePreviewCropper from './ImagePreviewCropper.vue';

interface Props {
  /** 存储桶名称 */
  bucketName?: string;
  /** 最大文件大小(MB) */
  maxSize?: number;
  /** 允许的文件类型 */
  accept?: string;
  /** 是否显示存储信息 */
  showStorageInfo?: boolean;
  /** 是否显示上传进度 */
  showProgress?: boolean;
  /** 自定义上传参数 */
  customParams?: Record<string, any>;
  /** 是否显示当前存储类型标签 */
  showStorageTypeLabel?: boolean;
  /** 是否启用图片预览功能 */
  enableImagePreview?: boolean;
  /** 是否启用图片裁剪功能 */
  enableImageCrop?: boolean;
  /** 裁剪宽高比 */
  cropAspectRatio?: number;
  /** 上传列表的类型 */
  listType?: 'text' | 'picture' | 'picture-card';
  /** 文件数量限制 */
  limit?: number;
  /** 是否将图片转换为WebP格式（GIF除外） */
  convertToWebP?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  maxSize: 10, // 10MB
  accept: '', // 默认不限制类型
  showStorageInfo: true,
  showProgress: true,
  customParams: () => ({}),
  showStorageTypeLabel: true,
  enableImagePreview: true,
  enableImageCrop: true,
  cropAspectRatio: 1,
  listType: 'picture-card',
  limit: 1,
  convertToWebP: true
});

const emit = defineEmits(['success', 'error', 'progress', 'change']);

// 响应式数据
interface StorageConfig {
  StorageType?: string;
  DefaultBucket?: string;
  Domain?: string;
  SupportDirectUpload?: boolean;
  ConfigParams?: Record<string, any>;
  // 上传凭证相关
  CredentialType?: string;
  UploadUrl?: string;
  ExpiresAt?: string;
  FileUrlTemplate?: string;
  UploadHeaders?: Record<string, string>;
  UploadParams?: Record<string, any>;
}

const currentStorageConfig = reactive({
  type: 'local', // 默认本地存储
  supportDirectUpload: false, // 是否支持前端直传
  config: {} as StorageConfig
});
const isUploading = ref(false);
const uploadProgress = ref(0);
const uploadProgressStatus = ref<'success' | 'exception' | undefined>(undefined);

// 计算属性
const shouldUseImagePreview = computed(() => {
  // 检查是否接受图片类型
  const acceptsImages = props.accept ? 
    props.accept.toLowerCase().includes('image') || props.accept.toLowerCase().includes('jpeg') || 
    props.accept.toLowerCase().includes('jpg') || props.accept.toLowerCase().includes('png') || 
    props.accept.toLowerCase().includes('gif') || props.accept.toLowerCase().includes('bmp') || 
    props.accept.toLowerCase().includes('webp')
    : false;
  
  // 只有在明确接受图片类型且启用预览功能时才使用图片预览组件
  return acceptsImages && (props.enableImagePreview || props.enableImageCrop);
});

const storageTypeName = computed(() => {
  const names: Record<string, string> = {
    'local': '本地存储',
    'aliyun': '阿里云OSS',
    'tencent': '腾讯云COS',
    'aws': 'AWS S3',
    'minio': 'MinIO',
    'qiniu': '七牛云'
  };
  return names[currentStorageConfig.type] || '未知存储';
});

const storageTypeTagType = computed(() => {
  const types: Record<string, '' | 'success' | 'warning' | 'info' | 'danger'> = {
    'local': 'info',
    'aliyun': 'success',
    'tencent': '',
    'aws': 'warning',
    'minio': 'danger',
    'qiniu': 'danger'
  };
  return types[currentStorageConfig.type] || 'info';
});

const uploadAction = computed(() => {
  // 如果支持前端直传，返回空字符串（使用customUpload）
  if (currentStorageConfig.supportDirectUpload) {
    return '';
  }
  // 否则返回后端上传地址
  return '/api/storage/upload';
});

const uploadHeaders = computed(() => {
  // 直传时可能需要额外的认证头
  if (currentStorageConfig.supportDirectUpload) {
    return currentStorageConfig.config.UploadHeaders || {};
  }
  return {};
});

const uploadData = computed(() => {
  const data: Record<string, any> = { ...props.customParams };
  if (props.bucketName) {
    data.bucketName = props.bucketName;
  }
  // 如果是直传，添加直传所需的参数
  if (currentStorageConfig.supportDirectUpload) {
    Object.assign(data, currentStorageConfig.config.UploadParams || {});
  }
  return data;
});

/**
 * 获取当前存储配置
 */
const loadStorageConfig = async () => {
  try {
    const response = await getApiStorageConfig();
    if (response) {
      Object.assign(currentStorageConfig, {
        type: response.StorageType,
        supportDirectUpload: response.SupportDirectUpload,
        config: response
      });
    } else {
      console.error('获取存储配置失败');
      // 降级到本地存储
      currentStorageConfig.type = 'local';
      currentStorageConfig.supportDirectUpload = false;
    }
  } catch (error) {
    console.error('获取存储配置异常:', error);
    // 降级到本地存储
      currentStorageConfig.type = 'local';
      currentStorageConfig.supportDirectUpload = false;
    ElMessage.error('获取存储配置失败，将使用本地上传');
  }
};

/**
 * 上传前检查
 */
const handleBeforeUpload = async (file: File) => {
  // 确保已加载存储配置
  if (currentStorageConfig.type === 'local') {
    await loadStorageConfig();
  }

  // 文件大小检查
  const isValidSize = file.size / 1024 / 1024 < props.maxSize;
  if (!isValidSize) {
    ElMessage.error(`文件大小不能超过 ${props.maxSize}MB!`);
    return false;
  }

  // 文件类型检查
  if (props.accept) {
    const acceptTypes = props.accept.split(',').map(t => t.trim().toLowerCase());
    const fileType = file.type.toLowerCase();
    const fileExt = file.name.toLowerCase().split('.').pop() || '';
    
    const isValidType = acceptTypes.some(acceptType => {
      if (acceptType.startsWith('.')) {
        // 检查扩展名
        return fileExt === acceptType.substring(1);
      } else if (acceptType.endsWith('/*')) {
        // 处理通配符类型，如 'image/*', 'video/*', 'audio/*'
        const generalType = acceptType.slice(0, -1); // 移除 '/*'
        return fileType.startsWith(generalType);
      } else {
        // 检查完整MIME类型
        return fileType.includes(acceptType);
      }
    });
    
    if (!isValidType) {
      ElMessage.error(`不支持的文件类型! 支持: ${props.accept}`);
      return false;
    }
  }

  // 如果支持直传，需要先获取上传凭证
  if (currentStorageConfig.supportDirectUpload) {
    try {
      const credentialResponse = await postApiStorageGetUploadCredential({
        FileName: file.name,
        FileSize: file.size,
        ContentType: file.type,
        BucketName: props.bucketName
      } as any);
      
      if (credentialResponse) {
        // 更新配置
        Object.assign(currentStorageConfig.config, credentialResponse);
      } else {
        throw new Error('获取上传凭证失败');
      }
    } catch (error: any) {
      console.error('获取上传凭证失败:', error);
      ElMessage.error(error.message || '获取上传凭证失败');
      return false;
    }
  }

  return true;
};

/**
 * 自定义上传方法（用于直传）
 */
const customUpload = async (options: {
  file: File;
  filename?: string;
  onError: (error: Error) => void;
  onSuccess: (response: any) => void;
  onProgress: (event: { percent: number }) => void;
}) => {
  isUploading.value = true;
  uploadProgress.value = 0;
  uploadProgressStatus.value = undefined;

  try {
    // 直传到OSS
    const formData = new FormData();
    
    // 添加直传参数
    const uploadParams = currentStorageConfig.config.UploadParams || {};
    for (const [key, value] of Object.entries(uploadParams)) {
      formData.append(key, String(value));
    }
    
    // 添加文件
    formData.append(props.bucketName || 'file', options.file, options.file.name);

    // 执行直传
    const uploadUrl = currentStorageConfig.config.UploadUrl;
    if (!uploadUrl) {
      throw new Error('上传URL未配置');
    }
    const response = await fetch(uploadUrl, {
      method: 'POST',
      body: formData,
      headers: currentStorageConfig.config.UploadHeaders || {}
    });
    console.log(response)
    if (response.ok) {
      // 直传成功
      const result = {
        code: 200,
        data: {
          url: currentStorageConfig.config.FileUrlTemplate?.replace('{filename}', uploadParams.key || options.file.name),
          fileName: options.file.name
        },
        message: '上传成功'
      };
      
      options.onSuccess(result);
      emit('success', result, options.file, []);
      emit('change', result.data.url);
      
      ElMessage.success('文件上传成功');
    } else {
      // 直传失败
      throw new Error(`上传失败，HTTP状态码: ${response.status}`);
    }
  } catch (error: any) {
    console.error('文件上传失败:', error);
    options.onError(error);
    emit('error', error, options.file, []);
    ElMessage.error(error.message || '文件上传失败');
  } finally {
    isUploading.value = false;
    uploadProgress.value = 100;
    uploadProgressStatus.value = 'success';
    setTimeout(() => {
      uploadProgress.value = 0;
    }, 1000);
  }
};

/**
 * 默认上传方法（用于后端代理上传）
 */
const defaultUpload = async (options: {
  file: File;
  filename?: string;
  onError: (error: Error) => void;
  onSuccess: (response: any) => void;
  onProgress: (event: { percent: number }) => void;
}) => {
  isUploading.value = true;
  uploadProgress.value = 0;
  uploadProgressStatus.value = undefined;

  try {
    const formData = new FormData();
    formData.append(props.bucketName || 'file', options.file, options.file.name);
    
    // 添加自定义参数
    for (const [key, value] of Object.entries(props.customParams)) {
      formData.append(key, String(value));
    }
    
    if (props.bucketName) {
      formData.append('bucketName', props.bucketName);
    }

    // 通过API上传
    const params = {
      bucketName: props.bucketName
    };
    const body = {};
    const response: any = await postApiStorageUpload(params, body, options.file, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
      timeout: 60000, // 60秒超时
      onUploadProgress: (progressEvent: any) => {
        const percentCompleted = Math.round(
          (progressEvent.loaded * 100) / progressEvent.total!
        );
        uploadProgress.value = percentCompleted;
        options.onProgress({ percent: percentCompleted });
        emit('progress', { percent: percentCompleted }, options.file, []);
      }
    });
console.log(response)
    // response现在直接是{ Url, FileName }格式
    if (response && response.Url) {
      // 标准化响应格式
      const normalizedResponse = {
        code: 200,
        data: {
          url: response.Url,
          fileName: response.FileName
        },
        message: '上传成功'
      };
      
      options.onSuccess(normalizedResponse);
      emit('success', normalizedResponse, options.file, []);
      emit('change', normalizedResponse.data.url);
      
      ElMessage.success('文件上传成功');
    } else {
      throw new Error('上传失败');
    }
  } catch (error: any) {
    console.error('文件上传失败:', error);
    options.onError(error);
    emit('error', error, options.file, []);
    ElMessage.error(error.message || '文件上传失败');
  } finally {
    isUploading.value = false;
    uploadProgressStatus.value = 'success';
    setTimeout(() => {
      uploadProgress.value = 0;
    }, 1000);
  }
};

/**
 * 处理上传成功
 */
const handleSuccess = (response: any, file: any, fileList: any) => {
  // 事件已在customUpload或defaultUpload中处理
};

/**
 * 处理上传错误
 */
const handleError = (error: any, file: any, fileList: any) => {
  // 事件已在customUpload或defaultUpload中处理
  uploadProgressStatus.value = 'exception';
};

/**
 * 处理上传进度
 */
const handleProgress = (event: any, file: any, fileList: any) => {
  // 事件已在customUpload或defaultUpload中处理
};

onMounted(async () => {
  // 初始化时加载存储配置
  await loadStorageConfig();
});

// 暴露方法给父组件
defineExpose({
  refreshConfig: loadStorageConfig
});
</script>

<style scoped>
.global-file-uploader {
  width: 100%;
}

.storage-info {
  margin-bottom: 10px;
}
</style>