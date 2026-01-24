<template>
  <div class="file-upload-demo-container">
    <el-card header="文件上传演示">
      <el-row :gutter="20">
        <el-col :span="12">
          <el-card header="基本上传">
            <!-- 基本用法 -->
            <GlobalFileUploader
              :max-size="5"
              accept=".jpg,.png,.pdf"
              :on-success="onBasicSuccess"
              :on-error="onBasicError"
            >
              <el-button type="primary">点击上传</el-button>
              <template #tip>
                <div class="el-upload__tip">
                  只能上传jpg/png/pdf文件，且不超过5MB
                </div>
              </template>
            </GlobalFileUploader>
          </el-card>
        </el-col>
        
        <el-col :span="12">
          <el-card header="图片上传">
            <!-- 图片上传 -->
            <GlobalFileUploader
              :max-size="2"
              accept="image/*"
              list-type="picture-card"
              :on-success="onImageSuccess"
              :on-error="onBasicError"
            >
              <el-icon><Plus /></el-icon>
            </GlobalFileUploader>
          </el-card>
        </el-col>
      </el-col>
    </el-row>

    <el-row :gutter="20" style="margin-top: 20px;">
      <el-col :span="24">
        <el-card header="高级用法">
          <!-- 高级用法：自定义参数、指定存储桶等 -->
          <GlobalFileUploader
            bucket-name="user-avatars"
            :max-size="10"
            accept="*"
            :custom-params="{ userId: '12345', category: 'avatar' }"
            :on-success="onAdvancedSuccess"
            :on-error="onBasicError"
          >
            <el-button type="success">上传头像</el-button>
            <template #tip>
              <div class="el-upload__tip">
                上传用户头像，最大10MB，支持所有格式
              </div>
            </template>
          </GlobalFileUploader>
        </el-card>
      </el-col>
    </el-row>

    <!-- 上传结果展示 -->
    <el-row :gutter="20" style="margin-top: 20px;">
      <el-col :span="24">
        <el-card header="上传结果">
          <el-table :data="uploadResults" style="width: 100%">
            <el-table-column prop="fileName" label="文件名" width="200" />
            <el-table-column prop="url" label="访问链接" show-overflow-tooltip>
              <template #default="{ row }">
                <el-link :href="row.url" target="_blank" type="primary">{{ row.url }}</el-link>
              </template>
            </el-table-column>
            <el-table-column prop="storageType" label="存储类型" width="120" />
            <el-table-column label="操作" width="150">
              <template #default="{ row }">
                <el-button size="small" @click="copyLink(row.url)">复制链接</el-button>
              </template>
            </el-table-column>
          </el-table>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { ElMessage, ElNotification } from 'element-plus';
import { Plus } from '@element-plus/icons-vue';
import GlobalFileUploader from '@/components/upload/GlobalFileUploader.vue';

// 上传结果列表
const uploadResults = ref<any[]>([]);

// 基本上传成功回调
const onBasicSuccess = (response: any, file: any, fileList: any) => {
  console.log('基本上传成功:', response);
  uploadResults.value.unshift({
    fileName: response.data.fileName,
    url: response.data.url,
    storageType: getCurrentStorageType(),
    timestamp: new Date()
  });
  ElMessage.success(`${response.data.fileName} 上传成功！`);
};

// 图片上传成功回调
const onImageSuccess = (response: any, file: any, fileList: any) => {
  console.log('图片上传成功:', response);
  uploadResults.value.unshift({
    fileName: response.data.fileName,
    url: response.data.url,
    storageType: getCurrentStorageType(),
    timestamp: new Date()
  });
  ElMessage.success(`${response.data.fileName} 上传成功！`);
};

// 高级上传成功回调
const onAdvancedSuccess = (response: any, file: any, fileList: any) => {
  console.log('高级上传成功:', response);
  uploadResults.value.unshift({
    fileName: response.data.fileName,
    url: response.data.url,
    storageType: getCurrentStorageType(),
    timestamp: new Date()
  });
  ElNotification({
    title: '上传成功',
    message: `${response.data.fileName} 已上传至 ${getCurrentStorageType()} 存储`,
    type: 'success'
  });
};

// 上传失败回调
const onBasicError = (error: any, file: any, fileList: any) => {
  console.error('上传失败:', error);
  ElMessage.error('文件上传失败，请重试！');
};

// 获取当前存储类型（模拟）
const getCurrentStorageType = (): string => {
  // 这里应该从全局状态或API获取实际的存储类型
  // 暂时返回模拟值
  return '本地存储'; // 或 '阿里云OSS', '腾讯云COS' 等
};

// 复制链接
const copyLink = async (url: string) => {
  try {
    await navigator.clipboard.writeText(url);
    ElMessage.success('链接已复制到剪贴板！');
  } catch (err) {
    console.error('复制失败:', err);
    ElMessage.error('复制失败！');
  }
};
</script>

<style scoped>
.file-upload-demo-container {
  padding: 20px;
}

.demo-card {
  margin-bottom: 20px;
}

.el-upload__tip {
  font-size: 12px;
  color: #909399;
  margin-top: 7px;
}
</style>