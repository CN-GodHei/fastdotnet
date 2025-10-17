<template>
	<div class="sys-preview-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="80%" top="5vh">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-View /> </el-icon>
					<span> {{ props.title }} </span>
				</div>
			</template>
			<el-tabs v-model="state.activeTab" type="card">
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

const props = defineProps({
	title: String,
});

const state = reactive({
	isShowDialog: false,
	activeTab: 'entity',
	entityCode: '',
	dtoCode: '',
	serviceCode: '',
	controllerCode: '',
	frontendCode: '',
});

// 打开弹窗
const openDialog = (row: any) => {
	// 模拟生成代码内容
	state.entityCode = `// ${row.tableName} 实体类\npublic class ${row.entityName} : BaseEntity\n{\n    // 实体属性将根据表结构生成\n}`;
	state.dtoCode = `// DTO 类将根据表结构生成`;
	state.serviceCode = `// Service 类将根据表结构生成`;
	state.controllerCode = `// Controller 类将根据表结构生成`;
	state.frontendCode = `// 前端页面将根据表结构生成`;
	
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