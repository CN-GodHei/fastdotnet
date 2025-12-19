<template>
	<div class="system-menu-dialog-container">
		<el-dialog :title="state.dialog.title" v-model="state.dialog.isShowDialog" width="769px">
			<el-form ref="menuDialogFormRef" :model="state.ruleForm" size="default" label-width="80px">
				<el-row :gutter="35">
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="菜单类型">
							<el-radio-group v-model="state.ruleForm.menuType" :disabled="state.dialog.type === 'edit'">
								<el-radio label="directory">目录</el-radio>
								<el-radio label="menu">菜单</el-radio>
								<el-radio label="btn">按钮</el-radio>
							</el-radio-group>
						</el-form-item>
					</el-col>
					<template v-if="state.ruleForm.menuType !== 'btn'">
						<!-- 目录和菜单需要上级菜单，按钮不需要 -->
						<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20" v-if="state.ruleForm.menuType !== 'directory'">
							<el-form-item label="上级菜单">
								<el-cascader
									:options="state.menuData"
									:props="{ checkStrictly: true, value: 'Code', label: 'Title', children: 'Children' }"
									placeholder="请选择上级菜单"
									clearable
									class="w100"
									v-model="state.ruleForm.menuSuperior"
								>
									<template #default="{ node, data }">
										<span>{{ data.Title || '未知菜单' }}</span>
										<span v-if="!node.isLeaf && data.Children && data.Children.length > 0"> ({{ data.Children.length }}) </span>
									</template>
								</el-cascader>
							</el-form-item>
						</el-col>
						<template v-if="state.ruleForm.menuType === 'menu'">
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="菜单名称">
									<el-input v-model="state.ruleForm.Title" placeholder="请输入菜单名称" clearable></el-input>
								</el-form-item>
							</el-col>
						</template>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20" v-if="state.ruleForm.menuType === 'menu'">
							<el-form-item label="组件名称">
								<el-input v-model="state.ruleForm.Name" placeholder="请输入菜单名称" clearable></el-input>
							</el-form-item>
						</el-col>

						<template v-if="state.ruleForm.menuType === 'directory'">
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="目录名称">
									<el-input v-model="state.ruleForm.Title" placeholder="请输入目录名称" clearable></el-input>
								</el-form-item>
							</el-col>
							<!-- 目录类型的特有字段 -->
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="菜单图标">
									<IconSelector placeholder="请输入菜单图标" v-model="state.ruleForm.Icon" />
								</el-form-item>
							</el-col>
						</template>
						<template v-if="state.ruleForm.menuType === 'menu'">
							<!-- 菜单类型的特有字段 -->
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="标识编码">
									<el-input v-model="state.ruleForm.Code" placeholder="标识编码不填则会自动生成" clearable></el-input>
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="路由路径">
									<el-input v-model="state.ruleForm.Path" placeholder="请输入路由路径" clearable></el-input>
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="重定向">
									<el-input v-model="state.ruleForm.Redirect" placeholder="请输入路由重定向" clearable></el-input>
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="菜单图标">
									<IconSelector placeholder="请输入菜单图标" v-model="state.ruleForm.Icon" />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="组件路径">
									<el-input v-model="state.ruleForm.Component" placeholder="请输入组件路径" clearable></el-input>
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="链接地址">
									<el-input
										v-model="state.ruleForm.ExternalUrl"
										placeholder="外链/内嵌时链接地址（http:xxx.com）"
										clearable
										:disabled="!state.ruleForm.isLink"
									>
									</el-input>
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="权限标识">
									<el-input v-model="state.ruleForm.PermissionCode" placeholder="请输入权限标识" clearable></el-input>
								</el-form-item>
							</el-col>
						</template>
					</template>
					<template v-if="state.ruleForm.menuType === 'btn'">
						<!-- 按钮类型特有字段 -->
						<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
							<el-form-item label="上级菜单">
								<el-cascader
									:options="state.menuData"
									:props="{ checkStrictly: true, value: 'Code', label: 'Title', children: 'Children' }"
									placeholder="请选择按钮关联的菜单"
									clearable
									class="w100"
									v-model="state.ruleForm.menuSuperior"
								>
									<template #default="{ node, data }">
										<span>{{ data.Title || '未知菜单' }}</span>
										<span v-if="!node.isLeaf && data.Children && data.Children.length > 0"> ({{ data.Children.length }}) </span>
									</template>
								</el-cascader>
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="按钮名称">
								<el-input v-model="state.ruleForm.Title" placeholder="请输入按钮名称" clearable></el-input>
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="权限标识">
								<el-input v-model="state.ruleForm.PermissionCode" placeholder="请输入权限标识" clearable></el-input>
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
							<el-form-item label="按钮编码">
								<el-input v-model="state.ruleForm.Code" placeholder="请输入按钮编码" :disabled="state.dialog.type === 'edit'" clearable></el-input>
							</el-form-item>
						</el-col>
					</template>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="菜单排序">
							<el-input-number v-model="state.ruleForm.Sort" controls-position="right" placeholder="请输入排序" class="w100" />
						</el-form-item>
					</el-col>
					<template v-if="state.ruleForm.menuType !== 'btn'">
						<!-- 目录和菜单类型的特有字段 -->
						<template v-if="state.ruleForm.menuType === 'menu'">
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="是否隐藏">
									<el-radio-group v-model="state.ruleForm.IsHide">
										<el-radio :label="true">隐藏</el-radio>
										<el-radio :label="false">不隐藏</el-radio>
									</el-radio-group>
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="页面缓存">
									<el-radio-group v-model="state.ruleForm.IsKeepAlive">
										<el-radio :label="true">缓存</el-radio>
										<el-radio :label="false">不缓存</el-radio>
									</el-radio-group>
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="是否固定">
									<el-radio-group v-model="state.ruleForm.IsAffix">
										<el-radio :label="true">固定</el-radio>
										<el-radio :label="false">不固定</el-radio>
									</el-radio-group>
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="是否外链">
									<el-radio-group v-model="state.ruleForm.isLink" :disabled="state.ruleForm.IsIframe">
										<el-radio :label="true">是</el-radio>
										<el-radio :label="false">否</el-radio>
									</el-radio-group>
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="是否内嵌">
									<el-radio-group v-model="state.ruleForm.IsIframe" @change="onSelectIframeChange">
										<el-radio :label="true">是</el-radio>
										<el-radio :label="false">否</el-radio>
									</el-radio-group>
								</el-form-item>
							</el-col>
						</template>
						<template v-else-if="state.ruleForm.menuType === 'directory'">
							<!-- 目录类型的特有字段 -->
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="是否隐藏">
									<el-radio-group v-model="state.ruleForm.IsHide">
										<el-radio :label="true">隐藏</el-radio>
										<el-radio :label="false">不隐藏</el-radio>
									</el-radio-group>
								</el-form-item>
							</el-col>
						</template>
					</template>
				</el-row>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="onCancel" size="default">取 消</el-button>
					<el-button type="primary" @click="onSubmit" size="default">{{ state.dialog.submitTxt }}</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script setup lang="ts" name="systemMenuDialog">
import { defineAsyncComponent, reactive, onMounted, ref } from 'vue';
import { storeToRefs } from 'pinia';
import { useRoutesList } from '@/stores/routesList';
import { i18n } from '@/i18n/index';
import * as MenuApi from '@/api/fd-system-api-admin/FdMenu';

import { ElMessage } from 'element-plus';
// import { setBackEndControlRefreshRoutes } from "@/router/backEnd";

// 定义子组件向父组件传值/事件
const emit = defineEmits(['refresh']);

// 引入组件
const IconSelector = defineAsyncComponent(() => import('@/components/iconSelector/index.vue'));

// 定义变量内容
const menuDialogFormRef = ref();
const stores = useRoutesList();
const { routesList } = storeToRefs(stores);
const state = reactive({
	// 参数请参考 `/src/router/route.ts` 中的 `dynamicRoutes` 路由菜单格式
	ruleForm: {
		menuSuperior: [], // 上级菜单
		menuType: 'menu', // 菜单类型
		Id: '', // 菜单ID
		Name: '', // 菜单名称（用于组件name属性）
		Title: '', // 显示标题（用于菜单显示名称）
		Code: '', // 路由名称
		Component: '', // 组件路径
		Icon: '', // 菜单图标
		IsHide: false, // 是否隐藏
		IsKeepAlive: true, // 是否缓存
		IsAffix: false, // 是否固定
		IsIframe: false, // 是否内嵌
		Path: '', // 路由路径
		Redirect: '', // 路由重定向，有子集 Children 时
		Sort: 0, // 菜单排序
		PermissionCode: '', // 权限标识
		ParentCode: '', // 上级菜单编码
		Type: 0, // 菜单类型 (0: 目录, 1: 菜单)
		IsExternal: false, // 是否外部链接
		ExternalUrl: '', // 外部链接地址
		IsEnabled: true, // 是否启用
		Module: '', // 模块
		Category: '', // 分类
		IsFdMicroApp: false, // 是否微应用
		Children: [] as APIModel.FdMenuDto[], // 子菜单
	},
	menuData: [] as APIModel.FdMenuDto[], // 上级菜单数据
	dialog: {
		isShowDialog: false,
		type: '',
		title: '',
		submitTxt: '',
	},
});

// 获取菜单数据
// const getMenuData = (routes: APIModel.FdMenuDto[]) => {
// 	const arr: APIModel.FdMenuDto[] = [];
// 	routes.map((val: APIModel.FdMenuDto) => {
// 		// 创建一个新的对象，不修改原始数据
// 		const newItem = { ...val };

// 		// 确保Name字段有值，如果没有则使用Code或Id
// 		let displayName = '未知菜单';
// 		if (val.Name && val.Name.trim() !== '') {
// 			try {
// 				displayName = i18n.global.t(val.Name);
// 			} catch (e) {
// 				displayName = val.Name;
// 			}
// 		} else if (val.Id && val.Id.trim() !== '') {
// 			displayName = val.Id;
// 		}

// 		newItem['Name'] = displayName;

// 		// 递归处理子菜单
// 		if (val.Children && Array.isArray(val.Children)) {
// 			newItem.Children = getMenuData(val.Children);
// 		}

// 		arr.push(newItem);
// 	});
// 	return arr;
// };
// 打开弹窗
const openDialog = (type: string, row?: APIModel.FdMenuDto) => {
	if (type === 'edit') {
		// 编辑模式：需要根据MenuType判断是目录、菜单或按钮
		state.ruleForm = JSON.parse(JSON.stringify(row));

		if (row) {
			// 根据实际MenuType判断
			if (row.Type === 0) {
				// Directory (后端的MenuType.Directory)
				state.ruleForm.menuType = 'directory';
				state.dialog.title = '修改目录';
			} else {
				// Menu (后端的MenuType.Menu)
				state.ruleForm.menuType = 'menu';
				state.dialog.title = '修改菜单';
			}
		}

		// 设置上级菜单的值
		if (row?.ParentCode) {
			state.ruleForm.menuSuperior = [row.ParentCode];
		} else {
			state.ruleForm.menuSuperior = [];
		}
		state.dialog.submitTxt = '修 改';
	} else {
		// 新增模式
		// 重置整个表单数据
		state.ruleForm = {
			menuSuperior: [], // 上级菜单
			menuType: 'menu', // 菜单类型
			Id: '', // 菜单ID
			Name: '', // 菜单名称（用于组件name属性）
			Title: '', // 显示标题（用于菜单显示名称）
			Code: '', // 路由名称
			Component: '', // 组件路径
			Icon: '', // 菜单图标
			IsHide: false, // 是否隐藏
			IsKeepAlive: true, // 是否缓存
			IsAffix: false, // 是否固定
			IsIframe: false, // 是否内嵌
			Path: '', // 路由路径
			Redirect: '', // 路由重定向，有子集 Children 时
			Sort: 0, // 菜单排序
			PermissionCode: '', // 权限标识
			ParentCode: '', // 上级菜单编码
			Type: 0, // 菜单类型 (0: 目录, 1: 菜单)
			IsExternal: false, // 是否外部链接
			ExternalUrl: '', // 外部链接地址
			IsEnabled: true, // 是否启用
			Module: '', // 模块
			Category: '', // 分类
			IsFdMicroApp: false, // 是否微应用
			Children: [] as APIModel.FdMenuDto[], // 子菜单
		};
		state.dialog.title = '新增目录/菜单/按钮';
		state.dialog.submitTxt = '新 增';
	}
	state.dialog.type = type;
	state.dialog.isShowDialog = true;
};
// 关闭弹窗
const closeDialog = () => {
	state.dialog.isShowDialog = false;
};
// 是否内嵌下拉改变
const onSelectIframeChange = () => {
	if (state.ruleForm.IsIframe) state.ruleForm.isLink = true;
	else state.ruleForm.isLink = false;
};
// 取消
const onCancel = () => {
	closeDialog();
};
// 提交
const onSubmit = async () => {
	// 根据menuType设置Type字段
	// 如果是按钮类型，使用按钮API处理
	if (state.ruleForm.menuType === 'btn') {
		// 按钮类型的处理 - 按钮与菜单关联
		try {
			// 验证菜单关联
			let menuCode = '';
			if (state.ruleForm.menuSuperior && state.ruleForm.menuSuperior.length > 0) {
				menuCode = state.ruleForm.menuSuperior[state.ruleForm.menuSuperior.length - 1];
			} else if (state.ruleForm.ParentCode) {
				menuCode = state.ruleForm.ParentCode;
			}

			if (!menuCode) {
				ElMessage.error('请选择按钮关联的菜单');
				return; // 不执行后续操作
			}

			if (state.dialog.type === 'edit') {
				// 编辑按钮
				if (state.ruleForm.Id) {
					// 准备更新数据
					const updateData: APIModel.UpdateFdFdMenuButtonDto = {
						Name: state.ruleForm.Name,
						Code: state.ruleForm.Code, // 按钮的权限码作为Code
						Description: state.ruleForm.Name, // 描述使用名称
						MenuCode: menuCode, // 关联的菜单Code
						Module: state.ruleForm.Module || 'System',
						Category: 'Admin',
						Sort: state.ruleForm.Sort,
						IsEnabled: state.ruleForm.IsEnabled ?? true,
						PermissionCode: state.ruleForm.PermissionCode, // 权限代码
					};

					await import('@/api/fd-system-api-admin/FdMenuButtons').then((MenuButtonsApi) => {
						MenuButtonsApi.putAdminFdMenuButtonsId({ id: state.ruleForm.Id }, updateData);
					});

					ElMessage.success('按钮更新成功');
				} else {
					ElMessage.error('按钮ID不存在，无法更新');
				}
			} else {
				// 新增按钮
				// 准备创建数据
				const createData: APIModel.CreateFdFdMenuButtonDto = {
					Name: state.ruleForm.Name,
					Code: state.ruleForm.Code, // 按钮的权限码
					Description: state.ruleForm.Name, // 描述使用名称
					MenuCode: menuCode, // 关联的菜单Code
					Module: state.ruleForm.Module || 'System',
					Category: 'Admin',
					Sort: state.ruleForm.Sort,
					IsEnabled: state.ruleForm.IsEnabled ?? true,
					PermissionCode: state.ruleForm.PermissionCode, // 权限代码
				};

				await import('@/api/fd-system-api-admin/FdMenuButtons').then((MenuButtonsApi) => {
					MenuButtonsApi.postAdminFdMenuButtons(createData);
				});

				// ElMessage.success('按钮创建成功');
			}

			closeDialog(); // 关闭弹窗
			emit('refresh');
		} catch (error) {
			ElMessage.error(state.dialog.type === 'edit' ? '按钮更新失败' : '按钮创建失败');
		}
	} else {
		// 菜单类型的处理
		try {
			// 如果Title为空，则使用Name作为Title
			if (!state.ruleForm.Title) {
				state.ruleForm.Title = state.ruleForm.Name;
			}

			if (state.dialog.type === 'edit') {
				// 编辑菜单/目录
				if (state.ruleForm.Id) {
					// 准备更新数据
					const updateData: APIModel.UpdateFdMenuDto = {
						...state.ruleForm,
						Type: state.ruleForm.menuType === 'menu' ? 1 : 0, // 1=Menu, 0=Directory
						ParentCode:
							state.ruleForm.menuSuperior && state.ruleForm.menuSuperior.length > 0
								? state.ruleForm.menuSuperior[state.ruleForm.menuSuperior.length - 1] // 使用最后一级菜单
								: null,
					};

					await MenuApi.putAdminFdMenuId({ id: state.ruleForm.Id }, updateData);

					ElMessage.success((state.ruleForm.menuType === 'directory' ? '目录' : '菜单') + '更新成功');
				} else {
					ElMessage.error('菜单ID不存在，无法更新');
				}
			} else {
				// 新增菜单/目录
				// 准备创建数据
				const createData: APIModel.CreateFdMenuDto = {
					...state.ruleForm,
					Type: state.ruleForm.menuType === 'menu' ? 1 : 0, // 1=Menu, 0=Directory
					ParentCode:
						state.ruleForm.menuSuperior && state.ruleForm.menuSuperior.length > 0
							? state.ruleForm.menuSuperior[state.ruleForm.menuSuperior.length - 1] // 使用最后一级菜单
							: null,
				};
				//console.log(11);

				createData.Category = 'Admin';
				await MenuApi.postAdminFdMenu(createData);
				ElMessage.success((state.ruleForm.menuType === 'directory' ? '目录' : '菜单') + '创建成功');
			}

			closeDialog(); // 关闭弹窗
			emit('refresh');
		} catch (error) {
			ElMessage.error(
				state.dialog.type === 'edit'
					? (state.ruleForm.menuType === 'directory' ? '目录' : '菜单') + '更新失败'
					: (state.ruleForm.menuType === 'directory' ? '目录' : '菜单') + '创建失败'
			);
		}
	}
};
// 页面加载时
onMounted(async () => {
	// 从API获取菜单数据而不是使用store中的数据
	const res = await MenuApi.getAdminFdMenuTree();
	state.menuData = res || [];
});

// 暴露变量
defineExpose({
	openDialog,
});
</script>