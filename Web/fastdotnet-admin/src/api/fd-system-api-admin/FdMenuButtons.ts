// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 获取所有记录 检索并返回系统中该类型的所有记录。 GET /api/admin/FdMenuButtons */
/** @deprecated 请使用 getFdMenuButtonsGetAll，原函数名: getApiAdminFdMenuButtons */
export async function getFdMenuButtonsGetAll(options?: { [key: string]: any }) {
	return request<APIModel.FdMenuButtonDto[]>('/api/admin/FdMenuButtons', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getFdMenuButtonsGetAll */
export const getApiAdminFdMenuButtons = getFdMenuButtonsGetAll;
/** 创建新记录 根据提供的数据创建一条新记录。 POST /api/admin/FdMenuButtons */
/** @deprecated 请使用 postFdMenuButtonsCreate，原函数名: postApiAdminFdMenuButtons */
export async function postFdMenuButtonsCreate(body: APIModel.CreateFdFdMenuButtonDto, options?: { [key: string]: any }) {
	return request<APIModel.FdMenuButtonDto>('/api/admin/FdMenuButtons', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postFdMenuButtonsCreate */
export const postApiAdminFdMenuButtons = postFdMenuButtonsCreate;
/** 根据ID获取记录 根据提供的唯一标识符(ID)检索特定记录的详细信息。 GET /api/admin/FdMenuButtons/${param0} */
/** @deprecated 请使用 getFdMenuButtonsGetById，原函数名: getApiAdminFdMenuButtonsId */
export async function getFdMenuButtonsGetById(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getFdMenuButtonsGetByIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<APIModel.FdMenuButtonDto>(`/api/admin/FdMenuButtons/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getFdMenuButtonsGetById */
export const getApiAdminFdMenuButtonsId = getFdMenuButtonsGetById;
/** 更新现有记录 根据提供的ID和更新数据，修改现有记录的信息。 PUT /api/admin/FdMenuButtons/${param0} */
/** @deprecated 请使用 putFdMenuButtonsUpdate，原函数名: putApiAdminFdMenuButtonsId */
export async function putFdMenuButtonsUpdate(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putFdMenuButtonsUpdateParams,
	body: APIModel.UpdateFdFdMenuButtonDto,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<APIModel.FdMenuButtonDto>(`/api/admin/FdMenuButtons/${param0}`, {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		params: { ...queryParams },
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putFdMenuButtonsUpdate */
export const putApiAdminFdMenuButtonsId = putFdMenuButtonsUpdate;
/** 删除记录 根据提供的ID，从系统中移除指定的记录。 DELETE /api/admin/FdMenuButtons/${param0} */
/** @deprecated 请使用 deleteFdMenuButtonsDelete，原函数名: deleteApiAdminFdMenuButtonsId */
export async function deleteFdMenuButtonsDelete(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteFdMenuButtonsDeleteParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/admin/FdMenuButtons/${param0}`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteFdMenuButtonsDelete */
export const deleteApiAdminFdMenuButtonsId = deleteFdMenuButtonsDelete;
/** 根据实体主键批量更新实体信息 根据实体主键批量更新实体信息 PUT /api/admin/FdMenuButtons/batch */
/** @deprecated 请使用 putGenericDtoControllerBase5UpdateMany，原函数名: putApiAdminFdMenuButtonsBatch */
export async function putGenericDtoControllerBase5UpdateMany(body: APIModel.UpdateFdFdMenuButtonDto[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdMenuButtons/batch', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putGenericDtoControllerBase5UpdateMany */
export const putApiAdminFdMenuButtonsBatch = putGenericDtoControllerBase5UpdateMany;
/** 批量创建新记录 根据提供的数据批量创建新记录。 POST /api/admin/FdMenuButtons/batch */
/** @deprecated 请使用 postGenericDtoControllerBase5CreateMany，原函数名: postApiAdminFdMenuButtonsBatch */
export async function postGenericDtoControllerBase5CreateMany(body: APIModel.CreateFdFdMenuButtonDto[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdMenuButtons/batch', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5CreateMany */
export const postApiAdminFdMenuButtonsBatch = postGenericDtoControllerBase5CreateMany;
/** 批量删除记录 根据提供的ID列表，批量删除多条记录。 DELETE /api/admin/FdMenuButtons/batch */
/** @deprecated 请使用 deleteGenericDtoControllerBase5BatchDelete，原函数名: deleteApiAdminFdMenuButtonsBatch */
export async function deleteGenericDtoControllerBase5BatchDelete(body: string[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdMenuButtons/batch', {
		method: 'DELETE',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteGenericDtoControllerBase5BatchDelete */
export const deleteApiAdminFdMenuButtonsBatch = deleteGenericDtoControllerBase5BatchDelete;
/** 根据条件批量更新实体属性（部分字段更新） 根据条件批量更新实体属性（部分字段更新） PUT /api/admin/FdMenuButtons/batch/updatebycondition */
/** @deprecated 请使用 putGenericDtoControllerBase5UpdateManyByCondition，原函数名: putApiAdminFdMenuButtonsBatchUpdatebycondition */
export async function putGenericDtoControllerBase5UpdateManyByCondition(
	body: APIModel.BatchUpdateByConditionDto1UpdateFdFdMenuButtonDto,
	options?: { [key: string]: any }
) {
	return request<number>('/api/admin/FdMenuButtons/batch/updatebycondition', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putGenericDtoControllerBase5UpdateManyByCondition */
export const putApiAdminFdMenuButtonsBatchUpdatebycondition = putGenericDtoControllerBase5UpdateManyByCondition;
/** 根据自定义条件获取列表(不分页) 根据自定义条件获取列表(不分页) POST /api/admin/FdMenuButtons/list-by-condition */
/** @deprecated 请使用 postGenericDtoControllerBase5GetListByCondition，原函数名: postApiAdminFdMenuButtonsListByCondition */
export async function postGenericDtoControllerBase5GetListByCondition(body: APIModel.QueryByConditionDto, options?: { [key: string]: any }) {
	return request<any>('/api/admin/FdMenuButtons/list-by-condition', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5GetListByCondition */
export const postApiAdminFdMenuButtonsListByCondition = postGenericDtoControllerBase5GetListByCondition;
/** 分页获取记录 根据页码和页面大小，分页检索记录。 GET /api/admin/FdMenuButtons/page */
/** @deprecated 请使用 getFdMenuButtonsGetPage，原函数名: getApiAdminFdMenuButtonsPage */
export async function getFdMenuButtonsGetPage(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getFdMenuButtonsGetPageParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.PageInfo>('/api/admin/FdMenuButtons/page', {
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

/** @deprecated 此函数名已变更，请使用 getFdMenuButtonsGetPage */
export const getApiAdminFdMenuButtonsPage = getFdMenuButtonsGetPage;
/** 根据条件分页获取记录 根据提供的查询条件和分页参数，分页检索记录。 POST /api/admin/FdMenuButtons/page/search */
/** @deprecated 请使用 postGenericDtoControllerBase5GetPageByCondition，原函数名: postApiAdminFdMenuButtonsPageSearch */
export async function postGenericDtoControllerBase5GetPageByCondition(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/admin/FdMenuButtons/page/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5GetPageByCondition */
export const postApiAdminFdMenuButtonsPageSearch = postGenericDtoControllerBase5GetPageByCondition;
/** 获取回收站数据 检索并返回已软删除的记录（回收站数据）。 GET /api/admin/FdMenuButtons/recyclebin */
/** @deprecated 请使用 getGenericDtoControllerBase5GetRecycleBin，原函数名: getApiAdminFdMenuButtonsRecyclebin */
export async function getGenericDtoControllerBase5GetRecycleBin(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getGenericDtoControllerBase5GetRecycleBinParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.PageInfo>('/api/admin/FdMenuButtons/recyclebin', {
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

/** @deprecated 此函数名已变更，请使用 getGenericDtoControllerBase5GetRecycleBin */
export const getApiAdminFdMenuButtonsRecyclebin = getGenericDtoControllerBase5GetRecycleBin;
/** 永久删除回收站中的记录 根据提供的ID，将已软删除的记录从数据库中永久移除。 DELETE /api/admin/FdMenuButtons/recyclebin/${param0}/permanent */
/** @deprecated 请使用 deleteGenericDtoControllerBase5PermanentDelete，原函数名: deleteApiAdminFdMenuButtonsRecyclebinIdPermanent */
export async function deleteGenericDtoControllerBase5PermanentDelete(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteGenericDtoControllerBase5PermanentDeleteParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/admin/FdMenuButtons/recyclebin/${param0}/permanent`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteGenericDtoControllerBase5PermanentDelete */
export const deleteApiAdminFdMenuButtonsRecyclebinIdPermanent = deleteGenericDtoControllerBase5PermanentDelete;
/** 恢复回收站中的记录 根据提供的ID，将已软删除的记录恢复到正常状态。 PUT /api/admin/FdMenuButtons/recyclebin/${param0}/restore */
/** @deprecated 请使用 putGenericDtoControllerBase5Restore，原函数名: putApiAdminFdMenuButtonsRecyclebinIdRestore */
export async function putGenericDtoControllerBase5Restore(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putGenericDtoControllerBase5RestoreParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/admin/FdMenuButtons/recyclebin/${param0}/restore`, {
		method: 'PUT',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putGenericDtoControllerBase5Restore */
export const putApiAdminFdMenuButtonsRecyclebinIdRestore = putGenericDtoControllerBase5Restore;
/** 根据条件永久删除回收站中的记录 根据提供的条件，将回收站中符合条件的记录从数据库中永久移除。 POST /api/admin/FdMenuButtons/recyclebin/permanent */
/** @deprecated 请使用 postGenericDtoControllerBase5PermanentDeleteBatch，原函数名: postApiAdminFdMenuButtonsRecyclebinPermanent */
export async function postGenericDtoControllerBase5PermanentDeleteBatch(
	body: APIModel.Expression1Func2FdMenuButton_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral,
	options?: { [key: string]: any }
) {
	return request<number>('/api/admin/FdMenuButtons/recyclebin/permanent', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5PermanentDeleteBatch */
export const postApiAdminFdMenuButtonsRecyclebinPermanent = postGenericDtoControllerBase5PermanentDeleteBatch;
/** 批量恢复回收站中的记录 根据提供的条件，批量将回收站中的记录恢复到正常状态。 POST /api/admin/FdMenuButtons/recyclebin/restore */
/** @deprecated 请使用 postGenericDtoControllerBase5RestoreBatch，原函数名: postApiAdminFdMenuButtonsRecyclebinRestore */
export async function postGenericDtoControllerBase5RestoreBatch(
	body: APIModel.Expression1Func2FdMenuButton_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral,
	options?: { [key: string]: any }
) {
	return request<number>('/api/admin/FdMenuButtons/recyclebin/restore', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5RestoreBatch */
export const postApiAdminFdMenuButtonsRecyclebinRestore = postGenericDtoControllerBase5RestoreBatch;
/** 根据条件查询回收站数据 根据提供的查询条件，检索回收站中的记录。 POST /api/admin/FdMenuButtons/recyclebin/search */
/** @deprecated 请使用 postGenericDtoControllerBase5SearchRecycleBin，原函数名: postApiAdminFdMenuButtonsRecyclebinSearch */
export async function postGenericDtoControllerBase5SearchRecycleBin(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/admin/FdMenuButtons/recyclebin/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5SearchRecycleBin */
export const postApiAdminFdMenuButtonsRecyclebinSearch = postGenericDtoControllerBase5SearchRecycleBin;
