// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 获取所有记录 检索并返回系统中该类型的所有记录。 GET /api/admin/FdRatelimitRule */
export async function getApiAdminFdRatelimitRule(options?: { [key: string]: any }) {
	return request<APIModel.FdRateLimitRuleDto[]>('/api/admin/FdRatelimitRule', {
		method: 'GET',
		...(options || {}),
	});
}
/** 创建新记录 根据提供的数据创建一条新记录。 POST /api/admin/FdRatelimitRule */
export async function postApiAdminFdRatelimitRule(body: APIModel.CreateFdRateLimitRuleDto, options?: { [key: string]: any }) {
	return request<APIModel.FdRateLimitRuleDto>('/api/admin/FdRatelimitRule', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 根据ID获取记录 根据提供的唯一标识符(ID)检索特定记录的详细信息。 GET /api/admin/FdRatelimitRule/${param0} */
export async function getApiAdminFdRatelimitRuleId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getApiAdminFdRatelimitRuleIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<APIModel.FdRateLimitRuleDto>(`/api/admin/FdRatelimitRule/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}
/** 更新现有记录 根据提供的ID和更新数据，修改现有记录的信息。 PUT /api/admin/FdRatelimitRule/${param0} */
export async function putApiAdminFdRatelimitRuleId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putApiAdminFdRatelimitRuleIdParams,
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
/** 删除记录 根据提供的ID，从系统中移除指定的记录。 DELETE /api/admin/FdRatelimitRule/${param0} */
export async function deleteApiAdminFdRatelimitRuleId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteApiAdminFdRatelimitRuleIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/admin/FdRatelimitRule/${param0}`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}
/** 根据实体主键批量更新实体信息 根据实体主键批量更新实体信息 PUT /api/admin/FdRatelimitRule/batch */
export async function putApiAdminFdRatelimitRuleBatch(body: APIModel.UpdateFdRateLimitRuleDto[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdRatelimitRule/batch', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 批量创建新记录 根据提供的数据批量创建新记录。 POST /api/admin/FdRatelimitRule/batch */
export async function postApiAdminFdRatelimitRuleBatch(body: APIModel.CreateFdRateLimitRuleDto[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdRatelimitRule/batch', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 批量删除记录 根据提供的ID列表，批量删除多条记录。 DELETE /api/admin/FdRatelimitRule/batch */
export async function deleteApiAdminFdRatelimitRuleBatch(body: string[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdRatelimitRule/batch', {
		method: 'DELETE',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 根据条件批量更新实体属性（部分字段更新） 根据条件批量更新实体属性（部分字段更新） PUT /api/admin/FdRatelimitRule/batch/updatebycondition */
export async function putApiAdminFdRatelimitRuleBatchUpdatebycondition(
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
/** 根据类型和键获取限流规则 GET /api/admin/FdRatelimitRule/by-type-and-key */
export async function getApiAdminFdRatelimitRuleByTypeAndKey(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getApiAdminFdRatelimitRuleByTypeAndKeyParams,
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
/** 检查是否触发限流 注意：这个方法仅作演示用途。实际的限流检查应该在中间件中完成，
而不是通过API调用。这里只是为了展示如何在控制器中使用仓储。 GET /api/admin/FdRatelimitRule/check */
export async function getApiAdminFdRatelimitRuleCheck(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getApiAdminFdRatelimitRuleCheckParams,
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
/** 根据自定义条件获取列表(不分页) 根据自定义条件获取列表(不分页) POST /api/admin/FdRatelimitRule/list-by-condition */
export async function postApiAdminFdRatelimitRuleListByCondition(body: APIModel.QueryByConditionDto, options?: { [key: string]: any }) {
	return request<any>('/api/admin/FdRatelimitRule/list-by-condition', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 分页获取记录 根据页码和页面大小，分页检索记录。 GET /api/admin/FdRatelimitRule/page */
export async function getApiAdminFdRatelimitRulePage(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getApiAdminFdRatelimitRulePageParams,
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
/** 根据条件分页获取记录 根据提供的查询条件和分页参数，分页检索记录。 POST /api/admin/FdRatelimitRule/page/search */
export async function postApiAdminFdRatelimitRulePageSearch(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/admin/FdRatelimitRule/page/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}
/** 获取回收站数据 检索并返回已软删除的记录（回收站数据）。 GET /api/admin/FdRatelimitRule/recyclebin */
export async function getApiAdminFdRatelimitRuleRecyclebin(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getApiAdminFdRatelimitRuleRecyclebinParams,
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
/** 永久删除回收站中的记录 根据提供的ID，将已软删除的记录从数据库中永久移除。 DELETE /api/admin/FdRatelimitRule/recyclebin/${param0}/permanent */
export async function deleteApiAdminFdRatelimitRuleRecyclebinIdPermanent(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteApiAdminFdRatelimitRuleRecyclebinIdPermanentParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/admin/FdRatelimitRule/recyclebin/${param0}/permanent`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}
/** 恢复回收站中的记录 根据提供的ID，将已软删除的记录恢复到正常状态。 PUT /api/admin/FdRatelimitRule/recyclebin/${param0}/restore */
export async function putApiAdminFdRatelimitRuleRecyclebinIdRestore(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putApiAdminFdRatelimitRuleRecyclebinIdRestoreParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/admin/FdRatelimitRule/recyclebin/${param0}/restore`, {
		method: 'PUT',
		params: { ...queryParams },
		...(options || {}),
	});
}
/** 根据条件永久删除回收站中的记录 根据提供的条件，将回收站中符合条件的记录从数据库中永久移除。 POST /api/admin/FdRatelimitRule/recyclebin/permanent */
export async function postApiAdminFdRatelimitRuleRecyclebinPermanent(
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
/** 批量恢复回收站中的记录 根据提供的条件，批量将回收站中的记录恢复到正常状态。 POST /api/admin/FdRatelimitRule/recyclebin/restore */
export async function postApiAdminFdRatelimitRuleRecyclebinRestore(
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
/** 根据条件查询回收站数据 根据提供的查询条件，检索回收站中的记录。 POST /api/admin/FdRatelimitRule/recyclebin/search */
export async function postApiAdminFdRatelimitRuleRecyclebinSearch(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/admin/FdRatelimitRule/recyclebin/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}
