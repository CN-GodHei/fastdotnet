// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 获取所有记录 检索并返回系统中该类型的所有记录。 GET /api/admin/FdNotice */
/** @deprecated 请使用 getGenericDtoControllerBase5GetAll，原函数名: getApiAdminFdNotice */
export async function getGenericDtoControllerBase5GetAll(options?: { [key: string]: any }) {
	return request<APIModel.FdNoticeDto[]>('/api/admin/FdNotice', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getGenericDtoControllerBase5GetAll */
export const getApiAdminFdNotice = getGenericDtoControllerBase5GetAll;
/** 创建新记录 根据提供的数据创建一条新记录。 POST /api/admin/FdNotice */
/** @deprecated 请使用 postGenericDtoControllerBase5Create，原函数名: postApiAdminFdNotice */
export async function postGenericDtoControllerBase5Create(body: APIModel.CreateFdNoticeDto, options?: { [key: string]: any }) {
	return request<APIModel.FdNoticeDto>('/api/admin/FdNotice', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5Create */
export const postApiAdminFdNotice = postGenericDtoControllerBase5Create;
/** 根据ID获取记录 根据提供的唯一标识符(ID)检索特定记录的详细信息。 GET /api/admin/FdNotice/${param0} */
/** @deprecated 请使用 getGenericDtoControllerBase5GetById，原函数名: getApiAdminFdNoticeId */
export async function getGenericDtoControllerBase5GetById(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getGenericDtoControllerBase5GetByIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<APIModel.FdNoticeDto>(`/api/admin/FdNotice/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getGenericDtoControllerBase5GetById */
export const getApiAdminFdNoticeId = getGenericDtoControllerBase5GetById;
/** 更新现有记录 根据提供的ID和更新数据，修改现有记录的信息。 PUT /api/admin/FdNotice/${param0} */
/** @deprecated 请使用 putGenericDtoControllerBase5Update，原函数名: putApiAdminFdNoticeId */
export async function putGenericDtoControllerBase5Update(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putGenericDtoControllerBase5UpdateParams,
	body: APIModel.UpdateFdNoticeDto,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<APIModel.FdNoticeDto>(`/api/admin/FdNotice/${param0}`, {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		params: { ...queryParams },
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putGenericDtoControllerBase5Update */
export const putApiAdminFdNoticeId = putGenericDtoControllerBase5Update;
/** 删除记录 根据提供的ID，从系统中移除指定的记录。 DELETE /api/admin/FdNotice/${param0} */
/** @deprecated 请使用 deleteGenericDtoControllerBase5Delete，原函数名: deleteApiAdminFdNoticeId */
export async function deleteGenericDtoControllerBase5Delete(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteGenericDtoControllerBase5DeleteParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/admin/FdNotice/${param0}`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteGenericDtoControllerBase5Delete */
export const deleteApiAdminFdNoticeId = deleteGenericDtoControllerBase5Delete;
/** 根据实体主键批量更新实体信息 根据实体主键批量更新实体信息 PUT /api/admin/FdNotice/batch */
/** @deprecated 请使用 putGenericDtoControllerBase5UpdateMany，原函数名: putApiAdminFdNoticeBatch */
export async function putGenericDtoControllerBase5UpdateMany(body: APIModel.UpdateFdNoticeDto[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdNotice/batch', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putGenericDtoControllerBase5UpdateMany */
export const putApiAdminFdNoticeBatch = putGenericDtoControllerBase5UpdateMany;
/** 批量创建新记录 根据提供的数据批量创建新记录。 POST /api/admin/FdNotice/batch */
/** @deprecated 请使用 postGenericDtoControllerBase5CreateMany，原函数名: postApiAdminFdNoticeBatch */
export async function postGenericDtoControllerBase5CreateMany(body: APIModel.CreateFdNoticeDto[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdNotice/batch', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5CreateMany */
export const postApiAdminFdNoticeBatch = postGenericDtoControllerBase5CreateMany;
/** 批量删除记录 根据提供的ID列表，批量删除多条记录。 DELETE /api/admin/FdNotice/batch */
/** @deprecated 请使用 deleteGenericDtoControllerBase5BatchDelete，原函数名: deleteApiAdminFdNoticeBatch */
export async function deleteGenericDtoControllerBase5BatchDelete(body: string[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdNotice/batch', {
		method: 'DELETE',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteGenericDtoControllerBase5BatchDelete */
export const deleteApiAdminFdNoticeBatch = deleteGenericDtoControllerBase5BatchDelete;
/** 根据条件批量更新实体属性（部分字段更新） 根据条件批量更新实体属性（部分字段更新） PUT /api/admin/FdNotice/batch/updatebycondition */
/** @deprecated 请使用 putGenericDtoControllerBase5UpdateManyByCondition，原函数名: putApiAdminFdNoticeBatchUpdatebycondition */
export async function putGenericDtoControllerBase5UpdateManyByCondition(
	body: APIModel.BatchUpdateByConditionDto1UpdateFdNoticeDto,
	options?: { [key: string]: any }
) {
	return request<number>('/api/admin/FdNotice/batch/updatebycondition', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putGenericDtoControllerBase5UpdateManyByCondition */
export const putApiAdminFdNoticeBatchUpdatebycondition = putGenericDtoControllerBase5UpdateManyByCondition;
/** 根据自定义条件获取列表(不分页) 根据自定义条件获取列表(不分页) POST /api/admin/FdNotice/list-by-condition */
/** @deprecated 请使用 postGenericDtoControllerBase5GetListByCondition，原函数名: postApiAdminFdNoticeListByCondition */
export async function postGenericDtoControllerBase5GetListByCondition(body: APIModel.QueryByConditionDto, options?: { [key: string]: any }) {
	return request<any>('/api/admin/FdNotice/list-by-condition', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5GetListByCondition */
export const postApiAdminFdNoticeListByCondition = postGenericDtoControllerBase5GetListByCondition;
/** 获取当前用户的通知 GET /api/admin/FdNotice/my */
/** @deprecated 请使用 getFdNoticeGetMyNotices，原函数名: getApiAdminFdNoticeMy */
export async function getFdNoticeGetMyNotices(options?: { [key: string]: any }) {
	return request<APIModel.FdNoticeDto[]>('/api/admin/FdNotice/my', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getFdNoticeGetMyNotices */
export const getApiAdminFdNoticeMy = getFdNoticeGetMyNotices;
/** 分页获取记录 根据页码和页面大小，分页检索记录。 GET /api/admin/FdNotice/page */
/** @deprecated 请使用 getGenericDtoControllerBase5GetPage，原函数名: getApiAdminFdNoticePage */
export async function getGenericDtoControllerBase5GetPage(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getGenericDtoControllerBase5GetPageParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.PageInfo>('/api/admin/FdNotice/page', {
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

/** @deprecated 此函数名已变更，请使用 getGenericDtoControllerBase5GetPage */
export const getApiAdminFdNoticePage = getGenericDtoControllerBase5GetPage;
/** 根据条件分页获取记录 根据提供的查询条件和分页参数，分页检索记录。 POST /api/admin/FdNotice/page/search */
/** @deprecated 请使用 postGenericDtoControllerBase5GetPageByCondition，原函数名: postApiAdminFdNoticePageSearch */
export async function postGenericDtoControllerBase5GetPageByCondition(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/admin/FdNotice/page/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5GetPageByCondition */
export const postApiAdminFdNoticePageSearch = postGenericDtoControllerBase5GetPageByCondition;
/** 标记为已读 POST /api/admin/FdNotice/read/${param0} */
/** @deprecated 请使用 postFdNoticeMarkAsRead，原函数名: postApiAdminFdNoticeReadId */
export async function postFdNoticeMarkAsRead(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.postFdNoticeMarkAsReadParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/admin/FdNotice/read/${param0}`, {
		method: 'POST',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postFdNoticeMarkAsRead */
export const postApiAdminFdNoticeReadId = postFdNoticeMarkAsRead;
/** 全部标记为已读 POST /api/admin/FdNotice/readAll */
/** @deprecated 请使用 postFdNoticeMarkAllAsRead，原函数名: postApiAdminFdNoticeReadAll */
export async function postFdNoticeMarkAllAsRead(options?: { [key: string]: any }) {
	return request<boolean>('/api/admin/FdNotice/readAll', {
		method: 'POST',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postFdNoticeMarkAllAsRead */
export const postApiAdminFdNoticeReadAll = postFdNoticeMarkAllAsRead;
/** 获取回收站数据 检索并返回已软删除的记录（回收站数据）。 GET /api/admin/FdNotice/recyclebin */
/** @deprecated 请使用 getGenericDtoControllerBase5GetRecycleBin，原函数名: getApiAdminFdNoticeRecyclebin */
export async function getGenericDtoControllerBase5GetRecycleBin(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getGenericDtoControllerBase5GetRecycleBinParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.PageInfo>('/api/admin/FdNotice/recyclebin', {
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
export const getApiAdminFdNoticeRecyclebin = getGenericDtoControllerBase5GetRecycleBin;
/** 永久删除回收站中的记录 根据提供的ID，将已软删除的记录从数据库中永久移除。 DELETE /api/admin/FdNotice/recyclebin/${param0}/permanent */
/** @deprecated 请使用 deleteGenericDtoControllerBase5PermanentDelete，原函数名: deleteApiAdminFdNoticeRecyclebinIdPermanent */
export async function deleteGenericDtoControllerBase5PermanentDelete(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteGenericDtoControllerBase5PermanentDeleteParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/admin/FdNotice/recyclebin/${param0}/permanent`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteGenericDtoControllerBase5PermanentDelete */
export const deleteApiAdminFdNoticeRecyclebinIdPermanent = deleteGenericDtoControllerBase5PermanentDelete;
/** 恢复回收站中的记录 根据提供的ID，将已软删除的记录恢复到正常状态。 PUT /api/admin/FdNotice/recyclebin/${param0}/restore */
/** @deprecated 请使用 putGenericDtoControllerBase5Restore，原函数名: putApiAdminFdNoticeRecyclebinIdRestore */
export async function putGenericDtoControllerBase5Restore(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putGenericDtoControllerBase5RestoreParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/admin/FdNotice/recyclebin/${param0}/restore`, {
		method: 'PUT',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putGenericDtoControllerBase5Restore */
export const putApiAdminFdNoticeRecyclebinIdRestore = putGenericDtoControllerBase5Restore;
/** 根据条件永久删除回收站中的记录 根据提供的条件，将回收站中符合条件的记录从数据库中永久移除。 POST /api/admin/FdNotice/recyclebin/permanent */
/** @deprecated 请使用 postGenericDtoControllerBase5PermanentDeleteBatch，原函数名: postApiAdminFdNoticeRecyclebinPermanent */
export async function postGenericDtoControllerBase5PermanentDeleteBatch(
	body: APIModel.Expression1Func2FdNotice_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral,
	options?: { [key: string]: any }
) {
	return request<number>('/api/admin/FdNotice/recyclebin/permanent', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5PermanentDeleteBatch */
export const postApiAdminFdNoticeRecyclebinPermanent = postGenericDtoControllerBase5PermanentDeleteBatch;
/** 批量恢复回收站中的记录 根据提供的条件，批量将回收站中的记录恢复到正常状态。 POST /api/admin/FdNotice/recyclebin/restore */
/** @deprecated 请使用 postGenericDtoControllerBase5RestoreBatch，原函数名: postApiAdminFdNoticeRecyclebinRestore */
export async function postGenericDtoControllerBase5RestoreBatch(
	body: APIModel.Expression1Func2FdNotice_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral,
	options?: { [key: string]: any }
) {
	return request<number>('/api/admin/FdNotice/recyclebin/restore', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5RestoreBatch */
export const postApiAdminFdNoticeRecyclebinRestore = postGenericDtoControllerBase5RestoreBatch;
/** 根据条件查询回收站数据 根据提供的查询条件，检索回收站中的记录。 POST /api/admin/FdNotice/recyclebin/search */
/** @deprecated 请使用 postGenericDtoControllerBase5SearchRecycleBin，原函数名: postApiAdminFdNoticeRecyclebinSearch */
export async function postGenericDtoControllerBase5SearchRecycleBin(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/admin/FdNotice/recyclebin/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5SearchRecycleBin */
export const postApiAdminFdNoticeRecyclebinSearch = postGenericDtoControllerBase5SearchRecycleBin;
