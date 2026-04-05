// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 健康检查接口 GET /Health */
export async function getHealth(options?: { [key: string]: any }) {
	return request<APIModel.HealthStatus>('/Health', {
		method: 'GET',
		...(options || {}),
	});
}
