// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 获取所有记录 检索并返回系统中该类型的所有记录。 GET /api/CodeGen */
/** @deprecated 请使用 getGenericDtoControllerBase5GetAll，原函数名: getApiCodeGen */
export async function getGenericDtoControllerBase5GetAll(options?: { [key: string]: any }) {
	return request<APIModel.CodeGenConfigDto[]>('/api/CodeGen', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getGenericDtoControllerBase5GetAll */
export const getApiCodeGen = getGenericDtoControllerBase5GetAll;
/** 创建新记录 根据提供的数据创建一条新记录。 POST /api/CodeGen */
/** @deprecated 请使用 postGenericDtoControllerBase5Create，原函数名: postApiCodeGen */
export async function postGenericDtoControllerBase5Create(body: APIModel.CreateCodeGenDto, options?: { [key: string]: any }) {
	return request<APIModel.CodeGenConfigDto>('/api/CodeGen', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5Create */
export const postApiCodeGen = postGenericDtoControllerBase5Create;
/** 根据ID获取记录 根据提供的唯一标识符(ID)检索特定记录的详细信息。 GET /api/CodeGen/${param0} */
/** @deprecated 请使用 getGenericDtoControllerBase5GetById，原函数名: getApiCodeGenId */
export async function getGenericDtoControllerBase5GetById(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getGenericDtoControllerBase5GetByIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<APIModel.CodeGenConfigDto>(`/api/CodeGen/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getGenericDtoControllerBase5GetById */
export const getApiCodeGenId = getGenericDtoControllerBase5GetById;
/** 更新现有记录 根据提供的ID和更新数据，修改现有记录的信息。 PUT /api/CodeGen/${param0} */
/** @deprecated 请使用 putGenericDtoControllerBase5Update，原函数名: putApiCodeGenId */
export async function putGenericDtoControllerBase5Update(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putGenericDtoControllerBase5UpdateParams,
	body: APIModel.UpdateCodeGenDto,
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

/** @deprecated 此函数名已变更，请使用 putGenericDtoControllerBase5Update */
export const putApiCodeGenId = putGenericDtoControllerBase5Update;
/** 删除记录 根据提供的ID，从系统中移除指定的记录。 DELETE /api/CodeGen/${param0} */
/** @deprecated 请使用 deleteGenericDtoControllerBase5Delete，原函数名: deleteApiCodeGenId */
export async function deleteGenericDtoControllerBase5Delete(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteGenericDtoControllerBase5DeleteParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/CodeGen/${param0}`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteGenericDtoControllerBase5Delete */
export const deleteApiCodeGenId = deleteGenericDtoControllerBase5Delete;
/** 获取应用命名空间列表 GET /api/CodeGen/applicationnamespaces */
/** @deprecated 请使用 getCodeGenGetApplicationNamespaces，原函数名: getApiCodeGenApplicationnamespaces */
export async function getCodeGenGetApplicationNamespaces(options?: { [key: string]: any }) {
	return request<string[]>('/api/CodeGen/applicationnamespaces', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getCodeGenGetApplicationNamespaces */
export const getApiCodeGenApplicationnamespaces = getCodeGenGetApplicationNamespaces;
/** 根据实体主键批量更新实体信息 根据实体主键批量更新实体信息 PUT /api/CodeGen/batch */
/** @deprecated 请使用 putGenericDtoControllerBase5UpdateMany，原函数名: putApiCodeGenBatch */
export async function putGenericDtoControllerBase5UpdateMany(body: APIModel.UpdateCodeGenDto[], options?: { [key: string]: any }) {
	return request<number>('/api/CodeGen/batch', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putGenericDtoControllerBase5UpdateMany */
export const putApiCodeGenBatch = putGenericDtoControllerBase5UpdateMany;
/** 批量创建新记录 根据提供的数据批量创建新记录。 POST /api/CodeGen/batch */
/** @deprecated 请使用 postGenericDtoControllerBase5CreateMany，原函数名: postApiCodeGenBatch */
export async function postGenericDtoControllerBase5CreateMany(body: APIModel.CreateCodeGenDto[], options?: { [key: string]: any }) {
	return request<number>('/api/CodeGen/batch', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5CreateMany */
export const postApiCodeGenBatch = postGenericDtoControllerBase5CreateMany;
/** 批量删除记录 根据提供的ID列表，批量删除多条记录。 DELETE /api/CodeGen/batch */
/** @deprecated 请使用 deleteGenericDtoControllerBase5BatchDelete，原函数名: deleteApiCodeGenBatch */
export async function deleteGenericDtoControllerBase5BatchDelete(body: string[], options?: { [key: string]: any }) {
	return request<number>('/api/CodeGen/batch', {
		method: 'DELETE',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteGenericDtoControllerBase5BatchDelete */
export const deleteApiCodeGenBatch = deleteGenericDtoControllerBase5BatchDelete;
/** 根据条件批量更新实体属性（部分字段更新） 根据条件批量更新实体属性（部分字段更新） PUT /api/CodeGen/batch/updatebycondition */
/** @deprecated 请使用 putGenericDtoControllerBase5UpdateManyByCondition，原函数名: putApiCodeGenBatchUpdatebycondition */
export async function putGenericDtoControllerBase5UpdateManyByCondition(
	body: APIModel.BatchUpdateByConditionDto1UpdateCodeGenDto,
	options?: { [key: string]: any }
) {
	return request<number>('/api/CodeGen/batch/updatebycondition', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putGenericDtoControllerBase5UpdateManyByCondition */
export const putApiCodeGenBatchUpdatebycondition = putGenericDtoControllerBase5UpdateManyByCondition;
/** 根据表名和配置ID获取列信息 GET /api/CodeGen/columnlist/${param0}/${param1} */
/** @deprecated 请使用 getCodeGenGetColumnListByTableNameAndConfigId，原函数名: getApiCodeGenColumnlistTableNameConfigId */
export async function getCodeGenGetColumnListByTableNameAndConfigId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getCodeGenGetColumnListByTableNameAndConfigIdParams,
	options?: { [key: string]: any }
) {
	const { tableName: param0, configId: param1, ...queryParams } = params;

	return request<APIModel.ColumnInfoDto[]>(`/api/CodeGen/columnlist/${param0}/${param1}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getCodeGenGetColumnListByTableNameAndConfigId */
export const getApiCodeGenColumnlistTableNameConfigId = getCodeGenGetColumnListByTableNameAndConfigId;
/** 下载生成的代码文件 GET /api/CodeGen/download */
/** @deprecated 请使用 getCodeGenDownloadFile，原函数名: getApiCodeGenDownload */
export async function getCodeGenDownloadFile(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getCodeGenDownloadFileParams,
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

/** @deprecated 此函数名已变更，请使用 getCodeGenDownloadFile */
export const getApiCodeGenDownload = getCodeGenDownloadFile;
/** 根据表名获取实体名 GET /api/CodeGen/getentityname */
/** @deprecated 请使用 getCodeGenGetEntityName，原函数名: getApiCodeGenGetentityname */
export async function getCodeGenGetEntityName(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getCodeGenGetEntityNameParams,
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

/** @deprecated 此函数名已变更，请使用 getCodeGenGetEntityName */
export const getApiCodeGenGetentityname = getCodeGenGetEntityName;
/** 获取表的列数据 GET /api/CodeGen/gettablecolumnlist */
/** @deprecated 请使用 getCodeGenGetTableColumnList，原函数名: getApiCodeGenGettablecolumnlist */
export async function getCodeGenGetTableColumnList(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getCodeGenGetTableColumnListParams,
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

/** @deprecated 此函数名已变更，请使用 getCodeGenGetTableColumnList */
export const getApiCodeGenGettablecolumnlist = getCodeGenGetTableColumnList;
/** 获取库表数据 GET /api/CodeGen/gettablelist */
/** @deprecated 请使用 getCodeGenGetTableList，原函数名: getApiCodeGenGettablelist */
export async function getCodeGenGetTableList(options?: { [key: string]: any }) {
	return request<APIModel.TableInfoDto[]>('/api/CodeGen/gettablelist', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getCodeGenGetTableList */
export const getApiCodeGenGettablelist = getCodeGenGetTableList;
/** 根据自定义条件获取列表(不分页) 根据自定义条件获取列表(不分页) POST /api/CodeGen/list-by-condition */
/** @deprecated 请使用 postGenericDtoControllerBase5GetListByCondition，原函数名: postApiCodeGenListByCondition */
export async function postGenericDtoControllerBase5GetListByCondition(body: APIModel.QueryByConditionDto, options?: { [key: string]: any }) {
	return request<any>('/api/CodeGen/list-by-condition', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5GetListByCondition */
export const postApiCodeGenListByCondition = postGenericDtoControllerBase5GetListByCondition;
/** 分页获取记录 根据页码和页面大小，分页检索记录。 GET /api/CodeGen/page */
/** @deprecated 请使用 getGenericDtoControllerBase5GetPage，原函数名: getApiCodeGenPage */
export async function getGenericDtoControllerBase5GetPage(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getGenericDtoControllerBase5GetPageParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.PageInfo>('/api/CodeGen/page', {
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
export const getApiCodeGenPage = getGenericDtoControllerBase5GetPage;
/** 根据条件分页获取记录 根据提供的查询条件和分页参数，分页检索记录。 POST /api/CodeGen/page/search */
/** @deprecated 请使用 postGenericDtoControllerBase5GetPageByCondition，原函数名: postApiCodeGenPageSearch */
export async function postGenericDtoControllerBase5GetPageByCondition(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/CodeGen/page/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5GetPageByCondition */
export const postApiCodeGenPageSearch = postGenericDtoControllerBase5GetPageByCondition;
/** 预览生成的代码 GET /api/CodeGen/preview/${param0} */
/** @deprecated 请使用 getCodeGenPreviewCode，原函数名: getApiCodeGenPreviewConfigId */
export async function getCodeGenPreviewCode(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getCodeGenPreviewCodeParams,
	options?: { [key: string]: any }
) {
	const { configId: param0, ...queryParams } = params;

	return request<string>(`/api/CodeGen/preview/${param0}`, {
		method: 'GET',
		params: {
			// type has a default value: entity
			type: 'entity',
			// apiscop has a default value: Admin
			apiscop: 'Admin',
			...queryParams,
		},
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getCodeGenPreviewCode */
export const getApiCodeGenPreviewConfigId = getCodeGenPreviewCode;
/** 获取回收站数据 检索并返回已软删除的记录（回收站数据）。 GET /api/CodeGen/recyclebin */
/** @deprecated 请使用 getGenericDtoControllerBase5GetRecycleBin，原函数名: getApiCodeGenRecyclebin */
export async function getGenericDtoControllerBase5GetRecycleBin(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getGenericDtoControllerBase5GetRecycleBinParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.PageInfo>('/api/CodeGen/recyclebin', {
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
export const getApiCodeGenRecyclebin = getGenericDtoControllerBase5GetRecycleBin;
/** 永久删除回收站中的记录 根据提供的ID，将已软删除的记录从数据库中永久移除。 DELETE /api/CodeGen/recyclebin/${param0}/permanent */
/** @deprecated 请使用 deleteGenericDtoControllerBase5PermanentDelete，原函数名: deleteApiCodeGenRecyclebinIdPermanent */
export async function deleteGenericDtoControllerBase5PermanentDelete(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteGenericDtoControllerBase5PermanentDeleteParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/CodeGen/recyclebin/${param0}/permanent`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteGenericDtoControllerBase5PermanentDelete */
export const deleteApiCodeGenRecyclebinIdPermanent = deleteGenericDtoControllerBase5PermanentDelete;
/** 恢复回收站中的记录 根据提供的ID，将已软删除的记录恢复到正常状态。 PUT /api/CodeGen/recyclebin/${param0}/restore */
/** @deprecated 请使用 putGenericDtoControllerBase5Restore，原函数名: putApiCodeGenRecyclebinIdRestore */
export async function putGenericDtoControllerBase5Restore(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putGenericDtoControllerBase5RestoreParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/CodeGen/recyclebin/${param0}/restore`, {
		method: 'PUT',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putGenericDtoControllerBase5Restore */
export const putApiCodeGenRecyclebinIdRestore = putGenericDtoControllerBase5Restore;
/** 根据条件永久删除回收站中的记录 根据提供的条件，将回收站中符合条件的记录从数据库中永久移除。 POST /api/CodeGen/recyclebin/permanent */
/** @deprecated 请使用 postGenericDtoControllerBase5PermanentDeleteBatch，原函数名: postApiCodeGenRecyclebinPermanent */
export async function postGenericDtoControllerBase5PermanentDeleteBatch(
	body: APIModel.Expression1Func2FdCodeGen_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral,
	options?: { [key: string]: any }
) {
	return request<number>('/api/CodeGen/recyclebin/permanent', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5PermanentDeleteBatch */
export const postApiCodeGenRecyclebinPermanent = postGenericDtoControllerBase5PermanentDeleteBatch;
/** 批量恢复回收站中的记录 根据提供的条件，批量将回收站中的记录恢复到正常状态。 POST /api/CodeGen/recyclebin/restore */
/** @deprecated 请使用 postGenericDtoControllerBase5RestoreBatch，原函数名: postApiCodeGenRecyclebinRestore */
export async function postGenericDtoControllerBase5RestoreBatch(
	body: APIModel.Expression1Func2FdCodeGen_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral,
	options?: { [key: string]: any }
) {
	return request<number>('/api/CodeGen/recyclebin/restore', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5RestoreBatch */
export const postApiCodeGenRecyclebinRestore = postGenericDtoControllerBase5RestoreBatch;
/** 根据条件查询回收站数据 根据提供的查询条件，检索回收站中的记录。 POST /api/CodeGen/recyclebin/search */
/** @deprecated 请使用 postGenericDtoControllerBase5SearchRecycleBin，原函数名: postApiCodeGenRecyclebinSearch */
export async function postGenericDtoControllerBase5SearchRecycleBin(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/CodeGen/recyclebin/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5SearchRecycleBin */
export const postApiCodeGenRecyclebinSearch = postGenericDtoControllerBase5SearchRecycleBin;
/** 根据配置ID获取表列表 GET /api/CodeGen/tablelist/${param0} */
/** @deprecated 请使用 getCodeGenGetTableListByConfigId，原函数名: getApiCodeGenTablelistConfigId */
export async function getCodeGenGetTableListByConfigId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getCodeGenGetTableListByConfigIdParams,
	options?: { [key: string]: any }
) {
	const { configId: param0, ...queryParams } = params;

	return request<APIModel.TableInfoDto[]>(`/api/CodeGen/tablelist/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getCodeGenGetTableListByConfigId */
export const getApiCodeGenTablelistConfigId = getCodeGenGetTableListByConfigId;
