<template>
  <div class="list-widget">
    <el-scrollbar>
      <div v-if="data && data.length > 0" class="list-container">
        <div v-for="(item, index) in data" :key="index" class="list-item">
          <div class="item-content">
            <div class="item-title">{{ getFieldValue(item, config.titleField) }}</div>
            <div class="item-info">
              <span class="item-subtitle">{{ getFieldValue(item, config.subTitleField) }}</span>
              <el-tag v-if="config.statusField" size="small" :type="getStatusType(item)">
                {{ getFieldValue(item, config.statusField) }}
              </el-tag>
            </div>
          </div>
        </div>
      </div>
      <el-empty v-else :image-size="40" description="暂无数据" />
    </el-scrollbar>
  </div>
</template>

<script setup lang="ts" name="ListWidget">
const props = defineProps({
  data: { type: Array, default: () => [] },
  config: { type: Object, default: () => ({}) }
});

// 根据配置动态获取字段值
const getFieldValue = (item: any, fieldName: string) => {
  if (!fieldName) return '';
  return item[fieldName] || '';
};

// 根据状态值动态返回 Element Plus 的标签类型
const getStatusType = (item: any) => {
  const status = getFieldValue(item, props.config.statusField);
  if (!status) return 'info';
  
  const statusStr = String(status).toLowerCase();
  if (statusStr.includes('已完成') || statusStr.includes('success')) return 'success';
  if (statusStr.includes('进行中') || statusStr.includes('running')) return 'primary';
  if (statusStr.includes('待处理') || statusStr.includes('pending')) return 'warning';
  if (statusStr.includes('失败') || statusStr.includes('error')) return 'danger';
  
  return 'info';
};
</script>

<style scoped lang="scss">
.list-widget {
  height: 100%;
  
  .list-container {
    padding-right: 10px;
  }
  
  .list-item {
    padding: 12px 8px;
    border-bottom: 1px dashed var(--el-border-color-lighter);
    transition: background 0.2s;
    
    &:hover {
      background-color: var(--el-fill-color-light);
      border-radius: 4px;
    }
    
    &:last-child {
      border-bottom: none;
    }
    
    .item-title {
      font-size: 14px;
      color: var(--el-text-color-primary);
      margin-bottom: 6px;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }
    
    .item-info {
      display: flex;
      justify-content: space-between;
      align-items: center;
      
      .item-subtitle {
        font-size: 12px;
        color: var(--el-text-color-secondary);
      }
    }
  }
}
</style>
