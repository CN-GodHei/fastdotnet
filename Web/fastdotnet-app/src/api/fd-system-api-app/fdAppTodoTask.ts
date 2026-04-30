// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 此处后端没有提供注释 GET /api/app/FdAppTodoTask */
/** @deprecated 请使用 getAppGenericDtoControllerBase5GetAll，原函数名: getApiAppFdAppTodoTask */
export async function getAppGenericDtoControllerBase5GetAll(options?: { [key: string]: any }) {
	return request<APIModel.FdTodoTaskDto[]>('/api/app/FdAppTodoTask', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getAppGenericDtoControllerBase5GetAll */
export const getApiAppFdAppTodoTask = getAppGenericDtoControllerBase5GetAll;
/** 此处后端没有提供注释 POST /api/app/FdAppTodoTask */
/** @deprecated 请使用 postAppGenericDtoControllerBase5Create，原函数名: postApiAppFdAppTodoTask */
export async function postAppGenericDtoControllerBase5Create(body: APIModel.CreateFdTodoTaskDto, options?: { [key: string]: any }) {
	return request<APIModel.FdTodoTaskDto>('/api/app/FdAppTodoTask', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postAppGenericDtoControllerBase5Create */
export const postApiAppFdAppTodoTask = postAppGenericDtoControllerBase5Create;
/** 此处后端没有提供注释 GET /api/app/FdAppTodoTask/${param0} */
/** @deprecated 请使用 getAppGenericDtoControllerBase5GetById，原函数名: getApiAppFdAppTodoTaskId */
export async function getAppGenericDtoControllerBase5GetById(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAppGenericDtoControllerBase5GetByIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<APIModel.FdTodoTaskDto>(`/api/app/FdAppTodoTask/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getAppGenericDtoControllerBase5GetById */
export const getApiAppFdAppTodoTaskId = getAppGenericDtoControllerBase5GetById;
/** 此处后端没有提供注释 PUT /api/app/FdAppTodoTask/${param0} */
/** @deprecated 请使用 putAppGenericDtoControllerBase5Update，原函数名: putApiAppFdAppTodoTaskId */
export async function putAppGenericDtoControllerBase5Update(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putAppGenericDtoControllerBase5UpdateParams,
	body: APIModel.UpdateFdTodoTaskDto,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<APIModel.FdTodoTaskDto>(`/api/app/FdAppTodoTask/${param0}`, {
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
export const putApiAppFdAppTodoTaskId = putAppGenericDtoControllerBase5Update;
/** 此处后端没有提供注释 DELETE /api/app/FdAppTodoTask/${param0} */
/** @deprecated 请使用 deleteAppGenericDtoControllerBase5Delete，原函数名: deleteApiAppFdAppTodoTaskId */
export async function deleteAppGenericDtoControllerBase5Delete(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteAppGenericDtoControllerBase5DeleteParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/app/FdAppTodoTask/${param0}`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteAppGenericDtoControllerBase5Delete */
export const deleteApiAppFdAppTodoTaskId = deleteAppGenericDtoControllerBase5Delete;
/** 此处后端没有提供注释 PUT /api/app/FdAppTodoTask/batch */
/** @deprecated 请使用 putAppGenericDtoControllerBase5UpdateMany，原函数名: putApiAppFdAppTodoTaskBatch */
export async function putAppGenericDtoControllerBase5UpdateMany(body: APIModel.UpdateFdTodoTaskDto[], options?: { [key: string]: any }) {
	return request<number>('/api/app/FdAppTodoTask/batch', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putAppGenericDtoControllerBase5UpdateMany */
export const putApiAppFdAppTodoTaskBatch = putAppGenericDtoControllerBase5UpdateMany;
/** 此处后端没有提供注释 POST /api/app/FdAppTodoTask/batch */
/** @deprecated 请使用 postAppGenericDtoControllerBase5CreateMany，原函数名: postApiAppFdAppTodoTaskBatch */
export async function postAppGenericDtoControllerBase5CreateMany(body: APIModel.CreateFdTodoTaskDto[], options?: { [key: string]: any }) {
	return request<number>('/api/app/FdAppTodoTask/batch', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postAppGenericDtoControllerBase5CreateMany */
export const postApiAppFdAppTodoTaskBatch = postAppGenericDtoControllerBase5CreateMany;
/** 此处后端没有提供注释 DELETE /api/app/FdAppTodoTask/batch */
/** @deprecated 请使用 deleteAppGenericDtoControllerBase5BatchDelete，原函数名: deleteApiAppFdAppTodoTaskBatch */
export async function deleteAppGenericDtoControllerBase5BatchDelete(body: string[], options?: { [key: string]: any }) {
	return request<number>('/api/app/FdAppTodoTask/batch', {
		method: 'DELETE',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteAppGenericDtoControllerBase5BatchDelete */
export const deleteApiAppFdAppTodoTaskBatch = deleteAppGenericDtoControllerBase5BatchDelete;
/** 此处后端没有提供注释 PUT /api/app/FdAppTodoTask/batch/updatebycondition */
/** @deprecated 请使用 putAppGenericDtoControllerBase5UpdateManyByCondition，原函数名: putApiAppFdAppTodoTaskBatchUpdatebycondition */
export async function putAppGenericDtoControllerBase5UpdateManyByCondition(
	body: APIModel.BatchUpdateByConditionDto1UpdateFdTodoTaskDto,
	options?: { [key: string]: any }
) {
	return request<number>('/api/app/FdAppTodoTask/batch/updatebycondition', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putAppGenericDtoControllerBase5UpdateManyByCondition */
export const putApiAppFdAppTodoTaskBatchUpdatebycondition = putAppGenericDtoControllerBase5UpdateManyByCondition;
/** 完成待办任务 POST /api/app/FdAppTodoTask/complete/${param0} */
/** @deprecated 请使用 postFdAppTodoTaskComplete，原函数名: postApiAppFdAppTodoTaskCompleteId */
export async function postFdAppTodoTaskComplete(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.postFdAppTodoTaskCompleteParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/app/FdAppTodoTask/complete/${param0}`, {
		method: 'POST',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postFdAppTodoTaskComplete */
export const postApiAppFdAppTodoTaskCompleteId = postFdAppTodoTaskComplete;
/** 此处后端没有提供注释 POST /api/app/FdAppTodoTask/list-by-condition */
/** @deprecated 请使用 postAppGenericDtoControllerBase5GetListByCondition，原函数名: postApiAppFdAppTodoTaskListByCondition */
export async function postAppGenericDtoControllerBase5GetListByCondition(body: APIModel.QueryByConditionDto, options?: { [key: string]: any }) {
	return request<any>('/api/app/FdAppTodoTask/list-by-condition', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postAppGenericDtoControllerBase5GetListByCondition */
export const postApiAppFdAppTodoTaskListByCondition = postAppGenericDtoControllerBase5GetListByCondition;
/** 获取当前用户的待办任务 GET /api/app/FdAppTodoTask/my */
/** @deprecated 请使用 getFdAppTodoTaskGetMyTodoTasks，原函数名: getApiAppFdAppTodoTaskMy */
export async function getFdAppTodoTaskGetMyTodoTasks(options?: { [key: string]: any }) {
	return request<APIModel.FdTodoTaskDto[]>('/api/app/FdAppTodoTask/my', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getFdAppTodoTaskGetMyTodoTasks */
export const getApiAppFdAppTodoTaskMy = getFdAppTodoTaskGetMyTodoTasks;
/** 此处后端没有提供注释 GET /api/app/FdAppTodoTask/page */
/** @deprecated 请使用 getAppGenericDtoControllerBase5GetPage，原函数名: getApiAppFdAppTodoTaskPage */
export async function getAppGenericDtoControllerBase5GetPage(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAppGenericDtoControllerBase5GetPageParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.PageInfo>('/api/app/FdAppTodoTask/page', {
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
export const getApiAppFdAppTodoTaskPage = getAppGenericDtoControllerBase5GetPage;
/** 此处后端没有提供注释 POST /api/app/FdAppTodoTask/page/search */
/** @deprecated 请使用 postAppGenericDtoControllerBase5GetPageByCondition，原函数名: postApiAppFdAppTodoTaskPageSearch */
export async function postAppGenericDtoControllerBase5GetPageByCondition(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/app/FdAppTodoTask/page/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postAppGenericDtoControllerBase5GetPageByCondition */
export const postApiAppFdAppTodoTaskPageSearch = postAppGenericDtoControllerBase5GetPageByCondition;
