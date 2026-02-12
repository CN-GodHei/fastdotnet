<template>
	<div class="fdadminuser-container">
		<el-card shadow="hover" :body-style="{ padding: 2 }">
			<el-form :model="state.queryParams" ref="queryForm" :inline="true">
				<div v-show="state.searchCollapsed">
					<el-form-item label="用户名" prop="Username">
						<el-input v-model="state.queryParams.Username" placeholder="请输入用户名" clearable
							style="width: 150px" />
					</el-form-item>
					<el-form-item label="姓名" prop="Name">
						<el-input v-model="state.queryParams.Name" placeholder="请输入姓名" clearable style="width: 150px" />
					</el-form-item>
					<el-form-item label="邮箱" prop="Email">
						<el-input v-model="state.queryParams.Email" placeholder="请输入邮箱" clearable
							style="width: 150px" />
					</el-form-item>
					<el-form-item label="电话" prop="Phone">
						<el-input v-model="state.queryParams.Phone" placeholder="请输入电话" clearable
							style="width: 150px" />
					</el-form-item>
					<el-form-item label="是否激活" prop="IsActive">
						<el-select v-model="state.queryParams.IsActive" placeholder="请选择是否激活" clearable
							style="width: 150px">
							<el-option label="是" :value="true" />
							<el-option label="否" :value="false" />
						</el-select>
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
				<el-table-column prop="Username" label="用户名" show-overflow-tooltip />
				<el-table-column prop="Name" label="姓名" show-overflow-tooltip />
				<el-table-column prop="Email" label="邮箱" show-overflow-tooltip />
				<el-table-column prop="Phone" label="电话" show-overflow-tooltip />

				<el-table-column prop="IsActive" label="是否激活" show-overflow-tooltip>
					<template #default="{ row }">
						<el-tag :type="row.IsActive ? 'success' : 'danger'" size="small">
							{{ row.IsActive ? '是' : '否' }}
						</el-tag>
					</template>
				</el-table-column>
				<el-table-column label="操作" width="260" fixed="right" align="center">
					<template #default="scope">
						<el-button icon="ele-Edit" size="small" text type="primary"
							@click="openEditDialog(scope.row)">修改</el-button>
						<el-button icon="ele-User" size="small" text type="primary"
							@click="openAssignRoleDialog(scope.row)">分配角色</el-button>
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
					<el-form-item label="用户名" prop="Username">
						<el-input v-model="state.formData.Username" placeholder="请输入用户名" maxlength="255" show-word-limit
							clearable />
					</el-form-item>
				</el-col>
				<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
					<el-form-item label="姓名" prop="Name">
						<el-input v-model="state.formData.Name" placeholder="请输入姓名" maxlength="255" show-word-limit
							clearable />
					</el-form-item>
				</el-col>
				<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
					<el-form-item label="邮箱" prop="Email">
						<el-input v-model="state.formData.Email" placeholder="请输入邮箱" maxlength="255" show-word-limit
							clearable />
					</el-form-item>
				</el-col>
				<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
					<el-form-item label="电话" prop="Phone">
						<el-input v-model="state.formData.Phone" placeholder="请输入电话" maxlength="255" show-word-limit
							clearable />
					</el-form-item>
				</el-col>
				<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
					<el-form-item label="头像" prop="Avatar">
						<el-input v-model="state.formData.Avatar" placeholder="请输入头像" maxlength="255" show-word-limit
							clearable />
					</el-form-item>
				</el-col>
				<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
					<el-form-item label="是否激活" prop="IsActive">
						<el-switch v-model="state.formData.IsActive" :active-value="true" :inactive-value="false" />
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

		<!-- 分配角色对话框 -->
		<el-dialog v-model="state.roleDialog.visible" draggable :close-on-click-modal="false" width="600px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-User /> </el-icon>
					<span> {{ state.roleDialog.title }} </span>
				</div>
			</template>
			<div class="role-selection-container">
				<el-transfer 
					v-model="state.roleDialog.selectedRoles" 
					:data="state.roleDialog.allRoles"
					:titles="['可分配角色', '已分配角色']"
					:button-texts="['移除', '添加']"
					:format="{
						noChecked: '${total}',
						hasChecked: '${checked}/${total}'
					}"
					@change="handleRoleTransferChange">
					<template #default="{ option }">
						<span>{{ option.name }}</span>
						<el-tag size="small" type="info" style="margin-left: 8px;">{{ option.code }}</el-tag>
					</template>
				</el-transfer>
			</div>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="state.roleDialog.visible = false">取 消</el-button>
					<el-button type="primary" @click="saveRoleAssignment">保 存</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="FdAdminUser">
import { ref, reactive, onMounted } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { buildMixedQuery } from '@/utils/queryBuilder';

import dayjs from 'dayjs'; // 引入日期处理库
import * as FdAdminUserApi from '@/api/fd-system-api-admin/FdAdminUser';
import * as FdRoleApi from '@/api/fd-system-api-admin/FdRole';
import * as FdAdminUserRoleApi from '@/api/fd-system-api-admin/FdAdminUserRole';

const queryForm = ref();
const formRef = ref();

const state = reactive({
	loading: false,
	searchCollapsed: true,
	tableData: {
		data: [] as APIModel.FdAdminUserDto[]
	},
	queryParams: {
		Username: null,
		Name: null,
		Email: null,
		Phone: null,
		IsActive: null,

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
	roleDialog: {
		visible: false,
		title: '',
		userId: '',
		selectedRoles: [] as string[],
		allRoles: [] as Array<{ key: string; name: string; code: string }>[],
		currentUserRoles: [] as string[],
	},
	formData: {
		Id: '',
		Username: '',
		Name: '',
		Email: '',
		Phone: '',
		Avatar: '',
		IsActive: false,
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
				{ field: 'Username', operator: 'contains', value: state.queryParams.Username, },
				{ field: 'Name', operator: 'contains', value: state.queryParams.Name, },
				{ field: 'Email', operator: 'eq', value: state.queryParams.Email, },
				{ field: 'Phone', operator: 'eq', value: state.queryParams.Phone, },
				{ field: 'IsActive', operator: 'eq', value: state.queryParams.IsActive, },
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
		const response = await FdAdminUserApi.postApiAdminFdAdminUserPageSearch(searchBody);
		state.tableData.data = response.Items as APIModel.FdAdminUserDto[] || [] as APIModel.FdAdminUserDto[];
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
				const updateData = { ...state.formData } as APIModel.UpdateFdAdminUserDto;
				await FdAdminUserApi.putApiAdminFdAdminUserId({ id: state.formData.Id }, updateData);
				ElMessage.success('更新成功');
			} else {
				// 新增接口调用
				const createData = { ...state.formData } as APIModel.CreateFdAdminUserDto;
				await FdAdminUserApi.postApiAdminFdAdminUser(createData);
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

// 打开分配角色对话框
const openAssignRoleDialog = async (row: APIModel.FdAdminUserDto) => {
	try {
		state.roleDialog.userId = row.Id as string;
		state.roleDialog.title = `为 "${row.Name}" 分配角色`;
		state.roleDialog.visible = true;
		
		// 获取所有角色
		await loadAllRoles();
		
		// 获取用户当前角色
		await loadUserRoles(row.Id as string);
		
	} catch (error) {
		ElMessage.error('获取角色数据失败');
	}
};

// 加载所有角色
const loadAllRoles = async () => {
	try {
		const response = await FdRoleApi.getApiAdminFdRole();
		state.roleDialog.allRoles = (response || []).map((role: APIModel.FdRoleDto) => ({
			key: role.Id as string,
			name: role.Name as string,
			code: role.Code as string
		}));
	} catch (error) {
		ElMessage.error('获取角色列表失败');
	}
};

// 加载用户当前角色
const loadUserRoles = async (userId: string) => {
	try {
		// 使用专门的用户角色查询接口
		const roleIds = await FdAdminUserRoleApi.getApiFdAdminUserRoleUserUserIdRoles({ userId });
		state.roleDialog.currentUserRoles = roleIds || [];
		state.roleDialog.selectedRoles = [...state.roleDialog.currentUserRoles];
	} catch (error) {
		ElMessage.error('获取用户角色失败');
	}
};

// 处理角色转移变化
const handleRoleTransferChange = (value: string[], direction: 'left' | 'right', movedKeys: string[]) => {
	state.roleDialog.selectedRoles = value;
};

// 保存角色分配
const saveRoleAssignment = async () => {
	try {
		const userId = state.roleDialog.userId;
		const selectedRoles = state.roleDialog.selectedRoles;
		
		// 使用后端事务安全的分配接口
		const assignDto: APIModel.AssignUserRolesDto = {
			UserId: userId,
			RoleIds: selectedRoles
		};
		
		const result = await FdAdminUserRoleApi.postApiFdAdminUserRoleAssignRoles(assignDto);
		
		if (result) {
			ElMessage.success('角色分配保存成功');
			state.roleDialog.visible = false;
		} else {
			ElMessage.error('角色分配保存失败');
		}
		
	} catch (error) {
		ElMessage.error('角色分配保存失败');
	}
};

// 删除
const handleDelete = (row: APIModel.FdAdminUserDto) => {
	ElMessageBox.confirm('确定删除吗？')
		.then(async () => {
			// 删除接口调用
			await FdAdminUserApi.deleteApiAdminFdAdminUserId({ id: row.Id as string });
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
// }

.role-selection-container {
	padding: 20px;
	
	:deep(.el-transfer) {
		display: flex;
		align-items: center;
		
		.el-transfer-panel {
			flex: 1;
			min-height: 300px;
		}
		
		.el-transfer__buttons {
			padding: 0 20px;
		}
	}
}
</style>
