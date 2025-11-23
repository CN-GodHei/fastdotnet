<template>
	<div class="fdadminuser-container">
		<el-card shadow="hover" :body-style="{ padding: 2 }">
			<el-form :model="state.queryParams" ref="queryForm" :inline="true">
				<div v-show="state.searchCollapsed">
					<el-form-item label="Username" prop="Username">
						<el-input placeholder="请输入Username" clearable v-model="state.queryParams.Username" style="width: 150px" />
					</el-form-item>
					<el-form-item label="Name" prop="Name">
						<el-input placeholder="请输入Name" clearable v-model="state.queryParams.Name" style="width: 150px" />
					</el-form-item>
					<el-form-item label="Email" prop="Email">
						<el-input placeholder="请输入Email" clearable v-model="state.queryParams.Email" style="width: 150px" />
					</el-form-item>
					<el-form-item label="Phone" prop="Phone">
						<el-input placeholder="请输入Phone" clearable v-model="state.queryParams.Phone" style="width: 150px" />
					</el-form-item>
					<el-form-item label="Avatar" prop="Avatar">
						<el-input placeholder="请输入Avatar" clearable v-model="state.queryParams.Avatar" style="width: 150px" />
					</el-form-item>
					<el-form-item label="IsActive" prop="IsActive">
						<el-input placeholder="请输入IsActive" clearable v-model="state.queryParams.IsActive" style="width: 150px" />
					</el-form-item>
				</div>
				<el-form-item>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" @click="handleQuery"> 查询 </el-button>
						<el-button icon="ele-Refresh" @click="resetQuery"> 重置 </el-button>
						<el-button @click="toggleSearchCollapse" :icon="state.searchCollapsed ? 'ele-ArrowUp' : 'ele-ArrowDown'">
							{{ state.searchCollapsed ? '收起' : '展开' }}
						</el-button>
					</el-button-group>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<div class="table-toolbar" style="margin-bottom: 15px">
				<el-button type="primary" icon="ele-Plus" @click="openAddDialog"> 新增 </el-button>
				<el-button icon="ele-Download"> 导出 </el-button>
			</div>
			<el-table :data="state.tableData.data" style="width: 100%" v-loading="state.loading" border>
				<el-table-column prop="Username" label="Username" align="center" show-overflow-tooltip />
				<el-table-column prop="Name" label="Name" align="center" show-overflow-tooltip />
				<el-table-column prop="Email" label="Email" align="center" show-overflow-tooltip />
				<el-table-column prop="Phone" label="Phone" align="center" show-overflow-tooltip />
				<el-table-column prop="Avatar" label="Avatar" align="center" show-overflow-tooltip />
				<el-table-column prop="IsActive" label="IsActive" align="center" show-overflow-tooltip />
				<el-table-column label="操作" width="180" fixed="right" align="center">
					<template #default="scope">
						<el-button icon="ele-Edit" size="small" text type="primary" @click="openEditDialog(scope.row)">修改</el-button>
						<el-button icon="ele-Delete" size="small" text type="danger" @click="handleDelete(scope.row)">删除</el-button>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination
				v-model:currentPage="state.tableParams.page"
				v-model:page-size="state.tableParams.pageSize"
				:total="state.tableParams.total"
				:page-sizes="[10, 20, 50, 100]"
				size="small"
				background
				@size-change="handleSizeChange"
				@current-change="handleCurrentChange"
				layout="total, sizes, prev, pager, next, jumper"
			/>
		</el-card>

		<el-dialog v-model="state.dialog.visible" draggable :close-on-click-modal="false" width="700px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> { state.dialog.title } </span>
				</div>
			</template>
			<el-form :model="state.formData" ref="formRef" label-width="auto">
				<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
					<el-form-item label="Username" prop="Username">
						<el-input v-model="state.formData.Username" placeholder="请输入Username" maxlength="255" show-word-limit clearable />
					</el-form-item>
				</el-col>
				<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
					<el-form-item label="Name" prop="Name">
						<el-input v-model="state.formData.Name" placeholder="请输入Name" maxlength="255" show-word-limit clearable />
					</el-form-item>
				</el-col>
				<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
					<el-form-item label="Email" prop="Email">
						<el-input v-model="state.formData.Email" placeholder="请输入Email" maxlength="255" show-word-limit clearable />
					</el-form-item>
				</el-col>
				<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
					<el-form-item label="Phone" prop="Phone">
						<el-input v-model="state.formData.Phone" placeholder="请输入Phone" maxlength="255" show-word-limit clearable />
					</el-form-item>
				</el-col>
				<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
					<el-form-item label="Avatar" prop="Avatar">
						<el-input v-model="state.formData.Avatar" placeholder="请输入Avatar" maxlength="255" show-word-limit clearable />
					</el-form-item>
				</el-col>
				<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
					<el-form-item label="IsActive" prop="IsActive">
						<el-input v-model="state.formData.IsActive" placeholder="请输入IsActive" clearable />
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

<script lang="ts" setup name="FdAdminUser">
import { ref, reactive, onMounted } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { buildMixedQuery } from '@/utils/queryBuilder';
import type { FdAdminUser } from '@/api/fd-system-api/typings';
import dayjs from 'dayjs'; // 引入日期处理库
const queryForm = ref();
const formRef = ref();

const state = reactive({
	loading: false,
	searchCollapsed: true,
	tableData: {
		data: [],
		total: 0,
		loading: false,
		param: {
			pageNum: 1,
			pageSize: 10,
		},
	},
	queryParams: {
		Username: null,
		Name: null,
		Email: null,
		Phone: null,
		Avatar: null,
		IsActive: null,
	},
	tableParams: {
		page: 1,
		pageSize: 20,
		total: 0,
	},
	dialog: {
		visible: false,
		title: '',
	},
	formData: {
		Username: '',
		Name: '',
		Email: '',
		Phone: '',
		Avatar: '',
		IsActive: '',
	},
});
const toggleSearchCollapse = () => {
	state.searchCollapsed = !state.searchCollapsed;
};
// 获取列表
const getList = async () => {
	state.loading = true;
	//构建查询条件
	const queryConfig = {
		customs: [
			{ field: 'Username', operator: 'eq', value: state.queryParams.Username },
			{ field: 'Name', operator: 'eq', value: state.queryParams.Name },
			{ field: 'Email', operator: 'eq', value: state.queryParams.Email },
			{ field: 'Phone', operator: 'eq', value: state.queryParams.Phone },
			{ field: 'Avatar', operator: 'eq', value: state.queryParams.Avatar },
			{ field: 'IsActive', operator: 'eq', value: state.queryParams.IsActive },
		],
		ranges: {},
	};
	const searchBody: APIModel.PageQueryByConditionDto = {
		PageIndex: state.tableData.param.pageNum,
		PageSize: state.tableData.param.pageSize,
	};
	const queryResult = buildMixedQuery(queryConfig);
	if (queryResult.dynamicQuery) {
		searchBody.DynamicQuery = queryResult.dynamicQuery;
		searchBody.QueryParameters = queryResult.queryParameters;
	}
	// 调试日志
	//console.log('Search request body:', searchBody);
	state.loading = false;
};

// 查询
const handleQuery = () => {
	state.tableParams.page = 1;
	getList();
};

// 重置
const resetQuery = () => {
	queryForm.value.resetFields();
	handleQuery();
};

// 改变页面容量
const handleSizeChange = (val: number) => {
	state.tableParams.pageSize = val;
	getList();
};

// 改变页码序号
const handleCurrentChange = (val: number) => {
	state.tableParams.page = val;
	getList();
};

// 打开新增对话框
const openAddDialog = () => {
	state.dialog.visible = true;
	state.dialog.title = '新增fd_admin_user';
	formRef.value?.resetFields();
	state.formData = {};
};

// 打开编辑对话框
const openEditDialog = (row: any) => {
	state.dialog.visible = true;
	state.dialog.title = '编辑fd_admin_user';
	state.formData = { ...row };
};

// 提交表单
const submitForm = () => {
	formRef.value.validate(async (valid: boolean) => {
		if (!valid) return;
		try {
			if (state.formData.Id) {
				// 更新接口调用
				ElMessage.success('更新成功');
			} else {
				// 新增接口调用
				ElMessage.success('新增成功');
			}
			state.dialog.visible = false;
			getList();
		} catch (error) {
			console.error(error);
		}
	});
};

// 删除
const handleDelete = (row: any) => {
	ElMessageBox.confirm('确定删除吗？')
		.then(async () => {
			// 删除接口调用
			ElMessage.success('删除成功');
			getList();
		})
		.catch(() => {});
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
.fdadminuser-container .el-card:first-child .el-form .el-form-item:last-of-type {
	margin-bottom: 0 !important;
}
</style>


