// @ts-ignore
/* eslint-disable */
import request from '@/utils/request';

/** 扫描插件目录以发现所有可用插件 GET /api/Plugin/scan */
export async function getApiPluginScan(options?: { [key: string]: any }) {
	return request<APIModel.PluginInfo[]>('/api/Plugin/scan', {
		method: 'GET',
		...(options || {}),
	});
}
