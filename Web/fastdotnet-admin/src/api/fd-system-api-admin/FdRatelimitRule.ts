// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 获取所有记录 检索并返回系统中该类型的所有记录。 GET /api/admin/FdRatelimitRule */
/** @deprecated 请使用 getGenericDtoControllerBase5GetAll，原函数名: getApiAdminFdRatelimitRule */
export async function getGenericDtoControllerBase5GetAll(options?: { [key: string]: any }) {
	return request<APIModel.FdRateLimitRuleDto[]>('/api/admin/FdRatelimitRule', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getGenericDtoControllerBase5GetAll */
export const getApiAdminFdRatelimitRule = getGenericDtoControllerBase5GetAll;
/** 创建新记录 根据提供的数据创建一条新记录。 POST /api/admin/FdRatelimitRule */
/** @deprecated 请使用 postGenericDtoControllerBase5Create，原函数名: postApiAdminFdRatelimitRule */
export async function postGenericDtoControllerBase5Create(body: APIModel.CreateFdRateLimitRuleDto, options?: { [key: string]: any }) {
	return request<APIModel.FdRateLimitRuleDto>('/api/admin/FdRatelimitRule', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5Create */
export const postApiAdminFdRatelimitRule = postGenericDtoControllerBase5Create;
/** 根据ID获取记录 根据提供的唯一标识符(ID)检索特定记录的详细信息。 GET /api/admin/FdRatelimitRule/${param0} */
/** @deprecated 请使用 getGenericDtoControllerBase5GetById，原函数名: getApiAdminFdRatelimitRuleId */
export async function getGenericDtoControllerBase5GetById(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getGenericDtoControllerBase5GetByIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<APIModel.FdRateLimitRuleDto>(`/api/admin/FdRatelimitRule/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getGenericDtoControllerBase5GetById */
export const getApiAdminFdRatelimitRuleId = getGenericDtoControllerBase5GetById;
/** 更新现有记录 根据提供的ID和更新数据，修改现有记录的信息。 PUT /api/admin/FdRatelimitRule/${param0} */
/** @deprecated 请使用 putGenericDtoControllerBase5Update，原函数名: putApiAdminFdRatelimitRuleId */
export async function putGenericDtoControllerBase5Update(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putGenericDtoControllerBase5UpdateParams,
	body: APIModel.UpdateFdRateLimitRuleDto,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<APIModel.FdRateLimitRuleDto>(`/api/admin/FdRatelimitRule/${param0}`, {
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
export const putApiAdminFdRatelimitRuleId = putGenericDtoControllerBase5Update;
/** 删除记录 根据提供的ID，从系统中移除指定的记录。 DELETE /api/admin/FdRatelimitRule/${param0} */
/** @deprecated 请使用 deleteGenericDtoControllerBase5Delete，原函数名: deleteApiAdminFdRatelimitRuleId */
export async function deleteGenericDtoControllerBase5Delete(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteGenericDtoControllerBase5DeleteParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/admin/FdRatelimitRule/${param0}`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteGenericDtoControllerBase5Delete */
export const deleteApiAdminFdRatelimitRuleId = deleteGenericDtoControllerBase5Delete;
/** 根据实体主键批量更新实体信息 根据实体主键批量更新实体信息 PUT /api/admin/FdRatelimitRule/batch */
/** @deprecated 请使用 putGenericDtoControllerBase5UpdateMany，原函数名: putApiAdminFdRatelimitRuleBatch */
export async function putGenericDtoControllerBase5UpdateMany(body: APIModel.UpdateFdRateLimitRuleDto[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdRatelimitRule/batch', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putGenericDtoControllerBase5UpdateMany */
export const putApiAdminFdRatelimitRuleBatch = putGenericDtoControllerBase5UpdateMany;
/** 批量创建新记录 根据提供的数据批量创建新记录。 POST /api/admin/FdRatelimitRule/batch */
/** @deprecated 请使用 postGenericDtoControllerBase5CreateMany，原函数名: postApiAdminFdRatelimitRuleBatch */
export async function postGenericDtoControllerBase5CreateMany(body: APIModel.CreateFdRateLimitRuleDto[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdRatelimitRule/batch', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5CreateMany */
export const postApiAdminFdRatelimitRuleBatch = postGenericDtoControllerBase5CreateMany;
/** 批量删除记录 根据提供的ID列表，批量删除多条记录。 DELETE /api/admin/FdRatelimitRule/batch */
/** @deprecated 请使用 deleteGenericDtoControllerBase5BatchDelete，原函数名: deleteApiAdminFdRatelimitRuleBatch */
export async function deleteGenericDtoControllerBase5BatchDelete(body: string[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdRatelimitRule/batch', {
		method: 'DELETE',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteGenericDtoControllerBase5BatchDelete */
export const deleteApiAdminFdRatelimitRuleBatch = deleteGenericDtoControllerBase5BatchDelete;
/** 根据条件批量更新实体属性（部分字段更新） 根据条件批量更新实体属性（部分字段更新） PUT /api/admin/FdRatelimitRule/batch/updatebycondition */
/** @deprecated 请使用 putGenericDtoControllerBase5UpdateManyByCondition，原函数名: putApiAdminFdRatelimitRuleBatchUpdatebycondition */
export async function putGenericDtoControllerBase5UpdateManyByCondition(
	body: APIModel.BatchUpdateByConditionDto1UpdateFdRateLimitRuleDto,
	options?: { [key: string]: any }
) {
	return request<number>('/api/admin/FdRatelimitRule/batch/updatebycondition', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putGenericDtoControllerBase5UpdateManyByCondition */
export const putApiAdminFdRatelimitRuleBatchUpdatebycondition = putGenericDtoControllerBase5UpdateManyByCondition;
/** 根据类型和键获取限流规则 GET /api/admin/FdRatelimitRule/by-type-and-key */
/** @deprecated 请使用 getFdRatelimitRuleGetByTypeAndKey，原函数名: getApiAdminFdRatelimitRuleByTypeAndKey */
export async function getFdRatelimitRuleGetByTypeAndKey(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getFdRatelimitRuleGetByTypeAndKeyParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.FdRateLimitRuleDto>('/api/admin/FdRatelimitRule/by-type-and-key', {
		method: 'GET',
		params: {
			...params,
		},
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getFdRatelimitRuleGetByTypeAndKey */
export const getApiAdminFdRatelimitRuleByTypeAndKey = getFdRatelimitRuleGetByTypeAndKey;
/** 检查是否触发限流 注意：这个方法仅作演示用途。实际的限流检查应该在中间件中完成，
而不是通过API调用。这里只是为了展示如何在控制器中使用仓储。 GET /api/admin/FdRatelimitRule/check */
/** @deprecated 请使用 getFdRatelimitRuleIsRateLimited，原函数名: getApiAdminFdRatelimitRuleCheck */
export async function getFdRatelimitRuleIsRateLimited(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getFdRatelimitRuleIsRateLimitedParams,
	options?: { [key: string]: any }
) {
	return request<boolean>('/api/admin/FdRatelimitRule/check', {
		method: 'GET',
		params: {
			...params,
		},
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getFdRatelimitRuleIsRateLimited */
export const getApiAdminFdRatelimitRuleCheck = getFdRatelimitRuleIsRateLimited;
/** 根据自定义条件获取列表(不分页) 根据自定义条件获取列表(不分页) POST /api/admin/FdRatelimitRule/list-by-condition */
/** @deprecated 请使用 postGenericDtoControllerBase5GetListByCondition，原函数名: postApiAdminFdRatelimitRuleListByCondition */
export async function postGenericDtoControllerBase5GetListByCondition(body: APIModel.QueryByConditionDto, options?: { [key: string]: any }) {
	return request<any>('/api/admin/FdRatelimitRule/list-by-condition', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5GetListByCondition */
export const postApiAdminFdRatelimitRuleListByCondition = postGenericDtoControllerBase5GetListByCondition;
/** 分页获取记录 根据页码和页面大小，分页检索记录。 GET /api/admin/FdRatelimitRule/page */
/** @deprecated 请使用 getGenericDtoControllerBase5GetPage，原函数名: getApiAdminFdRatelimitRulePage */
export async function getGenericDtoControllerBase5GetPage(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getGenericDtoControllerBase5GetPageParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.PageInfo>('/api/admin/FdRatelimitRule/page', {
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
export const getApiAdminFdRatelimitRulePage = getGenericDtoControllerBase5GetPage;
/** 根据条件分页获取记录 根据提供的查询条件和分页参数，分页检索记录。 POST /api/admin/FdRatelimitRule/page/search */
/** @deprecated 请使用 postGenericDtoControllerBase5GetPageByCondition，原函数名: postApiAdminFdRatelimitRulePageSearch */
export async function postGenericDtoControllerBase5GetPageByCondition(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/admin/FdRatelimitRule/page/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5GetPageByCondition */
export const postApiAdminFdRatelimitRulePageSearch = postGenericDtoControllerBase5GetPageByCondition;
/** 获取回收站数据 检索并返回已软删除的记录（回收站数据）。 GET /api/admin/FdRatelimitRule/recyclebin */
/** @deprecated 请使用 getGenericDtoControllerBase5GetRecycleBin，原函数名: getApiAdminFdRatelimitRuleRecyclebin */
export async function getGenericDtoControllerBase5GetRecycleBin(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getGenericDtoControllerBase5GetRecycleBinParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.PageInfo>('/api/admin/FdRatelimitRule/recyclebin', {
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
export const getApiAdminFdRatelimitRuleRecyclebin = getGenericDtoControllerBase5GetRecycleBin;
/** 永久删除回收站中的记录 根据提供的ID，将已软删除的记录从数据库中永久移除。 DELETE /api/admin/FdRatelimitRule/recyclebin/${param0}/permanent */
/** @deprecated 请使用 deleteGenericDtoControllerBase5PermanentDelete，原函数名: deleteApiAdminFdRatelimitRuleRecyclebinIdPermanent */
export async function deleteGenericDtoControllerBase5PermanentDelete(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteGenericDtoControllerBase5PermanentDeleteParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/admin/FdRatelimitRule/recyclebin/${param0}/permanent`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteGenericDtoControllerBase5PermanentDelete */
export const deleteApiAdminFdRatelimitRuleRecyclebinIdPermanent = deleteGenericDtoControllerBase5PermanentDelete;
/** 恢复回收站中的记录 根据提供的ID，将已软删除的记录恢复到正常状态。 PUT /api/admin/FdRatelimitRule/recyclebin/${param0}/restore */
/** @deprecated 请使用 putGenericDtoControllerBase5Restore，原函数名: putApiAdminFdRatelimitRuleRecyclebinIdRestore */
export async function putGenericDtoControllerBase5Restore(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putGenericDtoControllerBase5RestoreParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/admin/FdRatelimitRule/recyclebin/${param0}/restore`, {
		method: 'PUT',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putGenericDtoControllerBase5Restore */
export const putApiAdminFdRatelimitRuleRecyclebinIdRestore = putGenericDtoControllerBase5Restore;
/** 根据条件永久删除回收站中的记录 根据提供的条件，将回收站中符合条件的记录从数据库中永久移除。 POST /api/admin/FdRatelimitRule/recyclebin/permanent */
/** @deprecated 请使用 postGenericDtoControllerBase5PermanentDeleteBatch，原函数名: postApiAdminFdRatelimitRuleRecyclebinPermanent */
export async function postGenericDtoControllerBase5PermanentDeleteBatch(
	body: APIModel.Expression1Func2FdRateLimitRule_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral,
	options?: { [key: string]: any }
) {
	return request<number>('/api/admin/FdRatelimitRule/recyclebin/permanent', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5PermanentDeleteBatch */
export const postApiAdminFdRatelimitRuleRecyclebinPermanent = postGenericDtoControllerBase5PermanentDeleteBatch;
/** 批量恢复回收站中的记录 根据提供的条件，批量将回收站中的记录恢复到正常状态。 POST /api/admin/FdRatelimitRule/recyclebin/restore */
/** @deprecated 请使用 postGenericDtoControllerBase5RestoreBatch，原函数名: postApiAdminFdRatelimitRuleRecyclebinRestore */
export async function postGenericDtoControllerBase5RestoreBatch(
	body: APIModel.Expression1Func2FdRateLimitRule_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral,
	options?: { [key: string]: any }
) {
	return request<number>('/api/admin/FdRatelimitRule/recyclebin/restore', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5RestoreBatch */
export const postApiAdminFdRatelimitRuleRecyclebinRestore = postGenericDtoControllerBase5RestoreBatch;
/** 根据条件查询回收站数据 根据提供的查询条件，检索回收站中的记录。 POST /api/admin/FdRatelimitRule/recyclebin/search */
/** @deprecated 请使用 postGenericDtoControllerBase5SearchRecycleBin，原函数名: postApiAdminFdRatelimitRuleRecyclebinSearch */
export async function postGenericDtoControllerBase5SearchRecycleBin(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/admin/FdRatelimitRule/recyclebin/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5SearchRecycleBin */
export const postApiAdminFdRatelimitRuleRecyclebinSearch = postGenericDtoControllerBase5SearchRecycleBin;
