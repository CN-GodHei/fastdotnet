<template>
	<div class="sys-preview-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="80%" top="5vh">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-View /> </el-icon>
					<span> {{ state.dialogTitle }} </span>
				</div>
			</template>
			
			<el-tabs v-model="state.activeTab" type="card" @tab-change="onTabChange">
				<el-tab-pane label="Entity" name="entity">
					<pre class="code-content">{{ state.entityCode }}</pre>
				</el-tab-pane>
				<el-tab-pane label="DTO" name="dto">
					<pre class="code-content">{{ state.dtoCode }}</pre>
				</el-tab-pane>
				<el-tab-pane label="Service" name="service">
					<pre class="code-content">{{ state.serviceCode }}</pre>
				</el-tab-pane>
				<el-tab-pane label="Controller" name="controller">
					<pre class="code-content">{{ state.controllerCode }}</pre>
				</el-tab-pane>
				<el-tab-pane label="Frontend" name="frontend">
					<pre class="code-content">{{ state.frontendCode }}</pre>
				</el-tab-pane>
			</el-tabs>
			
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="cancel">关 闭</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="sysPreview">
import { reactive } from 'vue';
import { ElMessage } from 'element-plus';
import { getCodeGenPreviewConfigId } from '/@/api/fd-system-api/CodeGen';

interface PreviewRow {
	Id: string;
	TableName: string;
	EntityName: string;
	NameSpace: string;
	PagePath: string;
}

const state = reactive({
	isShowDialog: false,
	dialogTitle: '预览代码',
	activeTab: 'entity',
	entityCode: '// 正在加载...',
	dtoCode: '',
	serviceCode: '',
	controllerCode: '',
	frontendCode: '',
	currentRow: null as PreviewRow | null,
});

// 标签页切换事件
const onTabChange = async (tabName: string) => {
	if (state.currentRow?.Id) {
		await loadCodeContent(state.currentRow.Id, tabName);
	}
};

// 加载代码内容
const loadCodeContent = async (configId: string, type: string) => {
	try {
		const res = await getCodeGenPreviewConfigId({
			configId: configId,
			type: type
		});
		
		switch(type) {
			case 'entity':
				state.entityCode = res;
				break;
			case 'dto':
				state.dtoCode = res;
				break;
			case 'service':
				state.serviceCode = res;
				break;
			case 'controller':
				state.controllerCode = res;
				break;
			case 'frontend':
				state.frontendCode = res;
				break;
		}
	} catch (error) {
		console.error(`加载${type}代码失败`, error);
		ElMessage.error(`加载${type}代码失败`);
		
		// 设置错误信息到对应字段
		const errorContent = `// 加载${type}代码失败\n// 错误: ${(error as Error).message}`;
		switch(type) {
			case 'entity':
				state.entityCode = errorContent;
				break;
			case 'dto':
				state.dtoCode = errorContent;
				break;
			case 'service':
				state.serviceCode = errorContent;
				break;
			case 'controller':
				state.controllerCode = errorContent;
				break;
			case 'frontend':
				state.frontendCode = errorContent;
				break;
		}
	}
};

// 打开弹窗
const openDialog = async (row: PreviewRow) => {
	state.currentRow = row;
	state.dialogTitle = `预览代码 - ${row.EntityName || row.TableName}`;
	
	// 默认加载Entity代码，并预加载其他代码
	if (row.Id) {
		// 首先加载当前选中的标签页内容
		await loadCodeContent(row.Id, state.activeTab);
		
		// 预加载其他标签页内容
		const otherTypes = ['entity', 'dto', 'service', 'controller', 'frontend'].filter(t => t !== state.activeTab);
		for (const type of otherTypes) {
			// 我们不等待这些请求，以便用户界面能更快响应
			loadCodeContent(row.Id, type);
		}
	}
	
	state.isShowDialog = true;
};

// 取消
const cancel = () => {
	state.isShowDialog = false;
};

// 导出对象
defineExpose({ openDialog });
</script>

<style lang="scss" scoped>
.code-content {
	background-color: #f5f5f5;
	padding: 15px;
	border-radius: 4px;
	white-space: pre-wrap;
	font-family: 'Courier New', monospace;
	font-size: 12px;
	max-height: 500px;
	overflow-y: auto;
}
:deep(.el-dialog__body) {
	padding: 10px 20px;
}
</style>