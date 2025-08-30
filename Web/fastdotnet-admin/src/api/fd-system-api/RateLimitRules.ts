// @ts-ignore
/* eslint-disable */
import request from '/@/utils/request';

/** 获取所有记录 检索并返回系统中该类型的所有记录。 GET /api/admin/RateLimitRules */
export async function getAdminRateLimitRules(options?: { [key: string]: any }) {
	return request<APIModel.FdRateLimitRuleDto[]>('/api/admin/RateLimitRules', {
		method: 'GET',
		...(options || {}),
	});
}

/** 创建新记录 根据提供的数据创建一条新记录。 POST /api/admin/RateLimitRules */
export async function postAdminRateLimitRules(body: APIModel.CreateFdRateLimitRuleDto, options?: { [key: string]: any }) {
	return request<APIModel.FdRateLimitRuleDto>('/api/admin/RateLimitRules', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 根据ID获取记录 根据提供的唯一标识符(ID)检索特定记录的详细信息。 GET /api/admin/RateLimitRules/${param0} */
export async function getAdminRateLimitRulesId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAdminRateLimitRulesIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<APIModel.FdRateLimitRuleDto>(`/api/admin/RateLimitRules/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 更新现有记录 根据提供的ID和更新数据，修改现有记录的信息。 PUT /api/admin/RateLimitRules/${param0} */
export async function putAdminRateLimitRulesId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putAdminRateLimitRulesIdParams,
	body: APIModel.UpdateFdRateLimitRuleDto,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<APIModel.FdRateLimitRuleDto>(`/api/admin/RateLimitRules/${param0}`, {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		params: { ...queryParams },
		data: body,
		...(options || {}),
	});
}

/** 删除记录 根据提供的ID，从系统中移除指定的记录。 DELETE /api/admin/RateLimitRules/${param0} */
export async function deleteAdminRateLimitRulesId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteAdminRateLimitRulesIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<boolean>(`/api/admin/RateLimitRules/${param0}`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 批量删除记录 根据提供的ID列表，批量删除多条记录。 DELETE /api/admin/RateLimitRules/batch */
export async function deleteAdminRateLimitRulesBatch(body: string[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/RateLimitRules/batch', {
		method: 'DELETE',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 根据类型和键获取限流规则 GET /api/admin/RateLimitRules/by-type-and-key */
export async function getAdminRateLimitRulesByTypeAndKey(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAdminRateLimitRulesByTypeAndKeyParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.FdRateLimitRuleDto>('/api/admin/RateLimitRules/by-type-and-key', {
		method: 'GET',
		params: {
			...params,
		},
		...(options || {}),
	});
}

/** 检查是否触发限流 注意：这个方法仅作演示用途。实际的限流检查应该在中间件中完成，
而不是通过API调用。这里只是为了展示如何在控制器中使用仓储。 GET /api/admin/RateLimitRules/check */
export async function getAdminRateLimitRulesCheck(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAdminRateLimitRulesCheckParams,
	options?: { [key: string]: any }
) {
	return request<boolean>('/api/admin/RateLimitRules/check', {
		method: 'GET',
		params: {
			...params,
		},
		...(options || {}),
	});
}

/** 分页获取记录 根据页码和页面大小，分页检索记录。 GET /api/admin/RateLimitRules/page */
export async function getAdminRateLimitRulesPage(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAdminRateLimitRulesPageParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.FdRateLimitRuleDtoPageResult>('/api/admin/RateLimitRules/page', {
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

/** 根据条件分页获取记录 根据提供的查询条件和分页参数，分页检索记录。 POST /api/admin/RateLimitRules/page/search */
export async function postAdminRateLimitRulesPageSearch(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.FdRateLimitRuleDtoPageResult>('/api/admin/RateLimitRules/page/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}

/** 获取回收站数据 检索并返回已软删除的记录（回收站数据）。 GET /api/admin/RateLimitRules/recyclebin */
export async function getAdminRateLimitRulesRecyclebin(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAdminRateLimitRulesRecyclebinParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.FdRateLimitRuleDtoPageResult>('/api/admin/RateLimitRules/recyclebin', {
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

/** 永久删除回收站中的记录 根据提供的ID，将已软删除的记录从数据库中永久移除。 DELETE /api/admin/RateLimitRules/recyclebin/${param0}/permanent */
export async function deleteAdminRateLimitRulesRecyclebinIdPermanent(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteAdminRateLimitRulesRecyclebinIdPermanentParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<boolean>(`/api/admin/RateLimitRules/recyclebin/${param0}/permanent`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 恢复回收站中的记录 根据提供的ID，将已软删除的记录恢复到正常状态。 PUT /api/admin/RateLimitRules/recyclebin/${param0}/restore */
export async function putAdminRateLimitRulesRecyclebinIdRestore(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putAdminRateLimitRulesRecyclebinIdRestoreParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<boolean>(`/api/admin/RateLimitRules/recyclebin/${param0}/restore`, {
		method: 'PUT',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 根据条件永久删除回收站中的记录 根据提供的条件，将回收站中符合条件的记录从数据库中永久移除。 POST /api/admin/RateLimitRules/recyclebin/permanent */
export async function postAdminRateLimitRulesRecyclebinPermanent(
	body: APIModel.FdRateLimitRuleBooleanFuncExpression,
	options?: { [key: string]: any }
) {
	return request<number>('/api/admin/RateLimitRules/recyclebin/permanent', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 批量恢复回收站中的记录 根据提供的条件，批量将回收站中的记录恢复到正常状态。 POST /api/admin/RateLimitRules/recyclebin/restore */
export async function postAdminRateLimitRulesRecyclebinRestore(
	body: APIModel.FdRateLimitRuleBooleanFuncExpression,
	options?: { [key: string]: any }
) {
	return request<number>('/api/admin/RateLimitRules/recyclebin/restore', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 根据条件查询回收站数据 根据提供的查询条件，检索回收站中的记录。 POST /api/admin/RateLimitRules/recyclebin/search */
export async function postAdminRateLimitRulesRecyclebinSearch(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.FdRateLimitRuleDtoPageResult>('/api/admin/RateLimitRules/recyclebin/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}
