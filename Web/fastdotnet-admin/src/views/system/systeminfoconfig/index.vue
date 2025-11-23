<template>
	<div class="fdsysteminfoconfig-container">
		<el-card shadow="hover" :body-style="{ padding: 2 }">
			<el-form :model="state.queryParams" ref="queryForm" :inline="true">
				<div v-show="state.searchCollapsed">
					<el-form-item label="Name" prop="Name">
						<el-input placeholder="请输入Name" clearable v-model="state.queryParams.Name"
							style="width: 150px" />
					</el-form-item>
					<el-form-item label="Code" prop="Code">
						<el-input placeholder="请输入Code" clearable v-model="state.queryParams.Code"
							style="width: 150px" />
					</el-form-item>
					<el-form-item label="Value" prop="Value">
						<el-input placeholder="请输入Value" clearable v-model="state.queryParams.Value"
							style="width: 150px" />
					</el-form-item>
					<el-form-item label="Description" prop="Description">
						<el-input placeholder="请输入Description" clearable v-model="state.queryParams.Description"
							style="width: 150px" />
					</el-form-item>
					<el-form-item label="IsSystem" prop="IsSystem">
						<el-input placeholder="请输入IsSystem" clearable v-model="state.queryParams.IsSystem"
							style="width: 150px" />
					</el-form-item>
				</div>
				<el-form-item>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" @click="handleQuery"> 查询 </el-button>
						<el-button icon="ele-Refresh" @click="resetQuery"> 重置 </el-button>
						<el-button @click="toggleSearchCollapse"
							:icon="state.searchCollapsed ? 'ele-ArrowUp' : 'ele-ArrowDown'">
							{{ state.searchCollapsed ? '收起' : '展开' }}
						</el-button>
					</el-button-group>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<div class="table-toolbar" style="margin-bottom: 15px;">
				<el-button type="primary" icon="ele-Plus" @click="openAddDialog"> 新增 </el-button>
				<el-button icon="ele-Download"> 导出 </el-button>
			</div>
			<el-table :data="state.tableData.data" style="width: 100%" v-loading="state.loading" border>
				<el-table-column prop="Name" label="Name" align="center" show-overflow-tooltip />
				<el-table-column prop="Code" label="Code" align="center" show-overflow-tooltip />
				<el-table-column prop="Value" label="Value" align="center" show-overflow-tooltip />
				<el-table-column prop="Description" label="Description" align="center" show-overflow-tooltip />
				<el-table-column prop="IsSystem" label="IsSystem" align="center" show-overflow-tooltip />
				<el-table-column label="操作" width="180" fixed="right" align="center">
					<template #default="scope">
						<el-button icon="ele-Edit" size="small" text type="primary"
							@click="openEditDialog(scope.row)">修改</el-button>
						<el-button icon="ele-Delete" size="small" text type="danger"
							@click="handleDelete(scope.row)">删除</el-button>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination v-model:currentPage="state.pagination.page" v-model:page-size="state.pagination.pageSize"
				:total="state.pagination.total" :page-sizes="[10, 20, 50, 100]" size="small" background
				@size-change="handleSizeChange" @current-change="handleCurrentChange"
				layout="total, sizes, prev, pager, next, jumper" />
		</el-card>

		<el-dialog v-model="state.dialog.visible" draggable :close-on-click-modal="false" width="700px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit />
					</el-icon>
					<span> { state.dialog.title } </span>
				</div>
			</template>
			<el-form :model="state.formData" ref="formRef" label-width="auto">
				<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
					<el-form-item label="Name" prop="Name">
						<el-input v-model="state.formData.Name" placeholder="请输入Name" maxlength="100" show-word-limit
							clearable />
					</el-form-item>
				</el-col>
				<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
					<el-form-item label="Code" prop="Code">
						<el-input v-model="state.formData.Code" placeholder="请输入Code" maxlength="100" show-word-limit
							clearable />
					</el-form-item>
				</el-col>
				<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
					<el-form-item label="Value" prop="Value">
						<el-input v-model="state.formData.Value" placeholder="请输入Value" clearable />
					</el-form-item>
				</el-col>
				<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
					<el-form-item label="Description" prop="Description">
						<el-input v-model="state.formData.Description" placeholder="请输入Description" maxlength="500"
							show-word-limit clearable />
					</el-form-item>
				</el-col>
				<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
					<el-form-item label="IsSystem" prop="IsSystem">
						<el-input v-model="state.formData.IsSystem" placeholder="请输入IsSystem" clearable />
					</el-form-item>
				</el-col>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="state.dialog.visible = false">取 消</el-button>
					<el-button type="primary" @click="submitForm">确 定</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="FdSystemInfoConfig">
import { ref, reactive, onMounted } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { buildMixedQuery } from '@/utils/queryBuilder';

import dayjs from 'dayjs'; // 引入日期处理库
import * as FdSystemInfoConfigApi from '@/api/fd-system-api/FdSystemInfoConfig';

const queryForm = ref();
const formRef = ref();

const state = reactive({
	loading: false,
	searchCollapsed: true,
	tableData: {
		data: [] as APIModel.FdSystemInfoConfigDto[]
	},
	queryParams: {
		Name: null,
		Code: null,
		Value: null,
		Description: null,
		IsSystem: null,

	},
	pagination: {
		page: 1,
		pageSize: 20,
		total: 0,
	},
	dialog: {
		visible: false,
		title: '',
		type: 'create' as 'create' | 'update',
	},
	formData: {
		Id: '',
		Name: '',
		Code: '',
		Value: '',
		Description: '',
		IsSystem: false,
	}
});
const toggleSearchCollapse = () => {
	state.searchCollapsed = !state.searchCollapsed;
};
// 获取列表
const getList = async () => {
	state.loading = true;
	try {
		//构建查询条件
		const queryConfig =
		{
			customs: [
				{ field: 'Name', operator: 'eq', value: state.queryParams.Name, },
				{ field: 'Code', operator: 'eq', value: state.queryParams.Code, },
				{ field: 'Value', operator: 'eq', value: state.queryParams.Value, },
				{ field: 'Description', operator: 'eq', value: state.queryParams.Description, },
				{ field: 'IsSystem', operator: 'eq', value: state.queryParams.IsSystem, },
			],
			ranges: {}
		}
		const searchBody: APIModel.PageQueryByConditionDto = {
			PageIndex: state.pagination.page,
			PageSize: state.pagination.pageSize,
		};
		const queryResult = buildMixedQuery(queryConfig);
		if (queryResult.dynamicQuery) {
			searchBody.DynamicQuery = queryResult.dynamicQuery;
			searchBody.QueryParameters = queryResult.queryParameters;
		}
		// 调试日志
		////console.log('Search request body:', searchBody);
		const response = await FdSystemInfoConfigApi.postAdminFdSystemInfoConfigPageSearch(searchBody);
		state.tableData.data = response.Items as APIModel.FdSystemInfoConfigDto[] || [] as APIModel.FdSystemInfoConfigDto[];
		state.pagination.total = response.PageInfo?.Total || 0;
	} catch (error) {
		ElMessage.error('获取数据失败');
		//console.error(error);
	} finally {
		state.loading = false;
	}
};

// 查询
const handleQuery = () => {
	state.pagination.page = 1;
	getList();
};

// 重置
const resetQuery = () => {
	queryForm.value.resetFields();
	handleQuery();
};

// 改变页面容量
const handleSizeChange = (val: number) => {
	state.pagination.pageSize = val;
	getList();
};

// 改变页码序号
const handleCurrentChange = (val: number) => {
	state.pagination.page = val;
	getList();
};

// 打开新增对话框
const openAddDialog = () => {
	state.dialog.visible = true;
	state.dialog.title = '新增';
	state.dialog.type = 'create';
	formRef.value?.resetFields();
};

// 打开编辑对话框
const openEditDialog = (row: any) => {
	state.dialog.visible = true;
	state.dialog.title = '编辑';
	state.dialog.type = 'update';
	state.formData = { ...row };
};

// 提交表单
const submitForm = () => {
	formRef.value.validate(async (valid: boolean) => {
		if (!valid) return;
		try {
			if (state.dialog.type === 'update' && state.formData.Id) {
				// 更新接口调用
				const updateData = { ...state.formData } as APIModel.UpdateFdSystemInfoConfigDto;
				await FdSystemInfoConfigApi.putAdminFdSystemInfoConfigId({ id: state.formData.Id }, updateData);
				ElMessage.success('更新成功');
			} else {
				// 新增接口调用
				const createData = { ...state.formData } as APIModel.CreateFdSystemInfoConfigDto;
				await FdSystemInfoConfigApi.postAdminFdSystemInfoConfig(createData);
				ElMessage.success('添加成功');
			}
			state.dialog.visible = false;
			getList();
		} catch (error) {
			console.error(error);
			ElMessage.error(state.dialog.type === 'update' ? '更新失败' : '添加失败');
		}
	});
};

// 删除
const handleDelete = (row: APIModel.FdSystemInfoConfigDto) => {
	ElMessageBox.confirm('确定删除吗？')
		.then(async () => {
			// 删除接口调用
			await FdSystemInfoConfigApi.deleteAdminFdSystemInfoConfigId({ id: row.Id as string });
			ElMessage.success('删除成功');
			getList();
		})
		.catch(() => {
			ElMessage.error('删除失败');
			return;
		});
};

onMounted(() => {
	getList();
});
</script>
<style scoped lang="scss">
.el-form--inline .el-form-item {
	margin-right: 12px !important; // 稍微紧凑一点
	margin-bottom: 8px !important;
}

.fdsysteminfoconfig-container .el-card:first-child .el-form .el-form-item:last-of-type {
	margin-bottom: 0 !important;
}
</style>
