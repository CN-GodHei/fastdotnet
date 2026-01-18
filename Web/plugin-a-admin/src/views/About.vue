<!-- src/views/About.vue -->
<template>
  <div class="about" style="background-color: #fffbe6; padding: 50px; border: 2px solid #ffe58f;">
    <h1 style="font-size: 48px; color: #faad14;">Plugin A - ABOUT PAGE</h1>
    <p style="font-size: 24px;">这里是插件A的 **关于** 页面。</p>
    <p>This page provides more information about Plugin A.</p>
    <p>Current Route: /plugin-a/about</p>
    
    <!-- --- 添加查询构建器测试UI --- -->
    <div style="margin-top: 20px; padding: 20px; border: 1px solid #ccc;">
      <h2>查询构建器测试</h2>
      <div style="margin-bottom: 10px;">
        <label>名称: </label>
        <input v-model="searchForm.name" placeholder="请输入名称" />
      </div>
      <div style="margin-bottom: 10px;">
        <label>状态: </label>
        <select v-model="searchForm.status">
          <option value="">全部</option>
          <option value="active">启用</option>
          <option value="inactive">禁用</option>
        </select>
      </div>
      <div style="margin-bottom: 10px;">
        <label>最小年龄: </label>
        <input v-model.number="searchForm.minAge" type="number" placeholder="最小年龄" />
      </div>
      <div style="margin-bottom: 10px;">
        <label>最大年龄: </label>
        <input v-model.number="searchForm.maxAge" type="number" placeholder="最大年龄" />
      </div>
      <button @click="testQueryBuilder">构建查询条件</button>
      
      <div v-if="queryResult">
        <h3>构建的查询条件:</h3>
        <p><strong>DynamicQuery:</strong> {{ queryResult.dynamicQuery }}</p>
        <p><strong>QueryParameters:</strong> {{ queryResult.queryParameters }}</p>
      </div>
    </div>
    <!-- --- 添加结束 --- -->
    
    <!-- --- 添加测试调用的UI --- -->
    <div style="margin-top: 20px; padding: 20px; border: 1px solid #ccc;">
      <h2>API 测试</h2>
      <button @click="testGetApi" :disabled="loading">测试 GET 请求</button>
      <p v-if="loading">加载中...</p>
      <div v-if="responseData">
        <h3>响应数据:</h3>
        <pre>{{ responseData }}</pre>
      </div>
      <div v-if="error">
        <h3>错误信息:</h3>
        <pre style="color: red;">{{ error }}</pre>
      </div>
    </div>
    <!-- --- 添加结束 --- -->
    
    <!-- --- 添加上传组件测试UI --- -->
    <div style="margin-top: 20px; padding: 20px; border: 1px solid #ccc;">
      <h2>全局上传组件测试</h2>
      <p>使用主应用提供的统一上传服务，支持多种存储后端</p>
      
      <div style="margin-bottom: 20px;">
        <h3>基本上传</h3>
        <MicroAppFileUploader
          :max-size="5"
          accept=".jpg,.png,.pdf"
          @success="onBasicUploadSuccess"
          @error="onBasicUploadError"
        >
          <button>点击上传</button>
        </MicroAppFileUploader>
      </div>
      
      <div style="margin-bottom: 20px;">
        <h3>图片上传</h3>
        <MicroAppFileUploader
          :max-size="2"
          accept="image/*"
          list-type="picture-card"
          @success="onImageUploadSuccess"
          @error="onBasicUploadError"
        >
          <span>+</span>
        </MicroAppFileUploader>
      </div>
      
      <div style="margin-bottom: 20px;">
        <h3>带存储类型标签</h3>
        <MicroAppFileUploader
          :max-size="3"
          accept="image/*,.pdf"
          @success="onAdvancedUploadSuccess"
          @error="onBasicUploadError"
        >
          <button>上传带标签</button>
        </MicroAppFileUploader>
      </div>
      
      <div style="margin-top: 20px;">
        <h3>上传结果</h3>
        <ul>
          <li v-for="result in uploadResults" :key="result.id">
            {{ result.fileName }} - {{ result.url }} ({{ result.time }})
          </li>
        </ul>
      </div>
    </div>
    <!-- --- 添加结束 --- -->
  </div>
</template>

<script setup lang="ts">
// Plugin A About View
// --- 添加导入和逻辑 ---
import { ref } from 'vue';
import { ElMessage } from 'element-plus';
// 导入你想要测试的 API 函数
import { getApiPluginsPinyin_11375910391972869PluginATestDto } from '@/api/plugin-a/PluginATestDto';
// 导入共享的查询构建器
import { getSharedFdQueryBuilder } from '@/main';
// 导入全局上传组件
import MicroAppFileUploader from '@/components/upload/MicroAppFileUploader.vue';
import { UploadFilled } from '@element-plus/icons-vue';

const loading = ref(false);
const responseData = ref<any>(null); // 使用更具体的类型代替 any
const error = ref<string | null>(null);

// 上传结果
const uploadResults = ref<any[]>([]);

// 查询表单
const searchForm = ref({
  name: '',
  status: '',
  minAge: undefined,
  maxAge: undefined
});

// 查询结果
const queryResult = ref<any>(null);

// 测试查询构建器
const testQueryBuilder = () => {
  try {
    // 获取共享的查询构建器
    const queryBuilder = getSharedFdQueryBuilder();
    
    if (!queryBuilder || !queryBuilder.buildMixedQuery) {
      console.error('查询构建器未正确加载');
      return;
    }
    
    // 构建查询配置
    const queryConfig: any = {
      contains: {},
      equals: {},
      ranges: {}
    };
    
    // 添加名称查询条件
    if (searchForm.value.name) {
      queryConfig.contains.name = searchForm.value.name;
    }
    
    // 添加状态查询条件
    if (searchForm.value.status) {
      queryConfig.equals.status = searchForm.value.status;
    }
    
    // 添加年龄范围查询条件
    if (searchForm.value.minAge !== undefined || searchForm.value.maxAge !== undefined) {
      queryConfig.ranges.age = {};
      if (searchForm.value.minAge !== undefined) {
        queryConfig.ranges.age.from = searchForm.value.minAge;
      }
      if (searchForm.value.maxAge !== undefined) {
        queryConfig.ranges.age.to = searchForm.value.maxAge;
      }
    }
    
    // 使用查询构建器构建查询条件
    const result = queryBuilder.buildMixedQuery(queryConfig);
    //console.log('查询构建结果:', result);
    queryResult.value = result;
  } catch (err) {
    console.error('查询构建器测试错误:', err);
  }
};

const testGetApi = async () => {
  loading.value = true;
  responseData.value = null;
  error.value = null;
  try {
    // 调用 API 函数
    const data = await getApiPluginsPinyin_11375910391972869PluginATestDto();
    //console.log('API Response:', data);
    responseData.value = data;
  } catch (err) {
    console.error('API Call Error:', err);
    // @ts-ignore
    error.value = err.message || err.toString();
  } finally {
    loading.value = false;
  }
};
// --- 添加结束 ---

// 上传前处理和自定义上传函数已经在 MicroAppFileUploader 组件内部实现

// 上传成功回调
const onBasicUploadSuccess = (response: any, file: any, fileList: any) => {
  console.log('基本上传成功:', response);
  addUploadResult(response, 'basic');
};

const onImageUploadSuccess = (response: any, file: any, fileList: any) => {
  console.log('图片上传成功:', response);
  addUploadResult(response, 'image');
};

const onAdvancedUploadSuccess = (response: any, file: any, fileList: any) => {
  console.log('高级上传成功:', response);
  addUploadResult(response, 'advanced');
};

const onBasicUploadError = (error: any, file: any, fileList: any) => {
  console.error('上传失败:', error);
};

// 添加上传结果
const addUploadResult = (response: any, type: string) => {
  const result = {
    id: Date.now(),
    fileName: response.data?.fileName || 'Unknown',
    url: response.data?.url || 'No URL',
    type,
    time: new Date().toLocaleTimeString()
  };
  uploadResults.value.unshift(result);
  // 限制结果列表长度
  if (uploadResults.value.length > 10) {
    uploadResults.value.pop();
  }
};

</script>

<style scoped>
.about {
  text-align: center;
  border-radius: 8px;
}
</style>
