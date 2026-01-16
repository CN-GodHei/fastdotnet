<template>
	<div class="system-menu-container layout-pd">
		<el-card shadow="hover">
			<div class="system-menu-search mb15">
				<el-input size="default" placeholder="请输入菜单名称" style="max-width: 180px"> </el-input>
				<el-button v-auth="'menu_code_11365291745215493_queryModule'" size="default" type="primary" class="ml10">
					<el-icon>
						<ele-Search />
					</el-icon>
					查询
				</el-button>
				<el-button v-auth="'menu_code_11365291745215493_add'" size="default" type="success" class="ml10" @click="onOpenAddMenu">
					<el-icon>
						<ele-FolderAdd />
					</el-icon>
					新增菜单
				</el-button>
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
							<el-button v-auth="'menu_code_11365291745215493_add'" size="small" text type="primary" @click="onOpenAddMenu('add')">新增</el-button>
							<el-button v-auth="'menu_code_11365291745215493_edit'" size="small" text type="primary" @click="onOpenEditMenu('edit', scope.row)">修改</el-button>
							<el-button v-auth="'menu_code_11365291745215493_delete'" size="small" text type="primary" @click="onTabelRowDel(scope.row)">删除</el-button>
						</template>
					</el-table-column>
			</el-table>
		</el-card>
		<MenuDialog ref="menuDialogRef" @refresh="getTableData()" />
	</div>
</template>

<script setup lang="ts" name="systemMenu">
import { defineAsyncComponent, ref, onMounted, reactive } from 'vue';
import { RouteRecordRaw } from 'vue-router';
import { ElMessageBox, ElMessage } from 'element-plus';
import * as MenuApi from '@/api/fd-system-api-admin/FdMenu';
// 引入组件
const MenuDialog = defineAsyncComponent(() => import('@/views/system/menu/dialog.vue'));

// 定义变量内容
const menuDialogRef = ref();
const state = reactive({
	tableData: {
		data: [] as APIModel.FdMenuDto[],
		loading: true,
	},
});

// 获取路由数据
const getTableData = async () => {
	state.tableData.loading = true;
	try {
		const res = await MenuApi.getApiAdminFdMenu();
		state.tableData.data = res || [];
	} catch (error) {
		ElMessage.error('获取菜单数据失败');
	} finally {
		state.tableData.loading = false;
	}
};
// 打开新增菜单弹窗
const onOpenAddMenu = (type: string) => {
	menuDialogRef.value.openDialog('add');
};
// 打开编辑菜单弹窗
const onOpenEditMenu = (type: string, row: APIModel.FdMenuDto) => {
	menuDialogRef.value.openDialog(type, row);
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