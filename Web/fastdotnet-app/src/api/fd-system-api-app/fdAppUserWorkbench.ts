// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 此处后端没有提供注释 GET /api/app/FdAppUserWorkbench */
/** @deprecated 请使用 getAppGenericDtoControllerBase5GetAll，原函数名: getApiAppFdAppUserWorkbench */
export async function getAppGenericDtoControllerBase5GetAll(options?: { [key: string]: any }) {
	return request<APIModel.FdUserLayoutDto[]>('/api/app/FdAppUserWorkbench', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getAppGenericDtoControllerBase5GetAll */
export const getApiAppFdAppUserWorkbench = getAppGenericDtoControllerBase5GetAll;
/** 此处后端没有提供注释 POST /api/app/FdAppUserWorkbench */
/** @deprecated 请使用 postAppGenericDtoControllerBase5Create，原函数名: postApiAppFdAppUserWorkbench */
export async function postAppGenericDtoControllerBase5Create(body: APIModel.SaveFdUserLayoutDto, options?: { [key: string]: any }) {
	return request<APIModel.FdUserLayoutDto>('/api/app/FdAppUserWorkbench', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postAppGenericDtoControllerBase5Create */
export const postApiAppFdAppUserWorkbench = postAppGenericDtoControllerBase5Create;
/** 此处后端没有提供注释 GET /api/app/FdAppUserWorkbench/${param0} */
/** @deprecated 请使用 getAppGenericDtoControllerBase5GetById，原函数名: getApiAppFdAppUserWorkbenchId */
export async function getAppGenericDtoControllerBase5GetById(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAppGenericDtoControllerBase5GetByIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<APIModel.FdUserLayoutDto>(`/api/app/FdAppUserWorkbench/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getAppGenericDtoControllerBase5GetById */
export const getApiAppFdAppUserWorkbenchId = getAppGenericDtoControllerBase5GetById;
/** 此处后端没有提供注释 PUT /api/app/FdAppUserWorkbench/${param0} */
/** @deprecated 请使用 putAppGenericDtoControllerBase5Update，原函数名: putApiAppFdAppUserWorkbenchId */
export async function putAppGenericDtoControllerBase5Update(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putAppGenericDtoControllerBase5UpdateParams,
	body: APIModel.UpdateFdUserLayoutDto,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<APIModel.FdUserLayoutDto>(`/api/app/FdAppUserWorkbench/${param0}`, {
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
export const putApiAppFdAppUserWorkbenchId = putAppGenericDtoControllerBase5Update;
/** 此处后端没有提供注释 DELETE /api/app/FdAppUserWorkbench/${param0} */
/** @deprecated 请使用 deleteAppGenericDtoControllerBase5Delete，原函数名: deleteApiAppFdAppUserWorkbenchId */
export async function deleteAppGenericDtoControllerBase5Delete(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteAppGenericDtoControllerBase5DeleteParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/app/FdAppUserWorkbench/${param0}`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteAppGenericDtoControllerBase5Delete */
export const deleteApiAppFdAppUserWorkbenchId = deleteAppGenericDtoControllerBase5Delete;
/** 获取我可用的卡片列表 (已授权) GET /api/app/FdAppUserWorkbench/available-cards */
/** @deprecated 请使用 getFdAppUserWorkbenchGetAvailableCards，原函数名: getApiAppFdAppUserWorkbenchAvailableCards */
export async function getFdAppUserWorkbenchGetAvailableCards(options?: { [key: string]: any }) {
	return request<APIModel.FdWorkbenchCardDto[]>('/api/app/FdAppUserWorkbench/available-cards', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getFdAppUserWorkbenchGetAvailableCards */
export const getApiAppFdAppUserWorkbenchAvailableCards = getFdAppUserWorkbenchGetAvailableCards;
/** 此处后端没有提供注释 PUT /api/app/FdAppUserWorkbench/batch */
/** @deprecated 请使用 putAppGenericDtoControllerBase5UpdateMany，原函数名: putApiAppFdAppUserWorkbenchBatch */
export async function putAppGenericDtoControllerBase5UpdateMany(body: APIModel.UpdateFdUserLayoutDto[], options?: { [key: string]: any }) {
	return request<number>('/api/app/FdAppUserWorkbench/batch', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putAppGenericDtoControllerBase5UpdateMany */
export const putApiAppFdAppUserWorkbenchBatch = putAppGenericDtoControllerBase5UpdateMany;
/** 此处后端没有提供注释 POST /api/app/FdAppUserWorkbench/batch */
/** @deprecated 请使用 postAppGenericDtoControllerBase5CreateMany，原函数名: postApiAppFdAppUserWorkbenchBatch */
export async function postAppGenericDtoControllerBase5CreateMany(body: APIModel.SaveFdUserLayoutDto[], options?: { [key: string]: any }) {
	return request<number>('/api/app/FdAppUserWorkbench/batch', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postAppGenericDtoControllerBase5CreateMany */
export const postApiAppFdAppUserWorkbenchBatch = postAppGenericDtoControllerBase5CreateMany;
/** 此处后端没有提供注释 DELETE /api/app/FdAppUserWorkbench/batch */
/** @deprecated 请使用 deleteAppGenericDtoControllerBase5BatchDelete，原函数名: deleteApiAppFdAppUserWorkbenchBatch */
export async function deleteAppGenericDtoControllerBase5BatchDelete(body: string[], options?: { [key: string]: any }) {
	return request<number>('/api/app/FdAppUserWorkbench/batch', {
		method: 'DELETE',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteAppGenericDtoControllerBase5BatchDelete */
export const deleteApiAppFdAppUserWorkbenchBatch = deleteAppGenericDtoControllerBase5BatchDelete;
/** 此处后端没有提供注释 PUT /api/app/FdAppUserWorkbench/batch/updatebycondition */
/** @deprecated 请使用 putAppGenericDtoControllerBase5UpdateManyByCondition，原函数名: putApiAppFdAppUserWorkbenchBatchUpdatebycondition */
export async function putAppGenericDtoControllerBase5UpdateManyByCondition(
	body: APIModel.BatchUpdateByConditionDto1UpdateFdUserLayoutDto,
	options?: { [key: string]: any }
) {
	return request<number>('/api/app/FdAppUserWorkbench/batch/updatebycondition', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putAppGenericDtoControllerBase5UpdateManyByCondition */
export const putApiAppFdAppUserWorkbenchBatchUpdatebycondition = putAppGenericDtoControllerBase5UpdateManyByCondition;
/** 此处后端没有提供注释 POST /api/app/FdAppUserWorkbench/list-by-condition */
/** @deprecated 请使用 postAppGenericDtoControllerBase5GetListByCondition，原函数名: postApiAppFdAppUserWorkbenchListByCondition */
export async function postAppGenericDtoControllerBase5GetListByCondition(body: APIModel.QueryByConditionDto, options?: { [key: string]: any }) {
	return request<any>('/api/app/FdAppUserWorkbench/list-by-condition', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postAppGenericDtoControllerBase5GetListByCondition */
export const postApiAppFdAppUserWorkbenchListByCondition = postAppGenericDtoControllerBase5GetListByCondition;
/** 获取我的当前工作台布局 GET /api/app/FdAppUserWorkbench/my-layout */
/** @deprecated 请使用 getFdAppUserWorkbenchGetMyLayout，原函数名: getApiAppFdAppUserWorkbenchMyLayout */
export async function getFdAppUserWorkbenchGetMyLayout(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getFdAppUserWorkbenchGetMyLayoutParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.FdUserLayoutDto>('/api/app/FdAppUserWorkbench/my-layout', {
		method: 'GET',
		params: {
			// name has a default value: Default
			name: 'Default',
			...params,
		},
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getFdAppUserWorkbenchGetMyLayout */
export const getApiAppFdAppUserWorkbenchMyLayout = getFdAppUserWorkbenchGetMyLayout;
/** 此处后端没有提供注释 GET /api/app/FdAppUserWorkbench/page */
/** @deprecated 请使用 getAppGenericDtoControllerBase5GetPage，原函数名: getApiAppFdAppUserWorkbenchPage */
export async function getAppGenericDtoControllerBase5GetPage(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAppGenericDtoControllerBase5GetPageParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.PageInfo>('/api/app/FdAppUserWorkbench/page', {
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
export const getApiAppFdAppUserWorkbenchPage = getAppGenericDtoControllerBase5GetPage;
/** 此处后端没有提供注释 POST /api/app/FdAppUserWorkbench/page/search */
/** @deprecated 请使用 postAppGenericDtoControllerBase5GetPageByCondition，原函数名: postApiAppFdAppUserWorkbenchPageSearch */
export async function postAppGenericDtoControllerBase5GetPageByCondition(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/app/FdAppUserWorkbench/page/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postAppGenericDtoControllerBase5GetPageByCondition */
export const postApiAppFdAppUserWorkbenchPageSearch = postAppGenericDtoControllerBase5GetPageByCondition;
/** 保存我的工作台布局 (自定义业务逻辑) POST /api/app/FdAppUserWorkbench/save-layout */
/** @deprecated 请使用 postFdAppUserWorkbenchSaveLayout，原函数名: postApiAppFdAppUserWorkbenchSaveLayout */
export async function postFdAppUserWorkbenchSaveLayout(body: APIModel.SaveFdUserLayoutDto, options?: { [key: string]: any }) {
	return request<boolean>('/api/app/FdAppUserWorkbench/save-layout', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postFdAppUserWorkbenchSaveLayout */
export const postApiAppFdAppUserWorkbenchSaveLayout = postFdAppUserWorkbenchSaveLayout;
