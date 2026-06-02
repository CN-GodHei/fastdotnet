// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 获取所有记录 检索并返回系统中该类型的所有记录。 GET /api/admin/FdRole */
export async function getFdRoleGetAll(options?: { [key: string]: any }) {
	return request<APIModel.FdRoleDto[]>('/api/admin/FdRole', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getFdRoleGetAll */
export const getApiAdminFdRole = getFdRoleGetAll;
/** 创建新记录 根据提供的数据创建一条新记录。 POST /api/admin/FdRole */
export async function postFdRoleCreate(body: APIModel.CreateFdRoleDto, options?: { [key: string]: any }) {
	return request<APIModel.FdRoleDto>('/api/admin/FdRole', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postFdRoleCreate */
export const postApiAdminFdRole = postFdRoleCreate;
/** 根据ID获取记录 根据提供的唯一标识符(ID)检索特定记录的详细信息。 GET /api/admin/FdRole/${param0} */
export async function getFdRoleGetById(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getFdRoleGetByIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<APIModel.FdRoleDto>(`/api/admin/FdRole/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getFdRoleGetById */
export const getApiAdminFdRoleId = getFdRoleGetById;
/** 更新现有记录 根据提供的ID和更新数据，修改现有记录的信息。 PUT /api/admin/FdRole/${param0} */
export async function putFdRoleUpdate(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putFdRoleUpdateParams,
	body: APIModel.UpdateFdRoleDto,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<APIModel.FdRoleDto>(`/api/admin/FdRole/${param0}`, {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		params: { ...queryParams },
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putFdRoleUpdate */
export const putApiAdminFdRoleId = putFdRoleUpdate;
/** 删除记录 根据提供的ID，从系统中移除指定的记录。 DELETE /api/admin/FdRole/${param0} */
export async function deleteFdRoleDelete(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteFdRoleDeleteParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/admin/FdRole/${param0}`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteFdRoleDelete */
export const deleteApiAdminFdRoleId = deleteFdRoleDelete;
/** 此处后端没有提供注释 POST /api/admin/FdRole/${param0}/menu-btns */
export async function postFdRoleSave(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.postFdRoleSaveParams,
	body: APIModel.MenuBtnRe[],
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/admin/FdRole/${param0}/menu-btns`, {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		params: { ...queryParams },
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postFdRoleSave */
export const postApiAdminFdRoleIdMenuBtns = postFdRoleSave;
/** 此处后端没有提供注释 GET /api/admin/FdRole/${param0}/permissions */
export async function getFdRoleGetPermissionIds(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getFdRoleGetPermissionIdsParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<string[]>(`/api/admin/FdRole/${param0}/permissions`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getFdRoleGetPermissionIds */
export const getApiAdminFdRoleIdPermissions = getFdRoleGetPermissionIds;
/** 此处后端没有提供注释 POST /api/admin/FdRole/${param0}/permissions */
export async function postFdRoleAssignPermissions(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.postFdRoleAssignPermissionsParams,
	body: APIModel.AssignPermissionsDto,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/admin/FdRole/${param0}/permissions`, {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		params: { ...queryParams },
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postFdRoleAssignPermissions */
export const postApiAdminFdRoleIdPermissions = postFdRoleAssignPermissions;
/** 根据实体主键批量更新实体信息 根据实体主键批量更新实体信息 PUT /api/admin/FdRole/batch */
export async function putGenericDtoControllerBase5UpdateMany(body: APIModel.UpdateFdRoleDto[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdRole/batch', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putGenericDtoControllerBase5UpdateMany */
export const putApiAdminFdRoleBatch = putGenericDtoControllerBase5UpdateMany;
/** 批量创建新记录 根据提供的数据批量创建新记录。 POST /api/admin/FdRole/batch */
export async function postGenericDtoControllerBase5CreateMany(body: APIModel.CreateFdRoleDto[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdRole/batch', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5CreateMany */
export const postApiAdminFdRoleBatch = postGenericDtoControllerBase5CreateMany;
/** 批量删除记录 根据提供的ID列表，批量删除多条记录。 DELETE /api/admin/FdRole/batch */
export async function deleteGenericDtoControllerBase5BatchDelete(body: string[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdRole/batch', {
		method: 'DELETE',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteGenericDtoControllerBase5BatchDelete */
export const deleteApiAdminFdRoleBatch = deleteGenericDtoControllerBase5BatchDelete;
/** 根据条件批量更新实体属性（部分字段更新） 根据条件批量更新实体属性（部分字段更新） PUT /api/admin/FdRole/batch/updatebycondition */
export async function putGenericDtoControllerBase5UpdateManyByCondition(
	body: APIModel.BatchUpdateByConditionDto1UpdateFdRoleDto,
	options?: { [key: string]: any }
) {
	return request<number>('/api/admin/FdRole/batch/updatebycondition', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putGenericDtoControllerBase5UpdateManyByCondition */
export const putApiAdminFdRoleBatchUpdatebycondition = putGenericDtoControllerBase5UpdateManyByCondition;
/** 根据自定义条件获取列表(不分页) 根据自定义条件获取列表(不分页) POST /api/admin/FdRole/list-by-condition */
export async function postGenericDtoControllerBase5GetListByCondition(body: APIModel.QueryByConditionDto, options?: { [key: string]: any }) {
	return request<any>('/api/admin/FdRole/list-by-condition', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5GetListByCondition */
export const postApiAdminFdRoleListByCondition = postGenericDtoControllerBase5GetListByCondition;
/** 分页获取记录 根据页码和页面大小，分页检索记录。 GET /api/admin/FdRole/page */
export async function getFdRoleGetPage(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getFdRoleGetPageParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.PageInfo>('/api/admin/FdRole/page', {
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

/** @deprecated 此函数名已变更，请使用 getFdRoleGetPage */
export const getApiAdminFdRolePage = getFdRoleGetPage;
/** 根据条件分页获取记录 根据提供的查询条件和分页参数，分页检索记录。 POST /api/admin/FdRole/page/search */
export async function postGenericDtoControllerBase5GetPageByCondition(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/admin/FdRole/page/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5GetPageByCondition */
export const postApiAdminFdRolePageSearch = postGenericDtoControllerBase5GetPageByCondition;
/** 获取回收站数据 检索并返回已软删除的记录（回收站数据）。 GET /api/admin/FdRole/recyclebin */
export async function getGenericDtoControllerBase5GetRecycleBin(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getGenericDtoControllerBase5GetRecycleBinParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.PageInfo>('/api/admin/FdRole/recyclebin', {
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
export const getApiAdminFdRoleRecyclebin = getGenericDtoControllerBase5GetRecycleBin;
/** 永久删除回收站中的记录 根据提供的ID，将已软删除的记录从数据库中永久移除。 DELETE /api/admin/FdRole/recyclebin/${param0}/permanent */
export async function deleteGenericDtoControllerBase5PermanentDelete(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteGenericDtoControllerBase5PermanentDeleteParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/admin/FdRole/recyclebin/${param0}/permanent`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteGenericDtoControllerBase5PermanentDelete */
export const deleteApiAdminFdRoleRecyclebinIdPermanent = deleteGenericDtoControllerBase5PermanentDelete;
/** 恢复回收站中的记录 根据提供的ID，将已软删除的记录恢复到正常状态。 PUT /api/admin/FdRole/recyclebin/${param0}/restore */
export async function putGenericDtoControllerBase5Restore(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putGenericDtoControllerBase5RestoreParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/admin/FdRole/recyclebin/${param0}/restore`, {
		method: 'PUT',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putGenericDtoControllerBase5Restore */
export const putApiAdminFdRoleRecyclebinIdRestore = putGenericDtoControllerBase5Restore;
/** 根据条件永久删除回收站中的记录 根据提供的条件，将回收站中符合条件的记录从数据库中永久移除。 POST /api/admin/FdRole/recyclebin/permanent */
export async function postGenericDtoControllerBase5PermanentDeleteBatch(
	body: APIModel.Expression1Func2FdRole_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral,
	options?: { [key: string]: any }
) {
	return request<number>('/api/admin/FdRole/recyclebin/permanent', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5PermanentDeleteBatch */
export const postApiAdminFdRoleRecyclebinPermanent = postGenericDtoControllerBase5PermanentDeleteBatch;
/** 批量恢复回收站中的记录 根据提供的条件，批量将回收站中的记录恢复到正常状态。 POST /api/admin/FdRole/recyclebin/restore */
export async function postGenericDtoControllerBase5RestoreBatch(
	body: APIModel.Expression1Func2FdRole_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral,
	options?: { [key: string]: any }
) {
	return request<number>('/api/admin/FdRole/recyclebin/restore', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5RestoreBatch */
export const postApiAdminFdRoleRecyclebinRestore = postGenericDtoControllerBase5RestoreBatch;
/** 根据条件查询回收站数据 根据提供的查询条件，检索回收站中的记录。 POST /api/admin/FdRole/recyclebin/search */
export async function postGenericDtoControllerBase5SearchRecycleBin(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/admin/FdRole/recyclebin/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5SearchRecycleBin */
export const postApiAdminFdRoleRecyclebinSearch = postGenericDtoControllerBase5SearchRecycleBin;
