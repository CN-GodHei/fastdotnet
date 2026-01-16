// @ts-ignore
/* eslint-disable */
import request from '@/utils/request';

/** 获取所有当前活动的插件 GET /api/Plugin/active */
export async function getApiPluginActive(options?: { [key: string]: any }) {
	return request<any>('/api/Plugin/active', {
		method: 'GET',
		...(options || {}),
	});
}

/** 检查一个插件当前是否处于活动状态 GET /api/Plugin/active/${param0} */
export async function getApiPluginActivePluginId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getApiPluginActivePluginIdParams,
	options?: { [key: string]: any }
) {
	const { pluginId: param0, ...queryParams } = params;
	return request<any>(`/api/Plugin/active/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 停用一个插件（停止业务并卸载其代码） POST /api/Plugin/disable/${param0} */
export async function postApiPluginDisablePluginId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.postApiPluginDisablePluginIdParams,
	options?: { [key: string]: any }
) {
	const { pluginId: param0, ...queryParams } = params;
	return request<APIModel.ApiResult>(`/api/Plugin/disable/${param0}`, {
		method: 'POST',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 启用一个插件（如果未加载，则先加载） POST /api/Plugin/enable/${param0} */
export async function postApiPluginEnablePluginId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.postApiPluginEnablePluginIdParams,
	options?: { [key: string]: any }
) {
	const { pluginId: param0, ...queryParams } = params;
	return request<APIModel.ApiResult>(`/api/Plugin/enable/${param0}`, {
		method: 'POST',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 获取所有已加载的插件（无论是否激活） GET /api/Plugin/loaded */
export async function getApiPluginLoaded(options?: { [key: string]: any }) {
	return request<any>('/api/Plugin/loaded', {
		method: 'GET',
		...(options || {}),
	});
}

/** 扫描插件目录以发现所有可用插件 GET /api/Plugin/scan */
export async function getApiPluginScan(options?: { [key: string]: any }) {
	return request<APIModel.PluginInfo[]>('/api/Plugin/scan', {
		method: 'GET',
		...(options || {}),
	});
}

/** 从磁盘物理删除一个已停用的插件 POST /api/Plugin/uninstall/${param0} */
export async function postApiPluginUninstallPluginId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.postApiPluginUninstallPluginIdParams,
	options?: { [key: string]: any }
) {
	const { pluginId: param0, ...queryParams } = params;
	return request<APIModel.ApiResult>(`/api/Plugin/uninstall/${param0}`, {
		method: 'POST',
		params: { ...queryParams },
		...(options || {}),
	});
}
