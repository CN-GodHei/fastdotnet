<template>
	<div class="fdadminuser-container">
		<el-card shadow="hover" :body-style="{ padding: 5 }">
			<el-form :model="state.queryParams" ref="queryForm" :inline="true">
				<el-form-item label="搜索条件">
					<el-input placeholder="请输入搜索条件" clearable @keyup.enter="handleQuery"
						v-model="state.queryParams.searchValue" />
				</el-form-item>
				<el-form-item>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" @click="handleQuery"> 查询 </el-button>
						<el-button icon="ele-Refresh" @click="resetQuery"> 重置 </el-button>
					</el-button-group>
				</el-form-item>
				<el-form-item>
					<el-button type="primary" icon="ele-Plus" @click="openAddDialog"> 新增 </el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<el-table :data="state.tableData" style="width: 100%" v-loading="state.loading" border>
				<el-table-column prop="Username" label="用户名" align="center" show-overflow-tooltip />
				<el-table-column prop="Password" label="密码" align="center" show-overflow-tooltip />
				<el-table-column prop="Name" label="名称" align="center" show-overflow-tooltip />
				<el-table-column prop="Email" label="邮箱" align="center" show-overflow-tooltip />
				<el-table-column prop="Phone" label="Phone" align="center" show-overflow-tooltip />
				<el-table-column prop="Avatar" label="Avatar" align="center" show-overflow-tooltip />
				<el-table-column prop="IsActive" label="IsActive" align="center" show-overflow-tooltip />
				<el-table-column prop="LastLoginTime" label="LastLoginTime" align="center" show-overflow-tooltip />
				<el-table-column prop="LastLoginIp" label="LastLoginIp" align="center" show-overflow-tooltip />
				<el-table-column label="操作" width="180" fixed="right" align="center">
					<template #default="scope">
						<el-button icon="ele-Edit" size="small" text type="primary"
							@click="openEditDialog(scope.row)">修改</el-button>
						<el-button icon="ele-Delete" size="small" text type="danger"
							@click="handleDelete(scope.row)">删除</el-button>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination v-model:currentPage="state.tableParams.page" v-model:page-size="state.tableParams.pageSize"
				:total="state.tableParams.total" :page-sizes="[10, 20, 50, 100]" size="small" background
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
					<el-form-item label="username" prop="Username">
						<el-input v-model="state.formData.Username" placeholder="请输入username" clearable />
					</el-form-item>
				</el-col>
				<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
					<el-form-item label="password" prop="Password">
						<el-input v-model="state.formData.Password" placeholder="请输入password" clearable />
					</el-form-item>
				</el-col>
				<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
					<el-form-item label="name" prop="Name">
						<el-input v-model="state.formData.Name" placeholder="请输入name" clearable />
					</el-form-item>
				</el-col>
				<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
					<el-form-item label="email" prop="Email">
						<el-input v-model="state.formData.Email" placeholder="请输入email" clearable />
					</el-form-item>
				</el-col>
				<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
					<el-form-item label="phone" prop="Phone">
						<el-input v-model="state.formData.Phone" placeholder="请输入phone" clearable />
					</el-form-item>
				</el-col>
				<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
					<el-form-item label="avatar" prop="Avatar">
						<el-input v-model="state.formData.Avatar" placeholder="请输入avatar" clearable />
					</el-form-item>
				</el-col>
				<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
					<el-form-item label="is_active" prop="IsActive">
						<el-input v-model="state.formData.IsActive" placeholder="请输入is_active" clearable />
					</el-form-item>
				</el-col>
				<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
					<el-form-item label="last_login_time" prop="LastLoginTime">
						<el-input v-model="state.formData.LastLoginTime" placeholder="请输入last_login_time" clearable />
					</el-form-item>
				</el-col>
				<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
					<el-form-item label="last_login_ip" prop="LastLoginIp">
						<el-input v-model="state.formData.LastLoginIp" placeholder="请输入last_login_ip" clearable />
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

const queryForm = ref();
const formRef = ref();

const state = reactive({
	loading: false,
	tableData: [],
	queryParams: {
		searchValue: undefined
	},
	tableParams: {
		page: 1,
		pageSize: 20,
		total: 0
	},
	dialog: {
		visible: false,
		title: ''
	},
	formData: {}
});

// 获取列表
const getList = async () => {
	state.loading = true;
	// TODO: 实现获取列表接口调用
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
			if (state.formData.id) {
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
		.catch(() => { });
};

onMounted(() => {
	getList();
});
</script>