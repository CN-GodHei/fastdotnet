// @ts-ignore
/* eslint-disable */
import request from '@/utils/request';

/** 此处后端没有提供注释 GET /api/FdAppUser/getUserInfo */
export async function getApiFdAppUserGetUserInfo(options?: { [key: string]: any }) {
	return request<APIModel.FdAppUserDto>('/api/FdAppUser/getUserInfo', {
		method: 'GET',
		...(options || {}),
	});
}

/** 解锁屏幕 POST /api/FdAppUser/unlock */
export async function postApiFdAppUserUnlock(body: APIModel.UnlockDto, options?: { [key: string]: any }) {
	return request<boolean>('/api/FdAppUser/unlock', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
