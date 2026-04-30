<template>
  <el-card class="widget-wrapper" shadow="hover" :body-style="{ padding: '10px', height: 'calc(100% - 50px)' }">
    <template #header>
      <div class="card-header">
        <div class="header-left">
          <el-icon class="header-icon"><component :is="icon || Menu" /></el-icon>
          <span class="header-title">{{ title }}</span>
        </div>
        <div class="header-right">
          <el-tooltip content="刷新数据" placement="top">
            <el-button link type="primary" :icon="Refresh" @click="handleRefresh" />
          </el-tooltip>
          <el-tooltip v-if="editable" content="移除卡片" placement="top">
            <el-button link type="danger" :icon="Delete" @click="$emit('remove')" />
          </el-tooltip>
        </div>
      </div>
    </template>
    
    <!-- 内部渲染器 -->
    <WidgetRenderer
      ref="rendererRef"
      :type="type"
      :data-source-url="dataSourceUrl"
      :config-json="configJson"
    />
  </el-card>
</template>

<script setup lang="ts" name="WidgetWrapper">
import { ref } from 'vue';
import { Refresh, Delete, Menu } from '@element-plus/icons-vue';
import WidgetRenderer from './WidgetRenderer.vue';

const props = defineProps({
  title: { type: String, default: '未命名卡片' },
  icon: { type: String, default: 'Menu' },
  type: { type: String, default: 'List' },
  dataSourceUrl: { type: String, default: '' },
  configJson: { type: String, default: '{}' },
  editable: { type: Boolean, default: false }
});

const emit = defineEmits(['remove']);
const rendererRef = ref();

const handleRefresh = () => {
  rendererRef.value?.fetchData();
};
</script>

<style scoped lang="scss">
.widget-wrapper {
  height: 100%;
  display: flex;
  flex-direction: column;
  border: none !important;
  transition: all 0.3s;

  &:hover {
    transform: translateY(-2px);
    box-shadow: 0 8px 24px rgba(0, 0, 0, 0.1) !important;
  }

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 10px 0;
    
    .header-left {
      display: flex;
      align-items: center;
      gap: 8px;
      
      .header-icon {
        color: var(--el-color-primary);
        font-size: 18px;
      }
      
      .header-title {
        font-weight: 600;
        font-size: 15px;
        color: var(--el-text-color-primary);
      }
    }
  }

  :deep(.el-card__header) {
    padding: 0 15px;
    border-bottom: 1px solid var(--el-border-color-lighter);
  }
}
</style>
