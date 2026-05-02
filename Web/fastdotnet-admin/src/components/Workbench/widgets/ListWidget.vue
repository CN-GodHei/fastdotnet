<template>
  <div class="list-widget">
    <el-scrollbar>
      <div v-if="listData.length > 0" class="list-container">
        <div v-for="(item, index) in listData" :key="index" class="list-item">
          <div class="item-content">
            <div class="item-title">{{ getByRole(item, 'title') || getByRole(item, 'subTitle') || '(无标题)' }}</div>
            <div class="item-info">
              <span class="item-subtitle">{{ getByRole(item, 'subTitle') }}</span>
              <div class="item-right">
                <el-tag v-if="getByRole(item, 'status') !== ''" size="small" :type="resolveStatusType(getByRole(item, 'status'))">
                  {{ getByRole(item, 'status') }}
                </el-tag>
                <span class="item-date">{{ formatDate(getByRole(item, 'date')) }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
      <el-empty v-else :image-size="40" description="暂无数据"></el-empty>
    </el-scrollbar>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';

const props = defineProps({
  data: { type: [Array, Object], default: () => [] },
  config: { type: Object, default: () => ({}) }
});

// 标准化数据为数组（兼容已解包和未解包两种情况）
const listData = computed<any[]>(() => {
  const d = props.data as any;
  if (Array.isArray(d)) return d;
  if (d && Array.isArray(d.Data)) return d.Data;
  if (d && Array.isArray(d.data)) return d.data;
  if (d && Array.isArray(d.Items)) return d.Items;
  if (d && Array.isArray(d.items)) return d.items;
  return [];
});

// 兼容新格式（fields 数组）和旧格式（titleField 字符串）两种 config
const getByRole = (item: any, role: string): string => {
  const cfg = props.config as any;
  
  // 新格式：fields: [{ field: 'Title', role: 'title' }]
  if (Array.isArray(cfg.fields)) {
    const matched = cfg.fields.find((f: any) => f.role === role);
    if (matched?.field) return String(item[matched.field] ?? '');
  }
  
  // 旧格式兼容：titleField / subTitleField / dateField / statusField
  const legacyMap: Record<string, string> = {
    title: cfg.titleField,
    subTitle: cfg.subTitleField,
    date: cfg.dateField,
    status: cfg.statusField
  };
  const key = legacyMap[role];
  if (key) return String(item[key] ?? '');
  
  return '';
};

const resolveStatusType = (status: string) => {
  const s = String(status).toLowerCase();
  if (/完成|success|done/.test(s)) return 'success';
  if (/进行|running|active/.test(s)) return 'primary';
  if (/待|pending|wait/.test(s)) return 'warning';
  if (/失败|error|fail/.test(s)) return 'danger';
  return 'info';
};

const formatDate = (val: string) => {
  if (!val) return '';
  // 如果已经是可读日期字符串直接返回，否则尝试解析
  const d = new Date(val);
  if (isNaN(d.getTime())) return val;
  return d.toLocaleDateString('zh-CN', { month: '2-digit', day: '2-digit', hour: '2-digit', minute: '2-digit' });
};
</script>

<style scoped lang="scss">
.list-widget {
  height: 100%;

  .list-container { padding-right: 10px; }

  .list-item {
    padding: 10px 8px;
    border-bottom: 1px dashed var(--el-border-color-lighter);
    transition: background 0.2s;
    &:hover { background-color: var(--el-fill-color-light); border-radius: 4px; }
    &:last-child { border-bottom: none; }

    .item-title {
      font-size: 14px;
      color: var(--el-text-color-primary);
      margin-bottom: 5px;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }

    .item-info {
      display: flex;
      justify-content: space-between;
      align-items: center;
      .item-subtitle { font-size: 12px; color: var(--el-text-color-secondary); flex: 1; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
      .item-right { display: flex; align-items: center; gap: 6px; flex-shrink: 0; }
      .item-date { font-size: 11px; color: #bbb; }
    }
  }
}
</style>
