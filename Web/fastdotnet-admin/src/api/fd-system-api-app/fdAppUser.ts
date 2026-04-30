// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 此处后端没有提供注释 GET /api/FdAppUser */
/** @deprecated 请使用 getAppGenericDtoControllerBase5GetAll，原函数名: getApiFdAppUser */
export async function getAppGenericDtoControllerBase5GetAll(options?: { [key: string]: any }) {
	return request<APIModel.FdAppUserDto[]>('/api/FdAppUser', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getAppGenericDtoControllerBase5GetAll */
export const getApiFdAppUser = getAppGenericDtoControllerBase5GetAll;
/** 此处后端没有提供注释 POST /api/FdAppUser */
/** @deprecated 请使用 postAppGenericDtoControllerBase5Create，原函数名: postApiFdAppUser */
export async function postAppGenericDtoControllerBase5Create(body: APIModel.CreateFdAppUserDto, options?: { [key: string]: any }) {
	return request<APIModel.FdAppUserDto>('/api/FdAppUser', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postAppGenericDtoControllerBase5Create */
export const postApiFdAppUser = postAppGenericDtoControllerBase5Create;
/** 此处后端没有提供注释 GET /api/FdAppUser/${param0} */
/** @deprecated 请使用 getAppGenericDtoControllerBase5GetById，原函数名: getApiFdAppUserId */
export async function getAppGenericDtoControllerBase5GetById(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAppGenericDtoControllerBase5GetByIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<APIModel.FdAppUserDto>(`/api/FdAppUser/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getAppGenericDtoControllerBase5GetById */
export const getApiFdAppUserId = getAppGenericDtoControllerBase5GetById;
/** 此处后端没有提供注释 PUT /api/FdAppUser/${param0} */
/** @deprecated 请使用 putAppGenericDtoControllerBase5Update，原函数名: putApiFdAppUserId */
export async function putAppGenericDtoControllerBase5Update(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putAppGenericDtoControllerBase5UpdateParams,
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

/** @deprecated 此函数名已变更，请使用 putAppGenericDtoControllerBase5Update */
export const putApiFdAppUserId = putAppGenericDtoControllerBase5Update;
/** 此处后端没有提供注释 DELETE /api/FdAppUser/${param0} */
/** @deprecated 请使用 deleteAppGenericDtoControllerBase5Delete，原函数名: deleteApiFdAppUserId */
export async function deleteAppGenericDtoControllerBase5Delete(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteAppGenericDtoControllerBase5DeleteParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/FdAppUser/${param0}`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteAppGenericDtoControllerBase5Delete */
export const deleteApiFdAppUserId = deleteAppGenericDtoControllerBase5Delete;
/** 重置用户密码为系统默认密码 POST /api/FdAppUser/${param0}/reset-password */
/** @deprecated 请使用 postFdAppUserResetPassword，原函数名: postApiFdAppUserIdResetPassword */
export async function postFdAppUserResetPassword(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.postFdAppUserResetPasswordParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/FdAppUser/${param0}/reset-password`, {
		method: 'POST',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postFdAppUserResetPassword */
export const postApiFdAppUserIdResetPassword = postFdAppUserResetPassword;
/** 此处后端没有提供注释 PUT /api/FdAppUser/batch */
/** @deprecated 请使用 putAppGenericDtoControllerBase5UpdateMany，原函数名: putApiFdAppUserBatch */
export async function putAppGenericDtoControllerBase5UpdateMany(body: APIModel.UpdateFdAppUserDto[], options?: { [key: string]: any }) {
	return request<number>('/api/FdAppUser/batch', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putAppGenericDtoControllerBase5UpdateMany */
export const putApiFdAppUserBatch = putAppGenericDtoControllerBase5UpdateMany;
/** 此处后端没有提供注释 POST /api/FdAppUser/batch */
/** @deprecated 请使用 postAppGenericDtoControllerBase5CreateMany，原函数名: postApiFdAppUserBatch */
export async function postAppGenericDtoControllerBase5CreateMany(body: APIModel.CreateFdAppUserDto[], options?: { [key: string]: any }) {
	return request<number>('/api/FdAppUser/batch', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postAppGenericDtoControllerBase5CreateMany */
export const postApiFdAppUserBatch = postAppGenericDtoControllerBase5CreateMany;
/** 此处后端没有提供注释 DELETE /api/FdAppUser/batch */
/** @deprecated 请使用 deleteAppGenericDtoControllerBase5BatchDelete，原函数名: deleteApiFdAppUserBatch */
export async function deleteAppGenericDtoControllerBase5BatchDelete(body: string[], options?: { [key: string]: any }) {
	return request<number>('/api/FdAppUser/batch', {
		method: 'DELETE',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteAppGenericDtoControllerBase5BatchDelete */
export const deleteApiFdAppUserBatch = deleteAppGenericDtoControllerBase5BatchDelete;
/** 此处后端没有提供注释 PUT /api/FdAppUser/batch/updatebycondition */
/** @deprecated 请使用 putAppGenericDtoControllerBase5UpdateManyByCondition，原函数名: putApiFdAppUserBatchUpdatebycondition */
export async function putAppGenericDtoControllerBase5UpdateManyByCondition(
	body: APIModel.BatchUpdateByConditionDto1UpdateFdAppUserDto,
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

/** @deprecated 此函数名已变更，请使用 putAppGenericDtoControllerBase5UpdateManyByCondition */
export const putApiFdAppUserBatchUpdatebycondition = putAppGenericDtoControllerBase5UpdateManyByCondition;
/** 修改用户邮箱 POST /api/FdAppUser/change-email */
/** @deprecated 请使用 postFdAppUserChangeEmail，原函数名: postApiFdAppUserChangeEmail */
export async function postFdAppUserChangeEmail(body: APIModel.ChangeEmailDto, options?: { [key: string]: any }) {
	return request<boolean>('/api/FdAppUser/change-email', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postFdAppUserChangeEmail */
export const postApiFdAppUserChangeEmail = postFdAppUserChangeEmail;
/** 修改用户密码 POST /api/FdAppUser/change-password */
/** @deprecated 请使用 postFdAppUserChangePassword，原函数名: postApiFdAppUserChangePassword */
export async function postFdAppUserChangePassword(body: APIModel.ChangePasswordDto, options?: { [key: string]: any }) {
	return request<boolean>('/api/FdAppUser/change-password', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postFdAppUserChangePassword */
export const postApiFdAppUserChangePassword = postFdAppUserChangePassword;
/** 此处后端没有提供注释 GET /api/FdAppUser/getUserInfo */
/** @deprecated 请使用 getFdAppUserGetUserInfo，原函数名: getApiFdAppUserGetUserInfo */
export async function getFdAppUserGetUserInfo(options?: { [key: string]: any }) {
	return request<APIModel.FdAppUserDto>('/api/FdAppUser/getUserInfo', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getFdAppUserGetUserInfo */
export const getApiFdAppUserGetUserInfo = getFdAppUserGetUserInfo;
/** 此处后端没有提供注释 POST /api/FdAppUser/list-by-condition */
/** @deprecated 请使用 postAppGenericDtoControllerBase5GetListByCondition，原函数名: postApiFdAppUserListByCondition */
export async function postAppGenericDtoControllerBase5GetListByCondition(body: APIModel.QueryByConditionDto, options?: { [key: string]: any }) {
	return request<any>('/api/FdAppUser/list-by-condition', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postAppGenericDtoControllerBase5GetListByCondition */
export const postApiFdAppUserListByCondition = postAppGenericDtoControllerBase5GetListByCondition;
/** 此处后端没有提供注释 GET /api/FdAppUser/page */
/** @deprecated 请使用 getAppGenericDtoControllerBase5GetPage，原函数名: getApiFdAppUserPage */
export async function getAppGenericDtoControllerBase5GetPage(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAppGenericDtoControllerBase5GetPageParams,
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

/** @deprecated 此函数名已变更，请使用 getAppGenericDtoControllerBase5GetPage */
export const getApiFdAppUserPage = getAppGenericDtoControllerBase5GetPage;
/** 此处后端没有提供注释 POST /api/FdAppUser/page/search */
/** @deprecated 请使用 postAppGenericDtoControllerBase5GetPageByCondition，原函数名: postApiFdAppUserPageSearch */
export async function postAppGenericDtoControllerBase5GetPageByCondition(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/FdAppUser/page/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postAppGenericDtoControllerBase5GetPageByCondition */
export const postApiFdAppUserPageSearch = postAppGenericDtoControllerBase5GetPageByCondition;
/** 发送修改邮箱验证码 POST /api/FdAppUser/send-change-email-code */
/** @deprecated 请使用 postFdAppUserSendChangeEmailCode，原函数名: postApiFdAppUserSendChangeEmailCode */
export async function postFdAppUserSendChangeEmailCode(body: string, options?: { [key: string]: any }) {
	return request<boolean>('/api/FdAppUser/send-change-email-code', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postFdAppUserSendChangeEmailCode */
export const postApiFdAppUserSendChangeEmailCode = postFdAppUserSendChangeEmailCode;
/** 解锁屏幕 POST /api/FdAppUser/unlock */
/** @deprecated 请使用 postFdAppUserUnlock，原函数名: postApiFdAppUserUnlock */
export async function postFdAppUserUnlock(body: APIModel.UnlockDto, options?: { [key: string]: any }) {
	return request<boolean>('/api/FdAppUser/unlock', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postFdAppUserUnlock */
export const postApiFdAppUserUnlock = postFdAppUserUnlock;
