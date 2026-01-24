<template>
	<div class="fdadminuser-container">
		<el-card shadow="hover" :body-style="{ padding: 2 }">
			<el-form :model="state.queryParams" ref="queryForm" :inline="true">
				<div v-show="state.searchCollapsed">
					<el-form-item label="username" prop="Username">
						<el-input v-model="state.queryParams.Username" placeholder="请输入username" clearable
							style="width: 150px" />
					</el-form-item>
					<el-form-item label="password" prop="Password">
						<el-input v-model="state.queryParams.Password" placeholder="请输入password" clearable
							style="width: 150px" />
					</el-form-item>
					<el-form-item label="name" prop="Name">
						<el-input v-model="state.queryParams.Name" placeholder="请输入name" clearable
							style="width: 150px" />
					</el-form-item>
					<el-form-item label="email" prop="Email">
						<el-input v-model="state.queryParams.Email" placeholder="请输入email" clearable
							style="width: 150px" />
					</el-form-item>
					<el-form-item label="phone" prop="Phone">
						<el-input v-model="state.queryParams.Phone" placeholder="请输入phone" clearable
							style="width: 150px" />
					</el-form-item>
					<el-form-item label="avatar" prop="Avatar">
						<el-input v-model="state.queryParams.Avatar" placeholder="请输入avatar" clearable
							style="width: 150px" />
					</el-form-item>
					<el-form-item label="is_active" prop="IsActive">
						<el-select v-model="state.queryParams.IsActive" placeholder="请选择is_active" clearable
							style="width: 150px">
							<el-option label="是" :value="true" />
							<el-option label="否" :value="false" />
						</el-select>
					</el-form-item>
					<el-form-item label="last_login_time" prop="LastLoginTime">
						<el-date-picker v-model="state.queryParams.LastLoginTime" type="datetime"
							placeholder="请选择last_login_time" clearable style="width: 150px" />
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
				<el-table-column prop="Username" label="username" align="center" show-overflow-tooltip />
				<el-table-column prop="Password" label="password" align="center" show-overflow-tooltip />
				<el-table-column prop="Name" label="name" align="center" show-overflow-tooltip />
				<el-table-column prop="Email" label="email" align="center" show-overflow-tooltip />
				<el-table-column prop="Phone" label="phone" align="center" show-overflow-tooltip />
				<el-table-column prop="Avatar" label="avatar" align="center" show-overflow-tooltip />
				<el-table-column prop="IsActive" label="is_active" align="center" show-overflow-tooltip />
				<el-table-column prop="LastLoginTime" label="last_login_time" align="center" show-overflow-tooltip />
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
					<el-form-item label="username" prop="Username">
						<el-input v-model="state.formData.Username" placeholder="请输入username" maxlength="255"
							show-word-limit clearable />
					</el-form-item>
				</el-col>
				<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
					<el-form-item label="password" prop="Password">
						<el-input v-model="state.formData.Password" placeholder="请输入password" maxlength="255"
							show-word-limit type="password" show-password />
					</el-form-item>
				</el-col>
				<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
					<el-form-item label="name" prop="Name">
						<el-input v-model="state.formData.Name" placeholder="请输入name" maxlength="255" show-word-limit
							clearable />
					</el-form-item>
				</el-col>
				<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
					<el-form-item label="email" prop="Email">
						<el-input v-model="state.formData.Email" placeholder="请输入email" maxlength="255" show-word-limit
							clearable />
					</el-form-item>
				</el-col>
				<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
					<el-form-item label="phone" prop="Phone">
						<el-input v-model="state.formData.Phone" placeholder="请输入phone" maxlength="255" show-word-limit
							clearable />
					</el-form-item>
				</el-col>
				<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
					<el-form-item label="avatar" prop="Avatar">
						<el-input v-model="state.formData.Avatar" placeholder="请输入avatar" maxlength="255"
							show-word-limit clearable />
					</el-form-item>
				</el-col>
				<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
					<el-form-item label="头像上传" prop="AvatarUpload">
						<GlobalFileUploader
							bucket-name="user-avatars"
							:max-size="2"
							accept="image/*"
							:showStorageTypeLabel=false
							list-type="picture-card"
							@success="onAvatarUploadSuccess"
							@error="onAvatarUploadError"
						>
							<el-icon class="avatar-uploader-icon"></el-icon>
						</GlobalFileUploader>
						<div class="el-upload__tip" v-if="state.avatarUrl">
							当前头像：<el-link :href="state.avatarUrl" target="_blank" type="primary">预览</el-link>
						</div>
					</el-form-item>
				</el-col>
				<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
					<el-form-item label="is_active" prop="IsActive">
						<el-switch v-model="state.formData.IsActive" :active-value="true" :inactive-value="false" />
					</el-form-item>
				</el-col>
				<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
					<el-form-item label="last_login_time" prop="LastLoginTime">
						<el-date-picker v-model="state.formData.LastLoginTime" type="datetime"
							placeholder="请选择last_login_time" style="width: 100%" value-format="YYYY-MM-DD HH:mm:ss" />
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

import dayjs from 'dayjs'; // 引入日期处理库
import * as FdAdminUserApi from '@/api/fd-system-api-app/FdAppUser';
import GlobalFileUploader from '@/components/upload/GlobalFileUploader.vue';

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
		Password: null,
		Name: null,
		Email: null,
		Phone: null,
		Avatar: null,
		IsActive: null,
		LastLoginTime: null,

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
	avatarUrl: '',
	formData: {
		Id: '',
		Username: '',
		Password: '',
		Name: '',
		Email: '',
		Phone: '',
		Avatar: '',
		IsActive: false,
		LastLoginTime: '',
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
				{ field: 'Username', operator: 'eq', value: state.queryParams.Username, },
				{ field: 'Password', operator: 'eq', value: state.queryParams.Password, },
				{ field: 'Name', operator: 'eq', value: state.queryParams.Name, },
				{ field: 'Email', operator: 'eq', value: state.queryParams.Email, },
				{ field: 'Phone', operator: 'eq', value: state.queryParams.Phone, },
				{ field: 'Avatar', operator: 'eq', value: state.queryParams.Avatar, },
				{ field: 'IsActive', operator: 'eq', value: state.queryParams.IsActive, },
				{ field: 'LastLoginTime', operator: 'eq', value: state.queryParams.LastLoginTime, },
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

// 头像上传成功回调
const onAvatarUploadSuccess = (response: any, file: any, fileList: any) => {
	console.log('头像上传成功:', response);
	// 根据API响应格式处理
	if (response && typeof response === 'object') {
		// 如果是标准响应格式
		if (response.data && response.data.url) {
			state.formData.Avatar = response.data.url;
			state.avatarUrl = response.data.url;
			ElMessage.success(`头像上传成功: ${response.data.fileName || '文件'}`);
		} else if (response.url) {
			// 如果直接返回URL
			state.formData.Avatar = response.url;
			state.avatarUrl = response.url;
			ElMessage.success('头像上传成功');
		} else {
			// 如果是字符串URL
			state.formData.Avatar = response;
			state.avatarUrl = response;
			ElMessage.success('头像上传成功');
		}
	} else {
		// 如果是字符串URL
		state.formData.Avatar = response;
		state.avatarUrl = response;
		ElMessage.success('头像上传成功');
	}
};

// 头像上传失败回调
const onAvatarUploadError = (error: any, file: any, fileList: any) => {
	console.error('头像上传失败:', error);
	ElMessage.error('头像上传失败');
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

.avatar-uploader-icon {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
}

.el-upload__tip {
  font-size: 12px;
  color: #909399;
  margin-top: 5px;
}
</style>
