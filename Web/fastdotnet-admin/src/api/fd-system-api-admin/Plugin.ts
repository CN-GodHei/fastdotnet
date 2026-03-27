// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

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
/** 获取用户授权码 GET /api/Plugin/GetAuthCode */
export async function getApiPluginGetAuthCode(options?: { [key: string]: any }) {
	return request<string>('/api/Plugin/GetAuthCode', {
		method: 'GET',
		...(options || {}),
	});
}
/** 从URL下载并加载插件 POST /api/Plugin/load */
export async function postApiPluginLoad(body: APIModel.DownloadPluginDto, options?: { [key: string]: any }) {
	return request<APIModel.ApiResult>('/api/Plugin/load', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
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
/** 设置用户授权码 POST /api/Plugin/SetAuthCode */
export async function postApiPluginSetAuthCode(body: APIModel.SetAuthCodeDto, options?: { [key: string]: any }) {
	return request<boolean>('/api/Plugin/SetAuthCode', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 设置插件许可 POST /api/Plugin/SetPluginLicense */
export async function postApiPluginSetPluginLicense(body: APIModel.SetPluginLicenseDto, options?: { [key: string]: any }) {
	return request<boolean>('/api/Plugin/SetPluginLicense', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
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

	return request<APIModel.UninstallResDto>(`/api/Plugin/uninstall/${param0}`, {
		method: 'POST',
		params: { ...queryParams },
		...(options || {}),
	});
}
/** 在线更新授权 POST /api/Plugin/UpdatePluginLicenseOnline */
export async function postApiPluginUpdatePluginLicenseOnline(body: APIModel.UpdatePluginLicenseOnlineDto, options?: { [key: string]: any }) {
	return request<boolean>('/api/Plugin/UpdatePluginLicenseOnline', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 上传离线插件安装包 POST /api/Plugin/upload-offline */
export async function postApiPluginUploadOffline(body: {}, file?: File, options?: { [key: string]: any }) {
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
