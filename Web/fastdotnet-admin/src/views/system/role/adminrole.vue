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
				<el-table-column label="操作" width="280" fixed="right" align="center">
					<template #default="scope">
						<el-button icon="ele-Edit" size="small" text type="primary"
							@click="openEditDialog(scope.row)">修改</el-button>
						<el-button icon="ele-Tickets" size="small" text type="primary"
							@click="openAssignPermissionsDialog(scope.row)">分配权限</el-button>
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
					<span> {{ state.dialog.title }} </span>
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
					<el-button @click="resetForm">重 置</el-button>
				</span>
			</template>
		</el-dialog>
		
		<!-- 分配权限对话框 -->
		<el-dialog v-model="state.permissionDialog.visible" draggable :close-on-click-modal="false" width="800px" height="600px" style="--el-dialog-padding-primary: 0;">
			<template #header>
				<div>
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle">
						<ele-Tickets />
					</el-icon>
					<span> {{ state.permissionDialog.title }} </span>
				</div>
			</template>
			
			<div class="permission-tree-container">
				<el-tree
				ref="menuTreeRef"
				:data="state.menuBtnData"
				:props="state.treeProps"
				node-key="Id"
				:default-expand-all="true"
				:show-checkbox="true"
				:expand-on-click-node="false"
				:check-strictly="false"
			>
				<template #default="{ node, data }">
					<span class="custom-tree-node">
						<span class="menu-name">{{ data.Title }}</span>
						<span v-if="data.BtnList && data.BtnList.length > 0" class="btn-group">
							<el-checkbox-group v-model="data.selectedBtns" @change="(val: string[]) => handleBtnChange(data, val)">
								<el-checkbox
									v-for="btn in data.BtnList"
									:key="btn.Id"
									:label="btn.Id"
									:disabled="btn.disabled"
									class="btn-checkbox"
								>
									<span class="btn-label">{{ btn.Name }}</span>
								</el-checkbox>
							</el-checkbox-group>
						</span>
					</span>
				</template>
			</el-tree>
			</div>
			
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="state.permissionDialog.visible = false">取 消</el-button>
					<el-button type="primary" @click="savePermissions">保 存</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<style scoped lang="scss">
.custom-tree-node {
  display: flex;
  align-items: center;
  width: 100%;
}

.menu-name {
  flex: 1;
  margin-right: 12px;
  font-weight: 500;
}

.btn-group {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.btn-checkbox {
  display: inline-flex;
  align-items: center;
  padding: 2px 6px;
  border-radius: 4px;
  background-color: #f5f7fa;
  margin-right: 4px;
}

.btn-label {
  margin-left: 4px;
  font-size: 12px;
}

.el-tree-node__content {
  padding: 6px 0;
}

.el-checkbox {
  margin-right: 8px;
}

// 固定高度宽度的权限分配对话框
.permission-tree-container {
  height: 500px; /* 写死高度 */
  overflow-y: auto;
  padding: 16px;
}

.el-dialog {
  &.permission-dialog {
    .el-dialog__body {
      padding: 0;
    }
  }
}
</style>

<script lang="ts" setup name="FdRole">
import { ref, reactive, onMounted } from 'vue';
import { ElMessageBox, ElMessage, ElTree } from 'element-plus';
import { buildMixedQuery } from '@/utils/queryBuilder';

import dayjs from 'dayjs'; // 引入日期处理库
import * as FdRoleApi from '@/api/fd-system-api-admin/FdRole';
import * as FdMenuApi from '@/api/fd-system-api-admin/FdMenu';

const queryForm = ref();
const formRef = ref();
const menuTreeRef = ref<InstanceType<typeof ElTree>>();

interface MenuBtnRe {
  Id: string;
  Name: string;
  Title: string;
  DataStatus: number; // DataStatus enum value
  Exist: boolean;
  Children?: MenuBtnRe[];
  BtnList?: MenuBtnReStatusDto[];
  selectedBtns?: string[]; // 用于存储选中的按钮ID
}

interface MenuBtnReStatusDto {
  Id: string;
  Name: string;
  DataStatus: number;
  Exist: boolean;
}

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
	},
	permissionDialog: {
		visible: false,
		title: '',
	},
	menuBtnData: [] as MenuBtnRe[],
	treeProps: {
		children: 'Children',
		label: 'Name',
	},
	currentRoleId: '' // 记录当前正在分配权限的角色ID
});

// 打开分配权限对话框
const openAssignPermissionsDialog = async (row: APIModel.FdRoleDto) => {
  try {
    state.currentRoleId = row.Id as string;
    state.permissionDialog.visible = true;
	console.log(`为 "${row.Name}" 分配权限`)
    state.permissionDialog.title = `为 "${row.Name}" 分配权限`;
    
    // 获取菜单按钮数据
    const menuBtnData = (await FdMenuApi.getApiAdminFdMenuMenuBtns({ Belong: 0, RoleId: row.Id as string })) as unknown as APIModel.MenuBtnRe[]; // 封装的request返回的就是data的内容
    state.menuBtnData = processMenuBtnData(menuBtnData);
  } catch (error) {
    ElMessage.error('获取权限数据失败');
    console.error(error);
  }
};

// 处理菜单按钮数据，添加选中状态
const processMenuBtnData = (menuBtnList: APIModel.MenuBtnRe[]): MenuBtnRe[] => {
  return menuBtnList.map(item => {
    // 初始化选中的按钮
    const selectedBtns = item.BtnList?.filter((btn: APIModel.MenuBtnReStatusDto) => btn.Exist).map((btn: APIModel.MenuBtnReStatusDto) => btn.Id!) || [];
    
    return {
      Id: item.Id!,
      Name: item.Name!,
      Title: item.Title || '',
      DataStatus: item.DataStatus || 0,
      Exist: item.Exist || false,
      selectedBtns: selectedBtns,
      Children: item.Children ? processMenuBtnData(item.Children) : undefined,
      BtnList: item.BtnList?.map((btn: APIModel.MenuBtnReStatusDto) => ({
        Id: btn.Id!,
        Name: btn.Name!,
        DataStatus: btn.DataStatus || 0,
        Exist: btn.Exist || false
      }))
    };
  });
};

// 处理按钮选择变化
const handleBtnChange = (data: MenuBtnRe, val: string[]) => {
  // 更新按钮的选中状态
  if (data.BtnList) {
    data.BtnList.forEach(btn => {
      btn.DataStatus = val.includes(btn.Id) ? 1 : 3; // 1-Added, 3-Deleted (简化处理)
    });
  }
};

// 重置表单
const resetForm = () => {
  formRef.value?.resetFields();
};

// 保存权限分配
const savePermissions = async () => {
  try {
    // 构建权限数据
    const permissionData = buildPermissionData(state.menuBtnData);
    
    // 调用保存接口
    const result = await FdRoleApi.postApiAdminFdRoleIdMenuBtns(
      { id: state.currentRoleId },
      permissionData
    );
    
    if (result) {
      ElMessage.success('权限分配成功');
      state.permissionDialog.visible = false;
    } else {
      ElMessage.error('权限分配失败');
    }
  } catch (error) {
    ElMessage.error('权限分配失败');
    console.error(error);
  }
};

// 构建权限数据
const buildPermissionData = (menuBtnList: MenuBtnRe[]): APIModel.MenuBtnRe[] => {
  return menuBtnList.map(item => {
    // 处理按钮权限
    const BtnList = item.BtnList?.map((btn: MenuBtnReStatusDto) => {
      // 根据是否选中确定数据状态
      const isSelected = item.selectedBtns?.includes(btn.Id);
      return {
        Id: btn.Id,
        Name: btn.Name,
        DataStatus: isSelected ? 1 : 3, // 1-Added, 3-Deleted
        Exist: isSelected
      } as APIModel.MenuBtnReStatusDto;
    }) || [];
    
    return {
      Id: item.Id,
      Name: item.Name,
      Title: item.Title,
      DataStatus: item.Exist ? 2 : 1, // 2-Modified, 1-Added (简化处理)
      Exist: true, // 根据实际业务逻辑调整
      BtnList: BtnList,
      Children: item.Children ? buildPermissionData(item.Children) : undefined
    } as APIModel.MenuBtnRe;
  });
};

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
.custom-tree-node {
  display: flex;
  align-items: center;
  width: 100%;
}

.menu-name {
  flex: 1;
  margin-right: 12px;
  font-weight: 500;
}

.btn-group {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.btn-checkbox {
  display: inline-flex;
  align-items: center;
  padding: 2px 6px;
  border-radius: 4px;
  background-color: #f5f7fa;
  margin-right: 4px;
}

.btn-label {
  margin-left: 4px;
  font-size: 12px;
}

.el-tree-node__content {
  padding: 6px 0;
}

.el-checkbox {
  margin-right: 8px;
}

// 固定高度宽度的权限分配对话框
.permission-tree-container {
  height: 500px; /* 写死高度 */
  overflow-y: auto;
  padding: 16px;
}

.el-dialog {
  &.permission-dialog {
    .el-dialog__body {
      padding: 0;
    }
  }
}
</style>