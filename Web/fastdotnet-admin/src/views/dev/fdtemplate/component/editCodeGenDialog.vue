<template>
	<div class="sys-editCodeGen-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="700px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> {{ state.dialogTitle }} </span>
				</div>
			</template>
			<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="auto">
				<el-row :gutter="35">
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="生成表" prop="TableName" :rules="[{ required: true, message: '生成表不能为空', trigger: 'blur' }]">
							<el-select v-model="state.ruleForm.TableName" @change="tableChanged" filterable clearable class="w100">
								<el-option v-for="item in state.tableData" :key="item.TableName" :label="item.TableComment + ' [' + item.TableName + ']'" :value="item.TableName" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="生成菜单" prop="GenerateMenu">
							<el-radio-group v-model="state.ruleForm.GenerateMenu">
								<el-radio :value="true">是</el-radio>
								<el-radio :value="false">否</el-radio>
							</el-radio-group>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="菜单图标" prop="MenuIcon">
							<el-input v-model="state.ruleForm.MenuIcon" placeholder="菜单图标" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="命名空间" prop="NameSpace" :rules="[{ required: true, message: '请选择命名空间', trigger: 'blur' }]">
							<el-select v-model="state.ruleForm.NameSpace" filterable clearable class="w100" placeholder="命名空间">
								<el-option v-for="(item, index) in props.applicationNamespaces" :key="index" :label="item" :value="item" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="前端目录" prop="PagePath" :rules="[{ required: true, message: '前端目录不能为空', trigger: 'blur' }]">
							<el-input v-model="state.ruleForm.PagePath" clearable placeholder="请输入" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="作者姓名" prop="AuthorName">
							<el-input v-model="state.ruleForm.AuthorName" clearable placeholder="请输入" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="生成方式" prop="GenerateType">
              <el-select v-model="state.ruleForm.GenerateType" class="w100" filterable placeholder="生成方式">
								<el-option label="生成ZIP包" value="zip" />
								<el-option label="直接生成到项目" value="project" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-divider border-style="dashed" content-position="center">
						<div style="color: #b1b3b8">数据唯一性配置</div>
					</el-divider>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-button icon="ele-Plus" type="primary" plain @click="() => state.ruleForm.TableUniqueList?.push({ Columns: [], Message: '' })"> 增加配置 </el-button>
						<span style="font-size: 12px; color: gray; padding-left: 5px"> 保证字段值的唯一性，排除null值 </span>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<template v-if="state.ruleForm.TableUniqueList != undefined && state.ruleForm.TableUniqueList.length > 0">
							<el-row :gutter="35" v-for="(v, k) in state.ruleForm.TableUniqueList" :key="k">
								<el-col :xs="24" :sm="14" :md="14" :lg="14" :xl="14" class="mb20">
									<el-form-item label="字段" :prop="`TableUniqueList[${k}].Columns`" :rules="[{ required: true, message: '字段不能为空', trigger: 'blur' }]">
										<template #label>
												<el-button icon="ele-Delete" type="danger" circle plain size="small" @click="() => state.ruleForm.TableUniqueList?.splice(k, 1)" />
												<span class="ml5">字段</span>
										</template>
										<el-select v-model="state.ruleForm.TableUniqueList[k].Columns" @change="(val: any) => changeTableUniqueColumn(val, k)" multiple filterable clearable collapse-tags collapse-tags-tooltip class="w100">
											<el-option v-for="item in state.columnData" :key="item.PropertyName" :label="item.PropertyName + ' [' + item.ColumnComment + ']'" :value="item.PropertyName" />
										</el-select>
									</el-form-item>
								</el-col>
								<el-col :xs="24" :sm="10" :md="10" :lg="10" :xl="10" class="mb20">
									<el-form-item label="描述信息" :prop="`TableUniqueList[${k}].Message`" :rules="[{ required: true, message: '描述信息不能为空', trigger: 'blur' }]">
										<el-input v-model="state.ruleForm.TableUniqueList[k].Message" clearable placeholder="请输入" />
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
import { computed, onMounted, reactive, ref, nextTick } from 'vue';
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
	dialogTitle: '',
	ruleForm: {} as APIModel.CreateCodeGenDto & APIModel.UpdateCodeGenDto,
	tableData: [] as APIModel.TableInfoDto[],
	columnData: [] as APIModel.ColumnInfoDto[],
	isInitializing: false, // 标识是否正在初始化，避免初始化时重置 TableUniqueList
});


// table改变
const tableChanged = (tableName: string) => {
	// 只有在非初始化状态下才处理表名改变，避免在初始化时触发重置
	if (!state.isInitializing) {
		if(tableName) {
			// 查找对应的表信息
			const tableInfo = state.tableData.find(t => t.TableName === tableName);
			if(tableInfo) {
				state.ruleForm.TableName = tableInfo.TableName;
				state.ruleForm.TableComment = tableInfo.TableComment;
				// 只有在新增模式下才重置 TableUniqueList，避免在编辑时覆盖已有的数据
				if (!state.ruleForm.Id) {
					state.ruleForm.TableUniqueList = [];
				}
				getColumnInfoList(tableInfo.TableName);
			}
		} else {
			// 如果表名被清空，则也清空列数据
			state.columnData = [];
		}
	}
};

// 表唯一约束配置项字段改变事件
const changeTableUniqueColumn = (value: any, index: number) => {
  if (value?.length === 1 && !state.ruleForm.TableUniqueList[index]?.Message) {
    const column = state.columnData.find((u: any) => u.PropertyName === value[0]);
		if (column) {
			state.ruleForm.TableUniqueList[index].Message = column.ColumnComment;
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
const openDialog = async (row: APIModel.CreateCodeGenDto & APIModel.UpdateCodeGenDto) => {
	// 设置初始化标志
	state.isInitializing = true;
	
	// 保存原始的TableName等值，因为获取表列表后需要在DOM渲染后设置
	// 注意：API数据使用PascalCase命名法
	const originalTableName = row.TableName;
	const originalAuthorName = row.AuthorName;
	const originalGenerateType = row.GenerateType;
	const originalMenuIcon = row.MenuIcon;
	const originalPagePath = row.PagePath;
	const originalNameSpace = row.NameSpace;
	const originalGenerateMenu = row.GenerateMenu;
	const originalMenuPid = row.MenuPid;
	const originalPrintType = row.PrintType;
	const originalPrintName = row.PrintName;
	const originalTableComment = row.TableComment;
	
	// 复制数据到表单，但暂时不设置tableName（防止在表列表加载前触发tableChanged）
	const rowData = JSON.parse(JSON.stringify(row));
	
	// 为编辑模式确保所有必需字段都有适当的值（仅在原值为undefined时使用默认值，保留null值）
	// 使用与API一致的PascalCase格式
	state.ruleForm = {
		// 先设置除TableName外的其他值
		AuthorName: originalAuthorName !== undefined ? originalAuthorName : 'Developer',
		GenerateType: originalGenerateType !== undefined ? originalGenerateType : 'zip',
		MenuIcon: originalMenuIcon !== undefined ? originalMenuIcon : 'ele-Menu',
		PagePath: originalPagePath !== undefined ? originalPagePath : 'main',
		NameSpace: originalNameSpace !== undefined ? originalNameSpace : 'Fastdotnet.Service',
		GenerateMenu: originalGenerateMenu !== undefined ? originalGenerateMenu : false,
		// 暂时设置为null，等表列表加载后单独设置
		TableName: null,
		EntityName: rowData.EntityName,
		Id: rowData.Id,
		MenuPid: originalMenuPid,
		TableUniqueList: rowData.TableUniqueList || [],
		// 添加其他可能的字段
		PrintType: originalPrintType,
		PrintName: originalPrintName,
		TableComment: originalTableComment,
		// 展开其他可能存在的字段
	};
	
	// 确保 TableUniqueList 是数组格式，即使原数据为 null
	if (!state.ruleForm.TableUniqueList) {
		state.ruleForm.TableUniqueList = [];
	}
	
	// 根据是否有Id来判断是新增还是编辑
	state.dialogTitle = state.ruleForm.Id ? '编辑' : '增加';
	
	try {
		// 获取表列表
		const res = await getCodeGenGettablelist();
		state.tableData = res || [];
		
		// 等待界面更新
		await nextTick();
		
		// 在表列表加载完成后，设置表名但不触发tableChanged
		if (originalTableName) {
			// 检查表名是否存在于表列表中（不区分大小写）
			const tableInList = state.tableData.find(table => 
				table.TableName.toLowerCase() === originalTableName.toLowerCase()
			);
			
			if (tableInList) {
				// 使用列表中实际的表名格式，以确保el-select能正确匹配
				state.ruleForm.TableName = tableInList.TableName;
			} else {
				// 如果在列表中没找到，仍然设置原始值（尽管可能无法正确显示）
				state.ruleForm.TableName = originalTableName;
			}
		}
		
		// 如果有已选择的表，在界面更新后设置列信息
		if (state.ruleForm.TableName) {
			await getColumnInfoList(state.ruleForm.TableName);
		}
		
		// 再次等待DOM更新确保组件完全渲染
		await nextTick();
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
    if (state.ruleForm.TableUniqueList?.length === 0) state.ruleForm.TableUniqueList = null;

		try {
			if (state.ruleForm.Id && state.ruleForm.Id !== '') {
				// 更新操作
				await putCodeGenId({ id: state.ruleForm.Id }, state.ruleForm as APIModel.UpdateCodeGenDto);
				ElMessage.success('更新成功');
			} else {
				// 添加操作
				await postCodeGen(state.ruleForm as APIModel.CreateCodeGenDto);
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