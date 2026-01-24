<template>
  <div class="micro-app-upload-simple-container">
    <el-card header="微应用简单上传示例">
      <p>此示例展示了微应用如何使用全局上传服务进行文件上传</p>
      
      <div class="upload-section">
        <el-upload
          drag
          :auto-upload="false"
          :show-file-list="true"
          :on-change="handleFileChange"
          accept="image/*,.pdf"
          multiple
        >
          <el-icon class="el-icon--upload"><upload-filled /></el-icon>
          <div class="el-upload__text">
            拖拽文件到此处或<em>点击上传</em>
          </div>
          <template #tip>
            <div class="el-upload__tip">
              支持多文件上传，仅支持图片和PDF，单个文件不超过5MB
            </div>
          </template>
        </el-upload>
        
        <el-button 
          type="primary" 
          :loading="uploading" 
          @click="startUpload"
          style="margin-top: 20px;"
          :disabled="selectedFiles.length === 0"
        >
          {{ uploading ? '上传中...' : '开始上传' }}
        </el-button>
      </div>
      
      <div class="results-section" v-if="uploadResults.length > 0">
        <h3>上传结果</h3>
        <el-table :data="uploadResults" style="width: 100%; margin-top: 10px;">
          <el-table-column prop="fileName" label="文件名" width="200" />
          <el-table-column prop="status" label="状态" width="100">
            <template #default="{ row }">
              <el-tag :type="row.status === 'success' ? 'success' : 'danger'">
                {{ row.status === 'success' ? '成功' : '失败' }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="url" label="访问链接" show-overflow-tooltip>
            <template #default="{ row }">
              <el-link v-if="row.url" :href="row.url" target="_blank" type="primary">查看文件</el-link>
            </template>
          </el-table-column>
        </el-table>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { ElMessage } from 'element-plus';
import { UploadFilled } from '@element-plus/icons-vue';
import { microAppUploadFile, setMicroAppProps, isUploadServiceAvailable } from '@/utils/microAppUpload';

// 假设这是从主应用传递过来的props
const microAppProps = {
  uploadService: (window as any).__UPLOAD_SERVICE__
};

// 设置微应用props
setMicroAppProps(microAppProps);

// 响应式数据
const selectedFiles = ref<File[]>([]);
const uploading = ref(false);
const uploadResults = ref<any[]>([]);

/**
 * 文件选择变化处理
 */
const handleFileChange = (file: any, fileList: any) => {
  // 只保留有效的文件
  const validFiles = fileList
    .filter((f: any) => f.raw) // 确保是实际的文件对象
    .map((f: any) => f.raw);
  
  selectedFiles.value = validFiles;
};

/**
 * 开始上传
 */
const startUpload = async () => {
  if (selectedFiles.value.length === 0) {
    ElMessage.warning('请先选择要上传的文件');
    return;
  }

  // 验证上传服务是否可用
  if (!isUploadServiceAvailable()) {
    ElMessage.error('上传服务不可用，请联系管理员');
    return;
  }

  uploading.value = true;
  uploadResults.value = []; // 清空之前的上传结果

  try {
    // 逐个上传文件
    for (let i = 0; i < selectedFiles.value.length; i++) {
      const file = selectedFiles.value[i];
      
      try {
        // 验证文件
        const validation = {
          maxSize: 5, // 5MB
          accept: 'image/*,.pdf'
        };

        const sizeInMB = file.size / (1024 * 1024);
        const isSizeValid = sizeInMB <= validation.maxSize;
        
        let isTypeValid = true;
        if (validation.accept) {
          const acceptedTypes = validation.accept.split(',').map(t => t.trim());
          const fileType = file.type;
          const fileExt = '.' + file.name.split('.').pop()?.toLowerCase();
          
          isTypeValid = acceptedTypes.some(acceptType => {
            if (acceptType.startsWith('.')) {
              // 检查扩展名
              return fileExt === acceptType.toLowerCase();
            } else {
              // 检查MIME类型
              return fileType.includes(acceptType);
            }
          });
        }

        if (!isSizeValid) {
          throw new Error(`文件 ${file.name} 超过了${validation.maxSize}MB限制`);
        }
        
        if (!isTypeValid) {
          throw new Error(`文件 ${file.name} 类型不符合要求`);
        }

        // 执行上传
        const result = await microAppUploadFile(file, {
          bucketName: 'micro-app-demo', // 示例存储桶
          onProgress: (percent) => {
            console.log(`上传进度: ${file.name} - ${percent}%`);
          }
        });

        // 添加成功结果
        uploadResults.value.push({
          fileName: file.name,
          status: 'success',
          url: result.data.url,
          size: file.size,
          type: file.type
        });

        ElMessage.success(`${file.name} 上传成功`);
      } catch (error: any) {
        // 添加失败结果
        uploadResults.value.push({
          fileName: file.name,
          status: 'failed',
          error: error.message || '上传失败'
        });

        ElMessage.error(`${file.name} 上传失败: ${error.message}`);
      }
    }
  } catch (error: any) {
    console.error('上传过程中发生错误:', error);
    ElMessage.error('上传失败: ' + error.message);
  } finally {
    uploading.value = false;
  }
};

/**
 * 页面加载时检查上传服务是否可用
 */
onMounted(() => {
  if (isUploadServiceAvailable()) {
    ElMessage.success('上传服务已就绪');
  } else {
    ElMessage.error('上传服务未就绪，请检查主应用');
  }
});
</script>

<style scoped>
.micro-app-upload-simple-container {
  padding: 20px;
}

.upload-section {
  margin-bottom: 30px;
}

.results-section {
  margin-top: 30px;
}

.el-upload__tip {
  font-size: 12px;
  color: #909399;
  margin-top: 7px;
}
</style>