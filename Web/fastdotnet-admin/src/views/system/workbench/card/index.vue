<template>
  <div class="workbench-card-page layout-pd">
    <!-- 顶部操作栏 -->
    <div class="page-toolbar">
      <div class="toolbar-left">
        <span class="page-title">工作台卡片管理</span>
        <el-tag type="info" size="small">{{ tableData.length }} 个卡片</el-tag>
      </div>
      <div class="toolbar-right">
        <el-input
          v-model="searchText"
          placeholder="搜索卡片名称"
          style="width: 200px; margin-right: 12px;"
          clearable
        ></el-input>
        <el-button @click="loadData">刷新</el-button>
        <el-button type="primary" @click="openEditDialog()">新增卡片</el-button>
      </div>
    </div>

    <!-- 卡片画廊 -->
    <div v-loading="loading" class="card-gallery">
      <div
        v-for="card in filteredCards"
        :key="card.Id"
        class="card-preview"
      >
        <!-- 类型色条 -->
        <div class="type-bar" :class="'type-' + card.Type"></div>

        <div class="card-body">
          <!-- 图标 + 名称 -->
          <div class="card-main">
            <div class="card-icon-wrap" :class="'type-' + card.Type">
              <el-icon :size="22">
                <component :is="card.Icon || 'Menu'"></component>
              </el-icon>
            </div>
            <div class="card-info">
              <div class="card-name">{{ card.Name }}</div>
              <div class="card-desc">{{ card.Description || '暂无描述' }}</div>
            </div>
          </div>

          <!-- 卡片元信息 -->
          <div class="card-meta">
            <el-tag size="small" :type="getTypeTagColor(card.Type)">{{ card.Type }}</el-tag>
            <span class="card-size">{{ card.DefaultWidth }} × {{ card.DefaultHeight }}</span>
          </div>

          <!-- 操作 -->
          <div class="card-actions">
            <el-button size="small" @click="openEditDialog(card)">编辑</el-button>
            <el-button size="small" type="primary" @click="openAssignDialog(card)">权限</el-button>
            <el-popconfirm title="确定删除该卡片吗？" @confirm="handleDelete(card)">
              <template #reference>
                <el-button size="small" type="danger">删除</el-button>
              </template>
            </el-popconfirm>
          </div>
        </div>
      </div>

      <!-- 空态 -->
      <div v-if="!loading && filteredCards.length === 0" class="gallery-empty">
        <el-empty description="暂无卡片，点击右上角新增"></el-empty>
      </div>
    </div>

    <!-- 编辑弹窗：左右分栏 -->
    <el-dialog v-model="editDialog.visible" :title="editDialog.Id ? '编辑卡片' : '新增卡片'" width="780px" destroy-on-close>
      <div class="edit-layout">
        <!-- 左：表单 -->
        <div class="edit-form">
          <el-form :model="editForm" label-width="90px" label-position="left">
            <el-form-item label="卡片名称">
              <el-input v-model="editForm.Name" placeholder="请输入卡片名称"></el-input>
            </el-form-item>
            <el-form-item label="展示类型">
              <el-radio-group v-model="editForm.Type">
                <el-radio-button value="List">列表</el-radio-button>
                <el-radio-button value="Stat">统计</el-radio-button>
                <el-radio-button value="Chart">图表</el-radio-button>
              </el-radio-group>
            </el-form-item>
            <el-form-item label="数据源URL">
              <div class="url-row">
                <el-input v-model="editForm.DataSourceUrl" placeholder="/api/app/todo/my"></el-input>
                <el-button
                  type="primary"
                  :loading="schemaLoading"
                  :disabled="!editForm.DataSourceUrl"
                  @click="fetchSchema"
                >读取字段</el-button>
              </div>
              <div v-if="schemaMsg" class="schema-msg" :class="schemaError ? 'is-error' : 'is-success'">{{ schemaMsg }}</div>
            </el-form-item>
            <el-form-item label="卡片图标">
              <el-input v-model="editForm.Icon" placeholder="Element Plus 图标名，如 List"></el-input>
            </el-form-item>
            <el-form-item label="默认尺寸">
              <div class="size-row">
                <span>宽</span>
                <el-input-number v-model="editForm.DefaultWidth" :min="1" :max="12" controls-position="right"></el-input-number>
                <span>高</span>
                <el-input-number v-model="editForm.DefaultHeight" :min="2" :max="30" controls-position="right"></el-input-number>
              </div>
            </el-form-item>
            <el-form-item label="描述">
              <el-input v-model="editForm.Description" type="textarea" :rows="2" placeholder="简要描述卡片用途"></el-input>
            </el-form-item>

            <!-- 结构化配置：根据 Type 显示不同字段 -->
            <el-divider content-position="left">
              <span style="font-size: 12px; color: #999;">字段映射配置</span>
            </el-divider>

            <template v-if="editForm.Type === 'List'">
              <div class="field-config-header">
                <span class="field-config-label">字段列表</span>
                <el-button size="small" type="primary" link @click="addListField">+ 添加字段</el-button>
              </div>
              <div v-if="configModel.fields && configModel.fields.length > 0" class="field-rows">
                <div v-for="(field, index) in configModel.fields" :key="index" class="field-row">
                  <span class="field-index">{{ index + 1 }}</span>
                  <el-input
                    v-model="field.field"
                    placeholder="后端字段名，如 Title"
                    size="small"
                    style="flex: 1;"
                  ></el-input>
                  <el-select v-model="field.role" placeholder="显示角色" size="small" style="width: 110px;">
                    <el-option label="主标题" value="title"></el-option>
                    <el-option label="副标题" value="subTitle"></el-option>
                    <el-option label="时间" value="date"></el-option>
                    <el-option label="状态" value="status"></el-option>
                    <el-option label="链接" value="link"></el-option>
                    <el-option label="标签" value="tag"></el-option>
                  </el-select>
                  <el-button size="small" type="danger" link @click="removeListField(index)">删除</el-button>
                </div>
              </div>
              <div v-else class="field-empty-hint">暂未配置字段，点击上方添加</div>
            </template>

            <template v-if="editForm.Type === 'Stat'">
              <el-form-item label="数值字段">
                <el-input v-model="configModel.valueField" placeholder="如：Count"></el-input>
              </el-form-item>
              <el-form-item label="标签字段">
                <el-input v-model="configModel.labelField" placeholder="如：Label"></el-input>
              </el-form-item>
            </template>

            <template v-if="editForm.Type === 'Chart'">
              <el-form-item label="图表类型">
                <el-select v-model="configModel.chartType" placeholder="请选择">
                  <el-option label="折线图" value="line"></el-option>
                  <el-option label="柱状图" value="bar"></el-option>
                  <el-option label="饼图" value="pie"></el-option>
                </el-select>
              </el-form-item>
              <el-form-item label="X轴字段">
                <el-input v-model="configModel.xField" placeholder="如：Date"></el-input>
              </el-form-item>
              <el-form-item label="Y轴字段">
                <el-input v-model="configModel.yField" placeholder="如：Value"></el-input>
              </el-form-item>
            </template>
          </el-form>
        </div>

        <!-- 右：实时预览 -->
        <div class="edit-preview">
          <div class="preview-label">预览效果</div>
          <div class="preview-card" :class="'preview-type-' + editForm.Type">
            <div class="preview-header">
              <div class="preview-icon">
                <el-icon :size="16">
                  <component :is="editForm.Icon || 'Menu'"></component>
                </el-icon>
              </div>
              <span class="preview-title">{{ editForm.Name || '卡片名称' }}</span>
              <el-tag size="small" :type="getTypeTagColor(editForm.Type)" style="margin-left: auto;">{{ editForm.Type }}</el-tag>
            </div>
            <div class="preview-body">
              <div class="preview-hint">
                <div v-if="editForm.Type === 'List'" class="preview-list">
                  <div class="preview-list-item" v-for="n in 3" :key="n">
                    <div class="pli-dot"></div>
                    <div class="pli-text">{{ configModel.titleField || 'Title' }} 示例 {{ n }}</div>
                    <div class="pli-sub">{{ configModel.dateField || 'Date' }}</div>
                  </div>
                </div>
                <div v-else-if="editForm.Type === 'Stat'" class="preview-stat">
                  <div class="ps-number">2,048</div>
                  <div class="ps-label">{{ configModel.labelField || configModel.valueField || '统计值' }}</div>
                </div>
                <div v-else class="preview-chart">
                  <div class="pc-bar" v-for="(h, i) in [60, 80, 45, 90, 70]" :key="i" :style="{ height: h + '%' }"></div>
                </div>
              </div>
            </div>
            <div class="preview-footer">
              数据源: {{ editForm.DataSourceUrl || '未设置' }}
            </div>
          </div>
          <div class="preview-size-info">
            默认尺寸: {{ editForm.DefaultWidth }} 列 × {{ editForm.DefaultHeight }} 行
          </div>
        </div>
      </div>

      <template #footer>
        <el-button @click="editDialog.visible = false">取消</el-button>
        <el-button type="primary" @click="handleSave">保存卡片</el-button>
      </template>
    </el-dialog>

    <!-- 权限分配弹窗 -->
    <el-dialog v-model="assignDialog.visible" title="分配角色权限" width="480px">
      <div v-loading="assignDialog.loading" class="assign-body">
        <div class="assign-desc">选择哪些角色可以使用此卡片（默认对所有人可见）</div>
        <el-checkbox-group v-model="assignDialog.roleIds" class="role-list">
          <el-col :span="12" v-for="role in allRoles" :key="role.Id">
            <el-checkbox :value="role.Id" :label="role.Id">{{ role.Name }}</el-checkbox>
          </el-col>
        </el-checkbox-group>
      </div>
      <template #footer>
        <el-button @click="assignDialog.visible = false">取消</el-button>
        <el-button type="primary" @click="handleSaveAssign">确认分配</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted, watch } from 'vue';
import { ElMessage } from 'element-plus';
import request from '@/utils/request';
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
const searchText = ref('');

const editDialog = reactive({ visible: false, Id: '' });
const editForm = reactive({
  Name: '', Description: '', Icon: 'Menu',
  Type: 'List', DataSourceUrl: '',
  ConfigJson: '{}', DefaultWidth: 4, DefaultHeight: 6
});

// 结构化配置模型，避免用户手写 JSON
const configModel = reactive<any>({
  titleField: '', subTitleField: '', dateField: '',
  valueField: '', labelField: '',
  chartType: 'bar', xField: '', yField: ''
});

// 当 configModel 变化时同步到 editForm.ConfigJson
watch(configModel, () => {
  editForm.ConfigJson = JSON.stringify(configModel);
}, { deep: true });

// 动态字段操作（List 类型专用）
const addListField = () => {
  if (!configModel.fields) configModel.fields = [];
  configModel.fields.push({ field: '', role: 'title' });
};

const removeListField = (index: number) => {
  configModel.fields.splice(index, 1);
};

// 自动读取接口字段
const schemaLoading = ref(false);
const schemaMsg = ref('');
const schemaError = ref(false);

const fetchSchema = async () => {
  if (!editForm.DataSourceUrl) return;
  schemaLoading.value = true;
  schemaMsg.value = '';
  schemaError.value = false;
  try {
    const res = await request<any>(editForm.DataSourceUrl, { method: 'GET' });
    // 取第一条数据解析字段名
    let sample: any = null;
    if (Array.isArray(res) && res.length > 0) {
      sample = res[0];
    } else if (res && typeof res === 'object') {
      // 可能是 { items: [...] } 或 { data: [...] } 结构
      const arr = res.Items || res.items || res.Data || res.data || res.List || res.list;
      if (Array.isArray(arr) && arr.length > 0) sample = arr[0];
      else if (!Array.isArray(res)) sample = res;
    }

    if (!sample) {
      schemaMsg.value = '接口返回为空，无法解析字段';
      schemaError.value = true;
      return;
    }

    // 根据字段名自动猜测角色
    const roleGuess = (key: string): string => {
      const k = key.toLowerCase();
      if (/title|name|subject|标题|名称/.test(k)) return 'title';
      if (/desc|content|remark|备注|描述|内容/.test(k)) return 'subTitle';
      if (/time|date|at|创建|更新/.test(k)) return 'date';
      if (/status|state|状态/.test(k)) return 'status';
      if (/url|link|href|链接/.test(k)) return 'link';
      if (/tag|label|type|类型/.test(k)) return 'tag';
      return 'subTitle';
    };

    const fields = Object.keys(sample).map((key, i) => ({
      field: key,
      role: i === 0 ? 'title' : roleGuess(key)
    }));

    configModel.fields = fields;
    schemaMsg.value = `成功识别 ${fields.length} 个字段`;
    schemaError.value = false;
  } catch (e: any) {
    schemaMsg.value = `请求失败：${e?.message || '未知错误'}`;
    schemaError.value = true;
  } finally {
    schemaLoading.value = false;
  }
};

const assignDialog = reactive({
  cardId: '', roleIds: [] as string[], visible: false, loading: false
});

const filteredCards = computed(() => {
  if (!searchText.value) return tableData.value;
  return tableData.value.filter(c => c.Name?.includes(searchText.value));
});

const getTypeTagColor = (type: string) => {
  const map: Record<string, string> = { List: 'primary', Stat: 'success', Chart: 'warning' };
  return map[type] || 'info';
};

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
    // 反解 ConfigJson 到结构化模型
    try {
      const parsed = JSON.parse(row.ConfigJson || '{}');
      Object.assign(configModel, {
        fields: [],
        valueField: '', labelField: '',
        chartType: 'bar', xField: '', yField: ''
      });
      if (parsed.fields) configModel.fields = parsed.fields;
      if (parsed.valueField) configModel.valueField = parsed.valueField;
      if (parsed.labelField) configModel.labelField = parsed.labelField;
      if (parsed.chartType) configModel.chartType = parsed.chartType;
      if (parsed.xField) configModel.xField = parsed.xField;
      if (parsed.yField) configModel.yField = parsed.yField;
    } catch {}
  } else {
    editDialog.Id = '';
    Object.assign(editForm, {
      Name: '', Description: '', Icon: 'Menu', Type: 'List',
      DataSourceUrl: '', ConfigJson: '{}', DefaultWidth: 4, DefaultHeight: 6
    });
    Object.assign(configModel, {
      fields: [],
      valueField: '', labelField: '', chartType: 'bar', xField: '', yField: ''
    });
  }
  editDialog.visible = true;
};

const handleSave = async () => {
  try {
    editForm.ConfigJson = JSON.stringify(configModel);
    if (editDialog.Id) {
      await putGenericDtoControllerBase5Update({ id: editDialog.Id }, editForm as any);
    } else {
      await postGenericDtoControllerBase5Create(editForm as any);
    }
    ElMessage.success('保存成功');
    editDialog.visible = false;
    loadData();
  } catch {
    ElMessage.error('保存失败');
  }
};

const handleDelete = async (row: any) => {
  try {
    await deleteGenericDtoControllerBase5Delete({ id: row.Id });
    ElMessage.success('已删除');
    loadData();
  } catch {
    ElMessage.error('删除失败');
  }
};

const openAssignDialog = async (row: any) => {
  assignDialog.cardId = row.Id;
  assignDialog.visible = true;
  assignDialog.loading = true;
  try {
    if (allRoles.value.length === 0) {
      allRoles.value = await getFdRoleGetAll();
    }
    assignDialog.roleIds = await getFdWorkbenchGetCardRoles({ cardId: row.Id });
  } finally {
    assignDialog.loading = false;
  }
};

const handleSaveAssign = async () => {
  try {
    await postFdWorkbenchUpdateCardRoles({ cardId: assignDialog.cardId }, assignDialog.roleIds);
    ElMessage.success('权限分配成功');
    assignDialog.visible = false;
  } catch {
    ElMessage.error('权限分配失败');
  }
};

onMounted(loadData);
</script>

<style scoped lang="scss">
.workbench-card-page {
  padding: 20px;
  background: var(--el-fill-color-extra-light);
  min-height: 100%;
}

// 顶部工具栏
.page-toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: #fff;
  padding: 14px 20px;
  border-radius: 10px;
  margin-bottom: 20px;
  box-shadow: 0 1px 6px rgba(0,0,0,0.05);

  .page-title {
    font-size: 17px;
    font-weight: 600;
    margin-right: 10px;
  }
}

// 卡片画廊
.card-gallery {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(260px, 1fr));
  gap: 18px;
  min-height: 200px;
}

.card-preview {
  background: #fff;
  border-radius: 12px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.06);
  overflow: hidden;
  transition: all 0.3s;
  display: flex;
  flex-direction: column;

  &:hover {
    transform: translateY(-4px);
    box-shadow: 0 8px 24px rgba(0,0,0,0.12);
  }

  .type-bar {
    height: 4px;
    &.type-List { background: var(--el-color-primary); }
    &.type-Stat { background: var(--el-color-success); }
    &.type-Chart { background: var(--el-color-warning); }
  }

  .card-body {
    padding: 16px;
    display: flex;
    flex-direction: column;
    gap: 12px;
  }

  .card-main {
    display: flex;
    align-items: center;
    gap: 12px;
  }

  .card-icon-wrap {
    width: 44px;
    height: 44px;
    border-radius: 10px;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;
    &.type-List { background: var(--el-color-primary-light-9); color: var(--el-color-primary); }
    &.type-Stat { background: var(--el-color-success-light-9); color: var(--el-color-success); }
    &.type-Chart { background: var(--el-color-warning-light-9); color: var(--el-color-warning); }
  }

  .card-info {
    .card-name { font-weight: 600; font-size: 15px; }
    .card-desc { font-size: 12px; color: #999; margin-top: 3px; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; max-width: 160px; }
  }

  .card-meta {
    display: flex;
    align-items: center;
    gap: 8px;
    .card-size { font-size: 12px; color: #bbb; margin-left: auto; }
  }

  .card-actions {
    display: flex;
    gap: 6px;
    padding-top: 8px;
    border-top: 1px solid var(--el-border-color-lighter);
  }
}

// 编辑弹窗布局
.edit-layout {
  display: flex;
  gap: 24px;
  min-height: 460px;

  .edit-form { flex: 1; }

  .edit-preview {
    width: 220px;
    flex-shrink: 0;

    .preview-label {
      font-size: 12px;
      color: #999;
      margin-bottom: 10px;
      text-align: center;
    }

    .preview-card {
      border-radius: 10px;
      border: 1px solid var(--el-border-color-lighter);
      overflow: hidden;
      height: 260px;
      display: flex;
      flex-direction: column;

      .preview-header {
        display: flex;
        align-items: center;
        gap: 6px;
        padding: 10px 12px;
        border-bottom: 1px solid var(--el-border-color-lighter);
        .preview-title { font-weight: 600; font-size: 13px; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; max-width: 90px; }
        .preview-icon { color: var(--el-color-primary); }
      }

      .preview-body {
        flex: 1;
        padding: 10px 12px;
        overflow: hidden;
      }

      .preview-footer {
        font-size: 10px;
        color: #bbb;
        padding: 6px 12px;
        border-top: 1px solid var(--el-border-color-lighter);
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
      }
    }

    .preview-size-info {
      text-align: center;
      font-size: 11px;
      color: #bbb;
      margin-top: 8px;
    }
  }
}

// 预览内容
.preview-list {
  .preview-list-item {
    display: flex;
    align-items: center;
    gap: 6px;
    padding: 5px 0;
    border-bottom: 1px dashed #f0f0f0;
    .pli-dot { width: 6px; height: 6px; border-radius: 50%; background: var(--el-color-primary); flex-shrink: 0; }
    .pli-text { font-size: 11px; flex: 1; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
    .pli-sub { font-size: 10px; color: #bbb; flex-shrink: 0; }
  }
}

.preview-stat {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 100%;
  .ps-number { font-size: 36px; font-weight: bold; color: var(--el-color-primary); }
  .ps-label { font-size: 12px; color: #999; margin-top: 5px; }
}

.preview-chart {
  display: flex;
  align-items: flex-end;
  justify-content: center;
  gap: 6px;
  height: 100%;
  .pc-bar {
    width: 22px;
    background: linear-gradient(to top, var(--el-color-primary), var(--el-color-primary-light-5));
    border-radius: 3px 3px 0 0;
  }
}

// 权限弹窗
.assign-body {
  min-height: 120px;
  .assign-desc { font-size: 13px; color: #999; margin-bottom: 15px; }
  .role-list { display: flex; flex-wrap: wrap; gap: 8px; }
}

// 动态字段配置
.field-config-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
  .field-config-label { font-size: 13px; color: var(--el-text-color-secondary); }
}
.field-rows { display: flex; flex-direction: column; gap: 8px; margin-bottom: 8px; }
.field-row {
  display: flex;
  align-items: center;
  gap: 8px;
  background: var(--el-fill-color-extra-light);
  padding: 6px 10px;
  border-radius: 6px;
  .field-index { font-size: 12px; color: #bbb; width: 16px; flex-shrink: 0; }
}
.field-empty-hint {
  text-align: center;
  color: #bbb;
  font-size: 12px;
  padding: 15px;
  border: 1px dashed var(--el-border-color-lighter);
  border-radius: 6px;
}

// 尺寸输入行
.size-row {
  display: flex;
  align-items: center;
  gap: 8px;
  flex-wrap: wrap;
  font-size: 13px;

  .size-sep { color: var(--el-text-color-secondary); font-size: 12px; }
  .size-label { font-size: 12px; color: var(--el-color-primary); font-weight: 500; white-space: nowrap; }
  .size-divider { color: #ddd; margin: 0 4px; }
}

// 数据源 URL 行
.url-row {
  display: flex;
  gap: 8px;
  width: 100%;
  .el-input { flex: 1; }
}
.schema-msg {
  font-size: 12px;
  margin-top: 4px;
  &.is-success { color: var(--el-color-success); }
  &.is-error { color: var(--el-color-danger); }
}
</style>
