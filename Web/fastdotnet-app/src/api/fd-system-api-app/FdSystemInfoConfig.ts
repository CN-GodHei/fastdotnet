// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** [Public] 获取所有系统配置项（用于客户端初始化） GET /api/admin/FdSystemInfoConfig/public/all */
export async function getApiAdminFdSystemInfoConfigPublicAll(options?: { [key: string]: any }) {
	return request<Record<string, any>>('/api/admin/FdSystemInfoConfig/public/all', {
		method: 'GET',
		...(options || {}),
	});
}
