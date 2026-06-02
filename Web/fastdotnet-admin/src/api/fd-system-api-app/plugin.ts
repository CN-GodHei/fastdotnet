// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 获取已启用的插件列表（允许匿名访问） GET /api/Plugin/enabled */
export async function getPluginGetEnabledPlugins(options?: { [key: string]: any }) {
	return request<APIModel.PluginInfo[]>('/api/Plugin/enabled', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getPluginGetEnabledPlugins */
export const getApiPluginEnabled = getPluginGetEnabledPlugins;
/** 扫描插件目录以发现所有可用插件 GET /api/Plugin/scan */
export async function getPluginScanPlugins(options?: { [key: string]: any }) {
	return request<APIModel.PluginInfo[]>('/api/Plugin/scan', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getPluginScanPlugins */
export const getApiPluginScan = getPluginScanPlugins;
