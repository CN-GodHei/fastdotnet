// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 获取所有记录 检索并返回系统中该类型的所有记录。 GET /api/FdNationalStandard */
export async function getGenericDtoControllerBase5GetAll(options?: { [key: string]: any }) {
	return request<APIModel.FdNationalStandardDto[]>('/api/FdNationalStandard', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getGenericDtoControllerBase5GetAll */
export const getApiFdNationalStandard = getGenericDtoControllerBase5GetAll;
/** 创建新记录 根据提供的数据创建一条新记录。 POST /api/FdNationalStandard */
export async function postGenericDtoControllerBase5Create(body: APIModel.CreateFdNationalStandardDto, options?: { [key: string]: any }) {
	return request<APIModel.FdNationalStandardDto>('/api/FdNationalStandard', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5Create */
export const postApiFdNationalStandard = postGenericDtoControllerBase5Create;
/** 根据ID获取记录 根据提供的唯一标识符(ID)检索特定记录的详细信息。 GET /api/FdNationalStandard/${param0} */
export async function getGenericDtoControllerBase5GetById(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getGenericDtoControllerBase5GetByIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<APIModel.FdNationalStandardDto>(`/api/FdNationalStandard/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getGenericDtoControllerBase5GetById */
export const getApiFdNationalStandardId = getGenericDtoControllerBase5GetById;
/** 更新现有记录 根据提供的ID和更新数据，修改现有记录的信息。 PUT /api/FdNationalStandard/${param0} */
export async function putGenericDtoControllerBase5Update(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putGenericDtoControllerBase5UpdateParams,
	body: APIModel.UpdateFdNationalStandardDto,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<APIModel.FdNationalStandardDto>(`/api/FdNationalStandard/${param0}`, {
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
export const putApiFdNationalStandardId = putGenericDtoControllerBase5Update;
/** 删除记录 根据提供的ID，从系统中移除指定的记录。 DELETE /api/FdNationalStandard/${param0} */
export async function deleteGenericDtoControllerBase5Delete(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteGenericDtoControllerBase5DeleteParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/FdNationalStandard/${param0}`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteGenericDtoControllerBase5Delete */
export const deleteApiFdNationalStandardId = deleteGenericDtoControllerBase5Delete;
/** 根据标准编码获取标准详情（包含条目统计） GET /api/FdNationalStandard/${param0}/detail */
export async function getFdNationalStandardGetStandardDetail(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getFdNationalStandardGetStandardDetailParams,
	options?: { [key: string]: any }
) {
	const { standardCode: param0, ...queryParams } = params;

	return request<APIModel.FdNationalStandardDetailDto>(`/api/FdNationalStandard/${param0}/detail`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getFdNationalStandardGetStandardDetail */
export const getApiFdNationalStandardStandardCodeDetail = getFdNationalStandardGetStandardDetail;
/** 获取标准的完整树形结构 GET /api/FdNationalStandard/${param0}/tree */
export async function getFdNationalStandardGetStandardTree(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getFdNationalStandardGetStandardTreeParams,
	options?: { [key: string]: any }
) {
	const { standardCode: param0, ...queryParams } = params;

	return request<APIModel.TreeModel1FdNationalStandardItemDto>(`/api/FdNationalStandard/${param0}/tree`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getFdNationalStandardGetStandardTree */
export const getApiFdNationalStandardStandardCodeTree = getFdNationalStandardGetStandardTree;
/** 更新标准版本（版本升级） POST /api/FdNationalStandard/${param0}/version */
export async function postFdNationalStandardUpdateVersion(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.postFdNationalStandardUpdateVersionParams,
	body: APIModel.UpdateVersionRequest,
	options?: { [key: string]: any }
) {
	const { standardCode: param0, ...queryParams } = params;

	return request<any>(`/api/FdNationalStandard/${param0}/version`, {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		params: { ...queryParams },
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postFdNationalStandardUpdateVersion */
export const postApiFdNationalStandardStandardCodeVersion = postFdNationalStandardUpdateVersion;
/** 根据实体主键批量更新实体信息 根据实体主键批量更新实体信息 PUT /api/FdNationalStandard/batch */
export async function putGenericDtoControllerBase5UpdateMany(body: APIModel.UpdateFdNationalStandardDto[], options?: { [key: string]: any }) {
	return request<number>('/api/FdNationalStandard/batch', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putGenericDtoControllerBase5UpdateMany */
export const putApiFdNationalStandardBatch = putGenericDtoControllerBase5UpdateMany;
/** 批量创建新记录 根据提供的数据批量创建新记录。 POST /api/FdNationalStandard/batch */
export async function postGenericDtoControllerBase5CreateMany(body: APIModel.CreateFdNationalStandardDto[], options?: { [key: string]: any }) {
	return request<number>('/api/FdNationalStandard/batch', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5CreateMany */
export const postApiFdNationalStandardBatch = postGenericDtoControllerBase5CreateMany;
/** 批量删除记录 根据提供的ID列表，批量删除多条记录。 DELETE /api/FdNationalStandard/batch */
export async function deleteGenericDtoControllerBase5BatchDelete(body: string[], options?: { [key: string]: any }) {
	return request<number>('/api/FdNationalStandard/batch', {
		method: 'DELETE',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteGenericDtoControllerBase5BatchDelete */
export const deleteApiFdNationalStandardBatch = deleteGenericDtoControllerBase5BatchDelete;
/** 根据条件批量更新实体属性（部分字段更新） 根据条件批量更新实体属性（部分字段更新） PUT /api/FdNationalStandard/batch/updatebycondition */
export async function putGenericDtoControllerBase5UpdateManyByCondition(
	body: APIModel.BatchUpdateByConditionDto1UpdateFdNationalStandardDto,
	options?: { [key: string]: any }
) {
	return request<number>('/api/FdNationalStandard/batch/updatebycondition', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putGenericDtoControllerBase5UpdateManyByCondition */
export const putApiFdNationalStandardBatchUpdatebycondition = putGenericDtoControllerBase5UpdateManyByCondition;
/** 导入标准及其条目（批量操作） POST /api/FdNationalStandard/import */
export async function postFdNationalStandardImportStandard(body: APIModel.ImportStandardRequest, options?: { [key: string]: any }) {
	return request<string>('/api/FdNationalStandard/import', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postFdNationalStandardImportStandard */
export const postApiFdNationalStandardOpenApiImport = postFdNationalStandardImportStandard;
/** 根据自定义条件获取列表(不分页) 根据自定义条件获取列表(不分页) POST /api/FdNationalStandard/list-by-condition */
export async function postGenericDtoControllerBase5GetListByCondition(body: APIModel.QueryByConditionDto, options?: { [key: string]: any }) {
	return request<any>('/api/FdNationalStandard/list-by-condition', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5GetListByCondition */
export const postApiFdNationalStandardListByCondition = postGenericDtoControllerBase5GetListByCondition;
/** 分页获取记录 根据页码和页面大小，分页检索记录。 GET /api/FdNationalStandard/page */
export async function getGenericDtoControllerBase5GetPage(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getGenericDtoControllerBase5GetPageParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.PageInfo>('/api/FdNationalStandard/page', {
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
export const getApiFdNationalStandardPage = getGenericDtoControllerBase5GetPage;
/** 根据条件分页获取记录 根据提供的查询条件和分页参数，分页检索记录。 POST /api/FdNationalStandard/page/search */
export async function postGenericDtoControllerBase5GetPageByCondition(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/FdNationalStandard/page/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5GetPageByCondition */
export const postApiFdNationalStandardPageSearch = postGenericDtoControllerBase5GetPageByCondition;
/** 获取回收站数据 检索并返回已软删除的记录（回收站数据）。 GET /api/FdNationalStandard/recyclebin */
export async function getGenericDtoControllerBase5GetRecycleBin(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getGenericDtoControllerBase5GetRecycleBinParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.PageInfo>('/api/FdNationalStandard/recyclebin', {
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
export const getApiFdNationalStandardRecyclebin = getGenericDtoControllerBase5GetRecycleBin;
/** 永久删除回收站中的记录 根据提供的ID，将已软删除的记录从数据库中永久移除。 DELETE /api/FdNationalStandard/recyclebin/${param0}/permanent */
export async function deleteGenericDtoControllerBase5PermanentDelete(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteGenericDtoControllerBase5PermanentDeleteParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/FdNationalStandard/recyclebin/${param0}/permanent`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteGenericDtoControllerBase5PermanentDelete */
export const deleteApiFdNationalStandardRecyclebinIdPermanent = deleteGenericDtoControllerBase5PermanentDelete;
/** 恢复回收站中的记录 根据提供的ID，将已软删除的记录恢复到正常状态。 PUT /api/FdNationalStandard/recyclebin/${param0}/restore */
export async function putGenericDtoControllerBase5Restore(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putGenericDtoControllerBase5RestoreParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/FdNationalStandard/recyclebin/${param0}/restore`, {
		method: 'PUT',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putGenericDtoControllerBase5Restore */
export const putApiFdNationalStandardRecyclebinIdRestore = putGenericDtoControllerBase5Restore;
/** 根据条件永久删除回收站中的记录 根据提供的条件，将回收站中符合条件的记录从数据库中永久移除。 POST /api/FdNationalStandard/recyclebin/permanent */
export async function postGenericDtoControllerBase5PermanentDeleteBatch(
	body: APIModel.Expression1Func2FdNationalStandard_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral,
	options?: { [key: string]: any }
) {
	return request<number>('/api/FdNationalStandard/recyclebin/permanent', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5PermanentDeleteBatch */
export const postApiFdNationalStandardRecyclebinPermanent = postGenericDtoControllerBase5PermanentDeleteBatch;
/** 批量恢复回收站中的记录 根据提供的条件，批量将回收站中的记录恢复到正常状态。 POST /api/FdNationalStandard/recyclebin/restore */
export async function postGenericDtoControllerBase5RestoreBatch(
	body: APIModel.Expression1Func2FdNationalStandard_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral,
	options?: { [key: string]: any }
) {
	return request<number>('/api/FdNationalStandard/recyclebin/restore', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5RestoreBatch */
export const postApiFdNationalStandardRecyclebinRestore = postGenericDtoControllerBase5RestoreBatch;
/** 根据条件查询回收站数据 根据提供的查询条件，检索回收站中的记录。 POST /api/FdNationalStandard/recyclebin/search */
export async function postGenericDtoControllerBase5SearchRecycleBin(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/FdNationalStandard/recyclebin/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5SearchRecycleBin */
export const postApiFdNationalStandardRecyclebinSearch = postGenericDtoControllerBase5SearchRecycleBin;
