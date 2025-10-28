// @ts-ignore
/* eslint-disable */
import request from '/@/utils/request';

/** 获取所有记录 检索并返回系统中该类型的所有记录。 GET /api/admin/roles */
export async function getAdminRoles(options?: { [key: string]: any }) {
	return request<APIModel.RoleDto[]>('/api/admin/roles', {
		method: 'GET',
		...(options || {}),
	});
}

/** 创建新记录 根据提供的数据创建一条新记录。 POST /api/admin/roles */
export async function postAdminRoles(body: APIModel.CreateRoleDto, options?: { [key: string]: any }) {
	return request<APIModel.RoleDto>('/api/admin/roles', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 根据ID获取记录 根据提供的唯一标识符(ID)检索特定记录的详细信息。 GET /api/admin/roles/${param0} */
export async function getAdminRolesId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAdminRolesIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<APIModel.RoleDto>(`/api/admin/roles/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 更新现有记录 根据提供的ID和更新数据，修改现有记录的信息。 PUT /api/admin/roles/${param0} */
export async function putAdminRolesId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putAdminRolesIdParams,
	body: APIModel.UpdateRoleDto,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<APIModel.RoleDto>(`/api/admin/roles/${param0}`, {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		params: { ...queryParams },
		data: body,
		...(options || {}),
	});
}

/** 删除记录 根据提供的ID，从系统中移除指定的记录。 DELETE /api/admin/roles/${param0} */
export async function deleteAdminRolesId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteAdminRolesIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<boolean>(`/api/admin/roles/${param0}`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 此处后端没有提供注释 GET /api/admin/roles/${param0}/permissions */
export async function getAdminRolesIdPermissions(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAdminRolesIdPermissionsParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<string[]>(`/api/admin/roles/${param0}/permissions`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 此处后端没有提供注释 POST /api/admin/roles/${param0}/permissions */
export async function postAdminRolesIdPermissions(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.postAdminRolesIdPermissionsParams,
	body: APIModel.AssignPermissionsDto,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<any>(`/api/admin/roles/${param0}/permissions`, {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		params: { ...queryParams },
		data: body,
		...(options || {}),
	});
}

/** 根据实体主键批量更新实体信息 根据实体主键批量更新实体信息 PUT /api/admin/roles/batch */
export async function putAdminRolesBatch(body: APIModel.UpdateRoleDto[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/roles/batch', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 批量创建新记录 根据提供的数据批量创建新记录。 POST /api/admin/roles/batch */
export async function postAdminRolesBatch(body: APIModel.CreateRoleDto[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/roles/batch', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 批量删除记录 根据提供的ID列表，批量删除多条记录。 DELETE /api/admin/roles/batch */
export async function deleteAdminRolesBatch(body: string[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/roles/batch', {
		method: 'DELETE',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 根据条件批量更新实体属性（部分字段更新） 根据条件批量更新实体属性（部分字段更新） PUT /api/admin/roles/batch/updatebycondition */
export async function putAdminRolesBatchUpdatebycondition(body: APIModel.UpdateRoleDtoBatchUpdateByConditionDto, options?: { [key: string]: any }) {
	return request<number>('/api/admin/roles/batch/updatebycondition', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 分页获取记录 根据页码和页面大小，分页检索记录。 GET /api/admin/roles/page */
export async function getAdminRolesPage(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAdminRolesPageParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.RoleDtoPageResult>('/api/admin/roles/page', {
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

/** 根据条件分页获取记录 根据提供的查询条件和分页参数，分页检索记录。 POST /api/admin/roles/page/search */
export async function postAdminRolesPageSearch(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.RoleDtoPageResult>('/api/admin/roles/page/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}

/** 获取回收站数据 检索并返回已软删除的记录（回收站数据）。 GET /api/admin/roles/recyclebin */
export async function getAdminRolesRecyclebin(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAdminRolesRecyclebinParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.RoleDtoPageResult>('/api/admin/roles/recyclebin', {
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

/** 永久删除回收站中的记录 根据提供的ID，将已软删除的记录从数据库中永久移除。 DELETE /api/admin/roles/recyclebin/${param0}/permanent */
export async function deleteAdminRolesRecyclebinIdPermanent(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteAdminRolesRecyclebinIdPermanentParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<boolean>(`/api/admin/roles/recyclebin/${param0}/permanent`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 恢复回收站中的记录 根据提供的ID，将已软删除的记录恢复到正常状态。 PUT /api/admin/roles/recyclebin/${param0}/restore */
export async function putAdminRolesRecyclebinIdRestore(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putAdminRolesRecyclebinIdRestoreParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<boolean>(`/api/admin/roles/recyclebin/${param0}/restore`, {
		method: 'PUT',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 根据条件永久删除回收站中的记录 根据提供的条件，将回收站中符合条件的记录从数据库中永久移除。 POST /api/admin/roles/recyclebin/permanent */
export async function postAdminRolesRecyclebinPermanent(body: APIModel.FdRoleBooleanFuncExpression, options?: { [key: string]: any }) {
	return request<number>('/api/admin/roles/recyclebin/permanent', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 批量恢复回收站中的记录 根据提供的条件，批量将回收站中的记录恢复到正常状态。 POST /api/admin/roles/recyclebin/restore */
export async function postAdminRolesRecyclebinRestore(body: APIModel.FdRoleBooleanFuncExpression, options?: { [key: string]: any }) {
	return request<number>('/api/admin/roles/recyclebin/restore', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 根据条件查询回收站数据 根据提供的查询条件，检索回收站中的记录。 POST /api/admin/roles/recyclebin/search */
export async function postAdminRolesRecyclebinSearch(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.RoleDtoPageResult>('/api/admin/roles/recyclebin/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}
