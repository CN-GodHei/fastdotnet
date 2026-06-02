// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** [Public] 获取所有系统配置项（用于客户端初始化） GET /api/admin/FdSystemInfoConfig/public/all */
export async function getFdSystemInfoConfigGetPublicConfigs(options?: { [key: string]: any }) {
	return request<Record<string, any>>('/api/admin/FdSystemInfoConfig/public/all', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getFdSystemInfoConfigGetPublicConfigs */
export const getApiAdminFdSystemInfoConfigPublicAll = getFdSystemInfoConfigGetPublicConfigs;
