<template>
	<div class="gbdata-container">
		<el-row :gutter="10" style="height: calc(100vh - 120px);">
			<!-- 左侧：国标标准列表 -->
			<el-col :span="12" style="height: 100%; overflow: hidden;">
				<el-card shadow="hover" :body-style="{ padding: '10px', height: '100%' }" style="height: 100%;">
					<template #header>
						<div class="card-header">
							<span><el-icon><Document /></el-icon> 国家标准列表</span>
							<el-button type="primary" size="small" icon="Plus" @click="openStandardDialog">新增标准</el-button>
						</div>
					</template>
					
					<!-- 搜索区域 -->
					<el-form :model="standardState.queryParams" ref="standardQueryForm" :inline="true" size="default">
						<el-form-item label="标准编号" prop="StandardCode">
							<el-input v-model="standardState.queryParams.StandardCode" placeholder="如 GB/T 2260" clearable style="width: 120px" />
						</el-form-item>
						<el-form-item label="标准名称" prop="StandardName">
							<el-input v-model="standardState.queryParams.StandardName" placeholder="请输入名称" clearable style="width: 150px" />
						</el-form-item>
						<el-form-item>
							<el-button type="primary" icon="Search" @click="handleStandardQuery">查询</el-button>
							<el-button icon="Refresh" @click="resetStandardQuery">重置</el-button>
						</el-form-item>
					</el-form>
					
					<!-- 表格列表 -->
					<div class="table-wrapper">
						<el-table 
							:data="standardState.tableData.data" 
							style="width: 100%" 
							v-loading="standardState.loading" 
							border
							highlight-current-row
							@current-change="handleStandardSelect"
							row-key="Id"
						>
							<el-table-column prop="StandardCode" label="标准编号" show-overflow-tooltip width="100" />
							<el-table-column prop="StandardName" label="标准名称" show-overflow-tooltip width="200" />
							<el-table-column prop="CurrentVersion" label="版本号" width="80" />
							<el-table-column prop="TotalItems" label="条目数" width="70" align="center" />
							<el-table-column prop="Status" label="状态" width="70" align="center">
								<template #default="{ row }">
									<el-tag :type="row.Status ? 'success' : 'danger'" size="small">
										{{ row.Status ? '现行' : '废止' }}
									</el-tag>
								</template>
							</el-table-column>
							<el-table-column label="操作" width="150" fixed="right" align="center">
								<template #default="scope">
									<el-button icon="Edit" size="small" text type="primary" @click.stop="openStandardDialog(scope.row)">编辑</el-button>
									<el-button icon="Delete" size="small" text type="danger" @click.stop="handleStandardDelete(scope.row)">删除</el-button>
								</template>
							</el-table-column>
						</el-table>
					</div>
					
					<!-- 分页 -->
					<el-pagination
						v-model:currentPage="standardState.pagination.page"
						v-model:page-size="standardState.pagination.pageSize"
						:total="standardState.pagination.total"
						:page-sizes="[10, 20, 50]"
						size="small"
						background
						@size-change="handleStandardSizeChange"
						@current-change="handleStandardCurrentChange"
						layout="total, sizes, prev, pager, next"
						style="margin-top: 10px; justify-content: flex-end;"
					/>
				</el-card>
			</el-col>
			
			<!-- 右侧：标准条目详情 -->
			<el-col :span="12" style="height: 100%; overflow: hidden;">
				<el-card shadow="hover" :body-style="{ padding: '10px', height: '100%' }" style="height: 100%;">
					<template #header>
						<div class="card-header">
							<span><el-icon><List /></el-icon> 标准条目详情</span>
							<div>
								<el-button 
									type="primary" 
									size="small" 
									icon="Plus" 
									@click="openItemDialog"
									:disabled="!selectedStandardId"
								>
									新增条目
								</el-button>
								<el-button 
									size="small" 
									icon="Download" 
									@click="exportItems"
									:disabled="!selectedStandardId"
								>
									导出
								</el-button>
							</div>
						</div>
					</template>
					
					<!-- 当前选中的标准信息 -->
					<el-alert
						v-if="selectedStandardInfo && selectedStandardInfo.StandardCode"
						title="当前查看的标准"
						type="info"
						show-icon
						closable
						style="margin-bottom: 10px;"
					>
						<template #default>
							<strong>{{ selectedStandardInfo.StandardName }}</strong> 
							（编号：{{ selectedStandardInfo.StandardCode }}，版本：{{ selectedStandardInfo.CurrentVersion }}）
						</template>
					</el-alert>
					<el-empty v-else description="请从左侧选择要查看的标准" :image-size="80" />
					
					<!-- 条目搜索 -->
					<el-form v-show="selectedStandardId" :model="itemState.queryParams" :inline="true" size="default">
						<el-form-item label="条目编码" prop="ItemCode">
							<el-input v-model="itemState.queryParams.ItemCode" placeholder="请输入编码" clearable style="width: 120px" />
						</el-form-item>
						<el-form-item label="条目名称" prop="ItemName">
							<el-input v-model="itemState.queryParams.ItemName" placeholder="请输入名称" clearable style="width: 150px" />
						</el-form-item>
						<el-form-item>
							<el-button type="primary" icon="Search" @click="handleItemQuery">查询</el-button>
							<el-button icon="Refresh" @click="resetItemQuery">重置</el-button>
						</el-form-item>
					</el-form>
					
					<!-- 条目表格 -->
					<div v-show="selectedStandardId" class="table-wrapper">
						<el-table 
							:data="itemState.tableData.data" 
							style="width: 100%" 
							v-loading="itemState.loading" 
							border
							row-key="Id"
							tree-props="{children: 'Children'}"
						>
							<el-table-column prop="ItemCode" label="条目编码" show-overflow-tooltip width="120" />
							<el-table-column prop="ItemName" label="条目名称" show-overflow-tooltip width="200" />
							<el-table-column prop="Level" label="层级" width="60" align="center" />
							<el-table-column prop="Sort" label="排序" width="60" align="center" />
							<el-table-column prop="Status" label="状态" width="60" align="center">
								<template #default="{ row }">
									<el-tag :type="row.Status ? 'success' : 'danger'" size="small">
										{{ row.Status ? '启用' : '废止' }}
									</el-tag>
								</template>
							</el-table-column>
							<el-table-column label="操作" width="150" fixed="right" align="center">
								<template #default="scope">
									<el-button icon="Edit" size="small" text type="primary" @click.stop="openItemDialog(scope.row)">编辑</el-button>
									<el-button icon="Delete" size="small" text type="danger" @click.stop="handleItemDelete(scope.row)">删除</el-button>
								</template>
							</el-table-column>
						</el-table>
					</div>
					
					<!-- 条目的分页 -->
					<el-pagination
						v-show="selectedStandardId"
						v-model:currentPage="itemState.pagination.page"
						v-model:page-size="itemState.pagination.pageSize"
						:total="itemState.pagination.total"
						:page-sizes="[10, 20, 50]"
						size="small"
						background
						@size-change="handleItemSizeChange"
						@current-change="handleItemCurrentChange"
						layout="total, sizes, prev, pager, next"
						style="margin-top: 10px; justify-content: flex-end;"
					/>
				</el-card>
			</el-col>
		</el-row>
		
		<!-- 国标标准新增/编辑对话框 -->
		<el-dialog v-model="standardState.dialog.visible" :title="standardState.dialog.title" width="700px" draggable>
			<el-form :model="standardState.formData" ref="standardFormRef" label-width="120px">
				<el-row :gutter="10">
					<el-col :span="12">
						<el-form-item label="标准编号" prop="StandardCode" :rules="{ required: true, message: '请输入标准编号', trigger: 'blur' }">
							<el-input v-model="standardState.formData.StandardCode" placeholder="如 GB/T 2260" maxlength="255" show-word-limit clearable />
						</el-form-item>
					</el-col>
					<el-col :span="12">
						<el-form-item label="标准名称" prop="StandardName" :rules="{ required: true, message: '请输入标准名称', trigger: 'blur' }">
							<el-input v-model="standardState.formData.StandardName" placeholder="请输入中文名称" maxlength="255" show-word-limit clearable />
						</el-form-item>
					</el-col>
				</el-row>
				
				<el-row :gutter="10">
					<el-col :span="12">
						<el-form-item label="英文名称" prop="StandardNameEn">
							<el-input v-model="standardState.formData.StandardNameEn" placeholder="可选" maxlength="255" show-word-limit clearable />
						</el-form-item>
					</el-col>
					<el-col :span="12">
						<el-form-item label="标准类型" prop="StandardType" :rules="{ required: true, message: '请选择标准类型', trigger: 'change' }">
							<el-select v-model="standardState.formData.StandardType" placeholder="请选择" style="width: 100%">
								<el-option label="GB - 强制性国标" value="GB" />
								<el-option label="GB/T - 推荐性国标" value="GB/T" />
								<el-option label="DB - 地方标准" value="DB" />
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				
				<el-row :gutter="10">
					<el-col :span="12">
						<el-form-item label="发布部门" prop="PublishDepartment">
							<el-input v-model="standardState.formData.PublishDepartment" placeholder="如国家标准化管理委员会" maxlength="255" show-word-limit clearable />
						</el-form-item>
					</el-col>
					<el-col :span="12">
						<el-form-item label="发布日期" prop="PublishDate">
							<el-date-picker v-model="standardState.formData.PublishDate" type="date" placeholder="选择日期" style="width: 100%" value-format="YYYY-MM-DD" />
						</el-form-item>
					</el-col>
				</el-row>
				
				<el-row :gutter="10">
					<el-col :span="12">
						<el-form-item label="实施日期" prop="ImplementDate">
							<el-date-picker v-model="standardState.formData.ImplementDate" type="date" placeholder="选择日期" style="width: 100%" value-format="YYYY-MM-DD" />
						</el-form-item>
					</el-col>
					<el-col :span="12">
						<el-form-item label="当前版本" prop="CurrentVersion">
							<el-input v-model="standardState.formData.CurrentVersion" placeholder="如 2023" maxlength="50" clearable />
						</el-form-item>
					</el-col>
				</el-row>
				
				<el-row :gutter="10">
					<el-col :span="12">
						<el-form-item label="状态" prop="Status">
							<el-radio-group v-model="standardState.formData.Status">
								<el-radio :label="true">现行有效</el-radio>
								<el-radio :label="false">已废止</el-radio>
							</el-radio-group>
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			
			<template #footer>
				<el-button @click="standardState.dialog.visible = false">取消</el-button>
				<el-button type="primary" @click="submitStandardForm">确定</el-button>
			</template>
		</el-dialog>
		
		<!-- 标准条目新增/编辑对话框 -->
		<el-dialog v-model="itemState.dialog.visible" :title="itemState.dialog.title" width="700px" draggable>
			<el-form :model="itemState.formData" ref="itemFormRef" label-width="120px">
				<el-row :gutter="10">
					<el-col :span="12">
						<el-form-item label="条目编码" prop="ItemCode" :rules="{ required: true, message: '请输入条目编码', trigger: 'blur' }">
							<el-input v-model="itemState.formData.ItemCode" placeholder="如 110000" maxlength="255" show-word-limit clearable />
						</el-form-item>
					</el-col>
					<el-col :span="12">
						<el-form-item label="条目名称" prop="ItemName" :rules="{ required: true, message: '请输入条目名称', trigger: 'blur' }">
							<el-input v-model="itemState.formData.ItemName" placeholder="如 北京市" maxlength="255" show-word-limit clearable />
						</el-form-item>
					</el-col>
				</el-row>
				
				<el-row :gutter="10">
					<el-col :span="12">
						<el-form-item label="英文名称" prop="ItemNameEn">
							<el-input v-model="itemState.formData.ItemNameEn" placeholder="可选" maxlength="255" show-word-limit clearable />
						</el-form-item>
					</el-col>
					<el-col :span="12">
						<el-form-item label="父级编码" prop="ParentCode">
							<el-input v-model="itemState.formData.ParentCode" placeholder="顶级条目留空" maxlength="255" show-word-limit clearable />
						</el-form-item>
					</el-col>
				</el-row>
				
				<el-row :gutter="10">
					<el-col :span="12">
						<el-form-item label="层级" prop="Level" :rules="{ required: true, message: '请输入层级', trigger: 'blur' }">
							<el-input-number v-model="itemState.formData.Level" :min="1" :max="10" style="width: 100%" />
						</el-form-item>
					</el-col>
					<el-col :span="12">
						<el-form-item label="排序" prop="Sort">
							<el-input-number v-model="itemState.formData.Sort" :min="0" style="width: 100%" />
						</el-form-item>
					</el-col>
				</el-row>
				
				<el-row :gutter="10">
					<el-col :span="12">
						<el-form-item label="状态" prop="Status">
							<el-radio-group v-model="itemState.formData.Status">
								<el-radio :label="true">启用</el-radio>
								<el-radio :label="false">废止</el-radio>
							</el-radio-group>
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			
			<template #footer>
				<el-button @click="itemState.dialog.visible = false">取消</el-button>
				<el-button type="primary" @click="submitItemForm">确定</el-button>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="GbData">
import { ref, reactive, onMounted } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { Document, List, Plus, Edit, Delete, Search, Refresh, Download } from '@element-plus/icons-vue';
import { buildMixedQuery } from '@/utils/queryBuilder';
import * as StandardApi from '@/api/fd-system-api-admin/FdNationalStandard';
import * as ItemApi from '@/api/fd-system-api-admin/FdNationalStandardItem';

// ========== 左侧标准相关 ==========
const standardQueryForm = ref();
const standardFormRef = ref();
const selectedStandardId = ref<string>('');
const selectedStandardInfo = ref<any>(null);

const standardState = reactive({
	loading: false,
	tableData: {
		data: [] as APIModel.FdNationalStandardDto[]
	},
	queryParams: {
		StandardCode: null,
		StandardName: null,
	},
	pagination: { page: 1, pageSize: 20, total: 0 },
	dialog: {
		visible: false,
		title: '',
		type: 'create' as 'create' | 'update',
	},
	formData: {
		Id: '',
		StandardCode: '',
		StandardName: '',
		StandardNameEn: '',
		StandardType: '',
		PublishDepartment: '',
		PublishDate: '',
		ImplementDate: '',
		CurrentVersion: '',
		Status: true,
		TotalItems: 0,
	}
});

// 获取标准列表
const getStandardList = async () => {
	standardState.loading = true;
	try {
		const queryConfig = {
			customs: [
				{ field: 'StandardCode', operator: 'eq', value: standardState.queryParams.StandardCode },
				{ field: 'StandardName', operator: 'eq', value: standardState.queryParams.StandardName },
			],
			ranges: {}
		};
		const searchBody: APIModel.PageQueryByConditionDto = {
			PageIndex: standardState.pagination.page,
			PageSize: standardState.pagination.pageSize,
		};
		const queryResult = buildMixedQuery(queryConfig);
		if (queryResult.dynamicQuery) {
			searchBody.DynamicQuery = queryResult.dynamicQuery;
			searchBody.QueryParameters = queryResult.queryParameters;
		}
		
		const response = await StandardApi.postApiFdNationalStandardPageSearch(searchBody);
		standardState.tableData.data = response.Items || [];
		standardState.pagination.total = response.PageInfo?.Total || 0;
	} catch (error) {
		ElMessage.error('获取标准数据失败');
	} finally {
		standardState.loading = false;
	}
};

const handleStandardQuery = () => {
	standardState.pagination.page = 1;
	getStandardList();
};

const resetStandardQuery = () => {
	standardQueryForm.value?.resetFields();
	handleStandardQuery();
};

const handleStandardSizeChange = (val: number) => {
	standardState.pagination.pageSize = val;
	getStandardList();
};

const handleStandardCurrentChange = (val: number) => {
	standardState.pagination.page = val;
	getStandardList();
};

// 选择标准
const handleStandardSelect = (row: any) => {
	console.log('选中的行:', row);
	if (row && row.Id) {
		selectedStandardId.value = row.Id;
		selectedStandardInfo.value = row;
		console.log('StandardId:', selectedStandardId.value);
		// 加载该标准的条目
		itemState.pagination.page = 1;
		getItemList();
	} else {
		selectedStandardId.value = '';
		selectedStandardInfo.value = null;
		itemState.tableData.data = [];
	}
};

// 打开标准对话框
const openStandardDialog = (row?: any) => {
	standardState.dialog.visible = true;
	standardState.dialog.title = row ? '编辑标准' : '新增标准';
	standardState.dialog.type = row ? 'update' : 'create';
	
	if (row) {
		standardState.formData = { ...row };
	} else {
		standardFormRef.value?.resetFields();
		standardState.formData = {
			Id: '',
			StandardCode: '',
			StandardName: '',
			StandardNameEn: '',
			StandardType: 'GB/T',
			PublishDepartment: '',
			PublishDate: '',
			ImplementDate: '',
			CurrentVersion: '',
			Status: true,
			TotalItems: 0,
		};
	}
};

// 提交标准表单
const submitStandardForm = () => {
	standardFormRef.value?.validate(async (valid: boolean) => {
		if (!valid) return;
		try {
			if (standardState.dialog.type === 'update' && standardState.formData.Id) {
				await StandardApi.putApiFdNationalStandardId({ id: standardState.formData.Id }, standardState.formData);
				ElMessage.success('更新成功');
			} else {
				await StandardApi.postApiFdNationalStandard(standardState.formData);
				ElMessage.success('添加成功');
			}
			standardState.dialog.visible = false;
			getStandardList();
		} catch (error) {
			ElMessage.error(standardState.dialog.type === 'update' ? '更新失败' : '添加失败');
		}
	});
};

// 删除标准
const handleStandardDelete = (row: APIModel.FdNationalStandardDto) => {
	ElMessageBox.confirm('确定删除该标准吗？删除后其所有条目也将被删除！', '警告', {
		type: 'warning'
	})
	.then(async () => {
		await StandardApi.deleteApiFdNationalStandardId({ id: row.Id });
		ElMessage.success('删除成功');
		if (selectedStandardId.value === row.Id) {
			selectedStandardId.value = '';
			selectedStandardInfo.value = null;
			itemState.tableData.data = [];
		}
		getStandardList();
	})
	.catch(() => {});
};

// ========== 右侧条目相关 ==========
const itemFormRef = ref();

const itemState = reactive({
	loading: false,
	tableData: {
		data: [] as APIModel.FdNationalStandardItemDto[]
	},
	queryParams: {
		ItemCode: null,
		ItemName: null,
	},
	pagination: { page: 1, pageSize: 20, total: 0 },
	dialog: {
		visible: false,
		title: '',
		type: 'create' as 'create' | 'update',
	},
	formData: {
		Id: '',
		StandardId: '',
		ItemCode: '',
		ItemName: '',
		ItemNameEn: '',
		ParentCode: '',
		Level: 1,
		Sort: 0,
		Status: true,
	}
});

// 获取条目列表
const getItemList = async () => {
	console.log(selectedStandardId)
	if (!selectedStandardId.value) return;
	
	itemState.loading = true;
	try {
		const queryConfig = {
			customs: [
				{ field: 'StandardId', operator: 'eq', value: selectedStandardId.value },
				{ field: 'ItemCode', operator: 'eq', value: itemState.queryParams.ItemCode },
				{ field: 'ItemName', operator: 'eq', value: itemState.queryParams.ItemName },
			],
			ranges: {}
		};
		const searchBody: APIModel.PageQueryByConditionDto = {
			PageIndex: itemState.pagination.page,
			PageSize: itemState.pagination.pageSize,
		};
		const queryResult = buildMixedQuery(queryConfig);
		if (queryResult.dynamicQuery) {
			searchBody.DynamicQuery = queryResult.dynamicQuery;
			searchBody.QueryParameters = queryResult.queryParameters;
		}
		
		const response = await ItemApi.postApiFdNationalStandardItemPageSearch(searchBody);
		itemState.tableData.data = response.Items || [];
		itemState.pagination.total = response.PageInfo?.Total || 0;
	} catch (error) {
		ElMessage.error('获取条目数据失败');
	} finally {
		itemState.loading = false;
	}
};

const handleItemQuery = () => {
	itemState.pagination.page = 1;
	getItemList();
};

const resetItemQuery = () => {
	itemState.queryParams.ItemCode = null;
	itemState.queryParams.ItemName = null;
	handleItemQuery();
};

const handleItemSizeChange = (val: number) => {
	itemState.pagination.pageSize = val;
	getItemList();
};

const handleItemCurrentChange = (val: number) => {
	itemState.pagination.page = val;
	getItemList();
};

// 打开条目对话框
const openItemDialog = (row?: any) => {
	itemState.dialog.visible = true;
	itemState.dialog.title = row ? '编辑条目' : '新增条目';
	itemState.dialog.type = row ? 'update' : 'create';
	
	if (row) {
		itemState.formData = { ...row };
	} else {
		itemFormRef.value?.resetFields();
		itemState.formData = {
			Id: '',
			StandardId: selectedStandardId.value,
			ItemCode: '',
			ItemName: '',
			ItemNameEn: '',
			ParentCode: '',
			Level: 1,
			Sort: 0,
			Status: true,
		};
	}
};

// 提交条目表单
const submitItemForm = () => {
	itemFormRef.value?.validate(async (valid: boolean) => {
		if (!valid) return;
		try {
			if (itemState.dialog.type === 'update' && itemState.formData.Id) {
				await ItemApi.putApiFdNationalStandardItemId({ id: itemState.formData.Id }, itemState.formData);
				ElMessage.success('更新成功');
			} else {
				await ItemApi.postApiFdNationalStandardItem(itemState.formData);
				ElMessage.success('添加成功');
			}
			itemState.dialog.visible = false;
			getItemList();
		} catch (error) {
			ElMessage.error(itemState.dialog.type === 'update' ? '更新失败' : '添加失败');
		}
	});
};

// 删除条目
const handleItemDelete = (row: APIModel.FdNationalStandardItemDto) => {
	ElMessageBox.confirm('确定删除该条目吗？', '警告', {
		type: 'warning'
	})
	.then(async () => {
		await ItemApi.deleteApiFdNationalStandardItemId({ id: row.Id });
		ElMessage.success('删除成功');
		getItemList();
	})
	.catch(() => {});
};

// 导出条目
const exportItems = () => {
	ElMessage.info('导出功能开发中...');
};

onMounted(() => {
	getStandardList();
});
</script>

<style scoped lang="scss">
.gbdata-container {
	padding: 10px;
	height: 100%;
	
	.card-header {
		display: flex;
		justify-content: space-between;
		align-items: center;
		font-weight: bold;
		
		span {
			display: flex;
			align-items: center;
			gap: 8px;
		}
	}
	
	.table-wrapper {
		height: calc(100% - 150px);
		overflow-y: auto;
		margin-top: 10px;
	}
}
</style>
