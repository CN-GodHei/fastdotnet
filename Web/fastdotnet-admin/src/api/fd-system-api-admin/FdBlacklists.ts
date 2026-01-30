// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 获取所有记录 检索并返回系统中该类型的所有记录。 GET /api/admin/FdBlacklists */
export async function getApiAdminFdBlacklists(options?: { [key: string]: any }) {
	return request<APIModel.FdBlacklistDto[]>('/api/admin/FdBlacklists', {
		method: 'GET',
		...(options || {}),
	});
}
/** 创建新记录 根据提供的数据创建一条新记录。 POST /api/admin/FdBlacklists */
export async function postApiAdminFdBlacklists(body: APIModel.CreateFdBlacklistDto, options?: { [key: string]: any }) {
	return request<APIModel.FdBlacklistDto>('/api/admin/FdBlacklists', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 根据ID获取记录 根据提供的唯一标识符(ID)检索特定记录的详细信息。 GET /api/admin/FdBlacklists/${param0} */
export async function getApiAdminFdBlacklistsId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getApiAdminFdBlacklistsIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<APIModel.FdBlacklistDto>(`/api/admin/FdBlacklists/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}
/** 更新现有记录 根据提供的ID和更新数据，修改现有记录的信息。 PUT /api/admin/FdBlacklists/${param0} */
export async function putApiAdminFdBlacklistsId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putApiAdminFdBlacklistsIdParams,
	body: APIModel.UpdateFdBlacklistDto,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<APIModel.FdBlacklistDto>(`/api/admin/FdBlacklists/${param0}`, {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		params: { ...queryParams },
		data: body,
		...(options || {}),
	});
}
/** 删除记录 根据提供的ID，从系统中移除指定的记录。 DELETE /api/admin/FdBlacklists/${param0} */
export async function deleteApiAdminFdBlacklistsId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteApiAdminFdBlacklistsIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/admin/FdBlacklists/${param0}`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}
/** 根据实体主键批量更新实体信息 根据实体主键批量更新实体信息 PUT /api/admin/FdBlacklists/batch */
export async function putApiAdminFdBlacklistsBatch(body: APIModel.UpdateFdBlacklistDto[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdBlacklists/batch', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 批量创建新记录 根据提供的数据批量创建新记录。 POST /api/admin/FdBlacklists/batch */
export async function postApiAdminFdBlacklistsBatch(body: APIModel.CreateFdBlacklistDto[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdBlacklists/batch', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 批量删除记录 根据提供的ID列表，批量删除多条记录。 DELETE /api/admin/FdBlacklists/batch */
export async function deleteApiAdminFdBlacklistsBatch(body: string[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdBlacklists/batch', {
		method: 'DELETE',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 根据条件批量更新实体属性（部分字段更新） 根据条件批量更新实体属性（部分字段更新） PUT /api/admin/FdBlacklists/batch/updatebycondition */
export async function putApiAdminFdBlacklistsBatchUpdatebycondition(
	body: APIModel.UpdateFdBlacklistDtoBatchUpdateByConditionDto,
	options?: { [key: string]: any }
) {
	return request<number>('/api/admin/FdBlacklists/batch/updatebycondition', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 检查值是否在黑名单中 GET /api/admin/FdBlacklists/check */
export async function getApiAdminFdBlacklistsCheck(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getApiAdminFdBlacklistsCheckParams,
	options?: { [key: string]: any }
) {
	return request<boolean>('/api/admin/FdBlacklists/check', {
		method: 'GET',
		params: {
			...params,
		},
		...(options || {}),
	});
}
/** 分页获取记录 根据页码和页面大小，分页检索记录。 GET /api/admin/FdBlacklists/page */
export async function getApiAdminFdBlacklistsPage(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getApiAdminFdBlacklistsPageParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.PageInfo>('/api/admin/FdBlacklists/page', {
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
/** 根据条件分页获取记录 根据提供的查询条件和分页参数，分页检索记录。 POST /api/admin/FdBlacklists/page/search */
export async function postApiAdminFdBlacklistsPageSearch(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/admin/FdBlacklists/page/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}
/** 获取回收站数据 检索并返回已软删除的记录（回收站数据）。 GET /api/admin/FdBlacklists/recyclebin */
export async function getApiAdminFdBlacklistsRecyclebin(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getApiAdminFdBlacklistsRecyclebinParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.PageInfo>('/api/admin/FdBlacklists/recyclebin', {
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
/** 永久删除回收站中的记录 根据提供的ID，将已软删除的记录从数据库中永久移除。 DELETE /api/admin/FdBlacklists/recyclebin/${param0}/permanent */
export async function deleteApiAdminFdBlacklistsRecyclebinIdPermanent(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteApiAdminFdBlacklistsRecyclebinIdPermanentParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/admin/FdBlacklists/recyclebin/${param0}/permanent`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}
/** 恢复回收站中的记录 根据提供的ID，将已软删除的记录恢复到正常状态。 PUT /api/admin/FdBlacklists/recyclebin/${param0}/restore */
export async function putApiAdminFdBlacklistsRecyclebinIdRestore(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putApiAdminFdBlacklistsRecyclebinIdRestoreParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/admin/FdBlacklists/recyclebin/${param0}/restore`, {
		method: 'PUT',
		params: { ...queryParams },
		...(options || {}),
	});
}
/** 根据条件永久删除回收站中的记录 根据提供的条件，将回收站中符合条件的记录从数据库中永久移除。 POST /api/admin/FdBlacklists/recyclebin/permanent */
export async function postApiAdminFdBlacklistsRecyclebinPermanent(body: APIModel.FdBlacklistBooleanFuncExpression, options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdBlacklists/recyclebin/permanent', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 批量恢复回收站中的记录 根据提供的条件，批量将回收站中的记录恢复到正常状态。 POST /api/admin/FdBlacklists/recyclebin/restore */
export async function postApiAdminFdBlacklistsRecyclebinRestore(body: APIModel.FdBlacklistBooleanFuncExpression, options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdBlacklists/recyclebin/restore', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 根据条件查询回收站数据 根据提供的查询条件，检索回收站中的记录。 POST /api/admin/FdBlacklists/recyclebin/search */
export async function postApiAdminFdBlacklistsRecyclebinSearch(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/admin/FdBlacklists/recyclebin/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}
