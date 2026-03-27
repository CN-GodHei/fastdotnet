<template>
	<div class="dict-management-container">
		<el-row :gutter="10" style="height: calc(100vh - 120px);">
			<!-- 左侧：字典类型列表 -->
			<el-col :span="8" style="height: 100%; overflow-y: auto;">
				<el-card shadow="hover" :body-style="{ padding: '2px', height: '100%' }">
					<template #header>
						<div style="display: flex; justify-content: space-between; align-items: center;">
							<span style="font-weight: bold;">字典类型</span>
							<el-button type="primary" size="small" icon="ele-Plus" @click="openTypeAddDialog">新增</el-button>
						</div>
					</template>
					
					<!-- 搜索框 -->
					<el-form :model="typeQueryParams" :inline="true" size="small">
						<el-form-item>
							<el-input v-model="typeQueryParams.keyword" placeholder="搜索类型名称或编码" clearable 
								@keyup.enter="handleTypeQuery" prefix-icon="ele-Search" />
						</el-form-item>
					</el-form>
					
					<!-- 类型列表 -->
					<el-table :data="state.typeList" style="width: 100%" v-loading="state.typeLoading" 
						highlight-current-row @current-change="handleTypeChange">
						<el-table-column prop="Name" label="名称" show-overflow-tooltip />
						<el-table-column prop="Code" label="编码" show-overflow-tooltip />
						<el-table-column prop="OrderNo" label="排序" width="60" />
						<el-table-column label="操作" width="120" fixed="right" align="center">
							<template #default="scope">
								<el-button icon="ele-Edit" size="small" text type="primary" 
									@click.stop="openTypeEditDialog(scope.row)" />
								<el-button icon="ele-Delete" size="small" text type="danger" 
									@click.stop="handleTypeDelete(scope.row)" />
							</template>
						</el-table-column>
					</el-table>
				</el-card>
			</el-col>
			
			<!-- 右侧：字典值列表 -->
			<el-col :span="16" style="height: 100%; overflow-y: auto;">
				<el-card shadow="hover" :body-style="{ padding: '2px', height: '100%' }">
					<template #header>
						<div style="display: flex; justify-content: space-between; align-items: center;">
							<div>
								<span style="font-weight: bold;">字典值</span>
								<span v-if="state.selectedType" style="margin-left: 10px; color: #909399; font-size: 13px;">
									- {{ state.selectedType.Name }} ({{ state.selectedType.Code }})
								</span>
							</div>
							<el-button v-if="state.selectedType" type="primary" size="small" icon="ele-Plus" 
								@click="openDataAddDialog">新增</el-button>
						</div>
					</template>
					
					<!-- 数据列表 -->
					<el-table :data="state.dataList" style="width: 100%" v-loading="state.dataLoading" border
						row-key="Id" :tree-props="{children: 'Children'}">
						<el-table-column prop="Label" label="标签" show-overflow-tooltip />
						<el-table-column prop="Value" label="值" show-overflow-tooltip />
						<el-table-column prop="ValueType" label="类型" width="80">
							<template #default="scope">
								<el-tag size="small" :type="getValueTypeTagType(scope.row.ValueType)">
									{{ getValueTypeText(scope.row.ValueType) }}
								</el-tag>
							</template>
						</el-table-column>
						<el-table-column prop="Code" label="编码" show-overflow-tooltip />
						<el-table-column prop="OrderNo" label="排序" width="60" />
						<el-table-column prop="TagType" label="状态标识" width="80">
							<template #default="scope">
								<el-tag v-if="scope.row.TagType" :type="scope.row.TagType" size="small">
									{{ scope.row.TagType }}
								</el-tag>
							</template>
						</el-table-column>
						<el-table-column prop="IsDefault" label="默认" width="60">
							<template #default="scope">
								<el-tag v-if="scope.row.IsDefault === 1" type="success" size="small">是</el-tag>
								<el-tag v-if="scope.row.IsDefault === 0" type="success" size="small">否</el-tag>
							</template>
						</el-table-column>
						<el-table-column prop="Status" label="状态" width="60">
							<template #default="scope">
								<el-tag :type="scope.row.Status === 1 ? 'success' : 'danger'" size="small">
									{{ scope.row.Status === 1 ? '启用' : '停用' }}
								</el-tag>
							</template>
						</el-table-column>
						<el-table-column label="操作" width="140" fixed="right" align="center">
							<template #default="scope">
								<el-button icon="ele-Edit" size="small" text type="primary" 
									@click.stop="openDataEditDialog(scope.row)" />
								<el-button icon="ele-Delete" size="small" text type="danger" 
									@click.stop="handleDataDelete(scope.row)" />
								<el-button v-if="!scope.row.Children || scope.row.Children.length === 0" 
									icon="ele-Plus" size="small" text type="warning" 
									@click.stop="openDataAddChildDialog(scope.row)" title="添加子节点" />
							</template>
						</el-table-column>
					</el-table>
					
					<!-- 空状态提示 -->
					<el-empty v-if="!state.selectedType" description="请从左侧选择字典类型" :image-size="80" />
				</el-card>
			</el-col>
		</el-row>
		
		<!-- 字典类型对话框 -->
		<el-dialog v-model="state.typeDialog.visible" :title="state.typeDialog.title" width="600px" 
			:close-on-click-modal="false">
			<el-form :model="state.typeFormData" ref="typeFormRef" label-width="80px">
				<el-form-item label="名称" prop="Name" :rules="[{ required: true, message: '请输入名称', trigger: 'blur' }]">
					<el-input v-model="state.typeFormData.Name" placeholder="请输入名称" maxlength="100" show-word-limit />
				</el-form-item>
				<el-form-item label="编码" prop="Code" :rules="[{ required: true, message: '请输入编码', trigger: 'blur' }]">
					<el-input v-model="state.typeFormData.Code" placeholder="请输入编码（建议格式：SYS_XXX）" 
						maxlength="100" show-word-limit :disabled="state.typeDialog.type === 'update'" />
				</el-form-item>
				<el-form-item label="排序" prop="OrderNo">
					<el-input-number v-model="state.typeFormData.OrderNo" :min="0" :max="999" />
				</el-form-item>
				<el-form-item label="备注" prop="Remark">
					<el-input v-model="state.typeFormData.Remark" type="textarea" :rows="3" maxlength="500" show-word-limit />
				</el-form-item>
				<el-form-item label="状态" prop="Status">
					<el-radio-group v-model="state.typeFormData.Status">
						<el-radio :label="1">启用</el-radio>
						<el-radio :label="0">停用</el-radio>
					</el-radio-group>
				</el-form-item>
				<el-form-item label="系统内置" prop="SysFlag" v-if="state.typeDialog.type === 'update'">
					<el-switch v-model="state.typeFormData.SysFlag" :active-value="1" :inactive-value="0" 
						active-text="是" inactive-text="否" disabled />
				</el-form-item>
			</el-form>
			<template #footer>
				<el-button @click="state.typeDialog.visible = false">取消</el-button>
				<el-button type="primary" @click="submitTypeForm">确定</el-button>
			</template>
		</el-dialog>
		
		<!-- 字典值对话框 -->
		<el-dialog v-model="state.dataDialog.visible" :title="state.dataDialog.title" width="700px" 
			:close-on-click-modal="false">
			<el-form :model="state.dataFormData" ref="dataFormRef" label-width="100px">
				<el-form-item label="标签" prop="Label" :rules="[{ required: true, message: '请输入标签', trigger: 'blur' }]">
					<el-input v-model="state.dataFormData.Label" placeholder="前端显示的文本" maxlength="128" show-word-limit />
				</el-form-item>
				<el-form-item label="值" prop="Value" :rules="[{ required: true, message: '请输入值', trigger: 'blur' }]">
					<el-input v-model="state.dataFormData.Value" placeholder="存储到数据库的值" maxlength="500" />
				</el-form-item>
				<el-form-item label="数据类型" prop="ValueType">
					<el-select v-model="state.dataFormData.ValueType" placeholder="请选择数据类型" style="width: 100%">
						<el-option label="字符串" :value="0" />
						<el-option label="整数" :value="1" />
						<el-option label="长整型" :value="2" />
						<el-option label="浮点数" :value="3" />
						<el-option label="金额" :value="4" />
						<el-option label="布尔" :value="5" />
						<el-option label="日期时间" :value="6" />
						<el-option label="JSON 对象" :value="7" />
						<el-option label="JSON 数组" :value="8" />
					</el-select>
				</el-form-item>
				<el-form-item label="编码" prop="Code">
					<el-input v-model="state.dataFormData.Code" placeholder="用于代码访问的编码" maxlength="100" 
						:disabled="state.dataDialog.type === 'update'" />
				</el-form-item>
				<el-form-item label="父级 ID" prop="ParentId" v-if="state.dataDialog.type === 'addChild'">
					<el-input v-model="state.dataFormData.ParentId" disabled />
				</el-form-item>
				<el-form-item label="排序" prop="OrderNo">
					<el-input-number v-model="state.dataFormData.OrderNo" :min="0" :max="999" />
				</el-form-item>
				<el-form-item label="状态标识" prop="TagType">
					<el-select v-model="state.dataFormData.TagType" placeholder="Tag 颜色" clearable>
						<el-option label="primary" value="primary" />
						<el-option label="success" value="success" />
						<el-option label="warning" value="warning" />
						<el-option label="danger" value="danger" />
						<el-option label="info" value="info" />
					</el-select>
				</el-form-item>
				<el-form-item label="CSS 类名" prop="CssClass">
					<el-input v-model="state.dataFormData.CssClass" placeholder="自定义 CSS 类名" maxlength="100" />
				</el-form-item>
				<el-form-item label="列表样式" prop="ListClass">
					<el-input v-model="state.dataFormData.ListClass" placeholder="列表额外样式类" maxlength="100" />
				</el-form-item>
				<el-form-item label="是否默认" prop="IsDefault">
					<el-switch v-model="state.dataFormData.IsDefault" :active-value="1" :inactive-value="0" />
				</el-form-item>
				<el-form-item label="状态" prop="Status">
					<el-radio-group v-model="state.dataFormData.Status">
						<el-radio :label="1">启用</el-radio>
						<el-radio :label="0">停用</el-radio>
					</el-radio-group>
				</el-form-item>
				<el-form-item label="备注" prop="Remark">
					<el-input v-model="state.dataFormData.Remark" type="textarea" :rows="2" maxlength="500" />
				</el-form-item>
			</el-form>
			<template #footer>
				<el-button @click="state.dataDialog.visible = false">取消</el-button>
				<el-button type="primary" @click="submitDataForm">确定</el-button>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="DictManagement">
import { ref, reactive, onMounted } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import * as FdDictTypeApi from '@/api/fd-system-api-admin/FdDictType';
import * as FdDictDataApi from '@/api/fd-system-api-admin/FdDictData';

// 状态管理
const state = reactive({
	typeLoading: false,
	dataLoading: false,
	typeList: [] as any[],
	dataList: [] as any[],
	selectedType: null as any | null,
	
	// 类型对话框
	typeDialog: {
		visible: false,
		title: '',
		type: 'create' as 'create' | 'update',
	},
	typeFormData: {
		Id: '',
		Name: '',
		Code: '',
		OrderNo: 100,
		Remark: '',
		Status: 1,
		SysFlag: 0,
		PluginId: '',
	},
	
	// 数据对话框
	dataDialog: {
		visible: false,
		title: '',
		type: 'create' as 'create' | 'update' | 'addChild',
	},
	dataFormData: {
		Id: '',
		DictTypeId: '',
		DictTypeCode: '',
		Label: '',
		Value: '',
		ValueType: 0,
		Code: '',
		ParentId: '',
		Level: 0,
		OrderNo: 100,
		TagType: '',
		CssClass: '',
		ListClass: '',
		IsDefault: 0,
		Status: 1,
		Remark: '',
	},
});

// 查询参数
const typeQueryParams = reactive({
	keyword: '',
});

const typeFormRef = ref();
const dataFormRef = ref();

// 获取字典类型列表
const getTypeList = async () => {
	state.typeLoading = true;
	try {
		const response = await FdDictTypeApi.postApiAdminFdDictTypePageSearch({
			PageIndex: 1,
			PageSize: 1000, // 一次性加载所有类型
			DynamicQuery: typeQueryParams.keyword ? `Name.Contains("${typeQueryParams.keyword}") || Code.Contains("${typeQueryParams.keyword}")` : undefined
		});
		state.typeList = response.Items || [];
	} catch (error) {
		ElMessage.error('获取字典类型失败');
	} finally {
		state.typeLoading = false;
	}
};

// 获取字典值列表（树形结构）
const getDataList = async (typeId: string, typeCode: string) => {
	if (!typeId) return;
	
	state.dataLoading = true;
	try {
		const response = await FdDictDataApi.postApiAdminFdDictDataPageSearch({
			PageIndex: 1,
			PageSize: 1000,
			DynamicQuery: `DictTypeId == "${typeId}"`
		});
		
		// 构建树形结构
		state.dataList = buildTree(response.Items || []);
	} catch (error) {
		ElMessage.error('获取字典值失败');
	} finally {
		state.dataLoading = false;
	}
};

// 构建树形结构
const buildTree = (items: any[]) => {
	const itemMap = new Map();
	const roots: any[] = [];
	
	// 创建映射
	items.forEach(item => {
		itemMap.set(item.Id, { ...item, Children: [] });
	});
	
	// 构建树
	items.forEach(item => {
		const node = itemMap.get(item.Id);
		if (!item.ParentId || item.ParentId === '') {
			roots.push(node);
		} else {
			const parent = itemMap.get(item.ParentId);
			if (parent) {
				parent.Children.push(node);
			} else {
				// 找不到父节点，作为根节点
				roots.push(node);
			}
		}
	});
	
	return roots;
};

// 类型切换处理
const handleTypeChange = (row: any) => {
	state.selectedType = row;
	if (row?.Id) {
		getDataList(row.Id, row.Code);
	}
};

// 类型搜索
const handleTypeQuery = () => {
	getTypeList();
};



// 打开类型新增对话框
const openTypeAddDialog = () => {
	state.typeDialog.visible = true;
	state.typeDialog.title = '新增字典类型';
	state.typeDialog.type = 'create';
	state.typeFormData = {
		Id: '',
		Name: '',
		Code: '',
		OrderNo: 100,
		Remark: '',
		Status: 1,
		SysFlag: 0,
		PluginId: '',
	};
	typeFormRef.value?.resetFields();
};

// 打开类型编辑对话框
const openTypeEditDialog = (row: any) => {
	state.typeDialog.visible = true;
	state.typeDialog.title = '编辑字典类型';
	state.typeDialog.type = 'update';
	state.typeFormData = { 
		...row,
		PluginId: row.PluginId || '',
	};
};

// 提交类型表单
const submitTypeForm = async () => {
	if (!await typeFormRef.value?.validate()) return;
	
	try {
		if (state.typeDialog.type === 'update') {
			await FdDictTypeApi.putApiAdminFdDictTypeId({ id: state.typeFormData.Id }, state.typeFormData);
			ElMessage.success('更新成功');
		} else {
			await FdDictTypeApi.postApiAdminFdDictType(state.typeFormData);
			ElMessage.success('添加成功');
		}
		state.typeDialog.visible = false;
		getTypeList();
	} catch (error) {
		ElMessage.error(state.typeDialog.type === 'update' ? '更新失败' : '添加失败');
	}
};

// 删除类型
const handleTypeDelete = (row: APIModel.FdDictTypeDto) => {
	ElMessageBox.confirm(`确定删除字典类型 "${row.Name}" 吗？`, '警告', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning',
	}).then(async () => {
		try {
			await FdDictTypeApi.deleteApiAdminFdDictTypeId({ id: row.Id });
			ElMessage.success('删除成功');
			if ((state.selectedType as any)?.Id === row.Id) {
				state.selectedType = null;
				state.dataList = [];
			}
			getTypeList();
		} catch (error) {
			ElMessage.error('删除失败');
		}
	}).catch(() => {});
};

// 打开数据新增对话框
const openDataAddDialog = () => {
	if (!state.selectedType) {
		ElMessage.warning('请先选择字典类型');
		return;
	}
	
	state.dataDialog.visible = true;
	state.dataDialog.title = '新增字典值';
	state.dataDialog.type = 'create';
	state.dataFormData = {
		Id: '',
		DictTypeId: state.selectedType.Id,
		DictTypeCode: state.selectedType.Code,
		Label: '',
		Value: '',
		ValueType: 0,
		Code: '',
		ParentId: '',
		Level: 0,
		OrderNo: 100,
		TagType: '',
		CssClass: '',
		ListClass: '',
		IsDefault: 0,
		Status: 1,
		Remark: '',
	};
	dataFormRef.value?.resetFields();
};

// 打开子节点新增对话框
const openDataAddChildDialog = (parent: any) => {
	state.dataDialog.visible = true;
	state.dataDialog.title = '新增子节点';
	state.dataDialog.type = 'addChild';
	state.dataFormData = {
		Id: '',
		DictTypeId: parent.DictTypeId || '',
		DictTypeCode: parent.DictTypeCode || '',
		Label: '',
		Value: '',
		ValueType: 0,
		Code: '',
		ParentId: parent.Id || '',
		Level: (parent.Level || 0) + 1,
		OrderNo: 100,
		TagType: '',
		CssClass: '',
		ListClass: '',
		IsDefault: 0,
		Status: 1,
		Remark: '',
	};
};

// 打开数据编辑对话框
const openDataEditDialog = (row: APIModel.FdDictDataDto) => {
	state.dataDialog.visible = true;
	state.dataDialog.title = '编辑字典值';
	state.dataDialog.type = 'update';
	state.dataFormData = { ...row };
};

// 提交数据表单
const submitDataForm = async () => {
	if (!await dataFormRef.value?.validate()) return;
	
	try {
		if (state.dataDialog.type === 'update') {
			await FdDictDataApi.putApiAdminFdDictDataId({ id: state.dataFormData.Id }, state.dataFormData);
			ElMessage.success('更新成功');
		} else {
			await FdDictDataApi.postApiAdminFdDictData(state.dataFormData);
			ElMessage.success('添加成功');
		}
		state.dataDialog.visible = false;
		if (state.selectedType) {
			getDataList(state.selectedType.Id, state.selectedType.Code);
		}
	} catch (error) {
		ElMessage.error(state.dataDialog.type === 'update' ? '更新失败' : '添加失败');
	}
};

// 删除数据
const handleDataDelete = (row: APIModel.FdDictDataDto) => {
	ElMessageBox.confirm(`确定删除字典值 "${row.Label}" 吗？`, '警告', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning',
	}).then(async () => {
		try {
			await FdDictDataApi.deleteApiAdminFdDictDataId({ id: row.Id });
			ElMessage.success('删除成功');
			if (state.selectedType) {
				getDataList(state.selectedType.Id, state.selectedType.Code);
			}
		} catch (error) {
			ElMessage.error('删除失败');
		}
	}).catch(() => {});
};

// 获取 ValueType 显示文本
const getValueTypeText = (valueType: number) => {
	const types = ['字符串', '整数', '长整型', '浮点数', '金额', '布尔', '日期时间', 'JSON 对象', 'JSON 数组'];
	return types[valueType] || '未知';
};

// 获取 ValueType Tag 类型
const getValueTypeTagType = (valueType: number) => {
	const tagTypes = ['', 'success', 'primary', 'warning', 'danger', 'info', '', ''];
	return tagTypes[valueType] || '';
};

onMounted(() => {
	getTypeList();
});
</script>

<style scoped lang="scss">
.dict-management-container {
	padding: 10px;
	
	.el-col {
		height: 100%;
		
		.el-card {
			height: 100%;
			display: flex;
			flex-direction: column;
			
			:deep(.el-card__body) {
				flex: 1;
				overflow-y: auto;
			}
		}
	}
}

:deep(.el-table__row.current-row) {
	background-color: #ecf5ff !important;
}

// 自定义选中行样式
:deep(.el-table__row:hover) {
	background-color: #f5f7fa !important;
}
</style>
