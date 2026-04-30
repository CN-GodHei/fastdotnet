<template>
  <div class="stat-widget">
    <div class="stat-value" :style="{ color: config.color || 'var(--el-color-primary)' }">
      {{ displayValue }}
    </div>
    <div class="stat-label">{{ config.label || '总计' }}</div>
    <div v-if="config.trend" class="stat-trend" :class="trendClass">
      <el-icon><component :is="config.trend === 'up' ? 'CaretTop' : 'CaretBottom'" /></el-icon>
      <span>{{ config.trendValue || '0%' }}</span>
    </div>
  </div>
</template>

<script setup lang="ts" name="StatWidget">
import { computed } from 'vue';

const props = defineProps({
  data: { type: [Number, String, Object, Array], default: 0 },
  config: { type: Object, default: () => ({}) }
});

const displayValue = computed(() => {
  // 如果返回的是对象且指定了字段
  if (typeof props.data === 'object' && props.config.valueField) {
    return (props.data as any)[props.config.valueField] || 0;
  }
  // 如果是数组，展示长度
  if (Array.isArray(props.data)) {
    return props.data.length;
  }
  return props.data;
});

const trendClass = computed(() => {
  return props.config.trend === 'up' ? 'up' : 'down';
});
</script>

<style scoped lang="scss">
.stat-widget {
  height: 100%;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  text-align: center;
  
  .stat-value {
    font-size: 32px;
    font-weight: bold;
    margin-bottom: 4px;
    font-family: 'DIN Alternate', 'Arial', sans-serif;
  }
  
  .stat-label {
    font-size: 13px;
    color: var(--el-text-color-secondary);
  }
  
  .stat-trend {
    margin-top: 8px;
    display: flex;
    align-items: center;
    gap: 2px;
    font-size: 12px;
    
    &.up { color: var(--el-color-danger); }
    &.down { color: var(--el-color-success); }
  }
}
</style>
