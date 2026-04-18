<template>
  <div class="plugin-a-user-extension">
    <el-form :model="formData" label-width="120px">
      <el-form-item label="用户偏好">
        <el-input 
          v-model="formData.Preferences" 
          placeholder="请输入用户偏好设置"
          clearable
        />
      </el-form-item>
      
      <el-form-item label="用户积分">
        <el-input-number v-model="formData.Points" :min="0" />
      </el-form-item>
      
      <el-form-item>
        <el-button type="primary" @click="save" :loading="saving">
          保存
        </el-button>
        <el-button @click="reset">重置</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { ElMessage } from 'element-plus';
import {
  getPluginsSharedP11375910391972869PluginAUserExtensionUserId,
  putPluginsSharedP11375910391972869PluginAUserExtensionUserId
} from './api/plugin-a/PluginAUser';

interface Props {
  userId: string;
}

const props = defineProps<Props>();

interface FormData {
  Preferences: string;
  Points: number;
}

const formData = ref<FormData>({
  Preferences: '',
  Points: 0
});

const saving = ref(false);
const originalData = ref<FormData | null>(null);

/**
 * 加载用户扩展信息
 */
async function load() {
  try {
    console.log('[PluginA] 开始加载用户扩展信息, userId:', props.userId);
    const res = await getPluginsSharedP11375910391972869PluginAUserExtensionUserId({
      userId: props.userId
    });
    console.log('[PluginA] API 返回数据:', res);
    
    // 后端返回的数据结构是 { Data: { Extension: {...} } }
    const extension = res?.Data?.Extension || res?.Extension || res;
    console.log('[PluginA] 提取的扩展数据:', extension);
    
    // 字段与后端实体 PluginAUserExtension 完全对应（注意大小写一致）
    formData.value = {
      Preferences: extension.Preferences ?? '',
      Points: extension.Points ?? 0
    };
    originalData.value = { ...formData.value };
    console.log('[PluginA] 表单数据已更新:', formData.value);
  } catch (error: any) {
    console.error('[PluginA] 加载用户扩展信息失败:', error);
    ElMessage.warning('加载失败，使用默认值');
    originalData.value = { ...formData.value };
  }
}

/**
 * 保存用户扩展信息
 */
async function save() {
  saving.value = true;
  try {
    // 构造符合后端实体 PluginAUserExtension 结构的数据
    const extensionData = {
      FdAppUserId: props.userId,
      Preferences: formData.value.Preferences,
      Points: formData.value.Points
    };
    
    console.log('[PluginA] 准备保存的数据:', extensionData);
    
    await putPluginsSharedP11375910391972869PluginAUserExtensionUserId(
      { userId: props.userId },
      extensionData as any
    );
    ElMessage.success('保存成功');
    originalData.value = { ...formData.value };
  } catch (error: any) {
    console.error('[PluginA] 保存用户扩展信息失败:', error);
    ElMessage.error('保存失败: ' + (error.message || '未知错误'));
  } finally {
    saving.value = false;
  }
}

/**
 * 重置表单
 */
function reset() {
  if (originalData.value) {
    formData.value = { ...originalData.value };
  }
}

onMounted(() => {
  load();
});
</script>

<style scoped>
.plugin-a-user-extension {
  padding: 20px;
}
</style>
