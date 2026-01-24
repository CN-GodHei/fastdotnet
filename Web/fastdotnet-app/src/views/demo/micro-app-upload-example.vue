<template>
  <div class="micro-app-upload-container">
    <el-card header="微应用上传示例">
      <p>此示例展示了如何在微应用中使用全局上传服务</p>
      
      <div class="upload-section">
        <h3>使用全局上传服务</h3>
        <el-upload
          drag
          :auto-upload="false"
          :show-file-list="true"
          :on-change="handleFileChange"
          :on-remove="handleFileRemove"
          accept="image/*,.pdf,.doc,.docx"
        >
          <el-icon class="el-icon--upload"><upload-filled /></el-icon>
          <div class="el-upload__text">
            拖拽文件到此处或<em>点击上传</em>
          </div>
          <template #tip>
            <div class="el-upload__tip">
              支持单个或批量上传，仅支持图片、PDF和Word文档，单个文件不超过10MB
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
              <span v-else>{{ row.errorMessage || '无' }}</span>
            </template>
          </el-table-column>
          <el-table-column prop="storageType" label="存储位置" width="120" />
          <el-table-column label="操作" width="150">
            <template #default="{ row }">
              <el-button 
                v-if="row.url" 
                size="small" 
                @click="copyLink(row.url)"
              >
                复制链接
              </el-button>
            </template>
          </el-table-column>
        </el-table>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { ElMessage, ElNotification } from 'element-plus';
import { UploadFilled } from '@element-plus/icons-vue';
import uploadService from '@/services/uploadService';

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
 * 文件移除处理
 */
const handleFileRemove = (file: any, fileList: any) => {
  const validFiles = fileList
    .filter((f: any) => f.raw)
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

  uploading.value = true;
  uploadResults.value = []; // 清空之前的上传结果

  try {
    // 验证所有文件
    const validationErrors: string[] = [];
    for (const file of selectedFiles.value) {
      const validation = uploadService.validateFile(file, {
        maxSize: 10, // 10MB
        accept: 'image/*,.pdf,.doc,.docx'
      });
      
      if (!validation.valid) {
        validationErrors.push(`${file.name}: ${validation.errors.join(', ')}`);
      }
    }

    if (validationErrors.length > 0) {
      ElMessage.error(`文件验证失败：${validationErrors.join('; ')}`);
      return;
    }

    // 获取当前存储配置以显示信息
    const storageConfig = await uploadService.getCurrentStorageConfig();
    
    // 逐个上传文件
    for (let i = 0; i < selectedFiles.value.length; i++) {
      const file = selectedFiles.value[i];
      
      try {
        // 显示上传进度
        ElNotification({
          title: '上传进度',
          message: `正在上传 ${file.name} (${i + 1}/${selectedFiles.value.length})`,
          type: 'info',
          duration: 3000
        });

        // 执行上传
        const result = await uploadService.uploadFile(file, {
          bucketName: 'micro-app-uploads', // 微应用专用存储桶
          onProgress: (percent) => {
            console.log(`上传进度: ${file.name} - ${percent}%`);
          }
        });

        // 添加成功结果
        uploadResults.value.push({
          fileName: file.name,
          status: 'success',
          url: result.data.url,
          storageType: storageConfig.type || '未知',
          size: file.size,
          type: file.type
        });

        ElNotification({
          title: '上传成功',
          message: `${file.name} 上传成功`,
          type: 'success'
        });
      } catch (error: any) {
        // 添加失败结果
        uploadResults.value.push({
          fileName: file.name,
          status: 'failed',
          errorMessage: error.message || '上传失败',
          storageType: storageConfig.type || '未知'
        });

        ElNotification({
          title: '上传失败',
          message: `${file.name} 上传失败: ${error.message}`,
          type: 'error'
        });
      }
    }
  } catch (error: any) {
    console.error('批量上传失败:', error);
    ElMessage.error('批量上传失败: ' + error.message);
  } finally {
    uploading.value = false;
  }
};

/**
 * 复制链接到剪贴板
 */
const copyLink = async (url: string) => {
  try {
    await navigator.clipboard.writeText(url);
    ElMessage.success('链接已复制到剪贴板！');
  } catch (err) {
    console.error('复制失败:', err);
    ElMessage.error('复制失败！');
  }
};

/**
 * 页面加载时检查上传服务是否可用
 */
onMounted(() => {
  // 检查全局上传服务是否已就绪
  if (!(window as any).__UPLOAD_SERVICE__) {
    ElMessage.warning('全局上传服务未就绪，请检查主应用是否正常运行');
  } else {
    console.log('全局上传服务已就绪');
  }
});
</script>

<style scoped>
.micro-app-upload-container {
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