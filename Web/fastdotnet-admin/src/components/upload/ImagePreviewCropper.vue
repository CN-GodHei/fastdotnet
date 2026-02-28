<template>
  <div class="image-preview-cropper">
    <!-- 图片上传区域 -->
    <el-upload
      v-bind="$attrs"
      :action="uploadAction"
      :headers="uploadHeaders"
      :data="uploadData"
      :before-upload="handleBeforeUpload"
      :http-request="preventAutoUpload"
      :on-success="handleSuccess"
      :on-error="handleError"
      :on-change="handleChange"
      :file-list="fileList"
      :list-type="listType"
      :accept="accept || 'image/*'"
      :disabled="isUploading"
      :limit="limit"
      :on-exceed="handleExceed"
    >
      <slot v-if="$slots.default"></slot>
      <div v-else>
        <el-button v-if="listType === 'text'" type="primary">点击上传</el-button>
        <el-icon v-else-if="listType === 'picture'"><Plus /></el-icon>
        <div v-else-if="listType === 'picture-card'" class="upload-slot-content">
          <el-icon><Plus /></el-icon>
          <div class="upload-text">点击上传</div>
        </div>
      </div>
      <template #tip v-if="$slots.tip">
        <slot name="tip"></slot>
      </template>
    </el-upload>

    <!-- 图片预览对话框 -->
    <el-dialog
      v-model="previewDialogVisible"
      title="图片预览"
      width="80%"
      top="5vh"
      :fullscreen="isMobile"
    >
      <div v-if="selectedImage" class="preview-container">
        <div class="preview-image-wrapper">
          <img 
            :src="selectedImage.previewUrl" 
            :alt="selectedImage.name"
            class="preview-image"
            @load="onImageLoad"
          />
        </div>
        <div class="preview-actions">
          <el-button-group>
            <el-button @click="zoomIn" :icon="ZoomIn" />
            <el-button @click="zoomOut" :icon="ZoomOut" />
            <el-button @click="rotateLeft" :icon="RefreshLeft" />
            <el-button @click="rotateRight" :icon="RefreshRight" />
            <el-button @click="resetTransform" :icon="Refresh" />
          </el-button-group>
        </div>
      </div>
      <template #footer>
        <div class="preview-footer">
          <el-button @click="previewDialogVisible = false">取消</el-button>
          <el-button type="primary" @click="showCropperInterface" v-if="enableCrop">开始裁剪</el-button>
          <el-button type="primary" @click="uploadSelectedImage">直接上传</el-button>
        </div>
      </template>
    </el-dialog>

    <!-- 裁剪对话框 -->
    <el-dialog
      v-model="cropperDialogVisible"
      title="图片裁剪"
      width="80%"
      top="5vh"
      :fullscreen="isMobile"
    >
      <div v-if="selectedImage" class="cropper-container">
        <div class="cropper-wrapper">
          <img 
            ref="cropperImgRef"
            :src="selectedImage.previewUrl"
            alt="cropper image"
            class="cropper-image"
          />
        </div>
        <div class="cropper-preview">
          <div class="preview-title">预览</div>
          <div class="preview-box-wrapper">
            <img 
              :src="croppedPreviewUrl" 
              alt="cropped preview" 
              class="cropped-preview"
              :style="{ width: '120px', height: '120px', objectFit: 'cover' }"
            />
          </div>
          <div class="preview-info">
            <el-form label-position="left" label-width="80px">
              <el-form-item label="宽度:">
                <span>{{ croppedData.width }}px</span>
              </el-form-item>
              <el-form-item label="高度:">
                <span>{{ croppedData.height }}px</span>
              </el-form-item>
              <el-form-item label="比例:">
                <span>{{ cropAspectRatio === 0 ? '自由' : cropAspectRatio + ':1' }}</span>
              </el-form-item>
            </el-form>
          </div>
        </div>
      </div>
      <template #footer>
        <div class="cropper-footer">
          <el-button @click="cropperDialogVisible = false">取消</el-button>
          <el-button @click="resetCropper">重置</el-button>
          <el-button type="primary" @click="getCropData">确认裁剪</el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, reactive, nextTick, onMounted, onUnmounted } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { Plus, ZoomIn, ZoomOut, RefreshLeft, RefreshRight, Refresh } from '@element-plus/icons-vue';
import Cropper from 'cropperjs';
import 'cropperjs/dist/cropper.css';

import { getApiStorageConfig, postApiStorageGetUploadCredential } from '@/api/fd-system-api-admin/Storage';
import { uploadFile as uploadFileUtil } from '@/utils/upload';

interface Props {
  /** 存储桶名称 */
  bucketName?: string;
  /** 最大文件大小(MB) */
  maxSize?: number;
  /** 允许的文件类型 */
  accept?: string;
  /** 列表类型 */
  listType?: 'text' | 'picture' | 'picture-card';
  /** 是否显示存储信息 */
  showStorageInfo?: boolean;
  /** 是否显示上传进度 */
  showProgress?: boolean;
  /** 自定义上传参数 */
  customParams?: Record<string, any>;
  /** 文件数量限制 */
  limit?: number;
  /** 裁剪宽高比 0表示自由比例 */
  cropAspectRatio?: number;
  /** 是否启用裁剪功能 */
  enableCrop?: boolean;
  /** 是否启用预览功能 */
  enablePreview?: boolean;
  /** 是否将图片转换为WebP格式（GIF除外） */
  convertToWebP?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  maxSize: 10, // 10MB
  accept: 'image/*',
  listType: 'picture-card',
  showStorageInfo: true,
  showProgress: true,
  customParams: () => ({}),
  limit: 1,
  cropAspectRatio: 1, // 1:1 比例
  enableCrop: true,
  enablePreview: true,
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

// 图片预览相关
const previewDialogVisible = ref(false);
const cropperDialogVisible = ref(false);
const selectedImage = ref<any>(null);
const fileList = ref<any[]>([]);
const cropperImgRef = ref<HTMLImageElement>();
let cropper: Cropper | null = null;

// 图片变换相关
const scale = ref(1);
const rotation = ref(0);

// 裁剪数据
const croppedData = reactive({
  width: 0,
  height: 0,
  imageUrl: ''
});
const croppedPreviewUrl = ref('');

// 检测是否为移动设备
const isMobile = computed(() => {
  return window.innerWidth <= 768;
});

// 计算属性
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
 * 文件数量超出限制时的处理
 */
const handleExceed = (files: any, fileListParam: any) => {
  ElMessage.warning(`当前限制最多上传 ${props.limit} 个文件`);
};

/**
 * 文件状态改变时的处理
 */
const handleChange = async (uploadFile: any, uploadFileList: any) => {
  if (!props.enablePreview && !props.enableCrop) {
    // 如果不需要预览和裁剪，则直接使用原有上传逻辑
    return;
  }

  if (uploadFile.raw && uploadFile.raw.type.startsWith('image/')) {
    // 创建预览URL
    const previewUrl = URL.createObjectURL(uploadFile.raw);
    
    // 设置选中的图片
    selectedImage.value = {
      ...uploadFile,
      previewUrl,
      originalFile: uploadFile.raw
    };

    // 显示预览对话框
    if (props.enablePreview) {
      previewDialogVisible.value = true;
    } else if (props.enableCrop) {
      // 如果只启用裁剪，跳过预览直接进入裁剪
      previewDialogVisible.value = false;
      await showCropperInterface();
    }
  }
};

/**
 * 图片加载完成后的处理
 */
const onImageLoad = () => {
  // 图片加载完成后可以做一些初始化工作
};

/**
 * 放大
 */
const zoomIn = () => {
  scale.value += 0.1;
};

/**
 * 缩小
 */
const zoomOut = () => {
  scale.value -= 0.1;
  if (scale.value < 0.1) scale.value = 0.1;
};

/**
 * 左旋转
 */
const rotateLeft = () => {
  rotation.value -= 90;
};

/**
 * 右旋转
 */
const rotateRight = () => {
  rotation.value += 90;
};

/**
 * 重置变换
 */
const resetTransform = () => {
  scale.value = 1;
  rotation.value = 0;
};

/**
 * 上传选中的图片（预览后直接上传）
 */
const uploadSelectedImage = async () => {
  if (!selectedImage.value) return;

  try {
    // 检查是否需要转换为WebP格式
    let file = selectedImage.value.originalFile;
    if (props.convertToWebP && file.type !== 'image/gif') {
      file = await convertToWebP(file);
    }
    
    await uploadFile(file);
    previewDialogVisible.value = false;
  } catch (error: any) {
    console.error('上传失败:', error);
    ElMessage.error(error.message || '上传失败');
  }
};

/**
 * 显示裁剪界面
 */
const showCropperInterface = async () => {
  if (!selectedImage.value) return;
  
  previewDialogVisible.value = false;
  cropperDialogVisible.value = true;
  
  // 等待DOM更新后再初始化cropper
  await nextTick();
  initCropper();
};

/**
 * 初始化cropper
 */
const initCropper = () => {
  if (cropperImgRef.value) {
    // 销毁之前的cropper实例
    if (cropper) {
      cropper.destroy();
    }
    
    // 创建新的cropper实例
    cropper = new Cropper(cropperImgRef.value, {
      aspectRatio: props.cropAspectRatio > 0 ? props.cropAspectRatio : undefined,
      viewMode: 1,
      dragMode: 'crop',
      autoCropArea: 0.8,
      zoomable: true,
      scalable: true,
      rotatable: true,
      responsive: true,
      checkCrossOrigin: false,
      crop: (event) => {
        croppedData.width = Math.round(event.detail.width);
        croppedData.height = Math.round(event.detail.height);
      }
    });
  }
};

/**
 * 重置裁剪器
 */
const resetCropper = () => {
  if (cropper) {
    cropper.reset();
  }
};

/**
 * 将图片转换为WebP格式
 */
const convertToWebP = (file: File): Promise<File> => {
  return new Promise((resolve) => {
    const canvas = document.createElement('canvas');
    const ctx = canvas.getContext('2d');
    const img = new Image();
    
    img.onload = () => {
      canvas.width = img.width;
      canvas.height = img.height;
      
      ctx?.drawImage(img, 0, 0);
      
      canvas.toBlob((blob) => {
        if (blob) {
          // 创建新的File对象，使用WebP格式
          const webpFile = new File([blob], file.name.replace(/\.[^/.]+$/, '.webp'), {
            type: 'image/webp',
            lastModified: Date.now()
          });
          resolve(webpFile);
        } else {
          // 如果转换失败，返回原始文件
          resolve(file);
        }
      }, 'image/webp', 0.85); // 使用85%的质量
    };
    
    img.onerror = () => {
      // 如果加载失败，返回原始文件
      resolve(file);
    };
    
    img.src = URL.createObjectURL(file);
  });
};

/**
 * 获取裁剪数据
 */
const getCropData = () => {
  if (!cropper) {
    ElMessage.error('裁剪组件未初始化');
    return;
  }

  // 获取裁剪后的canvas
  const canvas = cropper.getCroppedCanvas();
  if (canvas) {
    // 获取裁剪后的图片数据
    const dataUrl = canvas.toDataURL('image/jpeg');
    croppedPreviewUrl.value = dataUrl;
    
    // 将canvas转换为blob
    canvas.toBlob(async (blob) => {
      if (!blob) return;
      
      // 创建新的文件对象
      let file = new File([blob], `cropped_${selectedImage.value.name.replace(/\.[^/.]+$/, '.webp')}`, { type: 'image/webp' });
      
      // 检查是否需要转换为WebP格式
      if (props.convertToWebP) {
        // 由于裁剪后的图片已经是WebP格式，这里我们保持不变
        // 如果需要其他格式转换逻辑，可以在此处添加
      }
      
      // 更新选中的图片为裁剪后的图片
      selectedImage.value = {
        ...selectedImage.value,
        originalFile: file,
        previewUrl: dataUrl
      };

      // 上传裁剪后的图片
      await uploadFile(file);
      cropperDialogVisible.value = false;
    }, 'image/webp', 0.85); // 使用WebP格式和85%质量
  }
};

/**
 * 阻止自动上传
 */
const preventAutoUpload = async (options: any) => {
  // 阻止自动上传，让用户在预览/裁剪后手动上传
  console.log('自动上传已被阻止，等待用户操作');
};

/**
 * 上传文件
 */
const uploadFile = async (file: File) => {
  isUploading.value = true;
  uploadProgress.value = 0;
  uploadProgressStatus.value = undefined;

  try {
    // 根据存储配置决定上传方式
    if (currentStorageConfig.supportDirectUpload) {
      await customUploadFile(file);
    } else {
      await defaultUploadFile(file);
    }
  } catch (error: any) {
    console.error('文件上传失败:', error);
    emit('error', error, file, []);
    ElMessage.error(error.message || '文件上传失败');
  } finally {
    isUploading.value = false;
    uploadProgress.value = 0;
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
const customUploadFile = async (file: File) => {
  try {
    // 直传到OSS
    const formData = new FormData();
    
    // 添加直传参数
    const uploadParams = currentStorageConfig.config.UploadParams || {};
    for (const [key, value] of Object.entries(uploadParams)) {
      formData.append(key, String(value));
    }
    
    // 添加文件
    formData.append(props.bucketName || 'file', file, file.name);

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

    if (response.ok) {
      // 直传成功
      const result = {
        code: 200,
        data: {
          url: currentStorageConfig.config.FileUrlTemplate?.replace('{filename}', uploadParams.key || file.name),
          fileName: file.name
        },
        message: '上传成功'
      };
      
      emit('success', result, file, []);
      emit('change', result.data.url);
      
      ElMessage.success('文件上传成功');
      return result;
    } else {
      // 直传失败
      throw new Error(`上传失败，HTTP状态码: ${response.status}`);
    }
  } catch (error: any) {
    console.error('文件上传失败:', error);
    throw error;
  }
};

/**
 * 默认上传方法（用于后端代理上传）
 */
const defaultUploadFile = async (file: File) => {
  try {
    const formData = new FormData();
    formData.append(props.bucketName || 'file', file, file.name);
    
    // 添加自定义参数
    for (const [key, value] of Object.entries(props.customParams)) {
      formData.append(key, String(value));
    }
    
    if (props.bucketName) {
      formData.append('bucketName', props.bucketName);
    }

    // 通过统一的上传工具上传
    const response: any = await uploadFileUtil({
      file: file,
      bucketName: props.bucketName,
      onProgress: (percent: number) => {
        uploadProgress.value = percent;
        emit('progress', { percent }, file, []);
      },
      timeout: 60000
    });

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
      
      emit('success', normalizedResponse, file, []);
      emit('change', normalizedResponse.data.url);
      
      ElMessage.success('文件上传成功');
      return normalizedResponse;
    } else {
      throw new Error('上传失败');
    }
  } catch (error: any) {
    console.error('文件上传失败:', error);
    throw error;
  }
};

/**
 * HTTP请求处理器
 */
const handleHttpRequest = async (options: any) => {
  // 使用自定义上传方法
  if (currentStorageConfig.supportDirectUpload) {
    await customUploadFile(options.file);
  } else {
    await defaultUploadFile(options.file);
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

onMounted(async () => {
  // 初始化时加载存储配置
  await loadStorageConfig();
});

onUnmounted(() => {
  // 销毁cropper实例
  if (cropper) {
    cropper.destroy();
  }
});

// 暴露方法给父组件
defineExpose({
  refreshConfig: loadStorageConfig
});
</script>

<style scoped>
.image-preview-cropper {
  width: 100%;
}

.upload-slot-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  text-align: center;
}

.upload-text {
  margin-top: 8px;
  font-size: 12px;
  color: #8c939d;
}

.preview-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  min-height: 400px;
}

.preview-image-wrapper {
  flex: 1;
  width: 100%;
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 300px;
  max-height: 50vh;
  overflow: auto;
  border: 1px solid #e4e7ed;
  border-radius: 4px;
  margin-bottom: 16px;
}

.preview-image {
  max-width: 100%;
  max-height: 50vh;
  object-fit: contain;
}

.preview-actions {
  width: 100%;
  text-align: center;
  margin-bottom: 16px;
}

.preview-footer {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}

.cropper-container {
  display: flex;
  flex-direction: column;
  height: 60vh;
}

@media (min-width: 768px) {
  .cropper-container {
    flex-direction: row;
    height: 60vh;
  }
}

.cropper-wrapper {
  flex: 1;
  height: 100%;
}

.cropper-image {
  max-width: 100%;
  max-height: 60vh;
}

.cropper-preview {
  width: 100%;
  max-width: 200px;
  padding: 16px;
  border-left: 1px solid #e4e7ed;
}

@media (min-width: 768px) {
  .cropper-preview {
    width: 200px;
  }
}

.preview-title {
  font-weight: bold;
  margin-bottom: 16px;
  text-align: center;
}

.preview-box-wrapper {
  width: 120px;
  height: 120px;
  margin: 0 auto 16px;
  overflow: hidden;
  border: 1px solid #e4e7ed;
  border-radius: 4px;
}

.cropped-preview {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.preview-info {
  text-align: left;
}

.cropper-footer {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}
</style>