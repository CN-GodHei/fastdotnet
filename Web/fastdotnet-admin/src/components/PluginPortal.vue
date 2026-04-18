<template>
  <div class="plugin-portal">
    <!-- 插件加载状态指示器 -->
    <div v-if="loading" class="loading-state">
      <el-skeleton :rows="6" animated />
    </div>
    
    <!-- 插件容器 -->
    <div v-else-if="currentPlugin" class="plugin-container">
      <div :id="pluginContainerId" class="plugin-mount-point"></div>
    </div>
    
    <!-- 错误状态 -->
    <div v-else-if="error" class="error-state">
      <el-result 
        icon="error" 
        title="插件加载失败" 
        :sub-title="error"
      >
        <template #extra>
          <el-button type="primary" @click="retryLoad">重新加载</el-button>
        </template>
      </el-result>
    </div>
    
    <!-- 空状态 -->
    <div v-else class="empty-state">
      <el-result 
        icon="info" 
        title="暂无插件" 
        sub-title="请选择要加载的插件"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch } from 'vue';
import { pluginManager } from '@/plugins/PluginManager';
import { pluginRegistry } from '@/plugins/PluginRegistry';

interface Props {
  pluginId: string;
  props?: Record<string, any>;
  autoLoad?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  autoLoad: true,
  props: () => ({})
});

// 响应式数据
const loading = ref(false);
const error = ref('');
const currentPlugin = ref<string | null>(null);
const pluginContainerId = `plugin-container-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;

// 加载插件
const loadPlugin = async (pluginId: string) => {
  if (!pluginId) {
    error.value = '插件ID不能为空';
    return;
  }

  // 检查插件是否存在且已启用
  if (!pluginRegistry.hasPlugin(pluginId)) {
    error.value = `插件 ${pluginId} 不存在`;
    return;
  }

  if (!pluginRegistry.isPluginEnabled(pluginId)) {
    error.value = `插件 ${pluginId} 未启用`;
    return;
  }

  loading.value = true;
  error.value = '';

  try {
    // 卸载当前插件（如果有）
    if (currentPlugin.value) {
      await pluginManager.unloadPlugin(currentPlugin.value);
      currentPlugin.value = null;
    }

    // 加载新插件
    const microApp = await pluginManager.loadPlugin(
      pluginId,
      `#${pluginContainerId}`,
      props.props
    );

    if (microApp) {
      currentPlugin.value = pluginId;
    } else {
      throw new Error(`Failed to load plugin ${pluginId}`);
    }
  } catch (err: any) {
    console.error('[PluginPortal] Error loading plugin:', err);
    error.value = err.message || '插件加载失败';
  } finally {
    loading.value = false;
  }
};

// 重新加载插件
const retryLoad = () => {
  if (props.pluginId) {
    loadPlugin(props.pluginId);
  }
};

// 监听插件ID变化
watch(
  () => props.pluginId,
  (newPluginId) => {
    if (newPluginId && props.autoLoad) {
      loadPlugin(newPluginId);
    }
  },
  { immediate: true }
);

// 组件卸载时卸载插件
onUnmounted(async () => {
  if (currentPlugin.value) {
    await pluginManager.unloadPlugin(currentPlugin.value);
    currentPlugin.value = null;
  }
});

// 暴露方法
defineExpose({
  loadPlugin,
  retryLoad
});
</script>

<style scoped>
.plugin-portal {
  width: 100%;
  height: 100%;
  position: relative;
}

.loading-state {
  padding: 20px;
}

.error-state {
  padding: 20px;
}

.empty-state {
  padding: 20px;
}

.plugin-container {
  width: 100%;
  height: 100%;
}

.plugin-mount-point {
  width: 100%;
  height: 100%;
}
</style>