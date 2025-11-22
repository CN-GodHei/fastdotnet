// @ts-ignore
/* eslint-disable */
import request from '/@/utils/request';

/** 获取所有记录 检索并返回系统中该类型的所有记录。 GET /api/admin/FdRateLimitRules */
export async function getAdminFdRateLimitRules(options?: { [key: string]: any }) {
	return request<APIModel.FdRateLimitRuleDto[]>('/api/admin/FdRateLimitRules', {
		method: 'GET',
		...(options || {}),
	});
}

/** 创建新记录 根据提供的数据创建一条新记录。 POST /api/admin/FdRateLimitRules */
export async function postAdminFdRateLimitRules(body: APIModel.CreateFdRateLimitRuleDto, options?: { [key: string]: any }) {
	return request<APIModel.FdRateLimitRuleDto>('/api/admin/FdRateLimitRules', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 根据ID获取记录 根据提供的唯一标识符(ID)检索特定记录的详细信息。 GET /api/admin/FdRateLimitRules/${param0} */
export async function getAdminFdRateLimitRulesId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAdminFdRateLimitRulesIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<APIModel.FdRateLimitRuleDto>(`/api/admin/FdRateLimitRules/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 更新现有记录 根据提供的ID和更新数据，修改现有记录的信息。 PUT /api/admin/FdRateLimitRules/${param0} */
export async function putAdminFdRateLimitRulesId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putAdminFdRateLimitRulesIdParams,
	body: APIModel.UpdateFdRateLimitRuleDto,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<APIModel.FdRateLimitRuleDto>(`/api/admin/FdRateLimitRules/${param0}`, {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		params: { ...queryParams },
		data: body,
		...(options || {}),
	});
}

/** 删除记录 根据提供的ID，从系统中移除指定的记录。 DELETE /api/admin/FdRateLimitRules/${param0} */
export async function deleteAdminFdRateLimitRulesId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteAdminFdRateLimitRulesIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<boolean>(`/api/admin/FdRateLimitRules/${param0}`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 根据实体主键批量更新实体信息 根据实体主键批量更新实体信息 PUT /api/admin/FdRateLimitRules/batch */
export async function putAdminFdRateLimitRulesBatch(body: APIModel.UpdateFdRateLimitRuleDto[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdRateLimitRules/batch', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 批量创建新记录 根据提供的数据批量创建新记录。 POST /api/admin/FdRateLimitRules/batch */
export async function postAdminFdRateLimitRulesBatch(body: APIModel.CreateFdRateLimitRuleDto[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdRateLimitRules/batch', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 批量删除记录 根据提供的ID列表，批量删除多条记录。 DELETE /api/admin/FdRateLimitRules/batch */
export async function deleteAdminFdRateLimitRulesBatch(body: string[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdRateLimitRules/batch', {
		method: 'DELETE',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 根据条件批量更新实体属性（部分字段更新） 根据条件批量更新实体属性（部分字段更新） PUT /api/admin/FdRateLimitRules/batch/updatebycondition */
export async function putAdminFdRateLimitRulesBatchUpdatebycondition(
	body: APIModel.UpdateFdRateLimitRuleDtoBatchUpdateByConditionDto,
	options?: { [key: string]: any }
) {
	return request<number>('/api/admin/FdRateLimitRules/batch/updatebycondition', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 根据类型和键获取限流规则 GET /api/admin/FdRateLimitRules/by-type-and-key */
export async function getAdminFdRateLimitRulesByTypeAndKey(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAdminFdRateLimitRulesByTypeAndKeyParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.FdRateLimitRuleDto>('/api/admin/FdRateLimitRules/by-type-and-key', {
		method: 'GET',
		params: {
			...params,
		},
		...(options || {}),
	});
}

/** 检查是否触发限流 注意：这个方法仅作演示用途。实际的限流检查应该在中间件中完成，
而不是通过API调用。这里只是为了展示如何在控制器中使用仓储。 GET /api/admin/FdRateLimitRules/check */
export async function getAdminFdRateLimitRulesCheck(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAdminFdRateLimitRulesCheckParams,
	options?: { [key: string]: any }
) {
	return request<boolean>('/api/admin/FdRateLimitRules/check', {
		method: 'GET',
		params: {
			...params,
		},
		...(options || {}),
	});
}

/** 分页获取记录 根据页码和页面大小，分页检索记录。 GET /api/admin/FdRateLimitRules/page */
export async function getAdminFdRateLimitRulesPage(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAdminFdRateLimitRulesPageParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.FdRateLimitRuleDtoPageResult>('/api/admin/FdRateLimitRules/page', {
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

/** 根据条件分页获取记录 根据提供的查询条件和分页参数，分页检索记录。 POST /api/admin/FdRateLimitRules/page/search */
export async function postAdminFdRateLimitRulesPageSearch(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.FdRateLimitRuleDtoPageResult>('/api/admin/FdRateLimitRules/page/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}

/** 获取回收站数据 检索并返回已软删除的记录（回收站数据）。 GET /api/admin/FdRateLimitRules/recyclebin */
export async function getAdminFdRateLimitRulesRecyclebin(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAdminFdRateLimitRulesRecyclebinParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.FdRateLimitRuleDtoPageResult>('/api/admin/FdRateLimitRules/recyclebin', {
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

/** 永久删除回收站中的记录 根据提供的ID，将已软删除的记录从数据库中永久移除。 DELETE /api/admin/FdRateLimitRules/recyclebin/${param0}/permanent */
export async function deleteAdminFdRateLimitRulesRecyclebinIdPermanent(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteAdminFdRateLimitRulesRecyclebinIdPermanentParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<boolean>(`/api/admin/FdRateLimitRules/recyclebin/${param0}/permanent`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 恢复回收站中的记录 根据提供的ID，将已软删除的记录恢复到正常状态。 PUT /api/admin/FdRateLimitRules/recyclebin/${param0}/restore */
export async function putAdminFdRateLimitRulesRecyclebinIdRestore(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putAdminFdRateLimitRulesRecyclebinIdRestoreParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<boolean>(`/api/admin/FdRateLimitRules/recyclebin/${param0}/restore`, {
		method: 'PUT',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 根据条件永久删除回收站中的记录 根据提供的条件，将回收站中符合条件的记录从数据库中永久移除。 POST /api/admin/FdRateLimitRules/recyclebin/permanent */
export async function postAdminFdRateLimitRulesRecyclebinPermanent(
	body: APIModel.FdRateLimitRuleBooleanFuncExpression,
	options?: { [key: string]: any }
) {
	return request<number>('/api/admin/FdRateLimitRules/recyclebin/permanent', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 批量恢复回收站中的记录 根据提供的条件，批量将回收站中的记录恢复到正常状态。 POST /api/admin/FdRateLimitRules/recyclebin/restore */
export async function postAdminFdRateLimitRulesRecyclebinRestore(
	body: APIModel.FdRateLimitRuleBooleanFuncExpression,
	options?: { [key: string]: any }
) {
	return request<number>('/api/admin/FdRateLimitRules/recyclebin/restore', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 根据条件查询回收站数据 根据提供的查询条件，检索回收站中的记录。 POST /api/admin/FdRateLimitRules/recyclebin/search */
export async function postAdminFdRateLimitRulesRecyclebinSearch(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.FdRateLimitRuleDtoPageResult>('/api/admin/FdRateLimitRules/recyclebin/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}
