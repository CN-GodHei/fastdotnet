// @ts-ignore
/* eslint-disable */
import request from '/@/utils/request';

/** 获取所有记录 检索并返回系统中该类型的所有记录。 GET /api/CodeGenConfig */
export async function getCodeGenConfig(options?: { [key: string]: any }) {
	return request<APIModel.FdCodeGenConfigDto[]>('/api/CodeGenConfig', {
		method: 'GET',
		...(options || {}),
	});
}

/** 创建新记录 根据提供的数据创建一条新记录。 POST /api/CodeGenConfig */
export async function postCodeGenConfig(body: APIModel.CreateFdCodeGenConfigDto, options?: { [key: string]: any }) {
	return request<APIModel.FdCodeGenConfigDto>('/api/CodeGenConfig', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 根据ID获取记录 根据提供的唯一标识符(ID)检索特定记录的详细信息。 GET /api/CodeGenConfig/${param0} */
export async function getCodeGenConfigId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getCodeGenConfigIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<APIModel.FdCodeGenConfigDto>(`/api/CodeGenConfig/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 更新现有记录 根据提供的ID和更新数据，修改现有记录的信息。 PUT /api/CodeGenConfig/${param0} */
export async function putCodeGenConfigId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putCodeGenConfigIdParams,
	body: APIModel.UpdateFdCodeGenConfigDto,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<APIModel.FdCodeGenConfigDto>(`/api/CodeGenConfig/${param0}`, {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		params: { ...queryParams },
		data: body,
		...(options || {}),
	});
}

/** 删除记录 根据提供的ID，从系统中移除指定的记录。 DELETE /api/CodeGenConfig/${param0} */
export async function deleteCodeGenConfigId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteCodeGenConfigIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<boolean>(`/api/CodeGenConfig/${param0}`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 批量删除记录 根据提供的ID列表，批量删除多条记录。 DELETE /api/CodeGenConfig/batch */
export async function deleteCodeGenConfigBatch(body: string[], options?: { [key: string]: any }) {
	return request<number>('/api/CodeGenConfig/batch', {
		method: 'DELETE',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 分页获取记录 根据页码和页面大小，分页检索记录。 GET /api/CodeGenConfig/page */
export async function getCodeGenConfigPage(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getCodeGenConfigPageParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.FdCodeGenConfigDtoPageResult>('/api/CodeGenConfig/page', {
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

/** 根据条件分页获取记录 根据提供的查询条件和分页参数，分页检索记录。 POST /api/CodeGenConfig/page/search */
export async function postCodeGenConfigPageSearch(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.FdCodeGenConfigDtoPageResult>('/api/CodeGenConfig/page/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}

/** 获取回收站数据 检索并返回已软删除的记录（回收站数据）。 GET /api/CodeGenConfig/recyclebin */
export async function getCodeGenConfigRecyclebin(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getCodeGenConfigRecyclebinParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.FdCodeGenConfigDtoPageResult>('/api/CodeGenConfig/recyclebin', {
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

/** 永久删除回收站中的记录 根据提供的ID，将已软删除的记录从数据库中永久移除。 DELETE /api/CodeGenConfig/recyclebin/${param0}/permanent */
export async function deleteCodeGenConfigRecyclebinIdPermanent(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteCodeGenConfigRecyclebinIdPermanentParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<boolean>(`/api/CodeGenConfig/recyclebin/${param0}/permanent`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 恢复回收站中的记录 根据提供的ID，将已软删除的记录恢复到正常状态。 PUT /api/CodeGenConfig/recyclebin/${param0}/restore */
export async function putCodeGenConfigRecyclebinIdRestore(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putCodeGenConfigRecyclebinIdRestoreParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<boolean>(`/api/CodeGenConfig/recyclebin/${param0}/restore`, {
		method: 'PUT',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 根据条件永久删除回收站中的记录 根据提供的条件，将回收站中符合条件的记录从数据库中永久移除。 POST /api/CodeGenConfig/recyclebin/permanent */
export async function postCodeGenConfigRecyclebinPermanent(body: APIModel.FdCodeGenConfigBooleanFuncExpression, options?: { [key: string]: any }) {
	return request<number>('/api/CodeGenConfig/recyclebin/permanent', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 批量恢复回收站中的记录 根据提供的条件，批量将回收站中的记录恢复到正常状态。 POST /api/CodeGenConfig/recyclebin/restore */
export async function postCodeGenConfigRecyclebinRestore(body: APIModel.FdCodeGenConfigBooleanFuncExpression, options?: { [key: string]: any }) {
	return request<number>('/api/CodeGenConfig/recyclebin/restore', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 根据条件查询回收站数据 根据提供的查询条件，检索回收站中的记录。 POST /api/CodeGenConfig/recyclebin/search */
export async function postCodeGenConfigRecyclebinSearch(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.FdCodeGenConfigDtoPageResult>('/api/CodeGenConfig/recyclebin/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}
