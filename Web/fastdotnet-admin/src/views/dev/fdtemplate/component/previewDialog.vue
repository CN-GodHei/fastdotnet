<template>
	<div class="sys-codeGenPreview-container">
		<el-dialog 
			v-model="state.isShowDialog" 
			draggable 
			:close-on-click-modal="false" 
			width="80%" 
			top="5vh"
			@opened="onDialogOpened"
		>
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-View /> </el-icon>
					<span> {{ state.dialogTitle }} </span>
				</div>
			</template>
			<div :class="[state.current?.endsWith('.cs') ? 'cs-style' : state.current?.endsWith('.vue') ? 'vue-style' : 'js-style']">
				<el-segmented v-model="state.current" :options="state.options" block @change="handleChange">
					<template #default="{ item }">
						<div class="pd4">
							<el-icon><component :is="item.icon" /></el-icon>
							<div>{{ item.value }}</div>
						</div>
					</template>
				</el-segmented>
			</div>
			<div ref="monacoEditorRef" style="width: 100%; height: 60vh; margin-top: 10px;" v-loading="state.loading"></div>
			<template #footer>
				<span class="dialog-footer">
					<el-button icon="ele-Close" @click="cancel">关 闭</el-button>
					<el-button icon="ele-CopyDocument" type="primary" @click="handleCopy">复 制</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="sysPreviewCode">
import { reactive, ref, nextTick } from 'vue';
import { ElMessage, ElIcon } from 'element-plus';
import * as monaco from 'monaco-editor';
import EditorWorker from 'monaco-editor/esm/vs/editor/editor.worker?worker';
import { getApiCodeGenPreviewConfigId } from '@/api/fd-system-api-admin/CodeGen';
import APIModel from '@/api/fd-system-api-admin';

const monacoEditorRef = ref();
const state = reactive({
	isShowDialog: false,
	dialogTitle: '代码预览',
	options: [] as any, // 分段器的选项
	current: '', // 选中的分段
	codes: {} as Record<string, string>, // 预览的代码
	loading: true,
	configRow: null as APIModel.CodeGenConfigDto | null //存储配置行数据
});

// 防止 monaco 报黄
self.MonacoEnvironment = {
	getWorker: (_: string, label: string) => new EditorWorker(),
};

// 初始化monacoEditor对象
let monacoEditor: any = null;

const initMonacoEditor = () => {
	if (!monacoEditorRef.value) {
		console.error('monacoEditorRef is not available');
		return;
	}
	
	monacoEditor = monaco.editor.create(monacoEditorRef.value, {
		theme: 'vs-dark', // 主题 vs vs-dark hc-black
		value: '', // 默认显示的值
		language: 'csharp',
		formatOnPaste: true,
		wordWrap: 'on', // 自动换行，注意大小写
		wrappingIndent: 'indent',
		folding: true, // 是否折叠
		foldingHighlight: true, // 折叠等高线
		foldingStrategy: 'indentation', // 折叠方式  auto | indentation
		showFoldingControls: 'always', // 是否一直显示折叠 always | mouSEOver
		disableLayerHinting: true, // 等宽优化
		emptySelectionClipboard: false, // 空选择剪切板
		selectionClipboard: false, // 选择剪切板
		automaticLayout: true, // 自动布局
		codeLens: false, // 代码镜头
		scrollBeyondLastLine: false, // 滚动完最后一行后再滚动一屏幕
		colorDecorators: true, // 颜色装饰器
		accessibilitySupport: 'auto', // 辅助功能支持  "auto" | "off" | "on"
		lineNumbers: 'on', // 行号 取值： "on" | "off" | "relative" | "interval" | function
		lineNumbersMinChars: 5, // 行号最小字符   number
		//enableSplitViewResizing: false,
		readOnly: true, // 是否只读  取值 true | false
	});
};

// 根据文件扩展名获取语言类型
const getLanguageByExtension = (fileName: string): string => {
	if (fileName.endsWith('.cs')) return 'csharp';
	if (fileName.endsWith('.vue')) return 'vue';
	if (fileName.endsWith('.ts')) return 'typescript';
	if (fileName.endsWith('.js')) return 'javascript';
	if (fileName.endsWith('.html')) return 'html';
	if (fileName.endsWith('.json')) return 'json';
	if (fileName.endsWith('.css')) return 'css';
	if (fileName.endsWith('.scss')) return 'scss';
	return 'text';
};

// 获取文件类型图标
const getFileIcon = (fileName: string): string => {
	if (fileName.endsWith('.cs')) return 'ele-Files';
	if (fileName.endsWith('.vue')) return 'ele-Vue';
	if (fileName.endsWith('.ts')) return 'ele-Document';
	if (fileName.endsWith('.js')) return 'ele-Document';
	if (fileName.endsWith('.html')) return 'ele-Document';
	if (fileName.endsWith('.json')) return 'ele-Document';
	if (fileName.endsWith('.css')) return 'ele-Document';
	return 'ele-Document';
};

// 对话框打开后初始化编辑器
const onDialogOpened = () => {
	if (monacoEditorRef.value && state.current && state.codes[state.current]) {
		if (!monacoEditor) {
			initMonacoEditor();
		}
		
		// 确保编辑器初始化完成后再设置值
		setTimeout(() => {
			if (monacoEditor) {
				monacoEditor.setValue(state.codes[state.current] || '');
				// 根据当前文件类型设置语言
				const language = getLanguageByExtension(state.current);
				const model = monacoEditor.getModel();
				if (model) {
					model.setLanguage(language);
				}
			}
		}, 100);
	}
};

// 打开弹窗
const openDialog = async (row: APIModel.CodeGenConfigDto) => {
	state.loading = true;
	state.configRow = row;
	state.dialogTitle = `代码预览 - ${row.EntityName || row.TableName}`;
	
	try {
		// 获取所有代码类型
		const types = ['entity', 'dto', 'service', 'controller', 'frontend'];
		const codes: Record<string, string> = {};
		const options: any[] = [];
		
		for (const type of types) {
			try {
				const content = await getApiCodeGenPreviewConfigId({
					configId: row.Id!,
					type: type
				});
				
				let fileName = '';
				let displayName = '';
				
				switch(type) {
					case 'entity':
						fileName = `${row.EntityName || 'Entity'}.cs`;
						displayName = '实体类';
						break;
					case 'dto':
						fileName = `${row.EntityName || 'Entity'}Dto.cs`;
						displayName = '数据传输对象';
						break;
					case 'service':
						fileName = `${row.EntityName || 'Entity'}Service.cs`;
						displayName = '服务类';
						break;
					case 'controller':
						fileName = `${row.EntityName || 'Entity'}Controller.cs`;
						displayName = '控制器';
						break;
					case 'frontend':
						fileName = `${row.EntityName || 'Entity'}.vue`;
						displayName = '前端页面';
						break;
				}
				
				codes[fileName] = content;
				options.push({
					value: fileName,
					icon: getFileIcon(fileName),
					displayName: displayName
				});
			} catch (error) {
				console.error(`加载${type}代码失败`, error);
				const fileName = `${type}.txt`;
				codes[fileName] = `// 加载${type}代码失败\n// 错误: ${(error as Error).message}`;
				options.push({
					value: fileName,
					icon: 'ele-Document',
					displayName: `${type} - 错误`
				});
			}
		}
		
		state.codes = codes;
		state.options = options;
		state.current = options[0]?.value ?? '';
	} catch (e) { 
		console.error('加载代码预览失败', e);
		ElMessage.error('加载代码预览失败');
	}
	
	state.loading = false;
	
	// 显示对话框，编辑器将在@opened事件中初始化
	state.isShowDialog = true;
};

// 分段器改变时切换代码
const handleChange = (current: any) => {
	if (monacoEditor && state.codes[current]) {
		monacoEditor.setValue(state.codes[current] || '');
		// 根据当前文件类型设置语言
		const language = getLanguageByExtension(current);
		const model = monacoEditor.getModel();
		if (model) {
			model.setLanguage(language);
		}
	}
};

// 取消
const cancel = () => {
	if (monacoEditor) {
		monacoEditor.dispose(); // 释放编辑器资源
		monacoEditor = null;
	}
	state.isShowDialog = false;
};

//复制代码
const handleCopy = () => {
	if (state.current && state.codes[state.current]) {
		navigator.clipboard.writeText(state.codes[state.current]).then(() => {
			ElMessage.success('代码已复制到剪贴板');
		}).catch(() => {
			ElMessage.error('复制失败');
		});
	}
};

// 导出对象
defineExpose({ openDialog });
</script>

<style scoped>
.sys-codeGenPreview-container {
	:deep(.el-dialog__body) {
		padding: 10px 15px;
	}
}
.cs-style .el-segmented {
	--el-segmented-item-selected-bg-color: #5c2d91;
}
.vue-style .el-segmented {
	--el-segmented-item-selected-bg-color: #42b883;
}
.js-style .el-segmented {
	--el-segmented-item-selected-bg-color: #e44d26;
}
.pd4 {
	padding: 4px;
}
.mb4 {
	margin-bottom: 4px;
}
:deep(.el-segmented__item) {
	min-width: 100px;
}
</style>