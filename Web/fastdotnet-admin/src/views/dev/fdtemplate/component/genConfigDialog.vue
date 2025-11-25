<template>
	<div class="sys-codeGenConfig-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="1500px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> 生成配置 </span>
				</div>
			</template>
			<el-table :data="state.tableData" style="width: 100%" v-loading="state.loading" border>
				<el-table-column type="index" label="序号" width="55" align="center" />
				<el-table-column prop="PropertyName" label="实体属性" show-overflow-tooltip />
				<el-table-column prop="ShowColumnName" label="显示名称" show-overflow-tooltip>
					<template #default="scope">
						<el-input v-model="scope.row.ShowColumnName" autocomplete="off" />
					</template>
				</el-table-column>
				<el-table-column prop="ColumnComment" label="字段描述" show-overflow-tooltip>
					<template #default="scope">
						<el-input v-model="scope.row.ColumnComment" autocomplete="off" />
					</template>
				</el-table-column>
				<el-table-column prop="NetType" label="数据类型" width="130" show-overflow-tooltip />
				<el-table-column prop="EffectType" label="作用类型" width="150" show-overflow-tooltip>
					<template #default="scope">
						<div class="effect-type-container">
							<el-select
								v-model="scope.row.EffectType"
								@change="effectTypeChange(scope.row, scope.$index)"
								:disabled="judgeColumns(scope.row)"
								class="m-2"
								placeholder="请选择作用类型"
							>
								<el-option label="文本框" value="Input" />
								<el-option label="文本域" value="Textarea" />
								<el-option label="数字输入框" value="NumberInput" />
								<el-option label="下拉选择框" value="Select" />
								<el-option label="单选框" value="Radio" />
								<el-option label="复选框" value="Checkbox" />
								<el-option label="日期选择器" value="DatePicker" />
								<el-option label="时间选择器" value="TimePicker" />
								<el-option label="开关" value="Switch" />
								<el-option label="字典选择器" value="DictSelector" />
								<el-option label="外键关联" value="ForeignKey" />
								<el-option label="树形选择器" value="ApiTreeSelector" />
							</el-select>
						</div>
					</template>
				</el-table-column>
				<el-table-column prop="DictTypeCode" label="字典编码" width="150" show-overflow-tooltip>
					<template #default="scope">
						<el-select v-model="scope.row.DictTypeCode" :disabled="effectTypeEnable(scope.row)" class="m-2" placeholder="请选择字典">
							<el-option v-for="item in state.dictList" :key="item.code" :label="item.name" :value="item.code" />
						</el-select>
					</template>
				</el-table-column>
				<el-table-column prop="EnableMask" label="脱敏" width="70" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-checkbox v-model="scope.row.EnableMask" @change="handleMaskChange(scope.row)"/>
					</template>
				</el-table-column>
				<el-table-column prop="WhetherTable" label="列表显示" width="70" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-checkbox v-model="scope.row.WhetherTable" true-value="是" false-value="否" />
					</template>
				</el-table-column>
				<el-table-column prop="WhetherAddUpdate" label="增改显示" width="70" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-checkbox v-model="scope.row.WhetherAddUpdate" true-value="是" false-value="否" />
					</template>
				</el-table-column>
				<el-table-column prop="WhetherImport" label="导入" width="70" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-checkbox v-model="scope.row.WhetherImport" true-value="是" false-value="否" />
					</template>
				</el-table-column>
				<el-table-column prop="WhetherRequired" label="必填" width="70" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-checkbox v-model="scope.row.WhetherRequired" true-value="是" false-value="否" :disabled="true" />
					</template>
				</el-table-column>
				<el-table-column prop="WhetherSortable" label="可排序" width="70" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-checkbox v-model="scope.row.WhetherSortable" true-value="是" false-value="否" />
					</template>
				</el-table-column>
				<el-table-column prop="WhetherQuery" label="查询" width="70" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-switch v-model="scope.row.WhetherQuery" active-value="是" inactive-value="否" />
					</template>
				</el-table-column>
				<el-table-column prop="QueryType" label="查询方式" width="110" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-select v-model="scope.row.QueryType" class="m-2" placeholder="Select" :disabled="!scope.row.WhetherQuery">
							<el-option label="=" value="eq" />
							<el-option label="!=" value="ne" />
							<el-option label=">" value="gt" />
							<el-option label="<" value="lt" />
							<el-option label=">=" value="gte" />
							<el-option label="<=" value="lte" />
							<el-option label="LIKE" value="contains" />
							<el-option label="BETWEEN" value="BETWEEN" />
							<el-option label="开始于xxx" value="startswith" />
							<el-option label="结束于xxx" value="endswith" />
						</el-select>
					</template>
				</el-table-column>
				<el-table-column prop="OrderNo" label="排序" width="80" show-overflow-tooltip>
					<template #default="scope">
						<el-input v-model="scope.row.OrderNo" autocomplete="off" type="number" />
					</template>
				</el-table-column>
			</el-table>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="cancel">取 消</el-button>
					<el-button type="primary" @click="submit">确 定</el-button>
				</span>
			</template>
		</el-dialog>
		
		<!-- 脱敏配置弹窗 -->
		<el-dialog v-model="state.maskConfigDialogVisible" title="脱敏配置" width="600px" append-to-body>
			<el-form :model="state.currentMaskConfig" label-width="120px">
				<el-form-item label="脱敏类型">
					<el-select v-model="state.currentMaskConfig.Type" placeholder="请选择脱敏类型">
						<el-option label="手机号" value="Phone" />
						<el-option label="邮箱" value="Email" />
						<el-option label="身份证" value="IdCard" />
						<el-option label="银行卡" value="BankCard" />
						<el-option label="姓名" value="Name" />
						<el-option label="自定义" value="Custom" />
					</el-select>
				</el-form-item>
				
				<template v-if="state.currentMaskConfig.Type && state.currentMaskConfig.Type !== 'Custom'">
					<el-form-item label="保留前缀字符数">
						<el-input-number v-model="state.currentMaskConfig.PrefixKeep" :min="0" controls-position="right" />
					</el-form-item>
					<el-form-item label="保留后缀字符数">
						<el-input-number v-model="state.currentMaskConfig.SuffixKeep" :min="0" controls-position="right" />
					</el-form-item>
					<el-form-item label="脱敏字符">
						<el-input v-model="state.currentMaskConfig.MaskChar" maxlength="1" />
					</el-form-item>
					<el-form-item label="脱敏字符长度">
						<el-input-number v-model="state.currentMaskConfig.MaskLength" :min="0" controls-position="right" />
					</el-form-item>
				</template>
				
				<template v-if="state.currentMaskConfig.Type === 'Custom'">
					<el-form-item label="自定义正则表达式">
						<el-input 
							v-model="state.currentMaskConfig.CustomPattern" 
							type="textarea" 
							@blur="validateRegex(state.currentMaskConfig.CustomPattern)"
							placeholder="(\d{6})\d+(\d{4})"
						/>
						<div v-if="state.customPatternError" class="form-error">{{ state.customPatternError }}</div>
					</el-form-item>
					<el-form-item label="替换字符串">
						<el-input v-model="state.currentMaskConfig.CustomReplacement" placeholder="$1****$2" />
					</el-form-item>
				</template>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="state.maskConfigDialogVisible = false">取 消</el-button>
					<el-button type="primary" @click="saveMaskConfig">确 定</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="sysCodeGenConfig">
import { onMounted, reactive, ref } from 'vue';
import {
	getCodeGenConfigPage,
	putCodeGenConfigId,
	postCodeGenConfig,
	postCodeGenConfigBatch,
	postCodeGenConfigPageSearch,
	putCodeGenConfigBatch,
} from '@/api/fd-system-api/CodeGenConfig';
import { buildMixedQuery } from '@/utils/queryBuilder';
import APIModel from '@/api/fd-system-api';
import { ElMessage } from 'element-plus';

const emits = defineEmits(['handleQuery']);
const state = reactive({
	isShowDialog: false,
	loading: false,
	dictList: [] as { code: string; name: string }[], // 字典列表
	tableData: [] as APIModel.FdCodeGenConfigDto[],
	originalData: [] as APIModel.FdCodeGenConfigDto[], // 原始数据备份
	configId: '', // 用于关联的CodeGenId
	
	// 脱敏配置相关
	maskConfigDialogVisible: false, // 脱敏配置弹窗显示状态
	currentEditingRow: null as APIModel.FdCodeGenConfigDto | null, // 当前正在编辑的行
	currentMaskConfig: {
		Type: '',
		PrefixKeep: undefined as number | undefined,
		SuffixKeep: undefined as number | undefined,
		MaskChar: '*',
		MaskLength: undefined as number | undefined,
		CustomPattern: '',
		CustomReplacement: ''
	}, // 当前脱敏配置
	customPatternError: '' // 自定义正则表达式错误信息
});

// 初始化时加载字典数据
onMounted(async () => {
	// 模拟加载字典数据，实际项目中应该从API获取
	state.dictList = [
		{ code: 'sex', name: '性别' },
		{ code: 'status', name: '状态' },
		{ code: 'type', name: '类型' },
	];
});

// 查询操作
const handleQuery = async (row: any) => {
	if (!row.Id) return;

	state.loading = true;
	state.configId = row.Id;

	try {
		// 使用混合查询构建器构建查询条件
		const queryConfig = {
			equals: {
				CodeGenId: row.Id, // 使用字符串类型的ID
			},
		};

		const queryResult = buildMixedQuery(queryConfig);

		// 使用搜索接口获取字段配置信息，按CodeGenId筛选
		const searchParams = {
			PageIndex: 1,
			PageSize: 1000, // 获取所有字段配置
			DynamicQuery: queryResult.dynamicQuery,
			QueryParameters: queryResult.queryParameters,
		} as APIModel.PageQueryByConditionDto;

		const configRes = await postCodeGenConfigPageSearch(searchParams);
		const existingConfigs = configRes?.Items || [];

		// 确保每行数据都有MaskConfig和EnableMask字段
		existingConfigs.forEach((item: any) => {
			if (!item.MaskConfig) {
				item.MaskConfig = {
					Type: '',
					PrefixKeep: undefined,
					SuffixKeep: undefined,
					MaskChar: '*',
					MaskLength: undefined,
					CustomPattern: '',
					CustomReplacement: ''
				};
			}
			if (item.EnableMask === undefined || item.EnableMask === null) {
				item.EnableMask = false;
			}
		});

		state.tableData = existingConfigs;
		state.originalData = JSON.parse(JSON.stringify(existingConfigs)); // 备份原始数据
	} catch (error) {
		console.error('加载字段配置失败', error);
		ElMessage.error('加载字段配置失败');
	} finally {
		state.loading = false;
	}
};

// 根据数据类型返回默认作用类型
function getDefaultEffectType(netType: string | undefined): string {
	if (!netType) return 'Input';

	const typeMap: { [key: string]: string } = {
		string: 'Input',
		int: 'NumberInput',
		long: 'NumberInput',
		decimal: 'NumberInput',
		double: 'NumberInput',
		date: 'DatePicker',
		datetime: 'DatePicker',
		bool: 'Switch',
		boolean: 'Switch',
	};

	return typeMap[netType.toLowerCase()] || 'Input';
}

// 控件类型改变
const effectTypeChange = (data: APIModel.FdCodeGenConfigDto, index: number) => {
	// 如果是特定类型，需要选择对应字典
	if (['Radio', 'Checkbox', 'DictSelector', 'ConstSelector', 'EnumSelector'].some((type) => data.EffectType === type)) {
		data.DictTypeCode = ''; // 需要重新选择字典
	}
};

// 判断是否（用于是否能选择或输入等）
function judgeColumns(data: APIModel.FdCodeGenConfigDto) {
	return data.WhetherCommon === '是' || data.ColumnKey === 'True';
}

function effectTypeEnable(data: APIModel.FdCodeGenConfigDto) {
	return !['Radio', 'Checkbox', 'DictSelector', 'ConstSelector', 'EnumSelector'].some((e: any) => e === data.EffectType);
}

// 验证正则表达式是否有效
function validateRegex(pattern: string): boolean {
	if (!pattern) return true; // 空字符串认为是有效的
	
	try {
		new RegExp(pattern);
		state.customPatternError = '';
		return true;
	} catch (e: any) {
		state.customPatternError = e.message || '无效的正则表达式';
		return false;
	}
}

// 打开脱敏配置弹窗
const handleMaskChange = (row: APIModel.FdCodeGenConfigDto) => {
	if (row.EnableMask) {
		state.currentEditingRow = row;
		
		// 初始化脱敏配置
		if (row.MaskConfig) {
			state.currentMaskConfig = {
				Type: row.MaskConfig.Type || '',
				PrefixKeep: row.MaskConfig.PrefixKeep,
				SuffixKeep: row.MaskConfig.SuffixKeep,
				MaskChar: row.MaskConfig.MaskChar || '*',
				MaskLength: row.MaskConfig.MaskLength,
				CustomPattern: row.MaskConfig.CustomPattern || '',
				CustomReplacement: row.MaskConfig.CustomReplacement || ''
			};
		} else {
			state.currentMaskConfig = {
				Type: '',
				PrefixKeep: undefined,
				SuffixKeep: undefined,
				MaskChar: '*',
				MaskLength: undefined,
				CustomPattern: '',
				CustomReplacement: '*'
			};
		}
		
		// 清除之前的错误信息
		state.customPatternError = '';
		
		state.maskConfigDialogVisible = true;
	}
};

// 保存脱敏配置
const saveMaskConfig = () => {
	if (state.currentEditingRow) {
		// 如果是自定义类型，验证正则表达式
		if (state.currentMaskConfig.Type === 'Custom') {
			if (!validateRegex(state.currentMaskConfig.CustomPattern)) {
				ElMessage.error('正则表达式格式不正确: ' + state.customPatternError);
				return;
			}
			
			// 如果自定义替换字符串为空，默认使用"*"
			// if (!state.currentMaskConfig.CustomReplacement) {
			// 	state.currentMaskConfig.CustomReplacement = '*';
			// }
		}
		
		// 如果没有选择脱敏类型，则不保存配置
		if (!state.currentMaskConfig.Type) {
			state.currentEditingRow.MaskConfig = undefined;
			state.currentEditingRow.EnableMask = false;
		} else {
			state.currentEditingRow.MaskConfig = { ...state.currentMaskConfig };
		}
	}
	state.maskConfigDialogVisible = false;
};

// 打开弹窗
const openDialog = (row: any) => {
	handleQuery(row);
	state.isShowDialog = true;
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
	state.loading = true;
	try {
		// 由于查询出来的都是数据库已有的记录，直接使用批量更新接口
		await putCodeGenConfigBatch(state.tableData as APIModel.UpdateFdCodeGenConfigDto[]);

		ElMessage.success('字段配置保存成功');
		closeDialog();
	} catch (error) {
		console.error('保存字段配置失败', error);
		ElMessage.error('保存字段配置失败');
	} finally {
		state.loading = false;
	}
};

// 导出对象
defineExpose({ openDialog });
</script>
<style scoped>
.effect-type-container {
	display: flex;
	align-items: center;
}

.form-error {
	color: #f56c6c;
	font-size: 12px;
	line-height: 1;
	padding-top: 4px;
}
</style>
