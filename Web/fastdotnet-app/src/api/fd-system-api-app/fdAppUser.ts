// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 此处后端没有提供注释 GET /api/FdAppUser */
export async function getAppGenericDtoControllerBase5GetAll(options?: { [key: string]: any }) {
	return request<APIModel.FdAppUserDto[]>('/api/FdAppUser', {
		method: 'GET',
		...(options || {}),
	});
}
/** 此处后端没有提供注释 POST /api/FdAppUser */
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
/** 此处后端没有提供注释 GET /api/FdAppUser/${param0} */
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
/** 此处后端没有提供注释 PUT /api/FdAppUser/${param0} */
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
/** 此处后端没有提供注释 DELETE /api/FdAppUser/${param0} */
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
/** 重置用户密码为系统默认密码 POST /api/FdAppUser/${param0}/reset-password */
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
/** 此处后端没有提供注释 PUT /api/FdAppUser/batch */
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
/** 此处后端没有提供注释 POST /api/FdAppUser/batch */
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
/** 此处后端没有提供注释 DELETE /api/FdAppUser/batch */
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
/** 此处后端没有提供注释 PUT /api/FdAppUser/batch/updatebycondition */
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
/** 修改用户邮箱 POST /api/FdAppUser/change-email */
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
/** 修改用户密码 POST /api/FdAppUser/change-password */
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
/** 此处后端没有提供注释 GET /api/FdAppUser/getUserInfo */
export async function getFdAppUserGetUserInfo(options?: { [key: string]: any }) {
	return request<APIModel.FdAppUserDto>('/api/FdAppUser/getUserInfo', {
		method: 'GET',
		...(options || {}),
	});
}
/** 此处后端没有提供注释 POST /api/FdAppUser/list-by-condition */
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
/** 此处后端没有提供注释 GET /api/FdAppUser/page */
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
/** 此处后端没有提供注释 POST /api/FdAppUser/page/search */
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
/** 发送修改邮箱验证码 POST /api/FdAppUser/send-change-email-code */
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
/** 解锁屏幕 POST /api/FdAppUser/unlock */
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
