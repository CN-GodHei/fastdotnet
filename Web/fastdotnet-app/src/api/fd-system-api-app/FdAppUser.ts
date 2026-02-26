// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 此处后端没有提供注释 GET /api/FdAppUser */
export async function getApiFdAppUser(options?: { [key: string]: any }) {
	return request<APIModel.FdAppUserDto[]>('/api/FdAppUser', {
		method: 'GET',
		...(options || {}),
	});
}
/** 此处后端没有提供注释 POST /api/FdAppUser */
export async function postApiFdAppUser(body: APIModel.CreateFdAppUserDto, options?: { [key: string]: any }) {
	return request<APIModel.FdAppUserDto>('/api/FdAppUser', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 此处后端没有提供注释 GET /api/FdAppUser/${param0} */
export async function getApiFdAppUserId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getApiFdAppUserIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<APIModel.FdAppUserDto>(`/api/FdAppUser/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}
/** 此处后端没有提供注释 PUT /api/FdAppUser/${param0} */
export async function putApiFdAppUserId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putApiFdAppUserIdParams,
	body: APIModel.UpdateFdAppUserDto,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<APIModel.FdAppUserDto>(`/api/FdAppUser/${param0}`, {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		params: { ...queryParams },
		data: body,
		...(options || {}),
	});
}
/** 此处后端没有提供注释 DELETE /api/FdAppUser/${param0} */
export async function deleteApiFdAppUserId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteApiFdAppUserIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/FdAppUser/${param0}`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}
/** 此处后端没有提供注释 PUT /api/FdAppUser/batch */
export async function putApiFdAppUserBatch(body: APIModel.UpdateFdAppUserDto[], options?: { [key: string]: any }) {
	return request<number>('/api/FdAppUser/batch', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 此处后端没有提供注释 POST /api/FdAppUser/batch */
export async function postApiFdAppUserBatch(body: APIModel.CreateFdAppUserDto[], options?: { [key: string]: any }) {
	return request<number>('/api/FdAppUser/batch', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 此处后端没有提供注释 DELETE /api/FdAppUser/batch */
export async function deleteApiFdAppUserBatch(body: string[], options?: { [key: string]: any }) {
	return request<number>('/api/FdAppUser/batch', {
		method: 'DELETE',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 此处后端没有提供注释 PUT /api/FdAppUser/batch/updatebycondition */
export async function putApiFdAppUserBatchUpdatebycondition(
	body: APIModel.UpdateFdAppUserDtoBatchUpdateByConditionDto,
	options?: { [key: string]: any }
) {
	return request<number>('/api/FdAppUser/batch/updatebycondition', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 此处后端没有提供注释 GET /api/FdAppUser/getUserInfo */
export async function getApiFdAppUserGetUserInfo(options?: { [key: string]: any }) {
	return request<APIModel.FdAppUserDto>('/api/FdAppUser/getUserInfo', {
		method: 'GET',
		...(options || {}),
	});
}
/** 此处后端没有提供注释 POST /api/FdAppUser/list-by-condition */
export async function postApiFdAppUserListByCondition(body: APIModel.QueryByConditionDto, options?: { [key: string]: any }) {
	return request<any>('/api/FdAppUser/list-by-condition', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 此处后端没有提供注释 GET /api/FdAppUser/page */
export async function getApiFdAppUserPage(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getApiFdAppUserPageParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.PageInfo>('/api/FdAppUser/page', {
		method: 'GET',
		params: {
			// pageIndex has a default value: 1
			pageIndex: '1',
			// pageSize has a default value: 10
			pageSize: '10',
			...params,
		},
		...(options || {}),
	});
}
/** 此处后端没有提供注释 POST /api/FdAppUser/page/search */
export async function postApiFdAppUserPageSearch(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/FdAppUser/page/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
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
