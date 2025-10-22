<template>
	<div class="sys-codeConfig-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="800px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Setting /> </el-icon>
					<span> 代码生成配置 </span>
				</div>
			</template>
			<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="auto">
				<el-tabs v-model="state.activeTab" tab-position="left" class="demo-tabs">
					<el-tab-pane label="基本信息" name="basic">
						<el-row :gutter="35">
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="库定位器" prop="configId">
									<el-input v-model="state.ruleForm.configId" disabled />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="表名" prop="tableName">
									<el-input v-model="state.ruleForm.tableName" disabled />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="实体名" prop="entityName">
									<el-input v-model="state.ruleForm.entityName" disabled />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="业务名" prop="busName">
									<el-input v-model="state.ruleForm.busName" placeholder="请输入业务名" />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="命名空间" prop="nameSpace">
									<el-input v-model="state.ruleForm.nameSpace" placeholder="请输入命名空间" />
								</el-form-item>
							</el-col>
						</el-row>
					</el-tab-pane>
					<el-tab-pane label="字段信息" name="columns">
						<el-table :data="state.columnData" style="width: 100%" border max-height="400">
							<el-table-column prop="columnName" label="字段名" width="150" />
							<el-table-column prop="propertyName" label="属性名" width="150" />
							<el-table-column prop="dataType" label="数据类型" width="120" />
							<el-table-column prop="netType" label=".NET类型" width="120" />
							<el-table-column prop="columnComment" label="字段描述" show-overflow-tooltip />
							<el-table-column label="操作" width="150" fixed="right">
								<template #default="scope">
									<el-button icon="ele-Edit" size="small" text type="primary" @click="editColumnConfig(scope.row)">配置</el-button>
								</template>
							</el-table-column>
						</el-table>
					</el-tab-pane>
					<el-tab-pane label="生成设置" name="generate">
						<el-row :gutter="35">
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="作者姓名" prop="authorName">
									<el-input v-model="state.ruleForm.authorName" placeholder="请输入作者姓名" />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="生成方式" prop="generateType">
									<el-select v-model="state.ruleForm.generateType" class="w100" placeholder="请选择生成方式">
										<el-option label="生成ZIP包" value="zip" />
										<el-option label="直接生成到项目" value="project" />
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="生成菜单" prop="generateMenu">
									<el-switch v-model="state.ruleForm.generateMenu" />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20" v-if="state.ruleForm.generateMenu">
								<el-form-item label="菜单图标" prop="menuIcon">
									<el-input v-model="state.ruleForm.menuIcon" placeholder="请输入菜单图标" />
								</el-form-item>
							</el-col>
						</el-row>
					</el-tab-pane>
				</el-tabs>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="cancel">取 消</el-button>
					<el-button type="primary" @click="submit">保 存</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="sysCodeConfig">
import { reactive, ref } from 'vue';
import { getCodeGenGettablecolumnlist, putCodeGenId } from '/@/api/fd-system-api/CodeGen';
import APIModel from '/@/api/fd-system-api';
import { ElMessage } from 'element-plus';

const emits = defineEmits(['handleQuery']);
const ruleFormRef = ref();

const state = reactive({
	isShowDialog: false,
	activeTab: 'basic',
	ruleForm: {} as APIModel.CodeGenConfigDto,
	columnData: [] as APIModel.ColumnInfoDto[],
});

// 打开弹窗
const openDialog = async (row: any) => {
	state.ruleForm = { ...row };
	// 获取列信息
	if (state.ruleForm.tableName) {
		const res = await getCodeGenGettablecolumnlist({ tableName: state.ruleForm.tableName });
		state.columnData = res || [];
	}
	state.isShowDialog = true;
};

// 编辑列配置
const editColumnConfig = (column: any) => {
	// 这里可以打开列配置的弹窗
	console.log('编辑列配置', column);
};

// 关闭弹窗
const closeDialog = () => {
	emits('handleQuery');
	state.isShowDialog = false;
};

// 取消
const cancel = () => {
	state.isShowDialog = false;
};

// 提交
const submit = async () => {
	try {
		// 调用API保存配置
		await putCodeGenId({ id: state.ruleForm.Id }, {
			TableName: state.ruleForm.tableName,
			EntityName: state.ruleForm.entityName,
			BusName: state.ruleForm.busName,
			NameSpace: state.ruleForm.nameSpace,
			AuthorName: state.ruleForm.authorName,
			GenerateType: state.ruleForm.generateType,
			GenerateMenu: state.ruleForm.generateMenu,
			MenuIcon: state.ruleForm.menuIcon
		} as APIModel.UpdateCodeGenDto);
		ElMessage.success('配置保存成功');
		closeDialog();
	} catch (error) {
		console.error('配置保存失败', error);
		ElMessage.error('配置保存失败');
	}
};

// 导出对象
defineExpose({ openDialog });
</script>

<style lang="scss" scoped>
:deep(.el-dialog__body) {
	min-height: 500px;
}
:deep(.el-tabs__content) {
	height: 400px;
	overflow-y: auto;
}
</style>