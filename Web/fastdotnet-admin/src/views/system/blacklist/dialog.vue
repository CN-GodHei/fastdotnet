<template>
  <div>
    <el-dialog 
      :title="state.dialogTitle" 
      v-model="state.dialogVisible" 
      width="500px"
      :before-close="handleClose"
      :destroy-on-close="true"
    >
      <el-form 
        :model="state.formData" 
        :rules="state.formRules" 
        ref="blacklistFormRef" 
        label-width="100px"
        label-position="right"
        status-icon
      >
        <el-form-item label="类型" prop="Type">
          <el-select 
            v-model="state.formData.Type" 
            placeholder="请选择类型" 
            :disabled="!!state.formData.Id"
            style="width: 100%"
          >
            <el-option label="IP" value="IP"></el-option>
            <el-option label="User" value="User"></el-option>
            <el-option label="ApiKey" value="ApiKey"></el-option>
          </el-select>
        </el-form-item>
        
        <el-form-item label="值" prop="Value">
          <el-input v-model="state.formData.Value" placeholder="请输入值"></el-input>
        </el-form-item>
        
        <el-form-item label="原因" prop="Reason">
          <el-input 
            v-model="state.formData.Reason" 
            type="textarea" 
            placeholder="请输入原因"
            :autosize="{ minRows: 2, maxRows: 4 }"
          ></el-input>
        </el-form-item>
        
        <el-form-item label="过期时间" prop="ExpiredAt">
          <el-date-picker
            v-model="state.formData.ExpiredAt"
            type="datetime"
            placeholder="请选择过期时间"
            format="YYYY-MM-DD HH:mm:ss"
            value-format="YYYY-MM-DDTHH:mm:ss"
            style="width: 100%"
          />
        </el-form-item>
      </el-form>
      
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="handleClose">取 消</el-button>
          <el-button 
            type="primary" 
            @click="handleSubmit" 
            :loading="state.submitLoading"
          >
            确 定
          </el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts" name="blacklistDialog">
import { reactive, ref, nextTick } from 'vue';
import { ElMessage } from 'element-plus';
import {
  postAdminBlacklists,
  putAdminBlacklistsId,
  getAdminBlacklistsId
} from '/@/api/fd-system-api/Blacklists';
import type { 
  FdBlacklistDto, 
  CreateFdBlacklistDto, 
  UpdateFdBlacklistDto 
} from '/@/api/fd-system-api/typings';
import type { FormInstance } from 'element-plus';

// 定义变量内容
const blacklistFormRef = ref<FormInstance>();
const state = reactive<BlacklistDialogState>({
  dialogVisible: false,
  dialogTitle: '',
  submitLoading: false,
  // 表单数据
  formData: {
    type: '',
    value: '',
    reason: '',
    expiredAt: undefined
  },
  // 表单验证规则
  formRules: {
    Type: [{ required: true, message: '请选择类型', trigger: 'change' }],
    Value: [{ required: true, message: '请输入值', trigger: 'blur' }]
  },
  // 当前操作类型 (add/edit)
  actionType: '',
  // 编辑时的原始数据
  rowData: undefined
});

// 打开弹窗
const openDialog = async (type: string, row?: FdBlacklistDto) => {
  state.dialogVisible = true;
  state.actionType = type;
  state.dialogTitle = type === 'add' ? '新增黑名单' : '编辑黑名单';
  
  if (type === 'edit' && row?.Id) {
    // 编辑时需要先获取最新数据
    try {
      const data = await getAdminBlacklistsId({ params: { id: row.Id } });
      state.rowData = data;
      state.formData = {
        Id: data.Id,
        Type: data.Type,
        Value: data.Value,
        Reason: data.Reason || '',
        ExpiredAt: data.ExpiredAt || undefined
      };
    } catch (error) {
      ElMessage.error('获取数据失败');
      console.error(error);
      handleClose();
      return;
    }
  } else {
    // 新增时重置表单
    resetForm();
  }
  
  // 确保DOM更新后再设置焦点
  nextTick(() => {
    blacklistFormRef.value?.clearValidate();
  });
};

// 关闭弹窗
const handleClose = () => {
  state.dialogVisible = false;
  resetForm();
};

// 重置表单
const resetForm = () => {
  state.formData = {
    Type: '',
    Value: '',
    Reason: '',
    ExpiredAt: undefined
  };
  state.rowData = undefined;
  blacklistFormRef.value?.resetFields();
};

// 提交表单
const handleSubmit = () => {
  if (!blacklistFormRef.value) return;
  
  blacklistFormRef.value.validate(async (valid) => {
    if (!valid) return;
    
    state.submitLoading = true;
    try {
      if (state.actionType === 'add') {
        // 新增
        const CreateTimea: CreateFdBlacklistDto = {
          Type: state.formData.Type,
          Value: state.formData.Value,
          Reason: state.formData.Reason,
          ExpiredAt: state.formData.ExpiredAt
        };
        await postAdminBlacklists(CreateTimea);
        ElMessage.success('新增成功');
      } else {
        // 编辑
        if (!state.formData.Id) {
          ElMessage.error('缺少ID参数');
          return;
        }
        const updateData: UpdateFdBlacklistDto = {
          Type: state.formData.Type,
          Value: state.formData.Value,
          Reason: state.formData.Reason,
          ExpiredAt: state.formData.ExpiredAt
        };
        await putAdminBlacklistsId({ 
          params: { id: state.formData.Id }, 
          body: updateData 
        });
        ElMessage.success('更新成功');
      }
      
      handleClose();
      // 触发父组件刷新数据
      emit('refresh');
    } catch (error) {
      ElMessage.error(state.actionType === 'add' ? '新增失败' : '更新失败');
      console.error(error);
    } finally {
      state.submitLoading = false;
    }
  });
};

// 定义事件
const emit = defineEmits(['refresh']);

// 暴露方法给父组件
defineExpose({
  openDialog
});
</script>

<style scoped lang="scss">
.dialog-footer {
  display: flex;
  justify-content: flex-end;
}
</style>