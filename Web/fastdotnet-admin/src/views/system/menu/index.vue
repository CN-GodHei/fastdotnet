<template>
	<div class="fdmenu-container">
		<el-card shadow="hover" :body-style="{ padding: 2 }">
			<el-form :model="state.queryParams" ref="queryForm" :inline="true">
				<div v-show="state.searchCollapsed">
					<el-form-item label="模块分类" prop="Belong">
						<el-select placeholder="请选择模块分类" clearable v-model="state.queryParams.Belong" style="width: 150px" @change="handleBelongChange">
							<el-option label="管理端" :value="0"></el-option>
							<el-option label="应用端" :value="1"></el-option>
						</el-select>
					</el-form-item>
					<el-form-item label="菜单名称" prop="Title">
						<el-input placeholder="请输入菜单名称" clearable v-model="state.queryParams.Title"
							style="width: 150px" />
					</el-form-item>
					<el-form-item label="组件名称" prop="Name">
						<el-input placeholder="请输入组件名称" clearable v-model="state.queryParams.Name"
							style="width: 150px" />
					</el-form-item>
					<el-form-item label="路由路径" prop="Path">
						<el-input placeholder="请输入路由路径" clearable v-model="state.queryParams.Path"
							style="width: 150px" />
					</el-form-item>
					<el-form-item label="权限标识" prop="PermissionCode">
						<el-input placeholder="请输入权限标识" clearable v-model="state.queryParams.PermissionCode"
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
				<el-button v-auth="'menu_code_11365291745215493_add'" type="primary" icon="ele-Plus" @click="onOpenAddMenu"> 新增 </el-button>
				<el-button icon="ele-Download"> 导出 </el-button>
			</div>
			<el-table
				:data="state.tableData.data"
				v-loading="state.tableData.loading"
				style="width: 100%"
				row-key="Id"
				:tree-props="{ children: 'Children', hasChildren: 'hasChildren' }"
			>
				<el-table-column label="菜单名称" show-overflow-tooltip>
					<template #default="scope">
						<SvgIcon :name="scope.row.Icon" />
						<span class="ml10">{{ scope.row.Title || scope.row.Name }}</span>
					</template>
				</el-table-column>
				<el-table-column prop="Name" label="组件名称" show-overflow-tooltip></el-table-column>
				<el-table-column prop="Path" label="路由路径" show-overflow-tooltip></el-table-column>
				<el-table-column label="组件路径" show-overflow-tooltip>
					<template #default="scope">
						<span>{{ scope.row.Component }}</span>
					</template>
				</el-table-column>
				<el-table-column label="权限标识" show-overflow-tooltip>
					<template #default="scope">
						<span>{{ scope.row.PermissionCode }}</span>
					</template>
				</el-table-column>
				<el-table-column prop="Sort" label="排序" show-overflow-tooltip width="80"></el-table-column>
				<el-table-column label="类型" show-overflow-tooltip width="80">
					<template #default="scope">
						<el-tag type="success" size="small">{{ scope.row.Type === 0 ? '目录' : '菜单' }}</el-tag>
					</template>
				</el-table-column>
				<el-table-column label="操作" show-overflow-tooltip width="140">
						<template #default="scope">
							<el-button v-auth="'menu_code_11365291745215493_add'" size="small" text type="primary" @click="onOpenAddMenu('add', scope.row)">新增</el-button>
							<el-button v-auth="'menu_code_11365291745215493_edit'" size="small" text type="primary" @click="onOpenEditMenu('edit', scope.row)">修改</el-button>
							<el-button v-auth="'menu_code_11365291745215493_delete'" size="small" text type="primary" @click="onTabelRowDel(scope.row)">删除</el-button>
						</template>
					</el-table-column>
			</el-table>
		</el-card>
		<MenuDialog ref="menuDialogRef" @refresh="getTableData()" />
	</div>
</template>

<script setup lang="ts" name="FdMenu">
import { defineAsyncComponent, ref, onMounted, reactive } from 'vue';
import { RouteRecordRaw } from 'vue-router';
import { ElMessageBox, ElMessage } from 'element-plus';
import * as MenuApi from '@/api/fd-system-api-admin/FdMenu';
// 引入组件
const MenuDialog = defineAsyncComponent(() => import('@/views/system/menu/dialog.vue'));
import { buildMixedQuery } from '@/utils/queryBuilder';

const queryForm = ref();
const menuDialogRef = ref();
const state = reactive({
	loading: false,
	searchCollapsed: true,
	tableData: {
		data: [] as APIModel.FdMenuDto[],
		loading: true,
	},
	queryParams: {
		Title: null,
		Name: null,
		Path: null,
		PermissionCode: null,
		Belong: 0,
	},
});

// 切换搜索区域折叠状态
const toggleSearchCollapse = () => {
	state.searchCollapsed = !state.searchCollapsed;
};

// 获取路由数据
const getTableData = async () => {
	state.tableData.loading = true;
	try {
				//构建查询条件
		const queryConfig =
		{
			customs: [
				{ field: 'Belong', operator: 'eq', value: state.queryParams.Belong !== null ? state.queryParams.Belong : 0, },
				{ field: 'Title', operator: 'eq', value: state.queryParams.Title, },
				{ field: 'Name', operator: 'eq', value: state.queryParams.Name, },
				{ field: 'Path', operator: 'eq', value: state.queryParams.Path, },
				{ field: 'PermissionCode', operator: 'eq', value: state.queryParams.PermissionCode, },
			],
			ranges: {}
		}
		const searchBody: APIModel.QueryByConditionDto = {

		};
		const queryResult = buildMixedQuery(queryConfig);
		if (queryResult.dynamicQuery) {
			searchBody.DynamicQuery = queryResult.dynamicQuery;
			searchBody.QueryParameters = queryResult.queryParameters;
		}
		// const res = await MenuApi.getApiAdminFdMenu();
		const res = await MenuApi.postApiAdminFdMenuListByCondition(searchBody);
		state.tableData.data = res || [];
	} catch (error) {
		ElMessage.error('获取菜单数据失败');
	} finally {
		state.tableData.loading = false;
	}
};

// 模块分类变更事件
const handleBelongChange = () => {
	getTableData();
};

// 查询
const handleQuery = () => {
	getTableData();
};

// 重置
const resetQuery = () => {
	queryForm.value.resetFields();
	handleQuery();
};
// 打开新增菜单弹窗
const onOpenAddMenu = (type: string, row?: APIModel.FdMenuDto) => {
	if (row) {
		// 如果传递了row参数，说明是在某一行下新增子菜单
		menuDialogRef.value.openDialog('add', row, state.queryParams.Belong);
	} else {
		// 否则是新增顶级菜单
		menuDialogRef.value.openDialog('add', undefined, state.queryParams.Belong);
	}
};
// 打开编辑菜单弹窗
const onOpenEditMenu = (type: string, row: APIModel.FdMenuDto) => {
	menuDialogRef.value.openDialog(type, row, state.queryParams.Belong);
};
// 删除当前行
const onTabelRowDel = (row: APIModel.FdMenuDto) => {
	ElMessageBox.confirm(`此操作将永久删除路由：${row.Path}, 是否继续?`, '提示', {
		confirmButtonText: '删除',
		cancelButtonText: '取消',
		type: 'warning',
	})
		.then(async () => {
			try {
				// 使用row.Id作为菜单的唯一标识符
				if (row.Id) {
					await MenuApi.deleteApiAdminFdMenuId({ id: row.Id });
					ElMessage.success('删除成功');
					getTableData();
				} else {
					ElMessage.error('菜单ID不存在，无法删除');
				}
			} catch (error) {
				ElMessage.error('删除菜单失败');
			}
		})
		.catch(() => {});
};
// 页面加载时
onMounted(() => {
	getTableData();
});
</script>