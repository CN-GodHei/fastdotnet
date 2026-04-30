// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 获取所有当前活动的插件 GET /api/Plugin/active */
/** @deprecated 请使用 getPluginGetActivePlugins，原函数名: getApiPluginActive */
export async function getPluginGetActivePlugins(options?: { [key: string]: any }) {
	return request<any>('/api/Plugin/active', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getPluginGetActivePlugins */
export const getApiPluginActive = getPluginGetActivePlugins;
/** 检查一个插件当前是否处于活动状态 GET /api/Plugin/active/${param0} */
/** @deprecated 请使用 getPluginIsPluginActive，原函数名: getApiPluginActivePluginId */
export async function getPluginIsPluginActive(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getPluginIsPluginActiveParams,
	options?: { [key: string]: any }
) {
	const { pluginId: param0, ...queryParams } = params;

	return request<any>(`/api/Plugin/active/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getPluginIsPluginActive */
export const getApiPluginActivePluginId = getPluginIsPluginActive;
/** 停用一个插件（停止业务并卸载其代码） POST /api/Plugin/disable/${param0} */
/** @deprecated 请使用 postPluginDisablePlugin，原函数名: postApiPluginDisablePluginId */
export async function postPluginDisablePlugin(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.postPluginDisablePluginParams,
	options?: { [key: string]: any }
) {
	const { pluginId: param0, ...queryParams } = params;

	return request<APIModel.ApiResult>(`/api/Plugin/disable/${param0}`, {
		method: 'POST',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postPluginDisablePlugin */
export const postApiPluginDisablePluginId = postPluginDisablePlugin;
/** 启用一个插件（如果未加载，则先加载） POST /api/Plugin/enable/${param0} */
/** @deprecated 请使用 postPluginEnablePlugin，原函数名: postApiPluginEnablePluginId */
export async function postPluginEnablePlugin(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.postPluginEnablePluginParams,
	options?: { [key: string]: any }
) {
	const { pluginId: param0, ...queryParams } = params;

	return request<APIModel.ApiResult>(`/api/Plugin/enable/${param0}`, {
		method: 'POST',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postPluginEnablePlugin */
export const postApiPluginEnablePluginId = postPluginEnablePlugin;
/** 获取用户授权码 GET /api/Plugin/GetAuthCode */
/** @deprecated 请使用 getPluginGetAuthCode，原函数名: getApiPluginGetAuthCode */
export async function getPluginGetAuthCode(options?: { [key: string]: any }) {
	return request<string>('/api/Plugin/GetAuthCode', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getPluginGetAuthCode */
export const getApiPluginGetAuthCode = getPluginGetAuthCode;
/** 从 URL 下载并加载插件 POST /api/Plugin/load */
/** @deprecated 请使用 postPluginLoadPlugin，原函数名: postApiPluginLoad */
export async function postPluginLoadPlugin(body: APIModel.DownloadPluginDto, options?: { [key: string]: any }) {
	return request<APIModel.ApiResult>('/api/Plugin/load', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postPluginLoadPlugin */
export const postApiPluginLoad = postPluginLoadPlugin;
/** 获取所有已加载的插件（无论是否激活） GET /api/Plugin/loaded */
/** @deprecated 请使用 getPluginGetLoadedPlugins，原函数名: getApiPluginLoaded */
export async function getPluginGetLoadedPlugins(options?: { [key: string]: any }) {
	return request<any>('/api/Plugin/loaded', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getPluginGetLoadedPlugins */
export const getApiPluginLoaded = getPluginGetLoadedPlugins;
/** 扫描插件目录以发现所有可用插件 GET /api/Plugin/scan */
/** @deprecated 请使用 getPluginScanPlugins，原函数名: getApiPluginScan */
export async function getPluginScanPlugins(options?: { [key: string]: any }) {
	return request<APIModel.PluginInfo[]>('/api/Plugin/scan', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getPluginScanPlugins */
export const getApiPluginScan = getPluginScanPlugins;
/** 设置用户授权码 POST /api/Plugin/SetAuthCode */
/** @deprecated 请使用 postPluginSetAuthCode，原函数名: postApiPluginSetAuthCode */
export async function postPluginSetAuthCode(body: APIModel.SetAuthCodeDto, options?: { [key: string]: any }) {
	return request<boolean>('/api/Plugin/SetAuthCode', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postPluginSetAuthCode */
export const postApiPluginSetAuthCode = postPluginSetAuthCode;
/** 设置插件许可 POST /api/Plugin/SetPluginLicense */
/** @deprecated 请使用 postPluginSetPluginLicense，原函数名: postApiPluginSetPluginLicense */
export async function postPluginSetPluginLicense(body: APIModel.SetPluginLicenseDto, options?: { [key: string]: any }) {
	return request<boolean>('/api/Plugin/SetPluginLicense', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postPluginSetPluginLicense */
export const postApiPluginSetPluginLicense = postPluginSetPluginLicense;
/** 从磁盘物理删除一个已停用的插件 POST /api/Plugin/uninstall/${param0} */
/** @deprecated 请使用 postPluginUninstallPlugin，原函数名: postApiPluginUninstallPluginId */
export async function postPluginUninstallPlugin(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.postPluginUninstallPluginParams,
	options?: { [key: string]: any }
) {
	const { pluginId: param0, ...queryParams } = params;

	return request<APIModel.UninstallResDto>(`/api/Plugin/uninstall/${param0}`, {
		method: 'POST',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postPluginUninstallPlugin */
export const postApiPluginUninstallPluginId = postPluginUninstallPlugin;
/** 在线更新授权 POST /api/Plugin/UpdatePluginLicenseOnline */
/** @deprecated 请使用 postPluginUpdatePluginLicenseOnline，原函数名: postApiPluginUpdatePluginLicenseOnline */
export async function postPluginUpdatePluginLicenseOnline(body: APIModel.UpdatePluginLicenseOnlineDto, options?: { [key: string]: any }) {
	return request<boolean>('/api/Plugin/UpdatePluginLicenseOnline', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postPluginUpdatePluginLicenseOnline */
export const postApiPluginUpdatePluginLicenseOnline = postPluginUpdatePluginLicenseOnline;
/** 上传离线插件安装包 POST /api/Plugin/upload-offline */
/** @deprecated 请使用 postPluginUploadOfflinePackage，原函数名: postApiPluginUploadOffline */
export async function postPluginUploadOfflinePackage(body: {}, file?: File, options?: { [key: string]: any }) {
	const formData = new FormData();

	if (file) {
		formData.append('file', file);
	}

	Object.keys(body).forEach((ele) => {
		const item = (body as any)[ele];

		if (item !== undefined && item !== null) {
			if (typeof item === 'object' && !(item instanceof File)) {
				if (item instanceof Array) {
					item.forEach((f) => formData.append(ele, f || ''));
				} else {
					formData.append(ele, new Blob([JSON.stringify(item)], { type: 'application/json' }));
				}
			} else {
				formData.append(ele, item);
			}
		}
	});

	return request<APIModel.ApiResult>('/api/Plugin/upload-offline', {
		method: 'POST',
		data: formData,
		requestType: 'form',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postPluginUploadOfflinePackage */
export const postApiPluginUploadOffline = postPluginUploadOfflinePackage;
