<template>
	<div class="sys-editCodeGen-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="700px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> {{ props.title }} </span>
				</div>
			</template>
			<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="auto">
				<el-row :gutter="35">
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="生成表" prop="tableName" :rules="[{ required: true, message: '生成表不能为空', trigger: 'blur' }]">
							<el-select v-model="state.ruleForm.tableName" @change="tableChanged" filterable clearable class="w100">
								<el-option v-for="item in state.tableData" :key="item.TableName" :label="item.TableComment + ' [' + item.TableName + ']'" :value="item.TableName" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="生成菜单" prop="generateMenu">
							<el-radio-group v-model="state.ruleForm.generateMenu">
								<el-radio :value="true">是</el-radio>
								<el-radio :value="false">否</el-radio>
							</el-radio-group>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="菜单图标" prop="menuIcon">
							<el-input v-model="state.ruleForm.menuIcon" placeholder="菜单图标" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="命名空间" prop="nameSpace" :rules="[{ required: true, message: '请选择命名空间', trigger: 'blur' }]">
							<el-select v-model="state.ruleForm.nameSpace" filterable clearable class="w100" placeholder="命名空间">
								<el-option v-for="(item, index) in props.applicationNamespaces" :key="index" :label="item" :value="item" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="前端目录" prop="pagePath" :rules="[{ required: true, message: '前端目录不能为空', trigger: 'blur' }]">
							<el-input v-model="state.ruleForm.pagePath" clearable placeholder="请输入" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="作者姓名" prop="authorName">
							<el-input v-model="state.ruleForm.authorName" clearable placeholder="请输入" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="生成方式" prop="generateType">
              <el-select v-model="state.ruleForm.generateType" class="w100" filterable placeholder="生成方式">
								<el-option label="生成ZIP包" value="zip" />
								<el-option label="直接生成到项目" value="project" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-divider border-style="dashed" content-position="center">
						<div style="color: #b1b3b8">数据唯一性配置</div>
					</el-divider>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-button icon="ele-Plus" type="primary" plain @click="() => state.ruleForm.tableUniqueList?.push({})"> 增加配置 </el-button>
						<span style="font-size: 12px; color: gray; padding-left: 5px"> 保证字段值的唯一性，排除null值 </span>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<template v-if="state.ruleForm.tableUniqueList != undefined && state.ruleForm.tableUniqueList.length > 0">
							<el-row :gutter="35" v-for="(v, k) in state.ruleForm.tableUniqueList" :key="k">
								<el-col :xs="24" :sm="14" :md="14" :lg="14" :xl="14" class="mb20">
									<el-form-item label="字段" :prop="`tableUniqueList[${k}].columns`" :rules="[{ required: true, message: '字段不能为空', trigger: 'blur' }]">
										<template #label>
												<el-button icon="ele-Delete" type="danger" circle plain size="small" @click="() => state.ruleForm.tableUniqueList?.splice(k, 1)" />
												<span class="ml5">字段</span>
										</template>
										<el-select v-model="state.ruleForm.tableUniqueList[k].columns" @change="(val: any) => changeTableUniqueColumn(val, k)" multiple filterable clearable collapse-tags collapse-tags-tooltip class="w100">
											<el-option v-for="item in state.columnData" :key="item.propertyName" :label="item.propertyName + ' [' + item.columnComment + ']'" :value="item.propertyName" />
										</el-select>
									</el-form-item>
								</el-col>
								<el-col :xs="24" :sm="10" :md="10" :lg="10" :xl="10" class="mb20">
									<el-form-item label="描述信息" :prop="`tableUniqueList[${k}].message`" :rules="[{ required: true, message: '描述信息不能为空', trigger: 'blur' }]">
										<el-input v-model="state.ruleForm.tableUniqueList[k].message" clearable placeholder="请输入" />
									</el-form-item>
								</el-col>
							</el-row>
						</template>
					</el-col>
				</el-row>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="cancel">取 消</el-button>
					<el-button type="primary" @click="submit">确 定</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="sysEditCodeGen">
import { computed, onMounted, reactive, ref } from 'vue';
import { 
  getCodeGenGettablelist, 
  getCodeGenGettablecolumnlist, 
  getCodeGenGetentityname,
  getCodeGenTablelistConfigId, 
  postCodeGen, 
  putCodeGenId 
} from '/@/api/fd-system-api/CodeGen';
import APIModel from '/@/api/fd-system-api';
import { ElMessage } from 'element-plus';

const props = defineProps({
	title: String,
	applicationNamespaces: Array<String>,
});
const emits = defineEmits(['handleQuery']);
const ruleFormRef = ref();

const state = reactive({
	isShowDialog: false,
	ruleForm: {} as APIModel.CreateCodeGenConfigDto & APIModel.UpdateCodeGenConfigDto,
	tableData: [] as APIModel.TableInfoDto[],
	columnData: [] as APIModel.ColumnInfoDto[],
	isInitializing: false, // 标识是否正在初始化，避免初始化时重置 tableUniqueList
});


// table改变
const tableChanged = (tableName: string) => {
	if(tableName) {
		// 查找对应的表信息
		const tableInfo = state.tableData.find(t => t.TableName === tableName);
		if(tableInfo) {
			state.ruleForm.tableName = tableInfo.TableName;
			// 只有在非初始化状态下才重置 tableUniqueList，避免在初始化时覆盖编辑的数据
			if (!state.isInitializing) {
				state.ruleForm.tableUniqueList = [];
			}
			getColumnInfoList(tableInfo.TableName);
		}
	}
};

// 表唯一约束配置项字段改变事件
const changeTableUniqueColumn = (value: any, index: number) => {
  if (value?.length === 1 && !state.ruleForm.tableUniqueList[index]?.message) {
    const column = state.columnData.find((u: any) => u.propertyName === value[0]);
		if (column) {
			state.ruleForm.tableUniqueList[index].message = column.columnComment;
		}
  }
}

const getColumnInfoList = async (tableName: string) => {
	if (!tableName) return;
	try {
		const res = await getCodeGenGettablecolumnlist({ tableName });
		state.columnData = res || [];
	} catch (error) {
		console.error('获取列信息失败', error);
		state.columnData = [];
	}
};

// 打开弹窗
const openDialog = async (row: APIModel.CreateCodeGenConfigDto & APIModel.UpdateCodeGenConfigDto) => {
	// 设置初始化标志
	state.isInitializing = true;
	state.ruleForm = JSON.parse(JSON.stringify(row));
	// 确保 tableUniqueList 是数组格式，即使原数据为 null
	state.ruleForm.tableUniqueList = state.ruleForm.tableUniqueList || [];
	try {
		// 获取表列表
		const res = await getCodeGenGettablelist();
		state.tableData = res || [];
		// 如果有已选择的表，获取列信息
		if (state.ruleForm.tableName) {
			getColumnInfoList(state.ruleForm.tableName);
		}
	} catch (error) {
		console.error('获取表列表失败', error);
		state.tableData = [];
	}
	// 重置初始化标志
	state.isInitializing = false;
	state.isShowDialog = true;
	// 移除 resetFields 调用，因为它会清除我们刚刚设置的值
	// ruleFormRef.value?.resetFields();
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
const submit = () => {
	ruleFormRef.value.validate(async (valid: boolean) => {
		if (!valid) return;
    if (state.ruleForm.tableUniqueList?.length === 0) state.ruleForm.tableUniqueList = null;

		try {
			if (state.ruleForm.id && state.ruleForm.id !== '') {
				// 更新操作
				await putCodeGenId({ id: state.ruleForm.id }, state.ruleForm as APIModel.UpdateCodeGenConfigDto);
				ElMessage.success('更新成功');
			} else {
				// 添加操作
				await postCodeGen(state.ruleForm as APIModel.CreateCodeGenConfigDto);
				ElMessage.success('添加成功');
			}
			closeDialog();
		} catch (error) {
			console.error('保存失败', error);
			ElMessage.error('保存失败');
		}
	});
};

// 导出对象
defineExpose({ openDialog });
</script>

<style lang="scss" scoped>
:deep(.el-dialog__body) {
	min-height: 450px;
}
</style>