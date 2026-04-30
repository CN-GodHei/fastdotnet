// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 获取当前服务器的唯一机器指纹 GET /api/System/machine-fingerprint */
/** @deprecated 请使用 getSystemGetMachineFingerprint，原函数名: getApiSystemMachineFingerprint */
export async function getSystemGetMachineFingerprint(options?: { [key: string]: any }) {
	return request<string>('/api/System/machine-fingerprint', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getSystemGetMachineFingerprint */
export const getApiSystemMachineFingerprint = getSystemGetMachineFingerprint;
