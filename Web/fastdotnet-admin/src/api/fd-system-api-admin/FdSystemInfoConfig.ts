// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 获取所有记录 检索并返回系统中该类型的所有记录。 GET /api/admin/FdSystemInfoConfig */
export async function getGenericDtoControllerBase5GetAll(options?: { [key: string]: any }) {
	return request<APIModel.FdSystemInfoConfigDto[]>('/api/admin/FdSystemInfoConfig', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getGenericDtoControllerBase5GetAll */
export const getApiAdminFdSystemInfoConfig = getGenericDtoControllerBase5GetAll;
/** 创建新记录 根据提供的数据创建一条新记录。 POST /api/admin/FdSystemInfoConfig */
export async function postGenericDtoControllerBase5Create(body: APIModel.CreateFdSystemInfoConfigDto, options?: { [key: string]: any }) {
	return request<APIModel.FdSystemInfoConfigDto>('/api/admin/FdSystemInfoConfig', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5Create */
export const postApiAdminFdSystemInfoConfig = postGenericDtoControllerBase5Create;
/** 根据ID获取记录 根据提供的唯一标识符(ID)检索特定记录的详细信息。 GET /api/admin/FdSystemInfoConfig/${param0} */
export async function getGenericDtoControllerBase5GetById(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getGenericDtoControllerBase5GetByIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<APIModel.FdSystemInfoConfigDto>(`/api/admin/FdSystemInfoConfig/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getGenericDtoControllerBase5GetById */
export const getApiAdminFdSystemInfoConfigId = getGenericDtoControllerBase5GetById;
/** 更新现有记录 根据提供的ID和更新数据，修改现有记录的信息。 PUT /api/admin/FdSystemInfoConfig/${param0} */
export async function putGenericDtoControllerBase5Update(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putGenericDtoControllerBase5UpdateParams,
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

/** @deprecated 此函数名已变更，请使用 putGenericDtoControllerBase5Update */
export const putApiAdminFdSystemInfoConfigId = putGenericDtoControllerBase5Update;
/** 删除记录 根据提供的ID，从系统中移除指定的记录。 DELETE /api/admin/FdSystemInfoConfig/${param0} */
export async function deleteGenericDtoControllerBase5Delete(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteGenericDtoControllerBase5DeleteParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/admin/FdSystemInfoConfig/${param0}`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteGenericDtoControllerBase5Delete */
export const deleteApiAdminFdSystemInfoConfigId = deleteGenericDtoControllerBase5Delete;
/** 根据实体主键批量更新实体信息 根据实体主键批量更新实体信息 PUT /api/admin/FdSystemInfoConfig/batch */
export async function putGenericDtoControllerBase5UpdateMany(body: APIModel.UpdateFdSystemInfoConfigDto[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdSystemInfoConfig/batch', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putGenericDtoControllerBase5UpdateMany */
export const putApiAdminFdSystemInfoConfigBatch = putGenericDtoControllerBase5UpdateMany;
/** 批量创建新记录 根据提供的数据批量创建新记录。 POST /api/admin/FdSystemInfoConfig/batch */
export async function postGenericDtoControllerBase5CreateMany(body: APIModel.CreateFdSystemInfoConfigDto[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdSystemInfoConfig/batch', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5CreateMany */
export const postApiAdminFdSystemInfoConfigBatch = postGenericDtoControllerBase5CreateMany;
/** 批量删除记录 根据提供的ID列表，批量删除多条记录。 DELETE /api/admin/FdSystemInfoConfig/batch */
export async function deleteGenericDtoControllerBase5BatchDelete(body: string[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdSystemInfoConfig/batch', {
		method: 'DELETE',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteGenericDtoControllerBase5BatchDelete */
export const deleteApiAdminFdSystemInfoConfigBatch = deleteGenericDtoControllerBase5BatchDelete;
/** 根据条件批量更新实体属性（部分字段更新） 根据条件批量更新实体属性（部分字段更新） PUT /api/admin/FdSystemInfoConfig/batch/updatebycondition */
export async function putGenericDtoControllerBase5UpdateManyByCondition(
	body: APIModel.BatchUpdateByConditionDto1UpdateFdSystemInfoConfigDto,
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

/** @deprecated 此函数名已变更，请使用 putGenericDtoControllerBase5UpdateManyByCondition */
export const putApiAdminFdSystemInfoConfigBatchUpdatebycondition = putGenericDtoControllerBase5UpdateManyByCondition;
/** 此处后端没有提供注释 GET /api/admin/FdSystemInfoConfig/GetServiceDateTime */
export async function getFdSystemInfoConfigGetServiceDateTime(options?: { [key: string]: any }) {
	return request<string>('/api/admin/FdSystemInfoConfig/GetServiceDateTime', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getFdSystemInfoConfigGetServiceDateTime */
export const getApiAdminFdSystemInfoConfigGetServiceDateTime = getFdSystemInfoConfigGetServiceDateTime;
/** 根据自定义条件获取列表(不分页) 根据自定义条件获取列表(不分页) POST /api/admin/FdSystemInfoConfig/list-by-condition */
export async function postGenericDtoControllerBase5GetListByCondition(body: APIModel.QueryByConditionDto, options?: { [key: string]: any }) {
	return request<any>('/api/admin/FdSystemInfoConfig/list-by-condition', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5GetListByCondition */
export const postApiAdminFdSystemInfoConfigListByCondition = postGenericDtoControllerBase5GetListByCondition;
/** 分页获取记录 根据页码和页面大小，分页检索记录。 GET /api/admin/FdSystemInfoConfig/page */
export async function getGenericDtoControllerBase5GetPage(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getGenericDtoControllerBase5GetPageParams,
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

/** @deprecated 此函数名已变更，请使用 getGenericDtoControllerBase5GetPage */
export const getApiAdminFdSystemInfoConfigPage = getGenericDtoControllerBase5GetPage;
/** 根据条件分页获取记录 根据提供的查询条件和分页参数，分页检索记录。 POST /api/admin/FdSystemInfoConfig/page/search */
export async function postGenericDtoControllerBase5GetPageByCondition(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/admin/FdSystemInfoConfig/page/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5GetPageByCondition */
export const postApiAdminFdSystemInfoConfigPageSearch = postGenericDtoControllerBase5GetPageByCondition;
/** [Public] 获取所有系统配置项（用于客户端初始化） GET /api/admin/FdSystemInfoConfig/public/all */
export async function getFdSystemInfoConfigGetPublicConfigs(options?: { [key: string]: any }) {
	return request<Record<string, any>>('/api/admin/FdSystemInfoConfig/public/all', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getFdSystemInfoConfigGetPublicConfigs */
export const getApiAdminFdSystemInfoConfigPublicAll = getFdSystemInfoConfigGetPublicConfigs;
/** 获取回收站数据 检索并返回已软删除的记录（回收站数据）。 GET /api/admin/FdSystemInfoConfig/recyclebin */
export async function getGenericDtoControllerBase5GetRecycleBin(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getGenericDtoControllerBase5GetRecycleBinParams,
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

/** @deprecated 此函数名已变更，请使用 getGenericDtoControllerBase5GetRecycleBin */
export const getApiAdminFdSystemInfoConfigRecyclebin = getGenericDtoControllerBase5GetRecycleBin;
/** 永久删除回收站中的记录 根据提供的ID，将已软删除的记录从数据库中永久移除。 DELETE /api/admin/FdSystemInfoConfig/recyclebin/${param0}/permanent */
export async function deleteGenericDtoControllerBase5PermanentDelete(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteGenericDtoControllerBase5PermanentDeleteParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/admin/FdSystemInfoConfig/recyclebin/${param0}/permanent`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteGenericDtoControllerBase5PermanentDelete */
export const deleteApiAdminFdSystemInfoConfigRecyclebinIdPermanent = deleteGenericDtoControllerBase5PermanentDelete;
/** 恢复回收站中的记录 根据提供的ID，将已软删除的记录恢复到正常状态。 PUT /api/admin/FdSystemInfoConfig/recyclebin/${param0}/restore */
export async function putGenericDtoControllerBase5Restore(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putGenericDtoControllerBase5RestoreParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/admin/FdSystemInfoConfig/recyclebin/${param0}/restore`, {
		method: 'PUT',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putGenericDtoControllerBase5Restore */
export const putApiAdminFdSystemInfoConfigRecyclebinIdRestore = putGenericDtoControllerBase5Restore;
/** 根据条件永久删除回收站中的记录 根据提供的条件，将回收站中符合条件的记录从数据库中永久移除。 POST /api/admin/FdSystemInfoConfig/recyclebin/permanent */
export async function postGenericDtoControllerBase5PermanentDeleteBatch(
	body: APIModel.Expression1Func2SystemInfoConfig_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral,
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

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5PermanentDeleteBatch */
export const postApiAdminFdSystemInfoConfigRecyclebinPermanent = postGenericDtoControllerBase5PermanentDeleteBatch;
/** 批量恢复回收站中的记录 根据提供的条件，批量将回收站中的记录恢复到正常状态。 POST /api/admin/FdSystemInfoConfig/recyclebin/restore */
export async function postGenericDtoControllerBase5RestoreBatch(
	body: APIModel.Expression1Func2SystemInfoConfig_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral,
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

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5RestoreBatch */
export const postApiAdminFdSystemInfoConfigRecyclebinRestore = postGenericDtoControllerBase5RestoreBatch;
/** 根据条件查询回收站数据 根据提供的查询条件，检索回收站中的记录。 POST /api/admin/FdSystemInfoConfig/recyclebin/search */
export async function postGenericDtoControllerBase5SearchRecycleBin(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/admin/FdSystemInfoConfig/recyclebin/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5SearchRecycleBin */
export const postApiAdminFdSystemInfoConfigRecyclebinSearch = postGenericDtoControllerBase5SearchRecycleBin;
