<template>
  <div class="user-extension-panels">
    <el-tabs v-if="panels.length > 0" v-model="activeTab">
      <el-tab-pane
        v-for="panel in panels"
        :key="panel.pluginId"
        :label="panel.pluginName"
        :name="panel.pluginId"
      >
        <component
          v-if="loadedComponents.has(panel.pluginId)"
          :is="loadedComponents.get(panel.pluginId)"
          :user-id="userId"
        />
      </el-tab-pane>
    </el-tabs>
    
    <el-empty v-else description="暂无插件扩展信息" />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue';
import { pluginAPI } from '@/plugins/PluginAPI';

interface Props {
  userId: string;
}

const props = defineProps<Props>();

// 定义已知的插件ID列表
const knownPluginIds = ['11365281228129286', '11375910391972869']; 

const panels = ref<{ pluginId: string; pluginName: string; order: number }[]>([]);
const activeTab = ref('');
const loadedComponents = ref(new Map<string, any>());

/**
 * 扫描并更新面板列表
 */
function scanAndLoadPanels() {
  panels.value = [];
  knownPluginIds.forEach(id => {
    const component = pluginAPI.getUIComponent(id, 'UserExtensionPanel');
    if (component) {
      const info = pluginAPI.getPluginInfo(id);
      panels.value.push({
        pluginId: id,
        pluginName: info?.name || id,
        order: 0
      });
      // 立即加载该组件
      loadedComponents.value.set(id, component);
    }
  });

  if (panels.value.length > 0 && !activeTab.value) {
    activeTab.value = panels.value[0].pluginId;
  }
}

onMounted(() => {
  scanAndLoadPanels();
  
  // 监听 pluginAPI 的变化（如果插件是异步加载的）
  // 这里我们设置一个简短的轮询，确保在插件加载完成后能捕捉到
  const timer = setInterval(() => {
    const oldLen = panels.value.length;
    scanAndLoadPanels();
    if (panels.value.length > oldLen) {
      // console.log('[UserExtensionPanels] 检测到新注册的插件面板');
    }
  }, 1000);

  // 组件销毁时清除定时器
  watch(() => props.userId, () => {
    // 用户切换时重新扫描
    scanAndLoadPanels();
  });
});
</script>

<style scoped>
.user-extension-panels {
  margin-top: 20px;
}
</style>
