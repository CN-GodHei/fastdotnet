// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 健康检查接口 GET /Health */
export async function getHealthGet(options?: { [key: string]: any }) {
	return request<APIModel.HealthStatus>('/Health', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getHealthGet */
export const getHealth = getHealthGet;
