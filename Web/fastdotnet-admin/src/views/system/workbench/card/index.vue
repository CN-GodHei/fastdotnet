<template>
  <div class="workbench-card-container layout-pd">
    <el-card shadow="hover">
      <div class="mb15">
        <el-button type="primary" :icon="Plus" @click="openEditDialog()">新增卡片</el-button>
        <el-button :icon="Search" class="ml10" @click="loadData">刷新数据</el-button>
      </div>

      <el-table :data="tableData" v-loading="loading" border stripe>
        <el-table-column prop="Name" label="卡片名称" width="150"></el-table-column>
        <el-table-column prop="Type" label="类型" width="100">
          <template #default="scope">
            <el-tag>{{ scope.row.Type }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Icon" label="图标" width="80" align="center">
          <template #default="scope">
            <el-icon v-if="scope.row.Icon">
              <component :is="scope.row.Icon"></component>
            </el-icon>
          </template>
        </el-table-column>
        <el-table-column prop="DataSourceUrl" label="数据源接口" show-overflow-tooltip></el-table-column>
        <el-table-column prop="Description" label="描述" show-overflow-tooltip></el-table-column>
        <el-table-column label="操作" width="220" fixed="right">
          <template #default="scope">
            <el-button link type="primary" :icon="Edit" @click="openEditDialog(scope.row)">编辑</el-button>
            <el-button link type="success" :icon="User" @click="openAssignDialog(scope.row)">分配权限</el-button>
            <el-popconfirm title="确定删除该卡片吗？" @confirm="handleDelete(scope.row)">
              <template #reference>
                <el-button link type="danger" :icon="Delete">删除</el-button>
              </template>
            </el-popconfirm>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- 编辑弹窗 -->
    <el-dialog v-model="editDialog.visible" :title="editDialog.Id ? '编辑卡片' : '新增卡片'" width="650px">
      <el-form :model="editForm" label-width="100px" ref="formRef">
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="卡片名称" required>
              <el-input v-model="editForm.Name" placeholder="请输入卡片名称"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="展示类型" required>
              <el-select v-model="editForm.Type" placeholder="请选择类型">
                <el-option label="列表 (List)" value="List"></el-option>
                <el-option label="统计 (Stat)" value="Stat"></el-option>
                <el-option label="图表 (Chart)" value="Chart"></el-option>
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="卡片图标">
              <el-input v-model="editForm.Icon" placeholder="图标名(如: Edit)"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="默认宽高">
              <div class="flex-row">
                <el-input-number v-model="editForm.DefaultWidth" :min="1" :max="12" size="small"></el-input-number>
                <span class="mx-2">x</span>
                <el-input-number v-model="editForm.DefaultHeight" :min="1" :max="20" size="small"></el-input-number>
              </div>
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="数据源URL" required>
          <el-input v-model="editForm.DataSourceUrl" placeholder="如: /api/app/todo/my"></el-input>
        </el-form-item>
        <el-form-item label="描述">
          <el-input v-model="editForm.Description" type="textarea" :rows="2"></el-input>
        </el-form-item>
        <el-form-item label="渲染配置">
          <el-input
            v-model="editForm.ConfigJson"
            type="textarea"
            :rows="5"
            placeholder='JSON格式, 如: {"titleField": "name"}'
          ></el-input>
          <div class="help-info">配置字段映射逻辑，如 titleField, subTitleField 等</div>
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="editDialog.visible = false">取消</el-button>
        <el-button type="primary" @click="handleSave">保存</el-button>
      </template>
    </el-dialog>

    <!-- 权限弹窗 -->
    <el-dialog v-model="assignDialog.visible" title="卡片权限分配" width="500px">
      <div v-loading="assignDialog.loading">
        <el-checkbox-group v-model="assignDialog.roleIds">
          <el-row>
            <el-col :span="12" v-for="role in allRoles" :key="role.Id">
              <el-checkbox :label="role.Id">{{ role.Name }}</el-checkbox>
            </el-col>
          </el-row>
        </el-checkbox-group>
      </div>
      <template #footer>
        <el-button @click="assignDialog.visible = false">取消</el-button>
        <el-button type="primary" @click="handleSaveAssign">提交分配</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts" name="WorkbenchCard">
import { ref, reactive, onMounted } from 'vue';
import { Plus, Edit, Delete, Search, User } from '@element-plus/icons-vue';
import { ElMessage } from 'element-plus';
import { 
  getFdWorkbenchGetAll, 
  postGenericDtoControllerBase5Create, 
  putGenericDtoControllerBase5Update, 
  deleteGenericDtoControllerBase5Delete,
  getFdWorkbenchGetCardRoles,
  postFdWorkbenchUpdateCardRoles
} from '@/api/fd-system-api-admin/fdWorkbench';
import { getFdRoleGetAll } from '@/api/fd-system-api-admin/fdRole';

const loading = ref(false);
const tableData = ref<any[]>([]);
const allRoles = ref<any[]>([]);

const editDialog = reactive({ visible: false, Id: '' });
const editForm = reactive({
  Name: '',
  Description: '',
  Icon: 'Menu',
  Type: 'List',
  DataSourceUrl: '',
  ConfigJson: '{}',
  DefaultWidth: 4,
  DefaultHeight: 6
});

const assignDialog = reactive({
  cardId: '',
  roleIds: [] as string[],
  visible: false,
  loading: false
});

const loadData = async () => {
  loading.value = true;
  try {
    const res = await getFdWorkbenchGetAll();
    tableData.value = res;
  } finally {
    loading.value = false;
  }
};

const openEditDialog = (row?: any) => {
  if (row) {
    editDialog.Id = row.Id;
    Object.assign(editForm, row);
  } else {
    editDialog.Id = '';
    Object.assign(editForm, {
      Name: '', Description: '', Icon: 'Menu', Type: 'List',
      DataSourceUrl: '', ConfigJson: '{}', DefaultWidth: 4, DefaultHeight: 6
    });
  }
  editDialog.visible = true;
};

const handleSave = async () => {
  try {
    if (editDialog.Id) {
      await putGenericDtoControllerBase5Update({ id: editDialog.Id }, editForm as any);
    } else {
      await postGenericDtoControllerBase5Create(editForm as any);
    }
    ElMessage.success('保存成功');
    editDialog.visible = false;
    loadData();
  } catch (err) {
    ElMessage.error('保存失败');
  }
};

const handleDelete = async (row: any) => {
  try {
    await deleteGenericDtoControllerBase5Delete({ id: row.Id });
    ElMessage.success('已删除');
    loadData();
  } catch (err) {
    ElMessage.error('删除失败');
  }
};

const openAssignDialog = async (row: any) => {
  assignDialog.cardId = row.Id;
  assignDialog.visible = true;
  assignDialog.loading = true;
  try {
    if (allRoles.value.length === 0) {
      const rolesRes = await getFdRoleGetAll();
      allRoles.value = rolesRes;
    }
    const res = await getFdWorkbenchGetCardRoles({ cardId: row.Id });
    assignDialog.roleIds = res;
  } catch (err) {
    console.error(err);
  } finally {
    assignDialog.loading = false;
  }
};

const handleSaveAssign = async () => {
  try {
    await postFdWorkbenchUpdateCardRoles({ cardId: assignDialog.cardId }, assignDialog.roleIds);
    ElMessage.success('权限分配成功');
    assignDialog.visible = false;
  } catch (err) {
    ElMessage.error('权限分配失败');
  }
};

onMounted(() => {
  loadData();
});
</script>

<style scoped lang="scss">
.help-info { font-size: 12px; color: var(--el-text-color-secondary); margin-top: 5px; }
.flex-row { display: flex; align-items: center; gap: 10px; }
.mx-2 { margin: 0 8px; }
</style>
