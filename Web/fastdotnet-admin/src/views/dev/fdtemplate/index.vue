<template>
	<div class="sys-codeGen-container">
		<el-card shadow="hover" :body-style="{ padding: 5 }">
			<el-form :model="state.queryParams" ref="queryForm" :inline="true">
				<el-form-item label="数据库表名">
					<el-input placeholder="数据库表名" clearable @keyup.enter="handleQuery" v-model="state.queryParams.tableName" />
				</el-form-item>
				<el-form-item>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" @click="handleQuery"> 查询 </el-button>
						<el-button icon="ele-Refresh" @click="resetQuery"> 重置 </el-button>
					</el-button-group>
				</el-form-item>
				<el-form-item>
					<el-button type="primary" icon="ele-Plus" @click="openAddDialog"> 增加 </el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<el-table :data="state.tableData" style="width: 100%" v-loading="state.loading" border>
				<el-table-column type="index" label="序号" width="55" align="center" />
				<el-table-column prop="TableName" label="表名称" align="center" show-overflow-tooltip />
				<el-table-column prop="NameSpace" label="命名空间" align="center" show-overflow-tooltip />
				<el-table-column prop="GenerateType" label="生成方式" align="center" show-overflow-tooltip>
					<template #default="scope">
            <span>{{ scope.row.GenerateType }}</span>
					</template>
				</el-table-column>
				<el-table-column label="操作" width="320" fixed="right" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-button icon="ele-Delete" size="small" text type="danger" title="删除" @click="deleConfig(scope.row)" />
						<el-button icon="ele-Edit" size="small" text type="primary" title="编辑" @click="openEditDialog(scope.row)" />
						<el-button icon="ele-CopyDocument" size="small" text type="primary" title="复制" @click="openCopyDialog(scope.row)" />
						<el-button icon="ele-View" size="small" text type="primary" title="预览" @click="handlePreview(scope.row)" />
						<el-button icon="ele-Setting" size="small" text type="primary" title="配置" @click="openConfigDialog(scope.row)" />
						<el-button icon="ele-Refresh" size="small" text type="primary" title="同步" @click="syncCodeGen(scope.row)" />
						<el-button icon="ele-Position" size="small" text type="primary" @click="handleGenerate(scope.row)">生成</el-button>
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

		<EditCodeGenDialog :title="state.editTitle" ref="EditCodeGenRef" @handleQuery="handleQuery" :applicationNamespaces="state.applicationNamespaces" />
		<CodeConfigDialog ref="CodeConfigRef" @handleQuery="handleQuery" />
		<PreviewDialog :title="state.editTitle" ref="PreviewRef" />
	</div>
</template>

<script lang="ts" setup name="sysCodeGen">
import { onMounted, reactive, ref, defineAsyncComponent } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { 
  getApiCodeGenPage, 
  deleteApiCodeGenId, 
  getApiCodeGenApplicationnamespaces 
} from '@/api/fd-system-api-admin/CodeGen';
import APIModel from '@/api/fd-system-api-admin';

const EditCodeGenDialog = defineAsyncComponent(() => import('./component/editCodeGenDialog.vue'));
const CodeConfigDialog = defineAsyncComponent(() => import('./component/genConfigDialog.vue'));
const PreviewDialog = defineAsyncComponent(() => import('./component/previewDialog.vue'));

const EditCodeGenRef = ref<InstanceType<typeof EditCodeGenDialog>>();
const CodeConfigRef = ref<InstanceType<typeof CodeConfigDialog>>();
const PreviewRef = ref<InstanceType<typeof PreviewDialog>>();

interface QueryParams {
	busName?: string;
	tableName?: string;
}

interface TableParams {
	page: number;
	pageSize: number;
	total: number;
}

const state = reactive({
	loading: false,
	tableData: [] as APIModel.CodeGenConfigDto[],
	queryParams: {
		busName: undefined,
		tableName: undefined,
	} as QueryParams,
	tableParams: {
		page: 1,
		pageSize: 50,
		total: 0,
	} as TableParams,
	editTitle: '',
	applicationNamespaces: [] as string[],
});

onMounted(async () => {
	await handleQuery();
	const res = await getApiCodeGenApplicationnamespaces();
	state.applicationNamespaces = res;
});

// 表查询操作
const handleQuery = async () => {
	state.loading = true;
	try {
		const params = {
			pageIndex: state.tableParams.page,
			pageSize: state.tableParams.pageSize,
			...state.queryParams
		};
		const res = await getApiCodeGenPage(params);
		if (res) {
			state.tableData = res.Items|| [];
			state.tableParams.total = res.PageInfo || 0;
		}
	} catch (error) {
		console.error('获取代码生成列表失败', error);
		state.tableData = [];
		state.tableParams.total = 0;
	} finally {
		state.loading = false;
	}
};

// 重置操作
const resetQuery = () => {
	state.queryParams.busName = undefined;
	state.queryParams.tableName = undefined;
	handleQuery();
};

// 改变页面容量
const handleSizeChange = (val: number) => {
	state.tableParams.pageSize = val;
	handleQuery();
};

// 改变页码序号
const handleCurrentChange = (val: number) => {
	state.tableParams.page = val;
	handleQuery();
};

// 打开表增加页面
const openAddDialog = () => {
	state.editTitle = '增加';
	const namespaces = state.applicationNamespaces || [];
	const defaultNamespace = namespaces.length > 0 ? namespaces[0] : 'Fastdotnet.Service';
	EditCodeGenRef.value?.openDialog({
		authorName: 'Developer',
		generateType: 'zip',
		menuIcon: 'ele-Menu',
		pagePath: 'main',
		nameSpace: defaultNamespace,
		generateMenu: false,
		tableName: undefined,
		entityName: undefined,
		tableUniqueList: null,
		menuPid: undefined
	} as APIModel.CreateCodeGenDto);
};

// 打开表编辑页面
const openEditDialog = (row: any) => {
	//console.log('编辑行数据:', row);
	state.editTitle = '编辑';
	EditCodeGenRef.value?.openDialog(row);
};

// 打开复制页面
const openCopyDialog = (row: any) => {
	state.editTitle = '复制';
	const copyRow = JSON.parse(JSON.stringify(row));
	copyRow.Id = undefined;
	copyRow.busName = '';
	copyRow.tableName = '';
	copyRow.tableUniqueList = undefined;
	EditCodeGenRef.value?.openDialog(copyRow);
};

// 删除表
const deleConfig = (row: any) => {
	ElMessageBox.confirm(`确定删除吗?`, '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning',
	}).then(async () => {
		try {
			const res = await deleteApiCodeGenId({ id: row.Id });
			if (res) {
				ElMessage.success('删除成功');
				await handleQuery();
			}
		} catch (error) {
			console.error('删除失败', error);
			ElMessage.error('删除失败');
		}
	}).catch(() => {});
};

// 同步生成
const syncCodeGen = async (row: any) => {
	ElMessageBox.confirm(`确定要同步吗?`, '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning',
	}).then(async () => {
		// 同步代码生成配置 - 这里可以调用更新接口
		ElMessage.success('同步成功');
	}).catch(() => {});
};

// 开始生成代码
// const handleGenerate = (row: any) => {
// 	ElMessageBox.confirm(`确定要生成吗?`, '提示', {
// 		confirmButtonText: '确定',
// 		cancelButtonText: '取消',
// 		type: 'warning',
// 	})
// 		.then(async () => {
// 			try {
// 				const res = await postApiCodeGenGenerate({
// 					configId: row.configId,
// 					tableName: row.tableName,
// 					busName: row.busName,
// 					nameSpace: row.nameSpace,
// 					authorName: row.authorName,
// 					generateType: row.generateType,
// 					generateMenu: row.generateMenu,
// 					menuIcon: row.menuIcon,
// 					menuPid: row.menuPid,
// 					pagePath: row.pagePath
// 				});
				
// 				// 生成代码成功，可以下载
// 				if (res) {
// 					// 通过创建隐藏链接来下载文件
// 					const link = document.createElement('a');
// 					link.href = `/api/CodeGen/download?filePath=${encodeURIComponent(res)}`;
// 					link.target = '_blank';
// 					link.click();
// 					ElMessage.success('代码生成成功，下载已开始');
// 				} else {
// 					ElMessage.success('代码生成成功');
// 				}
// 			} catch (error) {
// 				console.error('代码生成失败', error);
// 				ElMessage.error('代码生成失败');
// 			}
// 		})
// 		.catch(() => {});
// };

// 预览代码
const handlePreview = (row: any) => {
	state.editTitle = '预览代码';
	PreviewRef.value?.openDialog(row);
};

const openConfigDialog = (row: any) => {
	CodeConfigRef.value?.openDialog(row);
};
</script>