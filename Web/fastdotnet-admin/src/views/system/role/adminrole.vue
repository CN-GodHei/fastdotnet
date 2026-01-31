<template>
	<div class="fdrole-container">
		<el-card shadow="hover" :body-style="{ padding: 2 }">
			<el-form :model="state.queryParams" ref="queryForm" :inline="true">
				<div v-show="state.searchCollapsed">
					<el-form-item label="角色名称" prop="Name">
						<el-input v-model="state.queryParams.Name" placeholder="请输入角色名称" clearable
							style="width: 150px" />
					</el-form-item>
					<el-form-item label="角色编码" prop="Code">
						<el-input v-model="state.queryParams.Code" placeholder="请输入角色编码" clearable
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
				<el-table-column prop="Name" label="角色名称" show-overflow-tooltip />
				<el-table-column prop="Code" label="角色编码" show-overflow-tooltip />
				<el-table-column prop="Description" label="角色描述" show-overflow-tooltip />
				<el-table-column prop="ParentId" label="父级角色" show-overflow-tooltip />

				<el-table-column prop="IsSystem" label="是否为系统内置角色" show-overflow-tooltip>
					<template #default="{ row }">
						<el-tag :type="row.IsSystem ? 'success' : 'danger'" size="small">
							{{ row.IsSystem ? '是' : '否' }}
						</el-tag>
					</template>
				</el-table-column>

				<el-table-column prop="IsDefault" label="是否为默认角色" show-overflow-tooltip>
					<template #default="{ row }">
						<el-tag :type="row.IsDefault ? 'success' : 'danger'" size="small">
							{{ row.IsDefault ? '是' : '否' }}
						</el-tag>
					</template>
				</el-table-column>
				<!-- <el-table-column prop="Belong" label="属于管理端还是应用端" show-overflow-tooltip /> -->
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
					<el-form-item label="角色名称" prop="Name">
						<el-input v-model="state.formData.Name" placeholder="请输入角色名称" maxlength="255" show-word-limit
							clearable />
					</el-form-item>
				</el-col>
				<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
					<el-form-item label="角色描述" prop="Description">
						<el-input v-model="state.formData.Description" placeholder="请输入角色描述" maxlength="255"
							show-word-limit type="textarea" rows="4" />
					</el-form-item>
				</el-col>
				<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
					<el-form-item label="父级角色" prop="ParentId">
						<el-input v-model="state.formData.ParentId" placeholder="请输入父级角色" maxlength="255"
							show-word-limit clearable />
					</el-form-item>
				</el-col>
				<!-- <el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
					<el-form-item label="属于管理端还是应用端" prop="Belong">
						<el-checkbox-group v-model="state.formData.Belong">
							<el-checkbox :label="1">是</el-checkbox>
							<el-checkbox :label="0">否</el-checkbox>
						</el-checkbox-group>
					</el-form-item>
				</el-col> -->
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

<script lang="ts" setup name="FdRole">
import { ref, reactive, onMounted } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { buildMixedQuery } from '@/utils/queryBuilder';

import dayjs from 'dayjs'; // 引入日期处理库
import * as FdRoleApi from '@/api/fd-system-api-admin/FdRole';

const queryForm = ref();
const formRef = ref();

const state = reactive({
	loading: false,
	searchCollapsed: true,
	tableData: {
		data: [] as APIModel.FdRoleDto[]
	},
	queryParams: {
		Name: null,
		Code: null,

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
		Description: '',
		ParentId: '',
		Belong: 0,
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
				{ field: 'Belong', operator: 'eq', value: 0, },
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
		//console.log('Search request body:', searchBody);
		const response = await FdRoleApi.postApiAdminFdRolePageSearch(searchBody);
		state.tableData.data = response.Items as APIModel.FdRoleDto[] || [] as APIModel.FdRoleDto[];
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
				const updateData = { ...state.formData } as APIModel.UpdateFdRoleDto;
				await FdRoleApi.putApiAdminFdRoleId({ id: state.formData.Id }, updateData);
				ElMessage.success('更新成功');
			} else {
				// 新增接口调用
				const createData = { ...state.formData } as APIModel.CreateFdRoleDto;
				createData.Belong = 0;
				await FdRoleApi.postApiAdminFdRole(createData);
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
const handleDelete = (row: APIModel.FdRoleDto) => {
	ElMessageBox.confirm('确定删除吗？')
		.then(async () => {
			// 删除接口调用
			await FdRoleApi.deleteApiAdminFdRoleId({ id: row.Id as string });
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
// .el-form--inline .el-form-item {
// 	margin-right: 12px !important; // 稍微紧凑一点
// 	margin-bottom: 8px !important;
// }

// .fdadminuser-container .el-card:first-child .el-form .el-form-item:last-of-type {
// 	margin-bottom: 5 !important;
// }</style>
