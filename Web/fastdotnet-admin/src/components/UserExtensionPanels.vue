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

// 定义已知的插件ID列表（实际项目中可以从配置或API获取）
const knownPluginIds = ['11365281228129286', '11375910391972869']; 

const panels = ref<{ pluginId: string; pluginName: string; order: number }[]>([]);
const activeTab = ref('');
const loadedComponents = ref(new Map<string, any>());

onMounted(() => {
  // 初始化面板列表
  knownPluginIds.forEach(id => {
    // 尝试获取组件以确认插件是否注册了面板
    const component = pluginAPI.getUIComponent(id, 'UserExtensionPanel');
    if (component) {
      const info = pluginAPI.getPluginInfo(id);
      panels.value.push({
        pluginId: id,
        pluginName: info?.name || id,
        order: info ? parseInt(info.version.replace(/\./g, '')) : 0 // 简单用版本号做排序示例
      });
    }
  });

  if (panels.value.length > 0) {
    activeTab.value = panels.value[0].pluginId;
    loadPanel(activeTab.value);
  }
});

async function loadPanel(pluginId: string) {
  if (loadedComponents.value.has(pluginId)) return;

  const component = pluginAPI.getUIComponent(pluginId, 'UserExtensionPanel');
  if (component) {
    loadedComponents.value.set(pluginId, component);
  }
}

watch(activeTab, (newTab) => {
  if (newTab && !loadedComponents.value.has(newTab)) {
    loadPanel(newTab);
  }
});
</script>

<style scoped>
.user-extension-panels {
  margin-top: 20px;
}
</style>
