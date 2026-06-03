// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 此处后端没有提供注释 GET /api/app/FdAppNotice */
export async function getAppGenericDtoControllerBase5GetAll(options?: { [key: string]: any }) {
	return request<APIModel.FdNoticeDto[]>('/api/app/FdAppNotice', {
		method: 'GET',
		...(options || {}),
	});
}
/** 此处后端没有提供注释 POST /api/app/FdAppNotice */
export async function postAppGenericDtoControllerBase5Create(body: APIModel.CreateFdNoticeDto, options?: { [key: string]: any }) {
	return request<APIModel.FdNoticeDto>('/api/app/FdAppNotice', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 此处后端没有提供注释 GET /api/app/FdAppNotice/${param0} */
export async function getAppGenericDtoControllerBase5GetById(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAppGenericDtoControllerBase5GetByIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<APIModel.FdNoticeDto>(`/api/app/FdAppNotice/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}
/** 此处后端没有提供注释 PUT /api/app/FdAppNotice/${param0} */
export async function putAppGenericDtoControllerBase5Update(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putAppGenericDtoControllerBase5UpdateParams,
	body: APIModel.UpdateFdNoticeDto,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<APIModel.FdNoticeDto>(`/api/app/FdAppNotice/${param0}`, {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		params: { ...queryParams },
		data: body,
		...(options || {}),
	});
}
/** 此处后端没有提供注释 DELETE /api/app/FdAppNotice/${param0} */
export async function deleteAppGenericDtoControllerBase5Delete(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteAppGenericDtoControllerBase5DeleteParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/app/FdAppNotice/${param0}`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}
/** 此处后端没有提供注释 PUT /api/app/FdAppNotice/batch */
export async function putAppGenericDtoControllerBase5UpdateMany(body: APIModel.UpdateFdNoticeDto[], options?: { [key: string]: any }) {
	return request<number>('/api/app/FdAppNotice/batch', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 此处后端没有提供注释 POST /api/app/FdAppNotice/batch */
export async function postAppGenericDtoControllerBase5CreateMany(body: APIModel.CreateFdNoticeDto[], options?: { [key: string]: any }) {
	return request<number>('/api/app/FdAppNotice/batch', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 此处后端没有提供注释 DELETE /api/app/FdAppNotice/batch */
export async function deleteAppGenericDtoControllerBase5BatchDelete(body: string[], options?: { [key: string]: any }) {
	return request<number>('/api/app/FdAppNotice/batch', {
		method: 'DELETE',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 此处后端没有提供注释 PUT /api/app/FdAppNotice/batch/updatebycondition */
export async function putAppGenericDtoControllerBase5UpdateManyByCondition(
	body: APIModel.BatchUpdateByConditionDto1UpdateFdNoticeDto,
	options?: { [key: string]: any }
) {
	return request<number>('/api/app/FdAppNotice/batch/updatebycondition', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 此处后端没有提供注释 POST /api/app/FdAppNotice/list-by-condition */
export async function postAppGenericDtoControllerBase5GetListByCondition(body: APIModel.QueryByConditionDto, options?: { [key: string]: any }) {
	return request<any>('/api/app/FdAppNotice/list-by-condition', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 获取当前用户的通知 GET /api/app/FdAppNotice/my */
export async function getFdAppNoticeGetMyNotices(options?: { [key: string]: any }) {
	return request<APIModel.FdNoticeDto[]>('/api/app/FdAppNotice/my', {
		method: 'GET',
		...(options || {}),
	});
}
/** 此处后端没有提供注释 GET /api/app/FdAppNotice/page */
export async function getAppGenericDtoControllerBase5GetPage(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAppGenericDtoControllerBase5GetPageParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.PageInfo>('/api/app/FdAppNotice/page', {
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
/** 此处后端没有提供注释 POST /api/app/FdAppNotice/page/search */
export async function postAppGenericDtoControllerBase5GetPageByCondition(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/app/FdAppNotice/page/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}
/** 标记为已读 POST /api/app/FdAppNotice/read/${param0} */
export async function postFdAppNoticeMarkAsRead(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.postFdAppNoticeMarkAsReadParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/app/FdAppNotice/read/${param0}`, {
		method: 'POST',
		params: { ...queryParams },
		...(options || {}),
	});
}
/** 全部标记为已读 POST /api/app/FdAppNotice/readAll */
export async function postFdAppNoticeMarkAllAsRead(options?: { [key: string]: any }) {
	return request<boolean>('/api/app/FdAppNotice/readAll', {
		method: 'POST',
		...(options || {}),
	});
}
