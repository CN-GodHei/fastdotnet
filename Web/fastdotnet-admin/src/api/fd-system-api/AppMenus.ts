// @ts-ignore
/* eslint-disable */
import request from '/@/utils/request';

/** 此处后端没有提供注释 GET /api/app/menus/tree */
export async function getAppMenusTree(options?: { [key: string]: any }) {
	return request<any>('/api/app/menus/tree', {
		method: 'GET',
		...(options || {}),
	});
}
