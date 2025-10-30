// @ts-ignore
/* eslint-disable */
import request from '/@/utils/request';

/** 获取所有权限，按模块分组，方便前端展示 GET /api/admin/permissions */
export async function getAdminPermissions(options?: { [key: string]: any }) {
	return request<Record<string, any>>('/api/admin/permissions', {
		method: 'GET',
		...(options || {}),
	});
}
