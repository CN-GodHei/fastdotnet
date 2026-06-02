// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 此处后端没有提供注释 GET /api/FdDictDataApp */
export async function getAppGenericDtoControllerBase5GetAll(options?: { [key: string]: any }) {
	return request<APIModel.FdDictDataDto[]>('/api/FdDictDataApp', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getAppGenericDtoControllerBase5GetAll */
export const getApiFdDictDataApp = getAppGenericDtoControllerBase5GetAll;
/** 此处后端没有提供注释 POST /api/FdDictDataApp */
export async function postAppGenericDtoControllerBase5Create(body: APIModel.CreateFdDictDataDto, options?: { [key: string]: any }) {
	return request<APIModel.FdDictDataDto>('/api/FdDictDataApp', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postAppGenericDtoControllerBase5Create */
export const postApiFdDictDataApp = postAppGenericDtoControllerBase5Create;
/** 此处后端没有提供注释 GET /api/FdDictDataApp/${param0} */
export async function getAppGenericDtoControllerBase5GetById(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAppGenericDtoControllerBase5GetByIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<APIModel.FdDictDataDto>(`/api/FdDictDataApp/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getAppGenericDtoControllerBase5GetById */
export const getApiFdDictDataAppId = getAppGenericDtoControllerBase5GetById;
/** 此处后端没有提供注释 PUT /api/FdDictDataApp/${param0} */
export async function putAppGenericDtoControllerBase5Update(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putAppGenericDtoControllerBase5UpdateParams,
	body: APIModel.UpdateFdDictDataDto,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<APIModel.FdDictDataDto>(`/api/FdDictDataApp/${param0}`, {
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
export const putApiFdDictDataAppId = putAppGenericDtoControllerBase5Update;
/** 此处后端没有提供注释 DELETE /api/FdDictDataApp/${param0} */
export async function deleteAppGenericDtoControllerBase5Delete(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteAppGenericDtoControllerBase5DeleteParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/FdDictDataApp/${param0}`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteAppGenericDtoControllerBase5Delete */
export const deleteApiFdDictDataAppId = deleteAppGenericDtoControllerBase5Delete;
/** 此处后端没有提供注释 PUT /api/FdDictDataApp/batch */
export async function putAppGenericDtoControllerBase5UpdateMany(body: APIModel.UpdateFdDictDataDto[], options?: { [key: string]: any }) {
	return request<number>('/api/FdDictDataApp/batch', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putAppGenericDtoControllerBase5UpdateMany */
export const putApiFdDictDataAppBatch = putAppGenericDtoControllerBase5UpdateMany;
/** 此处后端没有提供注释 POST /api/FdDictDataApp/batch */
export async function postAppGenericDtoControllerBase5CreateMany(body: APIModel.CreateFdDictDataDto[], options?: { [key: string]: any }) {
	return request<number>('/api/FdDictDataApp/batch', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postAppGenericDtoControllerBase5CreateMany */
export const postApiFdDictDataAppBatch = postAppGenericDtoControllerBase5CreateMany;
/** 此处后端没有提供注释 DELETE /api/FdDictDataApp/batch */
export async function deleteAppGenericDtoControllerBase5BatchDelete(body: string[], options?: { [key: string]: any }) {
	return request<number>('/api/FdDictDataApp/batch', {
		method: 'DELETE',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteAppGenericDtoControllerBase5BatchDelete */
export const deleteApiFdDictDataAppBatch = deleteAppGenericDtoControllerBase5BatchDelete;
/** 此处后端没有提供注释 PUT /api/FdDictDataApp/batch/updatebycondition */
export async function putAppGenericDtoControllerBase5UpdateManyByCondition(
	body: APIModel.BatchUpdateByConditionDto1UpdateFdDictDataDto,
	options?: { [key: string]: any }
) {
	return request<number>('/api/FdDictDataApp/batch/updatebycondition', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putAppGenericDtoControllerBase5UpdateManyByCondition */
export const putApiFdDictDataAppBatchUpdatebycondition = putAppGenericDtoControllerBase5UpdateManyByCondition;
/** 获取用户相关配置 GET /api/FdDictDataApp/GetUserConfig */
export async function getFdDictDataAppUserConfig(options?: { [key: string]: any }) {
	return request<APIModel.FdDictDataDto[]>('/api/FdDictDataApp/GetUserConfig', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getFdDictDataAppUserConfig */
export const getApiFdDictDataAppGetUserConfig = getFdDictDataAppUserConfig;
/** 此处后端没有提供注释 POST /api/FdDictDataApp/list-by-condition */
export async function postAppGenericDtoControllerBase5GetListByCondition(body: APIModel.QueryByConditionDto, options?: { [key: string]: any }) {
	return request<any>('/api/FdDictDataApp/list-by-condition', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postAppGenericDtoControllerBase5GetListByCondition */
export const postApiFdDictDataAppListByCondition = postAppGenericDtoControllerBase5GetListByCondition;
/** 此处后端没有提供注释 GET /api/FdDictDataApp/page */
export async function getAppGenericDtoControllerBase5GetPage(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAppGenericDtoControllerBase5GetPageParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.PageInfo>('/api/FdDictDataApp/page', {
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
export const getApiFdDictDataAppPage = getAppGenericDtoControllerBase5GetPage;
/** 此处后端没有提供注释 POST /api/FdDictDataApp/page/search */
export async function postAppGenericDtoControllerBase5GetPageByCondition(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/FdDictDataApp/page/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postAppGenericDtoControllerBase5GetPageByCondition */
export const postApiFdDictDataAppPageSearch = postAppGenericDtoControllerBase5GetPageByCondition;
