// @ts-ignore
/* eslint-disable */
import request from '@/utils/request';

/** 获取当前服务器的唯一机器指纹 GET /api/System/machine-fingerprint */
export async function getApiSystemMachineFingerprint(options?: { [key: string]: any }) {
	return request<string>('/api/System/machine-fingerprint', {
		method: 'GET',
		...(options || {}),
	});
}
