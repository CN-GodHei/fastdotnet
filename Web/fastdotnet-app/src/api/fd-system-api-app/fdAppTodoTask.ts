// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 此处后端没有提供注释 GET /api/app/FdAppTodoTask */
export async function getAppGenericDtoControllerBase5GetAll(options?: { [key: string]: any }) {
	return request<APIModel.FdTodoTaskDto[]>('/api/app/FdAppTodoTask', {
		method: 'GET',
		...(options || {}),
	});
}
/** 此处后端没有提供注释 POST /api/app/FdAppTodoTask */
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
/** 此处后端没有提供注释 GET /api/app/FdAppTodoTask/${param0} */
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
/** 此处后端没有提供注释 PUT /api/app/FdAppTodoTask/${param0} */
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
/** 此处后端没有提供注释 DELETE /api/app/FdAppTodoTask/${param0} */
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
/** 此处后端没有提供注释 PUT /api/app/FdAppTodoTask/batch */
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
/** 此处后端没有提供注释 POST /api/app/FdAppTodoTask/batch */
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
/** 此处后端没有提供注释 DELETE /api/app/FdAppTodoTask/batch */
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
/** 此处后端没有提供注释 PUT /api/app/FdAppTodoTask/batch/updatebycondition */
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
/** 完成待办任务 POST /api/app/FdAppTodoTask/complete/${param0} */
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
/** 此处后端没有提供注释 POST /api/app/FdAppTodoTask/list-by-condition */
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
/** 获取当前用户的待办任务 GET /api/app/FdAppTodoTask/my */
export async function getFdAppTodoTaskGetMyTodoTasks(options?: { [key: string]: any }) {
	return request<APIModel.FdTodoTaskDto[]>('/api/app/FdAppTodoTask/my', {
		method: 'GET',
		...(options || {}),
	});
}
/** 此处后端没有提供注释 GET /api/app/FdAppTodoTask/page */
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
/** 此处后端没有提供注释 POST /api/app/FdAppTodoTask/page/search */
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
