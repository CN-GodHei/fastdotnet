// @ts-ignore
/* eslint-disable */
import request from '/@/utils/request';

/** 获取所有记录 检索并返回系统中该类型的所有记录。 GET /api/CodeGen */
export async function getCodeGen(options?: { [key: string]: any }) {
	return request<APIModel.CodeGenConfigDto[]>('/api/CodeGen', {
		method: 'GET',
		...(options || {}),
	});
}

/** 创建新记录 根据提供的数据创建一条新记录。 POST /api/CodeGen */
export async function postCodeGen(body: APIModel.CreateCodeGenConfigDto, options?: { [key: string]: any }) {
	return request<APIModel.CodeGenConfigDto>('/api/CodeGen', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 根据ID获取记录 根据提供的唯一标识符(ID)检索特定记录的详细信息。 GET /api/CodeGen/${param0} */
export async function getCodeGenId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getCodeGenIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<APIModel.CodeGenConfigDto>(`/api/CodeGen/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 更新现有记录 根据提供的ID和更新数据，修改现有记录的信息。 PUT /api/CodeGen/${param0} */
export async function putCodeGenId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putCodeGenIdParams,
	body: APIModel.UpdateCodeGenConfigDto,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<APIModel.CodeGenConfigDto>(`/api/CodeGen/${param0}`, {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		params: { ...queryParams },
		data: body,
		...(options || {}),
	});
}

/** 删除记录 根据提供的ID，从系统中移除指定的记录。 DELETE /api/CodeGen/${param0} */
export async function deleteCodeGenId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteCodeGenIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<boolean>(`/api/CodeGen/${param0}`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 获取应用命名空间列表 GET /api/CodeGen/applicationnamespaces */
export async function getCodeGenApplicationnamespaces(options?: { [key: string]: any }) {
	return request<string[]>('/api/CodeGen/applicationnamespaces', {
		method: 'GET',
		...(options || {}),
	});
}

/** 批量删除记录 根据提供的ID列表，批量删除多条记录。 DELETE /api/CodeGen/batch */
export async function deleteCodeGenBatch(body: string[], options?: { [key: string]: any }) {
	return request<number>('/api/CodeGen/batch', {
		method: 'DELETE',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 根据表名和配置ID获取列信息 GET /api/CodeGen/columnlist/${param0}/${param1} */
export async function getCodeGenColumnlistTableNameConfigId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getCodeGenColumnlistTableNameConfigIdParams,
	options?: { [key: string]: any }
) {
	const { tableName: param0, configId: param1, ...queryParams } = params;
	return request<APIModel.ColumnInfoDto[]>(`/api/CodeGen/columnlist/${param0}/${param1}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 获取数据库列表（模拟，实际项目中应从配置中获取） GET /api/CodeGen/databaselist */
export async function getCodeGenDatabaselist(options?: { [key: string]: any }) {
	return request<APIModel.CodeGenConfigDto[]>('/api/CodeGen/databaselist', {
		method: 'GET',
		...(options || {}),
	});
}

/** 下载生成的代码文件 GET /api/CodeGen/download */
export async function getCodeGenDownload(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getCodeGenDownloadParams,
	options?: { [key: string]: any }
) {
	return request<any>('/api/CodeGen/download', {
		method: 'GET',
		params: {
			...params,
		},
		...(options || {}),
	});
}

/** 执行代码生成 POST /api/CodeGen/generate */
export async function postCodeGenGenerate(body: APIModel.CodeGenInput, options?: { [key: string]: any }) {
	return request<string>('/api/CodeGen/generate', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 根据表名获取实体名 GET /api/CodeGen/getentityname */
export async function getCodeGenGetentityname(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getCodeGenGetentitynameParams,
	options?: { [key: string]: any }
) {
	return request<string>('/api/CodeGen/getentityname', {
		method: 'GET',
		params: {
			...params,
		},
		...(options || {}),
	});
}

/** 获取表的列数据 GET /api/CodeGen/gettablecolumnlist */
export async function getCodeGenGettablecolumnlist(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getCodeGenGettablecolumnlistParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.ColumnInfoDto[]>('/api/CodeGen/gettablecolumnlist', {
		method: 'GET',
		params: {
			...params,
		},
		...(options || {}),
	});
}

/** 获取库表数据 GET /api/CodeGen/gettablelist */
export async function getCodeGenGettablelist(options?: { [key: string]: any }) {
	return request<APIModel.TableInfoDto[]>('/api/CodeGen/gettablelist', {
		method: 'GET',
		...(options || {}),
	});
}

/** 分页获取记录 根据页码和页面大小，分页检索记录。 GET /api/CodeGen/page */
export async function getCodeGenPage(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getCodeGenPageParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.CodeGenConfigDtoPageResult>('/api/CodeGen/page', {
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

/** 根据条件分页获取记录 根据提供的查询条件和分页参数，分页检索记录。 POST /api/CodeGen/page/search */
export async function postCodeGenPageSearch(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.CodeGenConfigDtoPageResult>('/api/CodeGen/page/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}

/** 获取回收站数据 检索并返回已软删除的记录（回收站数据）。 GET /api/CodeGen/recyclebin */
export async function getCodeGenRecyclebin(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getCodeGenRecyclebinParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.CodeGenConfigDtoPageResult>('/api/CodeGen/recyclebin', {
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

/** 永久删除回收站中的记录 根据提供的ID，将已软删除的记录从数据库中永久移除。 DELETE /api/CodeGen/recyclebin/${param0}/permanent */
export async function deleteCodeGenRecyclebinIdPermanent(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteCodeGenRecyclebinIdPermanentParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<boolean>(`/api/CodeGen/recyclebin/${param0}/permanent`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 恢复回收站中的记录 根据提供的ID，将已软删除的记录恢复到正常状态。 PUT /api/CodeGen/recyclebin/${param0}/restore */
export async function putCodeGenRecyclebinIdRestore(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putCodeGenRecyclebinIdRestoreParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<boolean>(`/api/CodeGen/recyclebin/${param0}/restore`, {
		method: 'PUT',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 根据条件永久删除回收站中的记录 根据提供的条件，将回收站中符合条件的记录从数据库中永久移除。 POST /api/CodeGen/recyclebin/permanent */
export async function postCodeGenRecyclebinPermanent(body: APIModel.CodeGenConfigBooleanFuncExpression, options?: { [key: string]: any }) {
	return request<number>('/api/CodeGen/recyclebin/permanent', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 批量恢复回收站中的记录 根据提供的条件，批量将回收站中的记录恢复到正常状态。 POST /api/CodeGen/recyclebin/restore */
export async function postCodeGenRecyclebinRestore(body: APIModel.CodeGenConfigBooleanFuncExpression, options?: { [key: string]: any }) {
	return request<number>('/api/CodeGen/recyclebin/restore', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 根据条件查询回收站数据 根据提供的查询条件，检索回收站中的记录。 POST /api/CodeGen/recyclebin/search */
export async function postCodeGenRecyclebinSearch(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.CodeGenConfigDtoPageResult>('/api/CodeGen/recyclebin/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}

/** 根据配置ID获取表列表 GET /api/CodeGen/tablelist/${param0} */
export async function getCodeGenTablelistConfigId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getCodeGenTablelistConfigIdParams,
	options?: { [key: string]: any }
) {
	const { configId: param0, ...queryParams } = params;
	return request<APIModel.TableInfoDto[]>(`/api/CodeGen/tablelist/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}
