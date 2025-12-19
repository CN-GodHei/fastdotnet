// @ts-ignore
/* eslint-disable */
import request from '@/utils/request';

/** 获取所有记录 检索并返回系统中该类型的所有记录。 GET /api/admin/FdSystemInfoConfig */
export async function getAdminFdSystemInfoConfig(options?: { [key: string]: any }) {
	return request<APIModel.FdSystemInfoConfigDto[]>('/api/admin/FdSystemInfoConfig', {
		method: 'GET',
		...(options || {}),
	});
}

/** 创建新记录 根据提供的数据创建一条新记录。 POST /api/admin/FdSystemInfoConfig */
export async function postAdminFdSystemInfoConfig(body: APIModel.CreateFdSystemInfoConfigDto, options?: { [key: string]: any }) {
	return request<APIModel.FdSystemInfoConfigDto>('/api/admin/FdSystemInfoConfig', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 根据ID获取记录 根据提供的唯一标识符(ID)检索特定记录的详细信息。 GET /api/admin/FdSystemInfoConfig/${param0} */
export async function getAdminFdSystemInfoConfigId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAdminFdSystemInfoConfigIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<APIModel.FdSystemInfoConfigDto>(`/api/admin/FdSystemInfoConfig/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 更新现有记录 根据提供的ID和更新数据，修改现有记录的信息。 PUT /api/admin/FdSystemInfoConfig/${param0} */
export async function putAdminFdSystemInfoConfigId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putAdminFdSystemInfoConfigIdParams,
	body: APIModel.UpdateFdSystemInfoConfigDto,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<APIModel.FdSystemInfoConfigDto>(`/api/admin/FdSystemInfoConfig/${param0}`, {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		params: { ...queryParams },
		data: body,
		...(options || {}),
	});
}

/** 删除记录 根据提供的ID，从系统中移除指定的记录。 DELETE /api/admin/FdSystemInfoConfig/${param0} */
export async function deleteAdminFdSystemInfoConfigId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteAdminFdSystemInfoConfigIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<boolean>(`/api/admin/FdSystemInfoConfig/${param0}`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 根据实体主键批量更新实体信息 根据实体主键批量更新实体信息 PUT /api/admin/FdSystemInfoConfig/batch */
export async function putAdminFdSystemInfoConfigBatch(body: APIModel.UpdateFdSystemInfoConfigDto[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdSystemInfoConfig/batch', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 批量创建新记录 根据提供的数据批量创建新记录。 POST /api/admin/FdSystemInfoConfig/batch */
export async function postAdminFdSystemInfoConfigBatch(body: APIModel.CreateFdSystemInfoConfigDto[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdSystemInfoConfig/batch', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 批量删除记录 根据提供的ID列表，批量删除多条记录。 DELETE /api/admin/FdSystemInfoConfig/batch */
export async function deleteAdminFdSystemInfoConfigBatch(body: string[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdSystemInfoConfig/batch', {
		method: 'DELETE',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 根据条件批量更新实体属性（部分字段更新） 根据条件批量更新实体属性（部分字段更新） PUT /api/admin/FdSystemInfoConfig/batch/updatebycondition */
export async function putAdminFdSystemInfoConfigBatchUpdatebycondition(
	body: APIModel.UpdateFdSystemInfoConfigDtoBatchUpdateByConditionDto,
	options?: { [key: string]: any }
) {
	return request<number>('/api/admin/FdSystemInfoConfig/batch/updatebycondition', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 分页获取记录 根据页码和页面大小，分页检索记录。 GET /api/admin/FdSystemInfoConfig/page */
export async function getAdminFdSystemInfoConfigPage(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAdminFdSystemInfoConfigPageParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.PageInfo>('/api/admin/FdSystemInfoConfig/page', {
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

/** 根据条件分页获取记录 根据提供的查询条件和分页参数，分页检索记录。 POST /api/admin/FdSystemInfoConfig/page/search */
export async function postAdminFdSystemInfoConfigPageSearch(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/admin/FdSystemInfoConfig/page/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}

/** [Public] 获取所有系统配置项（用于客户端初始化） GET /api/admin/FdSystemInfoConfig/public/all */
export async function getAdminFdSystemInfoConfigPublicAll(options?: { [key: string]: any }) {
	return request<Record<string, any>>('/api/admin/FdSystemInfoConfig/public/all', {
		method: 'GET',
		...(options || {}),
	});
}

/** 获取回收站数据 检索并返回已软删除的记录（回收站数据）。 GET /api/admin/FdSystemInfoConfig/recyclebin */
export async function getAdminFdSystemInfoConfigRecyclebin(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAdminFdSystemInfoConfigRecyclebinParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.PageInfo>('/api/admin/FdSystemInfoConfig/recyclebin', {
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

/** 永久删除回收站中的记录 根据提供的ID，将已软删除的记录从数据库中永久移除。 DELETE /api/admin/FdSystemInfoConfig/recyclebin/${param0}/permanent */
export async function deleteAdminFdSystemInfoConfigRecyclebinIdPermanent(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteAdminFdSystemInfoConfigRecyclebinIdPermanentParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<boolean>(`/api/admin/FdSystemInfoConfig/recyclebin/${param0}/permanent`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 恢复回收站中的记录 根据提供的ID，将已软删除的记录恢复到正常状态。 PUT /api/admin/FdSystemInfoConfig/recyclebin/${param0}/restore */
export async function putAdminFdSystemInfoConfigRecyclebinIdRestore(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putAdminFdSystemInfoConfigRecyclebinIdRestoreParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<boolean>(`/api/admin/FdSystemInfoConfig/recyclebin/${param0}/restore`, {
		method: 'PUT',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 根据条件永久删除回收站中的记录 根据提供的条件，将回收站中符合条件的记录从数据库中永久移除。 POST /api/admin/FdSystemInfoConfig/recyclebin/permanent */
export async function postAdminFdSystemInfoConfigRecyclebinPermanent(
	body: APIModel.SystemInfoConfigBooleanFuncExpression,
	options?: { [key: string]: any }
) {
	return request<number>('/api/admin/FdSystemInfoConfig/recyclebin/permanent', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 批量恢复回收站中的记录 根据提供的条件，批量将回收站中的记录恢复到正常状态。 POST /api/admin/FdSystemInfoConfig/recyclebin/restore */
export async function postAdminFdSystemInfoConfigRecyclebinRestore(
	body: APIModel.SystemInfoConfigBooleanFuncExpression,
	options?: { [key: string]: any }
) {
	return request<number>('/api/admin/FdSystemInfoConfig/recyclebin/restore', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 根据条件查询回收站数据 根据提供的查询条件，检索回收站中的记录。 POST /api/admin/FdSystemInfoConfig/recyclebin/search */
export async function postAdminFdSystemInfoConfigRecyclebinSearch(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/admin/FdSystemInfoConfig/recyclebin/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}
