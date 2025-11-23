import { RouteRecordRaw } from 'vue-router';
import { storeToRefs } from 'pinia';
import pinia from '@/stores/index';
import { useUserInfo } from '@/stores/userInfo';
import { useRequestOldRoutes } from '@/stores/requestOldRoutes';
import { Session } from '@/utils/storage';
import { NextLoading } from '@/utils/loading';
import { dynamicRoutes, notFoundAndNoPower } from '@/router/route';
import { formatTwoStageRoutes, formatFlatteningRoutes, router } from '@/router/index';
import { useRoutesList } from '@/stores/routesList';
import { useTagsViewRoutes } from '@/stores/tagsViewRoutes';
import { useMenuApi } from '@/api/menu/index';

// 后端控制路由

// 引入 api 请求接口
const menuApi = useMenuApi();

/**
 * 获取目录下的 .vue、.tsx 全部文件
 * @method import.meta.glob
 * @link 参考：https://cn.vitejs.dev/guide/features.html#json
 */
const layouModules: any = import.meta.glob('../layout/routerView/*.{vue,tsx}');
const viewsModules: any = import.meta.glob('../views/**/*.{vue,tsx}');
const dynamicViewsModules: Record<string, Function> = Object.assign({}, { ...layouModules }, { ...viewsModules });

import { startQiankun } from '@/main';

/**
 * 后端控制路由：初始化方法，防止刷新时路由丢失
 * @method NextLoading 界面 loading 动画开始执行
 * @method useUserInfo().setUserInfos() 触发初始化用户信息 pinia
 * @method useRequestOldRoutes().setRequestOldRoutes() 存储接口原始路由（未处理component），根据需求选择使用
 * @method setAddRoute 添加动态路由
 * @method setFilterMenuAndCacheTagsViewRoutes 设置路由到 pinia routesList 中（已处理成多级嵌套路由）及缓存多级嵌套数组处理后的一维数组
 */
export async function initBackEndControlRoutes() {
	// 界面 loading 动画开始执行
	if (window.nextLoading === undefined) NextLoading.start();
	// 无 token 停止执行下一步
	if (!Session.get('token')) return false;
	// 触发初始化用户信息 pinia
	// https://gitee.com/lyt-top/vue-next-admin/issues/I5F1HP
	await useUserInfo().setUserInfos();
	// 获取路由菜单数据
	const res = await getBackEndControlRoutes();
	// 无登录权限时，添加判断
	// https://gitee.com/lyt-top/vue-next-admin/issues/I64HVO
	if (res.data.length <= 0) return Promise.resolve(true);
	// 存储接口原始路由（未处理component），根据需求选择使用
	useRequestOldRoutes().setRequestOldRoutes(JSON.parse(JSON.stringify(res.data)));
	// 处理路由（component），替换 dynamicRoutes（@/router/route）第一个顶级 children 的路由
	dynamicRoutes[0].children = await backEndComponent(res.data);
	// 添加动态路由
	await setAddRoute();
	// 设置路由到 pinia routesList 中（已处理成多级嵌套路由）及缓存多级嵌套数组处理后的一维数组
	setFilterMenuAndCacheTagsViewRoutes();
}

/**
 * 设置路由到 pinia routesList 中（已处理成多级嵌套路由）及缓存多级嵌套数组处理后的一维数组
 * @description 用于左侧菜单、横向菜单的显示
 * @description 用于 tagsView、菜单搜索中：未过滤隐藏的(isHide)
 */
export async function setFilterMenuAndCacheTagsViewRoutes() {
	const storesRoutesList = useRoutesList(pinia);
	storesRoutesList.setRoutesList(dynamicRoutes[0].children as any);
	setCacheTagsViewRoutes();
}

/**
 * 缓存多级嵌套数组处理后的一维数组
 * @description 用于 tagsView、菜单搜索中：未过滤隐藏的(isHide)
 */
export function setCacheTagsViewRoutes() {
	const storesTagsView = useTagsViewRoutes(pinia);
	storesTagsView.setTagsViewRoutes(formatTwoStageRoutes(formatFlatteningRoutes(dynamicRoutes))[0].children);
}

/**
 * 处理路由格式及添加捕获所有路由或 404 Not found 路由
 * @description 替换 dynamicRoutes（@/router/route）第一个顶级 children 的路由
 * @returns 返回替换后的路由数组
 */
export function setFilterRouteEnd() {
	let filterRouteEnd: any = formatTwoStageRoutes(formatFlatteningRoutes(dynamicRoutes));
	// notFoundAndNoPower 防止 404、401 不在 layout 布局中，不设置的话，404、401 界面将全屏显示
	// 关联问题 No match found for location with path 'xxx'
	filterRouteEnd[0].children = [...filterRouteEnd[0].children, ...notFoundAndNoPower];
	return filterRouteEnd;
}

/**
 * 添加动态路由
 * @method router.addRoute
 * @description 此处循环为 dynamicRoutes（@/router/route）第一个顶级 children 的路由一维数组，非多级嵌套
 * @link 参考：https://next.router.vuejs.org/zh/api/#addroute
 */
export async function setAddRoute() {
	await setFilterRouteEnd().forEach((route: RouteRecordRaw) => {
		router.addRoute(route);
	});
}

/**
 * 获取后端动态路由菜单(admin)
 * @returns 返回后端动态路由菜单数据 { data: [...] }
 */
export async function getBackEndControlRoutes() {
	try {
		// 检查是否有 token，如果没有则不请求菜单
		if (!Session.get('token')) {
			//console.log('无 token，跳过菜单请求');
			return { data: [] };
		}
		
		// 从适配的菜单 API 获取用户菜单树
		// 由于 request.ts 已修改为直接返回 res.Data，
		// 这里的 res 就是 res.Data，即菜单数组 [...]
		const menuTreeData = await useMenuApi().getUserMenuTree();
		//console.log('后端返回的菜单数据 (已处理):', menuTreeData); // 添加日志查看结构

		// 直接返回，包装成 { data: ... } 的格式以匹配 vue-next-admin 的预期
		// vue-next-admin 的后续逻辑会使用返回对象的 .data 属性
		return { data: menuTreeData };
	} catch (error) {
		console.error('获取后端菜单失败:', error);
		// 返回空数组以避免后续错误
		return { data: [] };
	}
}

/**
 * 重新请求后端路由菜单接口
 * @description 用于菜单管理界面刷新菜单（未进行测试）
 * @description 路径：/src/views/system/menu/component/addMenu.vue
 */
export async function setBackEndControlRefreshRoutes() {
	await getBackEndControlRoutes();
}

/**
 * 后端路由 component 转换
 * @param routes 后端返回的路由表数组
 * @returns 返回处理成函数后的 component 和符合 vue-router 格式的路由
 */
export function backEndComponent(routes: any) {
	//console.log("backEndComponent received routes:", routes, "Type:", typeof routes, "Is Array:", Array.isArray(routes)); // Debug log
	if (!routes || !Array.isArray(routes)) return [];

	return routes.map((item: any) => {
		//console.log("Processing menu item:", item.Name, "Path:", item.Path, "IsFdMicroApp:", item.IsFdMicroApp, "Module:", item.Module); // Log each item

		// 1. 创建符合 vue-router 格式的路由对象
		const route: any = {
			// 映射属性名
			name: item.Name || item.Title, // 使用后端返回的 Name 或 Title 作为路由 name
			path: item.Path, // 假设后端 Path 对应前端 path
			// component 处理
			// item.component 是 vue-next-admin 原有的逻辑，用于处理动态导入
			// 如果后端没有提供 Component 字段，可能需要一个默认值或处理逻辑
			// 这里先保留原有逻辑，但要确保 item.Component (如果存在) 被正确处理
			component: item.component, // 保留原有动态导入逻辑的结果
			// meta 信息 - 这是关键部分，需要从后端字段映射
			meta: {
				title: item.Title || item.Name, // 使用后端返回的 Title 或 Name 作为标题
				icon: item.Icon,  // 图标
				isHide: item.IsHide !== undefined ? item.IsHide : false,
				isKeepAlive: item.IsKeepAlive !== undefined ? item.IsKeepAlive : true,
				isAffix: item.IsAffix !== undefined ? item.IsAffix : false,
				isIframe: item.IsIframe !== undefined ? item.IsIframe : false,
				// 传递微应用相关标识
				isFdMicroApp: item.IsFdMicroApp || false,
				module: item.Module || '', // 传递 Module 字段作为微应用标识
                                // 关键：传递 Code 和 ParentCode 用于面包屑生成
                                code: item.Code,
                                parentCode: item.ParentCode,
			},
			// 映射 Children
			children: item.Children || item.children || [], // 后端可能是 Children, 前端递归可能是 children
		};

		// 确保路由name属性存在
		if (!route.name) {
			// 如果没有name，则使用路径生成一个
			route.name = item.Path.replace(/\//g, '') || 'route-' + Date.now();
			//console.log("Fallback route name:", route.name, "from path:", item.Path);
		}

		// 2. 处理 component (优先使用后端提供的 Component 字段)
		// 特殊处理微应用菜单项
		if (item.IsFdMicroApp) {
			//console.log("Setting micro app component for menu:", item.Name, item.Path); // Log when setting micro app component
			// 为微应用菜单项设置特殊的 component
			// 这个 component 将负责加载对应的 qiankun 微应用
			route.component = () => import('@/layout/routerView/parent.vue');
		} else if (item.Component) {
			// 如果后端提供了 Component 路径，则生成动态导入
			// 假设后端 Component 存储的是相对路径，例如 "home/index.vue"
			// 需要映射到实际的 views 目录下
			// 注意：这里的路径映射逻辑可能需要根据你的实际项目结构调整
			route.component = dynamicImport(dynamicViewsModules, item.Component);
			// 如果 dynamicImport 返回 false 或 undefined，可能需要设置默认组件或处理错误
			if (!route.component) {
				console.warn(`Failed to dynamically import component for path: ${item.Path}, component path: ${item.Component}`);
				// 可以设置一个默认的错误组件或者空组件
				// route.component = () => import('@/views/error/404.vue');
			}
		} else if (item.component) {
			// 保留原有逻辑（可能来自递归调用）
			route.component = item.component;
		}
		// 如果都没有 component，vue-router 可能会报错，需要确保有合理的默认值或处理

		// 3. 递归处理子路由
		if (route.children && route.children.length > 0) {
			route.children = backEndComponent(route.children);
		}

		// 4. 如果有其他 vue-next-admin 特有的处理逻辑，可以在这里添加
		// 例如处理 redirect, alias 等

		return route;
	});
}

/**
 * 后端路由 component 转换函数
 * @param dynamicViewsModules 获取目录下的 .vue、.tsx 全部文件
 * @param component 当前要处理项 component
 * @returns 返回处理成函数后的 component
 */
export function dynamicImport(dynamicViewsModules: Record<string, Function>, component: string) {
	const keys = Object.keys(dynamicViewsModules);
	const matchKeys = keys.filter((key) => {
		const k = key.replace(/..\/views|../, '');
		return k.startsWith(`${component}`) || k.startsWith(`/${component}`);
	});
	if (matchKeys?.length === 1) {
		const matchKey = matchKeys[0];
		return dynamicViewsModules[matchKey];
	}
	if (matchKeys?.length > 1) {
		return false;
	}
}
