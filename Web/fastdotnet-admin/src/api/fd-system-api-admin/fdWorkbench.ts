// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 获取所有记录 检索并返回系统中该类型的所有记录。 GET /api/admin/FdWorkbench */
/** @deprecated 请使用 getGenericDtoControllerBase5GetAll，原函数名: getApiAdminFdWorkbench */
export async function getGenericDtoControllerBase5GetAll(options?: { [key: string]: any }) {
	return request<APIModel.FdWorkbenchCardDto[]>('/api/admin/FdWorkbench', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getGenericDtoControllerBase5GetAll */
export const getApiAdminFdWorkbench = getGenericDtoControllerBase5GetAll;
/** 创建新记录 根据提供的数据创建一条新记录。 POST /api/admin/FdWorkbench */
/** @deprecated 请使用 postGenericDtoControllerBase5Create，原函数名: postApiAdminFdWorkbench */
export async function postGenericDtoControllerBase5Create(body: APIModel.CreateFdWorkbenchCardDto, options?: { [key: string]: any }) {
	return request<APIModel.FdWorkbenchCardDto>('/api/admin/FdWorkbench', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5Create */
export const postApiAdminFdWorkbench = postGenericDtoControllerBase5Create;
/** 根据ID获取记录 根据提供的唯一标识符(ID)检索特定记录的详细信息。 GET /api/admin/FdWorkbench/${param0} */
/** @deprecated 请使用 getGenericDtoControllerBase5GetById，原函数名: getApiAdminFdWorkbenchId */
export async function getGenericDtoControllerBase5GetById(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getGenericDtoControllerBase5GetByIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<APIModel.FdWorkbenchCardDto>(`/api/admin/FdWorkbench/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getGenericDtoControllerBase5GetById */
export const getApiAdminFdWorkbenchId = getGenericDtoControllerBase5GetById;
/** 更新现有记录 根据提供的ID和更新数据，修改现有记录的信息。 PUT /api/admin/FdWorkbench/${param0} */
/** @deprecated 请使用 putGenericDtoControllerBase5Update，原函数名: putApiAdminFdWorkbenchId */
export async function putGenericDtoControllerBase5Update(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putGenericDtoControllerBase5UpdateParams,
	body: APIModel.UpdateFdWorkbenchCardDto,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<APIModel.FdWorkbenchCardDto>(`/api/admin/FdWorkbench/${param0}`, {
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
export const putApiAdminFdWorkbenchId = putGenericDtoControllerBase5Update;
/** 删除记录 根据提供的ID，从系统中移除指定的记录。 DELETE /api/admin/FdWorkbench/${param0} */
/** @deprecated 请使用 deleteGenericDtoControllerBase5Delete，原函数名: deleteApiAdminFdWorkbenchId */
export async function deleteGenericDtoControllerBase5Delete(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteGenericDtoControllerBase5DeleteParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/admin/FdWorkbench/${param0}`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteGenericDtoControllerBase5Delete */
export const deleteApiAdminFdWorkbenchId = deleteGenericDtoControllerBase5Delete;
/** 获取所有定义的卡片 GET /api/admin/FdWorkbench/all */
/** @deprecated 请使用 getFdWorkbenchGetAll，原函数名: getApiAdminFdWorkbenchAll */
export async function getFdWorkbenchGetAll(options?: { [key: string]: any }) {
	return request<APIModel.FdWorkbenchCardDto[]>('/api/admin/FdWorkbench/all', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getFdWorkbenchGetAll */
export const getApiAdminFdWorkbenchAll = getFdWorkbenchGetAll;
/** 为卡片分配角色 POST /api/admin/FdWorkbench/assign-roles/${param0} */
/** @deprecated 请使用 postFdWorkbenchUpdateCardRoles，原函数名: postApiAdminFdWorkbenchAssignRolesCardId */
export async function postFdWorkbenchUpdateCardRoles(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.postFdWorkbenchUpdateCardRolesParams,
	body: string[],
	options?: { [key: string]: any }
) {
	const { cardId: param0, ...queryParams } = params;

	return request<boolean>(`/api/admin/FdWorkbench/assign-roles/${param0}`, {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		params: { ...queryParams },
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postFdWorkbenchUpdateCardRoles */
export const postApiAdminFdWorkbenchAssignRolesCardId = postFdWorkbenchUpdateCardRoles;
/** 为角色分配卡片 POST /api/admin/FdWorkbench/assign/${param0} */
/** @deprecated 请使用 postFdWorkbenchAssign，原函数名: postApiAdminFdWorkbenchAssignRoleId */
export async function postFdWorkbenchAssign(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.postFdWorkbenchAssignParams,
	body: string[],
	options?: { [key: string]: any }
) {
	const { roleId: param0, ...queryParams } = params;

	return request<boolean>(`/api/admin/FdWorkbench/assign/${param0}`, {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		params: { ...queryParams },
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postFdWorkbenchAssign */
export const postApiAdminFdWorkbenchAssignRoleId = postFdWorkbenchAssign;
/** 根据实体主键批量更新实体信息 根据实体主键批量更新实体信息 PUT /api/admin/FdWorkbench/batch */
/** @deprecated 请使用 putGenericDtoControllerBase5UpdateMany，原函数名: putApiAdminFdWorkbenchBatch */
export async function putGenericDtoControllerBase5UpdateMany(body: APIModel.UpdateFdWorkbenchCardDto[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdWorkbench/batch', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putGenericDtoControllerBase5UpdateMany */
export const putApiAdminFdWorkbenchBatch = putGenericDtoControllerBase5UpdateMany;
/** 批量创建新记录 根据提供的数据批量创建新记录。 POST /api/admin/FdWorkbench/batch */
/** @deprecated 请使用 postGenericDtoControllerBase5CreateMany，原函数名: postApiAdminFdWorkbenchBatch */
export async function postGenericDtoControllerBase5CreateMany(body: APIModel.CreateFdWorkbenchCardDto[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdWorkbench/batch', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5CreateMany */
export const postApiAdminFdWorkbenchBatch = postGenericDtoControllerBase5CreateMany;
/** 批量删除记录 根据提供的ID列表，批量删除多条记录。 DELETE /api/admin/FdWorkbench/batch */
/** @deprecated 请使用 deleteGenericDtoControllerBase5BatchDelete，原函数名: deleteApiAdminFdWorkbenchBatch */
export async function deleteGenericDtoControllerBase5BatchDelete(body: string[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdWorkbench/batch', {
		method: 'DELETE',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteGenericDtoControllerBase5BatchDelete */
export const deleteApiAdminFdWorkbenchBatch = deleteGenericDtoControllerBase5BatchDelete;
/** 根据条件批量更新实体属性（部分字段更新） 根据条件批量更新实体属性（部分字段更新） PUT /api/admin/FdWorkbench/batch/updatebycondition */
/** @deprecated 请使用 putGenericDtoControllerBase5UpdateManyByCondition，原函数名: putApiAdminFdWorkbenchBatchUpdatebycondition */
export async function putGenericDtoControllerBase5UpdateManyByCondition(
	body: APIModel.BatchUpdateByConditionDto1UpdateFdWorkbenchCardDto,
	options?: { [key: string]: any }
) {
	return request<number>('/api/admin/FdWorkbench/batch/updatebycondition', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putGenericDtoControllerBase5UpdateManyByCondition */
export const putApiAdminFdWorkbenchBatchUpdatebycondition = putGenericDtoControllerBase5UpdateManyByCondition;
/** 获取卡片已分配的角色ID列表 GET /api/admin/FdWorkbench/card-roles/${param0} */
/** @deprecated 请使用 getFdWorkbenchGetCardRoles，原函数名: getApiAdminFdWorkbenchCardRolesCardId */
export async function getFdWorkbenchGetCardRoles(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getFdWorkbenchGetCardRolesParams,
	options?: { [key: string]: any }
) {
	const { cardId: param0, ...queryParams } = params;

	return request<string[]>(`/api/admin/FdWorkbench/card-roles/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getFdWorkbenchGetCardRoles */
export const getApiAdminFdWorkbenchCardRolesCardId = getFdWorkbenchGetCardRoles;
/** 根据自定义条件获取列表(不分页) 根据自定义条件获取列表(不分页) POST /api/admin/FdWorkbench/list-by-condition */
/** @deprecated 请使用 postGenericDtoControllerBase5GetListByCondition，原函数名: postApiAdminFdWorkbenchListByCondition */
export async function postGenericDtoControllerBase5GetListByCondition(body: APIModel.QueryByConditionDto, options?: { [key: string]: any }) {
	return request<any>('/api/admin/FdWorkbench/list-by-condition', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5GetListByCondition */
export const postApiAdminFdWorkbenchListByCondition = postGenericDtoControllerBase5GetListByCondition;
/** 分页获取记录 根据页码和页面大小，分页检索记录。 GET /api/admin/FdWorkbench/page */
/** @deprecated 请使用 getGenericDtoControllerBase5GetPage，原函数名: getApiAdminFdWorkbenchPage */
export async function getGenericDtoControllerBase5GetPage(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getGenericDtoControllerBase5GetPageParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.PageInfo>('/api/admin/FdWorkbench/page', {
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
export const getApiAdminFdWorkbenchPage = getGenericDtoControllerBase5GetPage;
/** 根据条件分页获取记录 根据提供的查询条件和分页参数，分页检索记录。 POST /api/admin/FdWorkbench/page/search */
/** @deprecated 请使用 postGenericDtoControllerBase5GetPageByCondition，原函数名: postApiAdminFdWorkbenchPageSearch */
export async function postGenericDtoControllerBase5GetPageByCondition(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/admin/FdWorkbench/page/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5GetPageByCondition */
export const postApiAdminFdWorkbenchPageSearch = postGenericDtoControllerBase5GetPageByCondition;
/** 获取回收站数据 检索并返回已软删除的记录（回收站数据）。 GET /api/admin/FdWorkbench/recyclebin */
/** @deprecated 请使用 getGenericDtoControllerBase5GetRecycleBin，原函数名: getApiAdminFdWorkbenchRecyclebin */
export async function getGenericDtoControllerBase5GetRecycleBin(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getGenericDtoControllerBase5GetRecycleBinParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.PageInfo>('/api/admin/FdWorkbench/recyclebin', {
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
export const getApiAdminFdWorkbenchRecyclebin = getGenericDtoControllerBase5GetRecycleBin;
/** 永久删除回收站中的记录 根据提供的ID，将已软删除的记录从数据库中永久移除。 DELETE /api/admin/FdWorkbench/recyclebin/${param0}/permanent */
/** @deprecated 请使用 deleteGenericDtoControllerBase5PermanentDelete，原函数名: deleteApiAdminFdWorkbenchRecyclebinIdPermanent */
export async function deleteGenericDtoControllerBase5PermanentDelete(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteGenericDtoControllerBase5PermanentDeleteParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/admin/FdWorkbench/recyclebin/${param0}/permanent`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteGenericDtoControllerBase5PermanentDelete */
export const deleteApiAdminFdWorkbenchRecyclebinIdPermanent = deleteGenericDtoControllerBase5PermanentDelete;
/** 恢复回收站中的记录 根据提供的ID，将已软删除的记录恢复到正常状态。 PUT /api/admin/FdWorkbench/recyclebin/${param0}/restore */
/** @deprecated 请使用 putGenericDtoControllerBase5Restore，原函数名: putApiAdminFdWorkbenchRecyclebinIdRestore */
export async function putGenericDtoControllerBase5Restore(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putGenericDtoControllerBase5RestoreParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/admin/FdWorkbench/recyclebin/${param0}/restore`, {
		method: 'PUT',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putGenericDtoControllerBase5Restore */
export const putApiAdminFdWorkbenchRecyclebinIdRestore = putGenericDtoControllerBase5Restore;
/** 根据条件永久删除回收站中的记录 根据提供的条件，将回收站中符合条件的记录从数据库中永久移除。 POST /api/admin/FdWorkbench/recyclebin/permanent */
/** @deprecated 请使用 postGenericDtoControllerBase5PermanentDeleteBatch，原函数名: postApiAdminFdWorkbenchRecyclebinPermanent */
export async function postGenericDtoControllerBase5PermanentDeleteBatch(
	body: APIModel.Expression1Func2FdWorkbenchCard_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral,
	options?: { [key: string]: any }
) {
	return request<number>('/api/admin/FdWorkbench/recyclebin/permanent', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5PermanentDeleteBatch */
export const postApiAdminFdWorkbenchRecyclebinPermanent = postGenericDtoControllerBase5PermanentDeleteBatch;
/** 批量恢复回收站中的记录 根据提供的条件，批量将回收站中的记录恢复到正常状态。 POST /api/admin/FdWorkbench/recyclebin/restore */
/** @deprecated 请使用 postGenericDtoControllerBase5RestoreBatch，原函数名: postApiAdminFdWorkbenchRecyclebinRestore */
export async function postGenericDtoControllerBase5RestoreBatch(
	body: APIModel.Expression1Func2FdWorkbenchCard_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral,
	options?: { [key: string]: any }
) {
	return request<number>('/api/admin/FdWorkbench/recyclebin/restore', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5RestoreBatch */
export const postApiAdminFdWorkbenchRecyclebinRestore = postGenericDtoControllerBase5RestoreBatch;
/** 根据条件查询回收站数据 根据提供的查询条件，检索回收站中的记录。 POST /api/admin/FdWorkbench/recyclebin/search */
/** @deprecated 请使用 postGenericDtoControllerBase5SearchRecycleBin，原函数名: postApiAdminFdWorkbenchRecyclebinSearch */
export async function postGenericDtoControllerBase5SearchRecycleBin(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/admin/FdWorkbench/recyclebin/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5SearchRecycleBin */
export const postApiAdminFdWorkbenchRecyclebinSearch = postGenericDtoControllerBase5SearchRecycleBin;
/** 获取角色已分配的卡片ID列表 GET /api/admin/FdWorkbench/role-cards/${param0} */
/** @deprecated 请使用 getFdWorkbenchGetRoleCards，原函数名: getApiAdminFdWorkbenchRoleCardsRoleId */
export async function getFdWorkbenchGetRoleCards(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getFdWorkbenchGetRoleCardsParams,
	options?: { [key: string]: any }
) {
	const { roleId: param0, ...queryParams } = params;

	return request<string[]>(`/api/admin/FdWorkbench/role-cards/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getFdWorkbenchGetRoleCards */
export const getApiAdminFdWorkbenchRoleCardsRoleId = getFdWorkbenchGetRoleCards;
