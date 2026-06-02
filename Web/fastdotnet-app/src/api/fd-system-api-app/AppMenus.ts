// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 此处后端没有提供注释 GET /api/app/menus/tree */
export async function getAppMenusGetUserMenuTree(options?: { [key: string]: any }) {
	return request<APIModel.FdMenuDto[]>('/api/app/menus/tree', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getAppMenusGetUserMenuTree */
export const getApiAppMenusTree = getAppMenusGetUserMenuTree;
