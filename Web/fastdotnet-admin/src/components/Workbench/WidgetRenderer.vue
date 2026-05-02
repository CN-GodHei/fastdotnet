<template>
  <div class="widget-renderer" v-loading="loading">
    <component
      :is="currentWidget"
      v-if="!loading && !error"
      :data="widgetData"
      :config="safeConfig"
    />
    <el-empty v-else-if="error" :description="error" :image-size="60" />
  </div>
</template>

<script setup lang="ts" name="WidgetRenderer">
import { ref, onMounted, computed, defineAsyncComponent } from 'vue';
import request from '@/utils/request';

const props = defineProps({
  type: { type: String, required: true },
  dataSourceUrl: { type: String, required: true },
  configJson: { type: String, default: '{}' }
});

const loading = ref(true);
const error = ref<string | null>(null);
const widgetData = ref<any>(null);

// 动态载入具体的卡片组件
const widgets = {
  'List': defineAsyncComponent(() => import('./widgets/ListWidget.vue')),
  'Stat': defineAsyncComponent(() => import('./widgets/StatWidget.vue')),
  'Chart': defineAsyncComponent(() => import('./widgets/ChartWidget.vue')),
};

const currentWidget = computed(() => widgets[props.type as keyof typeof widgets] || null);

const safeConfig = computed(() => {
  try {
    return JSON.parse(props.configJson || '{}');
  } catch (e) {
    return {};
  }
});

const fetchData = async () => {
  if (!props.dataSourceUrl) {
    loading.value = false;
    return;
  }
  
  loading.value = true;
  error.value = null;
  
  try {
    const res = await request<any>(props.dataSourceUrl, { method: 'GET' });
    widgetData.value = res;
  } catch (err: any) {
    console.error('Widget data fetch failed:', err);
    error.value = '获取数据失败';
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  fetchData();
});
</script>

<style scoped lang="scss">
.widget-renderer {
  height: 100%;
  width: 100%;
  overflow: hidden;
}
</style>
