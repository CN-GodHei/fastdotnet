// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 此处后端没有提供注释 GET /api/app/FdAppUserWorkbench */
export async function getAppGenericDtoControllerBase5GetAll(options?: { [key: string]: any }) {
	return request<APIModel.FdUserLayoutDto[]>('/api/app/FdAppUserWorkbench', {
		method: 'GET',
		...(options || {}),
	});
}
/** 此处后端没有提供注释 POST /api/app/FdAppUserWorkbench */
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
/** 此处后端没有提供注释 GET /api/app/FdAppUserWorkbench/${param0} */
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
/** 此处后端没有提供注释 PUT /api/app/FdAppUserWorkbench/${param0} */
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
/** 此处后端没有提供注释 DELETE /api/app/FdAppUserWorkbench/${param0} */
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
/** 获取我可用的卡片列表 (已授权) GET /api/app/FdAppUserWorkbench/available-cards */
export async function getFdAppUserWorkbenchGetAvailableCards(options?: { [key: string]: any }) {
	return request<APIModel.FdWorkbenchCardDto[]>('/api/app/FdAppUserWorkbench/available-cards', {
		method: 'GET',
		...(options || {}),
	});
}
/** 此处后端没有提供注释 PUT /api/app/FdAppUserWorkbench/batch */
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
/** 此处后端没有提供注释 POST /api/app/FdAppUserWorkbench/batch */
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
/** 此处后端没有提供注释 DELETE /api/app/FdAppUserWorkbench/batch */
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
/** 此处后端没有提供注释 PUT /api/app/FdAppUserWorkbench/batch/updatebycondition */
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
/** 此处后端没有提供注释 POST /api/app/FdAppUserWorkbench/list-by-condition */
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
/** 获取我的当前工作台布局 GET /api/app/FdAppUserWorkbench/my-layout */
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
/** 此处后端没有提供注释 GET /api/app/FdAppUserWorkbench/page */
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
/** 此处后端没有提供注释 POST /api/app/FdAppUserWorkbench/page/search */
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
/** 保存我的工作台布局 (自定义业务逻辑) POST /api/app/FdAppUserWorkbench/save-layout */
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
