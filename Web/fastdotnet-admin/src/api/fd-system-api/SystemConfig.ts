// @ts-ignore
/* eslint-disable */
import request from '/@/utils/request';

/** 获取所有记录 检索并返回系统中该类型的所有记录。 GET /api/admin/SystemConfig */
export async function getAdminSystemConfig(options?: { [key: string]: any }) {
	return request<APIModel.SystemConfigDto[]>('/api/admin/SystemConfig', {
		method: 'GET',
		...(options || {}),
	});
}

/** 创建新记录 根据提供的数据创建一条新记录。 POST /api/admin/SystemConfig */
export async function postAdminSystemConfig(body: APIModel.SystemConfigDto, options?: { [key: string]: any }) {
	return request<APIModel.SystemConfigDto>('/api/admin/SystemConfig', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 根据ID获取记录 根据提供的唯一标识符(ID)检索特定记录的详细信息。 GET /api/admin/SystemConfig/${param0} */
export async function getAdminSystemConfigId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAdminSystemConfigIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<APIModel.SystemConfigDto>(`/api/admin/SystemConfig/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 更新现有记录 根据提供的ID和更新数据，修改现有记录的信息。 PUT /api/admin/SystemConfig/${param0} */
export async function putAdminSystemConfigId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putAdminSystemConfigIdParams,
	body: APIModel.SystemConfigDto,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<APIModel.SystemConfigDto>(`/api/admin/SystemConfig/${param0}`, {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		params: { ...queryParams },
		data: body,
		...(options || {}),
	});
}

/** 删除记录 根据提供的ID，从系统中移除指定的记录。 DELETE /api/admin/SystemConfig/${param0} */
export async function deleteAdminSystemConfigId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteAdminSystemConfigIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<boolean>(`/api/admin/SystemConfig/${param0}`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 批量创建新记录 根据提供的数据批量创建新记录。 POST /api/admin/SystemConfig/batch */
export async function postAdminSystemConfigBatch(body: APIModel.SystemConfigDto[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/SystemConfig/batch', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 批量删除记录 根据提供的ID列表，批量删除多条记录。 DELETE /api/admin/SystemConfig/batch */
export async function deleteAdminSystemConfigBatch(body: string[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/SystemConfig/batch', {
		method: 'DELETE',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 分页获取记录 根据页码和页面大小，分页检索记录。 GET /api/admin/SystemConfig/page */
export async function getAdminSystemConfigPage(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAdminSystemConfigPageParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.SystemConfigDtoPageResult>('/api/admin/SystemConfig/page', {
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

/** 根据条件分页获取记录 根据提供的查询条件和分页参数，分页检索记录。 POST /api/admin/SystemConfig/page/search */
export async function postAdminSystemConfigPageSearch(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.SystemConfigDtoPageResult>('/api/admin/SystemConfig/page/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}

/** [Public] 获取所有系统配置项（用于客户端初始化） GET /api/admin/SystemConfig/public/all */
export async function getAdminSystemConfigPublicAll(options?: { [key: string]: any }) {
	return request<Record<string, any>>('/api/admin/SystemConfig/public/all', {
		method: 'GET',
		...(options || {}),
	});
}

/** 获取回收站数据 检索并返回已软删除的记录（回收站数据）。 GET /api/admin/SystemConfig/recyclebin */
export async function getAdminSystemConfigRecyclebin(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAdminSystemConfigRecyclebinParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.SystemConfigDtoPageResult>('/api/admin/SystemConfig/recyclebin', {
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

/** 永久删除回收站中的记录 根据提供的ID，将已软删除的记录从数据库中永久移除。 DELETE /api/admin/SystemConfig/recyclebin/${param0}/permanent */
export async function deleteAdminSystemConfigRecyclebinIdPermanent(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteAdminSystemConfigRecyclebinIdPermanentParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<boolean>(`/api/admin/SystemConfig/recyclebin/${param0}/permanent`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 恢复回收站中的记录 根据提供的ID，将已软删除的记录恢复到正常状态。 PUT /api/admin/SystemConfig/recyclebin/${param0}/restore */
export async function putAdminSystemConfigRecyclebinIdRestore(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putAdminSystemConfigRecyclebinIdRestoreParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<boolean>(`/api/admin/SystemConfig/recyclebin/${param0}/restore`, {
		method: 'PUT',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 根据条件永久删除回收站中的记录 根据提供的条件，将回收站中符合条件的记录从数据库中永久移除。 POST /api/admin/SystemConfig/recyclebin/permanent */
export async function postAdminSystemConfigRecyclebinPermanent(body: APIModel.SystemConfigBooleanFuncExpression, options?: { [key: string]: any }) {
	return request<number>('/api/admin/SystemConfig/recyclebin/permanent', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 批量恢复回收站中的记录 根据提供的条件，批量将回收站中的记录恢复到正常状态。 POST /api/admin/SystemConfig/recyclebin/restore */
export async function postAdminSystemConfigRecyclebinRestore(body: APIModel.SystemConfigBooleanFuncExpression, options?: { [key: string]: any }) {
	return request<number>('/api/admin/SystemConfig/recyclebin/restore', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 根据条件查询回收站数据 根据提供的查询条件，检索回收站中的记录。 POST /api/admin/SystemConfig/recyclebin/search */
export async function postAdminSystemConfigRecyclebinSearch(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.SystemConfigDtoPageResult>('/api/admin/SystemConfig/recyclebin/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}
