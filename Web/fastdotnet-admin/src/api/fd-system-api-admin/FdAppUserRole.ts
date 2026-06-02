// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 获取所有记录 检索并返回系统中该类型的所有记录。 GET /api/FdAppUserRole */
export async function getGenericDtoControllerBase5GetAll(options?: { [key: string]: any }) {
	return request<APIModel.FdAppUserRoleDto[]>('/api/FdAppUserRole', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getGenericDtoControllerBase5GetAll */
export const getApiFdAppUserRole = getGenericDtoControllerBase5GetAll;
/** 创建新记录 根据提供的数据创建一条新记录。 POST /api/FdAppUserRole */
export async function postGenericDtoControllerBase5Create(body: APIModel.CreateFdAppUserRoleDto, options?: { [key: string]: any }) {
	return request<APIModel.FdAppUserRoleDto>('/api/FdAppUserRole', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5Create */
export const postApiFdAppUserRole = postGenericDtoControllerBase5Create;
/** 根据ID获取记录 根据提供的唯一标识符(ID)检索特定记录的详细信息。 GET /api/FdAppUserRole/${param0} */
export async function getGenericDtoControllerBase5GetById(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getGenericDtoControllerBase5GetByIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<APIModel.FdAppUserRoleDto>(`/api/FdAppUserRole/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getGenericDtoControllerBase5GetById */
export const getApiFdAppUserRoleId = getGenericDtoControllerBase5GetById;
/** 更新现有记录 根据提供的ID和更新数据，修改现有记录的信息。 PUT /api/FdAppUserRole/${param0} */
export async function putGenericDtoControllerBase5Update(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putGenericDtoControllerBase5UpdateParams,
	body: APIModel.UpdateFdAppUserRoleDto,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<APIModel.FdAppUserRoleDto>(`/api/FdAppUserRole/${param0}`, {
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
export const putApiFdAppUserRoleId = putGenericDtoControllerBase5Update;
/** 删除记录 根据提供的ID，从系统中移除指定的记录。 DELETE /api/FdAppUserRole/${param0} */
export async function deleteGenericDtoControllerBase5Delete(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteGenericDtoControllerBase5DeleteParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/FdAppUserRole/${param0}`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteGenericDtoControllerBase5Delete */
export const deleteApiFdAppUserRoleId = deleteGenericDtoControllerBase5Delete;
/** 为用户分配角色（事务安全的一次性操作） POST /api/FdAppUserRole/assign-roles */
export async function postFdAppUserRoleAssignUserRoles(body: APIModel.AssignUserRolesDto, options?: { [key: string]: any }) {
	return request<boolean>('/api/FdAppUserRole/assign-roles', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postFdAppUserRoleAssignUserRoles */
export const postApiFdAppUserRoleAssignRoles = postFdAppUserRoleAssignUserRoles;
/** 根据实体主键批量更新实体信息 根据实体主键批量更新实体信息 PUT /api/FdAppUserRole/batch */
export async function putGenericDtoControllerBase5UpdateMany(body: APIModel.UpdateFdAppUserRoleDto[], options?: { [key: string]: any }) {
	return request<number>('/api/FdAppUserRole/batch', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putGenericDtoControllerBase5UpdateMany */
export const putApiFdAppUserRoleBatch = putGenericDtoControllerBase5UpdateMany;
/** 批量创建新记录 根据提供的数据批量创建新记录。 POST /api/FdAppUserRole/batch */
export async function postGenericDtoControllerBase5CreateMany(body: APIModel.CreateFdAppUserRoleDto[], options?: { [key: string]: any }) {
	return request<number>('/api/FdAppUserRole/batch', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5CreateMany */
export const postApiFdAppUserRoleBatch = postGenericDtoControllerBase5CreateMany;
/** 批量删除记录 根据提供的ID列表，批量删除多条记录。 DELETE /api/FdAppUserRole/batch */
export async function deleteGenericDtoControllerBase5BatchDelete(body: string[], options?: { [key: string]: any }) {
	return request<number>('/api/FdAppUserRole/batch', {
		method: 'DELETE',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteGenericDtoControllerBase5BatchDelete */
export const deleteApiFdAppUserRoleBatch = deleteGenericDtoControllerBase5BatchDelete;
/** 根据条件批量更新实体属性（部分字段更新） 根据条件批量更新实体属性（部分字段更新） PUT /api/FdAppUserRole/batch/updatebycondition */
export async function putGenericDtoControllerBase5UpdateManyByCondition(
	body: APIModel.BatchUpdateByConditionDto1UpdateFdAppUserRoleDto,
	options?: { [key: string]: any }
) {
	return request<number>('/api/FdAppUserRole/batch/updatebycondition', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putGenericDtoControllerBase5UpdateManyByCondition */
export const putApiFdAppUserRoleBatchUpdatebycondition = putGenericDtoControllerBase5UpdateManyByCondition;
/** 根据自定义条件获取列表(不分页) 根据自定义条件获取列表(不分页) POST /api/FdAppUserRole/list-by-condition */
export async function postGenericDtoControllerBase5GetListByCondition(body: APIModel.QueryByConditionDto, options?: { [key: string]: any }) {
	return request<any>('/api/FdAppUserRole/list-by-condition', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5GetListByCondition */
export const postApiFdAppUserRoleListByCondition = postGenericDtoControllerBase5GetListByCondition;
/** 分页获取记录 根据页码和页面大小，分页检索记录。 GET /api/FdAppUserRole/page */
export async function getGenericDtoControllerBase5GetPage(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getGenericDtoControllerBase5GetPageParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.PageInfo>('/api/FdAppUserRole/page', {
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
export const getApiFdAppUserRolePage = getGenericDtoControllerBase5GetPage;
/** 根据条件分页获取记录 根据提供的查询条件和分页参数，分页检索记录。 POST /api/FdAppUserRole/page/search */
export async function postGenericDtoControllerBase5GetPageByCondition(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/FdAppUserRole/page/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5GetPageByCondition */
export const postApiFdAppUserRolePageSearch = postGenericDtoControllerBase5GetPageByCondition;
/** 获取回收站数据 检索并返回已软删除的记录（回收站数据）。 GET /api/FdAppUserRole/recyclebin */
export async function getGenericDtoControllerBase5GetRecycleBin(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getGenericDtoControllerBase5GetRecycleBinParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.PageInfo>('/api/FdAppUserRole/recyclebin', {
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
export const getApiFdAppUserRoleRecyclebin = getGenericDtoControllerBase5GetRecycleBin;
/** 永久删除回收站中的记录 根据提供的ID，将已软删除的记录从数据库中永久移除。 DELETE /api/FdAppUserRole/recyclebin/${param0}/permanent */
export async function deleteGenericDtoControllerBase5PermanentDelete(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteGenericDtoControllerBase5PermanentDeleteParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/FdAppUserRole/recyclebin/${param0}/permanent`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteGenericDtoControllerBase5PermanentDelete */
export const deleteApiFdAppUserRoleRecyclebinIdPermanent = deleteGenericDtoControllerBase5PermanentDelete;
/** 恢复回收站中的记录 根据提供的ID，将已软删除的记录恢复到正常状态。 PUT /api/FdAppUserRole/recyclebin/${param0}/restore */
export async function putGenericDtoControllerBase5Restore(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putGenericDtoControllerBase5RestoreParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/FdAppUserRole/recyclebin/${param0}/restore`, {
		method: 'PUT',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putGenericDtoControllerBase5Restore */
export const putApiFdAppUserRoleRecyclebinIdRestore = putGenericDtoControllerBase5Restore;
/** 根据条件永久删除回收站中的记录 根据提供的条件，将回收站中符合条件的记录从数据库中永久移除。 POST /api/FdAppUserRole/recyclebin/permanent */
export async function postGenericDtoControllerBase5PermanentDeleteBatch(
	body: APIModel.Expression1Func2FdAppUserRole_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral,
	options?: { [key: string]: any }
) {
	return request<number>('/api/FdAppUserRole/recyclebin/permanent', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5PermanentDeleteBatch */
export const postApiFdAppUserRoleRecyclebinPermanent = postGenericDtoControllerBase5PermanentDeleteBatch;
/** 批量恢复回收站中的记录 根据提供的条件，批量将回收站中的记录恢复到正常状态。 POST /api/FdAppUserRole/recyclebin/restore */
export async function postGenericDtoControllerBase5RestoreBatch(
	body: APIModel.Expression1Func2FdAppUserRole_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral,
	options?: { [key: string]: any }
) {
	return request<number>('/api/FdAppUserRole/recyclebin/restore', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5RestoreBatch */
export const postApiFdAppUserRoleRecyclebinRestore = postGenericDtoControllerBase5RestoreBatch;
/** 根据条件查询回收站数据 根据提供的查询条件，检索回收站中的记录。 POST /api/FdAppUserRole/recyclebin/search */
export async function postGenericDtoControllerBase5SearchRecycleBin(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/FdAppUserRole/recyclebin/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5SearchRecycleBin */
export const postApiFdAppUserRoleRecyclebinSearch = postGenericDtoControllerBase5SearchRecycleBin;
/** 获取指定用户当前角色 GET /api/FdAppUserRole/user/${param0}/roles */
export async function getFdAppUserRoleGetUserRoles(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getFdAppUserRoleGetUserRolesParams,
	options?: { [key: string]: any }
) {
	const { userId: param0, ...queryParams } = params;

	return request<string[]>(`/api/FdAppUserRole/user/${param0}/roles`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getFdAppUserRoleGetUserRoles */
export const getApiFdAppUserRoleUserUserIdRoles = getFdAppUserRoleGetUserRoles;
