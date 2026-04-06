// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 获取所有记录 检索并返回系统中该类型的所有记录。 GET /api/FdNationalStandardItem */
export async function getApiFdNationalStandardItem(options?: { [key: string]: any }) {
	return request<APIModel.FdNationalStandardItemDto[]>('/api/FdNationalStandardItem', {
		method: 'GET',
		...(options || {}),
	});
}
/** 创建新记录 根据提供的数据创建一条新记录。 POST /api/FdNationalStandardItem */
export async function postApiFdNationalStandardItem(body: APIModel.CreateFdNationalStandardItemDto, options?: { [key: string]: any }) {
	return request<APIModel.FdNationalStandardItemDto>('/api/FdNationalStandardItem', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 根据ID获取记录 根据提供的唯一标识符(ID)检索特定记录的详细信息。 GET /api/FdNationalStandardItem/${param0} */
export async function getApiFdNationalStandardItemId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getApiFdNationalStandardItemIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<APIModel.FdNationalStandardItemDto>(`/api/FdNationalStandardItem/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}
/** 更新现有记录 根据提供的ID和更新数据，修改现有记录的信息。 PUT /api/FdNationalStandardItem/${param0} */
export async function putApiFdNationalStandardItemId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putApiFdNationalStandardItemIdParams,
	body: APIModel.UpdateFdNationalStandardItemDto,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<APIModel.FdNationalStandardItemDto>(`/api/FdNationalStandardItem/${param0}`, {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		params: { ...queryParams },
		data: body,
		...(options || {}),
	});
}
/** 删除记录 根据提供的ID，从系统中移除指定的记录。 DELETE /api/FdNationalStandardItem/${param0} */
export async function deleteApiFdNationalStandardItemId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteApiFdNationalStandardItemIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/FdNationalStandardItem/${param0}`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}
/** 根据实体主键批量更新实体信息 根据实体主键批量更新实体信息 PUT /api/FdNationalStandardItem/batch */
export async function putApiFdNationalStandardItemBatch(body: APIModel.UpdateFdNationalStandardItemDto[], options?: { [key: string]: any }) {
	return request<number>('/api/FdNationalStandardItem/batch', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 批量创建新记录 根据提供的数据批量创建新记录。 POST /api/FdNationalStandardItem/batch */
export async function postApiFdNationalStandardItemBatch(body: APIModel.CreateFdNationalStandardItemDto[], options?: { [key: string]: any }) {
	return request<number>('/api/FdNationalStandardItem/batch', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 批量删除记录 根据提供的ID列表，批量删除多条记录。 DELETE /api/FdNationalStandardItem/batch */
export async function deleteApiFdNationalStandardItemBatch(body: string[], options?: { [key: string]: any }) {
	return request<number>('/api/FdNationalStandardItem/batch', {
		method: 'DELETE',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 根据条件批量更新实体属性（部分字段更新） 根据条件批量更新实体属性（部分字段更新） PUT /api/FdNationalStandardItem/batch/updatebycondition */
export async function putApiFdNationalStandardItemBatchUpdatebycondition(
	body: APIModel.BatchUpdateByConditionDto1UpdateFdNationalStandardItemDto,
	options?: { [key: string]: any }
) {
	return request<number>('/api/FdNationalStandardItem/batch/updatebycondition', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 根据自定义条件获取列表(不分页) 根据自定义条件获取列表(不分页) POST /api/FdNationalStandardItem/list-by-condition */
export async function postApiFdNationalStandardItemListByCondition(body: APIModel.QueryByConditionDto, options?: { [key: string]: any }) {
	return request<any>('/api/FdNationalStandardItem/list-by-condition', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 分页获取记录 根据页码和页面大小，分页检索记录。 GET /api/FdNationalStandardItem/page */
export async function getApiFdNationalStandardItemPage(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getApiFdNationalStandardItemPageParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.PageInfo>('/api/FdNationalStandardItem/page', {
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
/** 根据条件分页获取记录 根据提供的查询条件和分页参数，分页检索记录。 POST /api/FdNationalStandardItem/page/search */
export async function postApiFdNationalStandardItemPageSearch(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/FdNationalStandardItem/page/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}
/** 获取回收站数据 检索并返回已软删除的记录（回收站数据）。 GET /api/FdNationalStandardItem/recyclebin */
export async function getApiFdNationalStandardItemRecyclebin(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getApiFdNationalStandardItemRecyclebinParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.PageInfo>('/api/FdNationalStandardItem/recyclebin', {
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
/** 永久删除回收站中的记录 根据提供的ID，将已软删除的记录从数据库中永久移除。 DELETE /api/FdNationalStandardItem/recyclebin/${param0}/permanent */
export async function deleteApiFdNationalStandardItemRecyclebinIdPermanent(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteApiFdNationalStandardItemRecyclebinIdPermanentParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/FdNationalStandardItem/recyclebin/${param0}/permanent`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}
/** 恢复回收站中的记录 根据提供的ID，将已软删除的记录恢复到正常状态。 PUT /api/FdNationalStandardItem/recyclebin/${param0}/restore */
export async function putApiFdNationalStandardItemRecyclebinIdRestore(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putApiFdNationalStandardItemRecyclebinIdRestoreParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/FdNationalStandardItem/recyclebin/${param0}/restore`, {
		method: 'PUT',
		params: { ...queryParams },
		...(options || {}),
	});
}
/** 根据条件永久删除回收站中的记录 根据提供的条件，将回收站中符合条件的记录从数据库中永久移除。 POST /api/FdNationalStandardItem/recyclebin/permanent */
export async function postApiFdNationalStandardItemRecyclebinPermanent(
	body: APIModel.Expression1Func2FdNationalStandardItem_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral,
	options?: { [key: string]: any }
) {
	return request<number>('/api/FdNationalStandardItem/recyclebin/permanent', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 批量恢复回收站中的记录 根据提供的条件，批量将回收站中的记录恢复到正常状态。 POST /api/FdNationalStandardItem/recyclebin/restore */
export async function postApiFdNationalStandardItemRecyclebinRestore(
	body: APIModel.Expression1Func2FdNationalStandardItem_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral,
	options?: { [key: string]: any }
) {
	return request<number>('/api/FdNationalStandardItem/recyclebin/restore', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 根据条件查询回收站数据 根据提供的查询条件，检索回收站中的记录。 POST /api/FdNationalStandardItem/recyclebin/search */
export async function postApiFdNationalStandardItemRecyclebinSearch(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/FdNationalStandardItem/recyclebin/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}
