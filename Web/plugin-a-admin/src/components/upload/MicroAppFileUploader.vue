<template>
  <div class="micro-app-file-uploader">
    <!-- 上传组件 -->
    <el-upload
      v-bind="$attrs"
      action=""
      :before-upload="handleBeforeUpload"
      :http-request="customUpload"
      :on-success="handleSuccess"
      :on-error="handleError"
      :on-progress="handleProgress"
      :disabled="isUploading"
    >
      <slot>
        <el-button type="primary">

          <span>点击上传</span>
        </el-button>
      </slot>
      <slot name="tip" v-if="$slots.tip"></slot>
    </el-upload>
    
    <!-- 上传进度条 -->
    <el-progress 
      v-if="showProgress && isUploading" 
      :percentage="uploadProgress" 
      :status="uploadProgressStatus"
      :stroke-width="2"
      style="margin-top: 10px;"
    ></el-progress>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { ElMessage, ElProgress, ElButton, ElUpload } from 'element-plus';


import { getSharedUploadService } from '@/main'; // 使用插件从主应用接收到的上传服务

interface Props {
  /** 存储桶名称 */
  bucketName?: string;
  /** 最大文件大小(MB) */
  maxSize?: number;
  /** 允许的文件类型 */
  accept?: string;
  /** 是否显示上传进度 */
  showProgress?: boolean;
  /** 自定义上传参数 */
  customParams?: Record<string, any>;
}

const props = withDefaults(defineProps<Props>(), {
  maxSize: 10, // 10MB
  accept: '', // 默认不限制类型
  showProgress: true,
  customParams: () => ({})
});

const emit = defineEmits(['success', 'error', 'progress', 'change']);

const isUploading = ref(false);
const uploadProgress = ref(0);
const uploadProgressStatus = ref<'success' | 'exception' | undefined>(undefined);

// 从主应用获取共享的上传服务
const uploadService = getSharedUploadService();

if (!uploadService) {
  console.error('共享的上传服务未就绪');
}

/**
 * 上传前检查
 */
const handleBeforeUpload = async (file: File) => {
  if (!uploadService) {
    ElMessage.error('上传服务未就绪，请检查主应用');
    return false;
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

  isUploading.value = true;
  uploadProgress.value = 0;
  uploadProgressStatus.value = undefined;

  return true;
};

/**
 * 自定义上传方法（使用主应用的上传服务）
 */
const customUpload = async (options: {
  file: File;
  filename?: string;
  onError: (error: Error) => void;
  onSuccess: (response: any) => void;
  onProgress: (event: { percent: number }) => void;
}) => {
  if (!uploadService) {
    options.onError(new Error('上传服务未就绪'));
    isUploading.value = false;
    return;
  }

  try {
    // 使用主应用的上传服务上传文件
    const result = await uploadService.uploadFile(options.file, {
      bucketName: props.bucketName,
      onProgress: (percent: number) => {
        uploadProgress.value = percent;
        options.onProgress({ percent });
        emit('progress', { percent }, options.file, []);
      }
    });
    console.log(result)

    // 标准化响应格式
    // 检查 result 是否已经是包含 Url 和 FileName 的对象，或者是一个包含了这些字段的 data 对象
    // 注意：后端返回的属性名通常是 PascalCase
    let url, fileName;
    
    if (result && typeof result === 'object') {
      // 检查 result 是否直接包含 Url 和 FileName (PascalCase)
      if (result.Url && result.FileName) {
        url = result.Url;
        fileName = result.FileName;
      }
      // 检查 result 是否直接包含 url 和 fileName (camelCase)
      else if (result.url && result.fileName) {
        url = result.url;
        fileName = result.fileName;
      } 
      // 检查 result.data 是否包含 Url 和 FileName
      else if (result.data && typeof result.data === 'object' && result.data.Url && result.data.FileName) {
        url = result.data.Url;
        fileName = result.data.FileName;
      }
      // 检查 result.data 是否包含 url 和 fileName
      else if (result.data && typeof result.data === 'object' && result.data.url && result.data.fileName) {
        url = result.data.url;
        fileName = result.data.fileName;
      }
      // 如果 result 是一个简单对象，尝试从中提取，优先使用 PascalCase
      else {
        url = result.Url || result.data?.Url || result.url || result.data?.url;
        fileName = result.FileName || result.data?.FileName || result.fileName || result.data?.fileName || options.file.name;
      }
    }
    
    const normalizedResponse = {
      code: 200,
      data: {
        url: url || '',
        fileName: fileName || options.file.name
      },
      message: '上传成功'
    };

    options.onSuccess(normalizedResponse);
    emit('success', normalizedResponse, options.file, []);
    emit('change', normalizedResponse.data.url);

    ElMessage.success(`${options.file.name} 上传成功`);
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
  // 事件已在customUpload中处理
};

/**
 * 处理上传错误
 */
const handleError = (error: any, file: any, fileList: any) => {
  // 事件已在customUpload中处理
  uploadProgressStatus.value = 'exception';
};

/**
 * 处理上传进度
 */
const handleProgress = (event: any, file: any, fileList: any) => {
  // 事件已在customUpload中处理
};
</script>

<style scoped>
.micro-app-file-uploader {
  width: 100%;
}
</style>