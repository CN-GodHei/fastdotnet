// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

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
