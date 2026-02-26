// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 此处后端没有提供注释 GET /api/app/menus/tree */
export async function getApiAppMenusTree(options?: { [key: string]: any }) {
	return request<APIModel.FdMenuDto[]>('/api/app/menus/tree', {
		method: 'GET',
		...(options || {}),
	});
}
