<template>
  <div class="storage-demo">
    <el-card class="box-card">
      <template #header>
        <div class="card-header">
          <span>存储服务完整示例</span>
          <el-tag type="success">上传 + 删除 + 管理</el-tag>
        </div>
      </template>

      <!-- 配置信息 -->
      <el-alert
        title="提示"
        type="info"
        :closable="false"
        style="margin-bottom: 20px"
      >
        <p>本示例演示完整的文件存储操作,包括上传、删除等功能</p>
        <p>系统会自动根据配置选择<strong>前端直传</strong>或<strong>后端代理</strong>模式</p>
        <p>特殊场景可通过 <code>force-proxy</code> 属性强制走后端代理(如文件内容校验、权限控制等)</p>
      </el-alert>

      <!-- 上传区域 -->
      <el-form label-width="120px">
        <el-form-item label="存储路径前缀(可选)">
          <el-input 
            v-model="pathPrefix" 
            placeholder="留空使用默认路径，如：plugin-icons/"
            clearable
          />
        </el-form-item>

        <el-form-item label="基本上传">
          <MicroAppFileUploader
            :path-prefix="pathPrefix || undefined"
            :max-size="10"
            @success="onUploadSuccess"
            @error="onUploadError"
          >
            <el-button type="primary">点击上传</el-button>
          </MicroAppFileUploader>
        </el-form-item>

        <el-form-item label="强制代理上传">
          <MicroAppFileUploader
            :path-prefix="pathPrefix || undefined"
            :max-size="10"
            :force-proxy="true"
            @success="onProxyUploadSuccess"
            @error="onUploadError"
          >
            <el-button type="warning">强制走后端代理</el-button>
          </MicroAppFileUploader>
          <div class="form-tip">
            适用场景: 需要后端进行文件校验、权限控制、敏感文件处理等
          </div>
        </el-form-item>

        <el-form-item label="图片上传">
          <MicroAppFileUploader
            :path-prefix="pathPrefix || undefined"
            :max-size="5"
            accept="image/*"
            list-type="picture-card"
            @success="onImageUploadSuccess"
            @error="onUploadError"
          >
            <span>+</span>
          </MicroAppFileUploader>
        </el-form-item>
      </el-form>

      <!-- 上传结果 -->
      <div v-if="uploadResults.length > 0" class="result-section">
        <h3>上传结果 ({{ uploadResults.length }})</h3>
        <el-table :data="uploadResults" border stripe>
          <el-table-column prop="fileName" label="文件名" width="200" />
          <el-table-column prop="url" label="访问URL">
            <template #default="{ row }">
              <el-link :href="row.url" target="_blank" type="primary">
                {{ row.url }}
              </el-link>
            </template>
          </el-table-column>
          <el-table-column prop="type" label="类型" width="80" />
          <el-table-column prop="mode" label="上传模式" width="90">
            <template #default="{ row }">
              <el-tag :type="row.mode === '强制代理' ? 'warning' : 'success'" size="small">
                {{ row.mode }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="time" label="时间" width="120" />
          <el-table-column label="操作" width="150">
            <template #default="{ row }">
              <el-button size="small" @click="copyUrl(row.url)">复制</el-button>
              <el-button size="small" type="danger" @click="removeResult(row)">删除</el-button>
            </template>
          </el-table-column>
        </el-table>
        
        <!-- 图片预览 -->
        <div v-if="lastImageUrl" class="image-preview">
          <p><strong>最新图片预览:</strong></p>
          <el-image 
            :src="lastImageUrl" 
            fit="contain"
            style="max-width: 300px; max-height: 300px"
          />
        </div>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
// 导入全局上传组件
import MicroAppFileUploader from '@/components/upload/MicroAppFileUploader.vue';
// 导入共享的上传服务
import { getSharedUploadService } from '@/main';

// 存储路径前缀
const pathPrefix = ref('');

// 上传结果列表
interface UploadResult {
  id: number;
  fileName: string;
  url: string;
  type: string;
  time: string;
  mode?: string; // 上传模式: 自动/强制代理
}

const uploadResults = ref<UploadResult[]>([]);

// 最新图片URL
const lastImageUrl = computed(() => {
  const imageResult = uploadResults.value.find(r => r.type === 'image');
  return imageResult ? imageResult.url : '';
});

/**
 * 基本上传成功回调
 */
const onUploadSuccess = (response: any, file: any, fileList: any) => {
  addUploadResult(response, 'file', '自动模式');
};

/**
 * 强制代理上传成功回调
 */
const onProxyUploadSuccess = (response: any, file: any, fileList: any) => {
  addUploadResult(response, 'file', '强制代理');
};

/**
 * 图片上传成功回调
 */
const onImageUploadSuccess = (response: any, file: any, fileList: any) => {
  addUploadResult(response, 'image', '自动模式');
};

/**
 * 上传失败回调
 */
const onUploadError = (error: any, file: any, fileList: any) => {
  ElMessage.error(`上传失败: ${error.message || '未知错误'}`);
};

/**
 * 添加上传结果到列表
 */
const addUploadResult = (response: any, type: string, mode?: string) => {
  // 兼容不同的响应格式
  let url = '';
  let fileName = '';
  
  if (response?.data) {
    url = response.data.url || response.data.Url || '';
    fileName = response.data.fileName || response.data.FileName || '';
  } else if (response) {
    url = response.url || response.Url || '';
    fileName = response.fileName || response.FileName || '';
  }
  
  const result: UploadResult = {
    id: Date.now(),
    fileName: fileName || 'Unknown',
    url: url || 'No URL',
    type,
    time: new Date().toLocaleTimeString(),
    mode: mode || '自动'
  };
  
  uploadResults.value.unshift(result);
  
  // 限制结果列表长度
  if (uploadResults.value.length > 10) {
    uploadResults.value.pop();
  }
};

/**
 * 复制URL到剪贴板
 */
const copyUrl = (url: string) => {
  if (url) {
    navigator.clipboard.writeText(url);
    ElMessage.success('URL已复制到剪贴板');
  }
};

/**
 * 删除结果(同时删除服务器文件)
 */
const removeResult = async (row: UploadResult) => {
  try {
    // 确认删除
    await ElMessageBox.confirm(
      `确定要删除文件 "${row.fileName}" 吗？此操作将同时删除服务器上的文件。`,
      '警告',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    );

    // 获取共享的上传服务
    const uploadService = getSharedUploadService();
    if (!uploadService) {
      ElMessage.error('上传服务未就绪');
      return;
    }

    // 调用上传服务的删除方法(会自动从URL中提取文件路径)
    await uploadService.deleteFile(row.url);

    // 从列表中移除
    const index = uploadResults.value.findIndex(r => r.id === row.id);
    if (index > -1) {
      uploadResults.value.splice(index, 1);
    }

    ElMessage.success('文件删除成功');
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error(`删除失败: ${error.message || '未知错误'}`);
    }
  }
};
</script>

<style scoped>
.storage-demo {
  padding: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-weight: bold;
}

.result-section {
  margin-top: 20px;
}

.result-section h3 {
  margin-bottom: 10px;
  color: #303133;
}

.image-preview {
  margin-top: 20px;
  text-align: center;
  padding: 15px;
  background-color: #f5f7fa;
  border-radius: 4px;
}

.form-tip {
  font-size: 12px;
  color: #909399;
  margin-top: 5px;
  line-height: 1.5;
}
</style>
