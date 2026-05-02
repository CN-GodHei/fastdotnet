<template>
  <div class="workbench-container">
    <!-- 右上角控制区 -->
    <div class="workbench-controls">
      <!-- 非编辑模式：只显示一个设置图标 -->
      <div v-if="!editable" class="control-trigger" @click="editable = true">
        <el-button circle>
          <el-icon><Setting></Setting></el-icon>
        </el-button>
      </div>
      
      <!-- 编辑模式：显示功能按钮 -->
      <div v-else class="control-menu">
        <el-button-group>
          <el-button type="primary" size="small" @click="marketVisible = true">添加卡片</el-button>
          <el-button type="success" size="small" @click="toggleEdit">保存布局</el-button>
          <el-button type="danger" size="small" @click="clearLayout">重置</el-button>
          <el-button size="small" @click="editable = false">取消</el-button>
        </el-button-group>
      </div>
    </div>

    <!-- 网格布局区域 -->
    <div class="grid-wrapper">
      <GridLayout
        v-if="layout.length > 0"
        v-model:layout="layout"
        :col-num="12"
        :row-height="30"
        :is-draggable="editable"
        :is-resizable="editable"
        :vertical-compact="true"
        :margin="[15, 15]"
        :use-css-transforms="true"
      >
        <GridItem
          v-for="item in layout"
          :key="item.i"
          :x="item.x"
          :y="item.y"
          :w="item.w"
          :h="item.h"
          :i="item.i"
          class="custom-grid-item"
        >
          <WidgetWrapper
            :title="getWidgetTitle(item)"
            :icon="getWidgetIcon(item)"
            :type="getWidgetType(item)"
            :data-source-url="getWidgetUrl(item)"
            :config-json="getWidgetConfig(item)"
            :editable="editable"
            @remove="removeCard(item.i)"
          ></WidgetWrapper>
        </GridItem>
      </GridLayout>
      <el-empty v-else description="开始打造你的专属工作台吧"></el-empty>
    </div>

    <!-- 卡片集市弹窗 -->
    <el-dialog v-model="marketVisible" title="卡片集市" width="600px">
      <div class="market-list">
        <div 
          v-for="card in availableCards" 
          :key="card.Id" 
          class="market-card"
          @click="addCard(card)"
        >
          <div class="m-title">{{ card.Name }}</div>
          <div class="m-desc">{{ card.Description }}</div>
        </div>
        <div v-if="availableCards.length === 0" style="text-align: center; padding: 30px; color: #999;">所有卡片均已添加</div>
      </div>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { GridLayout, GridItem } from 'vue-grid-layout-v3';
import { Setting } from '@element-plus/icons-vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { 
  getFdAppUserWorkbenchGetAvailableCards, 
  getFdAppUserWorkbenchGetMyLayout, 
  postFdAppUserWorkbenchSaveLayout 
} from '@/api/fd-system-api-app/fdAppUserWorkbench';
import WidgetWrapper from '@/components/Workbench/WidgetWrapper.vue';

const editable = ref(false);
const marketVisible = ref(false);
const layout = ref<any[]>([]); 
const allCardsMetadata = ref<any[]>([]); 

const getMetadata = (item: any) => allCardsMetadata.value.find(c => c.Id === (item.realCardId || item.i));
const getWidgetTitle = (item: any) => getMetadata(item)?.Name || '未知卡片';
const getWidgetIcon = (item: any) => getMetadata(item)?.Icon || 'Grid';
const getWidgetType = (item: any) => getMetadata(item)?.Type || 'List';
const getWidgetUrl = (item: any) => getMetadata(item)?.DataSourceUrl || '';
const getWidgetConfig = (item: any) => getMetadata(item)?.ConfigJson || '{}';

// 过滤已添加的卡片：布局中已存在的 realCardId 不再显示
const availableCards = computed(() => {
  const addedIds = new Set(layout.value.map(item => item.realCardId || item.i));
  return allCardsMetadata.value.filter(card => !addedIds.has(card.Id));
});

const loadData = async () => {
  try {
    const cards = await getFdAppUserWorkbenchGetAvailableCards();
    allCardsMetadata.value = cards || [];
    const layoutInfo = await getFdAppUserWorkbenchGetMyLayout({ name: 'Default' });
    if (layoutInfo && layoutInfo.LayoutData) layout.value = JSON.parse(layoutInfo.LayoutData);
  } catch (err) {}
};

const toggleEdit = () => {
  saveLayout();
  editable.value = false;
};

const saveLayout = async () => {
  try {
    await postFdAppUserWorkbenchSaveLayout({
      LayoutData: JSON.stringify(layout.value),
      Name: 'Default'
    } as any);
    ElMessage.success('已保存工作台布局');
  } catch (err) {
    ElMessage.error('保存失败');
  }
};

const clearLayout = () => {
  ElMessageBox.confirm('确定要清空工作台吗？', '提示').then(() => {
    layout.value = [];
    saveLayout();
  });
};

const addCard = (card: any) => {
  layout.value.push({
    i: `${card.Id}_${Date.now()}`,
    realCardId: card.Id,
    x: (layout.value.length * 4) % 12,
    y: 0,
    w: card.DefaultWidth || 4,
    h: card.DefaultHeight || 6
  });
  marketVisible.value = false;
};

const removeCard = (id: string) => {
  layout.value = layout.value.filter(item => item.i !== id);
};

onMounted(loadData);
</script>

<style scoped lang="scss">
.workbench-container {
  min-height: calc(100vh - 120px);
  padding: 0; // 彻底去掉边距
  background-color: var(--el-fill-color-extra-light);
  position: relative;

  .workbench-controls {
    position: absolute;
    top: 5px;
    right: 5px;
    z-index: 1000;
    
    .control-trigger {
      cursor: pointer;
      opacity: 0.7;
      transition: all 0.3s;
      &:hover { opacity: 1; transform: scale(1.1); }
      .el-button { box-shadow: 0 4px 15px rgba(0,0,0,0.15); border: none; }
    }
    
    .control-menu {
      background: rgba(255, 255, 255, 0.95);
      backdrop-filter: blur(4px);
      padding: 6px;
      border-radius: 25px;
      box-shadow: 0 8px 25px rgba(0,0,0,0.15);
      border: 1px solid rgba(0,0,0,0.05);
    }
  }

  .grid-wrapper {
    margin-top: 0;
    padding: 0;
  }

  .market-list {
    max-height: 400px;
    overflow-y: auto;
    .market-card {
      padding: 15px;
      margin-bottom: 15px;
      border: 1px solid var(--el-border-color-lighter);
      border-radius: 8px;
      cursor: pointer;
      &:hover {
        border-color: var(--el-color-primary);
        background: var(--el-color-primary-light-9);
      }
      .m-title { font-weight: bold; font-size: 15px; }
      .m-desc { font-size: 12px; color: #999; margin-top: 5px; }
    }
  }
}

.custom-grid-item {
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.05);
  overflow: hidden;
  &:hover { box-shadow: 0 4px 15px rgba(0,0,0,0.1); }
}

:deep(.vue-grid-layout) {
  width: 100%;
}
</style>
