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
				:check-strictly="true"
				@check-change="handleTreeCheckChange"
				>
				<template #default="{ node, data }">
					<span class="custom-tree-node">
						<span class="menu-name">{{ data.Title }}</span>
						<span v-if="data.BtnList && data.BtnList.length > 0" class="btn-group">
							<el-checkbox
								v-for="btn in data.BtnList"
								:key="btn.Id"
								:checked="data.selectedBtns?.includes(btn.Id)"
								@change="(checked: boolean) => handleSingleBtnChange(data, btn.Id, checked)"
								:disabled="btn.disabled"
								class="btn-checkbox"
							>
								<span class="btn-label">{{ btn.Name }}</span>
							</el-checkbox>
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
  DataStatus: number;
  Exist: boolean;
  Children?: MenuBtnRe[];
  BtnList?: MenuBtnReStatusDto[];
  selectedBtns: string[]; // 保持简单，确保是数组
}

interface MenuBtnReStatusDto {
  Id: string;
  Name: string;
  DataStatus: number;
  Exist: boolean;
  disabled?: boolean;
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
    //console.log(`为 "${row.Name}" 分配权限`)
    state.permissionDialog.title = `为 "${row.Name}" 分配权限`;
    
    // 先清空现有数据，避免显示旧数据
    state.menuBtnData = [];
    
    // 获取菜单按钮数据
    const menuBtnData = (await FdMenuApi.getApiAdminFdMenuMenuBtns({ Belong: 0, RoleId: row.Id as string })) as unknown as APIModel.MenuBtnRe[];
    state.menuBtnData = processMenuBtnData(menuBtnData);
    
    // 延迟设置 Tree 组件的选中状态，确保 DOM 已经渲染
    setTimeout(() => {
      initializeTreeCheckedState(state.menuBtnData);
    }, 100);
  } catch (error) {
    ElMessage.error('获取权限数据失败');
    //console.error(error);
  }
};

// 处理菜单按钮数据，添加选中状态
const processMenuBtnData = (menuBtnList: APIModel.MenuBtnRe[]): MenuBtnRe[] => {
  return menuBtnList.map(item => {
    // 根据 Exist 字段初始化选中的按钮
    const selectedBtns: string[] = [];
    if (item.BtnList) {
      item.BtnList.forEach((btn: APIModel.MenuBtnReStatusDto) => {
        if (btn.Exist) {
          selectedBtns.push(btn.Id!);
        }
      });
    }
    
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
        Exist: btn.Exist || false,
        disabled: false
      }))
    };
  });
};

// 初始化 Tree 组件的选中状态
const initializeTreeCheckedState = (menuList: MenuBtnRe[]) => {
  menuList.forEach(menu => {
    // 根据 Exist 字段设置菜单节点选中状态
    if (menu.Exist) {
      menuTreeRef.value?.setChecked(menu.Id, true, false);
    }
    
    // 递归处理子菜单
    if (menu.Children) {
      initializeTreeCheckedState(menu.Children);
    }
  });
  
  // 处理父子菜单联动状态
  menuList.forEach(menu => {
    if (menu.Children) {
      updateParentMenuState(menu.Id);
    }
  });
};

// 更新父级菜单状态
const updateParentMenuState = (childId: string) => {
  // 在树形数据中查找包含此子节点的父节点
  const findAndProcessParents = (menuList: MenuBtnRe[]) => {
    for (const menu of menuList) {
      // 检查当前菜单的直接子节点
      if (menu.Children) {
        const hasChild = menu.Children.some(child => child.Id === childId);
        if (hasChild) {
          // 找到了父节点，更新其状态
          updateMenuNodeState(menu);
          return true;
        }
        // 递归检查更深层的子节点
        if (findAndProcessParents(menu.Children)) {
          return true;
        }
      }
    }
    return false;
  };
  
  findAndProcessParents(state.menuBtnData);
};

// 更新单个菜单节点的状态
const updateMenuNodeState = (menu: MenuBtnRe) => {
  // 检查子节点的选中状态
  let hasCheckedChildren = false;
  let hasUncheckedChildren = false;
  
  // 检查直接子菜单
  if (menu.Children) {
    for (const child of menu.Children) {
      const isChecked = menuTreeRef.value?.getCheckedKeys().includes(child.Id) || false;
      const isHalfChecked = menuTreeRef.value?.getHalfCheckedKeys().includes(child.Id) || false;
      const hasButtons = child.selectedBtns && child.selectedBtns.length > 0;
      
      if (isChecked || isHalfChecked || hasButtons) {
        hasCheckedChildren = true;
      } else {
        hasUncheckedChildren = true;
      }
    }
  }
  
  // 根据子节点状态更新当前节点
  if (hasCheckedChildren && hasUncheckedChildren) {
    // 部分子节点被选中 - 设置为半选中
    menuTreeRef.value?.setChecked(menu.Id, false, false);
    // Element Plus 的半选中需要特殊处理
    const treeNode = menuTreeRef.value?.getNode(menu.Id);
    if (treeNode) {
      treeNode.indeterminate = true;
    }
  } else if (hasCheckedChildren && !hasUncheckedChildren) {
    // 所有子节点都被选中 - 设置为全选中
    menuTreeRef.value?.setChecked(menu.Id, true, false);
  } else {
    // 没有子节点被选中 - 保持当前状态或取消选中
    const currentChecked = menuTreeRef.value?.getCheckedKeys().includes(menu.Id) || false;
    if (!currentChecked) {
      menuTreeRef.value?.setChecked(menu.Id, false, false);
    }
  }
};

// 处理树节点选中状态变化
const handleTreeCheckChange = (data: MenuBtnRe, checked: boolean) => {
  // 只有当用户主动取消菜单节点选中时，才清空按钮
  // 注意：这里不处理按钮变化触发的节点状态变化
  if (!checked) {
    // 检查是否真的是用户主动取消，而不是按钮变化引起的
    const hasSelectedButtons = data.selectedBtns && data.selectedBtns.length > 0;
    if (hasSelectedButtons) {
      // 如果有按钮被选中，说明这是由按钮变化触发的，不应清空按钮
      // 延迟执行，让按钮处理完成后再检查
      setTimeout(() => {
        const stillHasButtons = data.selectedBtns && data.selectedBtns.length > 0;
        if (stillHasButtons) {
          // 重新选中节点，因为还有按钮被选中
          menuTreeRef.value?.setChecked(data.Id, true, false);
        } else {
          // 真的是用户主动取消菜单节点
          data.selectedBtns = [];
        }
      }, 0);
    } else {
      // 确实没有按钮被选中，可以清空
      data.selectedBtns = [];
    }
  }
  
  // 处理父级菜单联动
  if (checked) {
    updateParentMenuState(data.Id);
  }
};

// 处理单个按钮选择变化
const handleSingleBtnChange = (data: MenuBtnRe, btnId: string, checked: boolean) => {
  // 确保数组存在
  if (!data.selectedBtns) {
    data.selectedBtns = [];
  }
  
  const currentIndex = data.selectedBtns.indexOf(btnId);
  
  if (checked && currentIndex === -1) {
    // 添加选中项
    data.selectedBtns.push(btnId);
  } else if (!checked && currentIndex > -1) {
    // 移除选中项
    data.selectedBtns.splice(currentIndex, 1);
  }
  
  // 强制触发响应式更新
  data.selectedBtns = [...data.selectedBtns];
  
  // 同步树节点状态 - 只有当没有任何按钮被选中时才取消菜单选中
  if (data.selectedBtns.length > 0) {
    // 有按钮被选中，确保菜单节点也被选中
    menuTreeRef.value?.setChecked(data.Id, true, false);
  } else {
    // 没有按钮被选中，且菜单节点不是被直接选中的情况下才取消
    const checkedNodes = menuTreeRef.value?.getCheckedKeys() || [];
    const halfCheckedNodes = menuTreeRef.value?.getHalfCheckedKeys() || [];
    
    // 只有当菜单节点既不是直接选中也不是半选中时才取消
    if (!checkedNodes.includes(data.Id) && !halfCheckedNodes.includes(data.Id)) {
      menuTreeRef.value?.setChecked(data.Id, false, false);
    }
  }
  
  // 处理父级菜单联动
  updateParentMenuState(data.Id);
  
  //console.log('按钮状态:', data.Title, data.selectedBtns);
};

// 处理按钮选择变化
const handleBtnChange = (data: MenuBtnRe, val: string[]) => {
  // 确保 selectedBtns 是数组类型
  if (!Array.isArray(val)) {
    val = [];
  }
  
  // 更新选中按钮数组
  data.selectedBtns = [...val]; // 创建新数组确保响应式更新
  
  // 同步更新树节点的选中状态
  if (val.length > 0) {
    // 如果有按钮被选中，确保对应的菜单节点也被选中
    menuTreeRef.value?.setChecked(data.Id, true, false);
  } else {
    // 如果没有按钮被选中，检查是否需要取消菜单节点选中
    // 只有当菜单节点本身未被直接选中时才取消
    const checkedNodes = menuTreeRef.value?.getCheckedKeys() || [];
    if (!checkedNodes.includes(data.Id)) {
      menuTreeRef.value?.setChecked(data.Id, false, false);
    }
  }
  
  //console.log('按钮选择变化:', data.Title, data.selectedBtns);
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
    //console.log(permissionData)
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
    //console.error(error);
  }
};

// 构建权限数据
const buildPermissionData = (menuBtnList: MenuBtnRe[]): APIModel.MenuBtnRe[] => {
  return menuBtnList.flatMap(item => {
    // 检查当前菜单是否被选中（通过Tree组件）或是否有按钮被选中
    const isMenuSelected = menuTreeRef.value?.getCheckedKeys().includes(item.Id) || false;
    const hasSelectedButtons = item.selectedBtns && item.selectedBtns.length > 0;
    const hasSelectedChildren = item.Children ? buildPermissionData(item.Children) : [];
    
    // 如果菜单被选中、或有按钮被选中、或有子菜单被选中，则返回该菜单
    if (isMenuSelected || hasSelectedButtons || hasSelectedChildren.length > 0) {
      // 过滤出被选中的按钮
      const selectedBtnList = item.BtnList?.filter(btn => item.selectedBtns?.includes(btn.Id)) || [];
      
      const BtnList = selectedBtnList.map(btn => {
        return {
          Id: btn.Id,
          Name: btn.Name,
          DataStatus: 1, // 1-Added
          Exist: true
        } as APIModel.MenuBtnReStatusDto;
      });
      
      return [{
        Id: item.Id,
        Name: item.Name,
        Title: item.Title,
        DataStatus: 1, // 1-Added
        Exist: true,
        BtnList: BtnList,
        Children: hasSelectedChildren, // 只返回被选中的子菜单
      } as APIModel.MenuBtnRe];
    } else {
      return []; // 没有选中菜单或其按钮或子菜单，不返回该项
    }
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
		////console.log('Search request body:', searchBody);
		const response = await FdRoleApi.postApiAdminFdRolePageSearch(searchBody);
		state.tableData.data = response.Items as APIModel.FdRoleDto[] || [] as APIModel.FdRoleDto[];
		state.pagination.total = response.PageInfo?.Total || 0;
	} catch (error) {
		ElMessage.error('获取数据失败');
		////console.error(error);
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
			//console.error(error);
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