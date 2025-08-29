<!-- src/views/About.vue -->
<template>
  <div class="about" style="background-color: #fffbe6; padding: 50px; border: 2px solid #ffe58f;">
    <h1 style="font-size: 48px; color: #faad14;">Plugin A - ABOUT PAGE</h1>
    <p style="font-size: 24px;">这里是插件A的 **关于** 页面。</p>
    <p>This page provides more information about Plugin A.</p>
    <p>Current Route: {{ $route.path }}</p>
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
  </div>
</template>

<script setup lang="ts">
// Plugin A About View
// --- 添加导入和逻辑 ---
import { ref } from 'vue';
// 导入你想要测试的 API 函数
import { getApiPluginsPinyin_11375910391972869PluginATestDto } from '@/api/plugin-a/PluginATestDto';

const loading = ref(false);
const responseData = ref<any>(null); // 使用更具体的类型代替 any
const error = ref<string | null>(null);

const testGetApi = async () => {
  loading.value = true;
  responseData.value = null;
  error.value = null;
  try {
    // 调用 API 函数
    const data = await getApiPluginsPinyin_11375910391972869PluginATestDto();
    console.log('API Response:', data);
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
</script>

<style scoped>
.about {
  text-align: center;
  border-radius: 8px;
}
</style>
