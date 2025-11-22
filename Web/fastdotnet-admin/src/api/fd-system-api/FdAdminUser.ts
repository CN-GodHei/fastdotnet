// @ts-ignore
/* eslint-disable */
import request from '/@/utils/request';

/** 获取所有管理员用户 (自定义注释) GET /api/admin/FdAdminUser */
export async function getAdminFdAdminUser(options?: { [key: string]: any }) {
	return request<APIModel.FdAdminUserDto[]>('/api/admin/FdAdminUser', {
		method: 'GET',
		...(options || {}),
	});
}

/** 创建新记录 根据提供的数据创建一条新记录。 POST /api/admin/FdAdminUser */
export async function postAdminFdAdminUser(body: APIModel.CreateFdAdminUserDto, options?: { [key: string]: any }) {
	return request<APIModel.FdAdminUserDto>('/api/admin/FdAdminUser', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 根据ID获取记录 根据提供的唯一标识符(ID)检索特定记录的详细信息。 GET /api/admin/FdAdminUser/${param0} */
export async function getAdminFdAdminUserId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAdminFdAdminUserIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<APIModel.FdAdminUserDto>(`/api/admin/FdAdminUser/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 更新现有记录 根据提供的ID和更新数据，修改现有记录的信息。 PUT /api/admin/FdAdminUser/${param0} */
export async function putAdminFdAdminUserId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putAdminFdAdminUserIdParams,
	body: APIModel.UpdateFdAdminUserDto,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<APIModel.FdAdminUserDto>(`/api/admin/FdAdminUser/${param0}`, {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		params: { ...queryParams },
		data: body,
		...(options || {}),
	});
}

/** 删除记录 根据提供的ID，从系统中移除指定的记录。 DELETE /api/admin/FdAdminUser/${param0} */
export async function deleteAdminFdAdminUserId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteAdminFdAdminUserIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<boolean>(`/api/admin/FdAdminUser/${param0}`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 此处后端没有提供注释 POST /api/admin/FdAdminUser/${param0}/reset-password */
export async function postAdminFdAdminUserIdResetPassword(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.postAdminFdAdminUserIdResetPasswordParams,
	body: APIModel.ResetPasswordDto,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<boolean>(`/api/admin/FdAdminUser/${param0}/reset-password`, {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		params: { ...queryParams },
		data: body,
		...(options || {}),
	});
}

/** 根据实体主键批量更新实体信息 根据实体主键批量更新实体信息 PUT /api/admin/FdAdminUser/batch */
export async function putAdminFdAdminUserBatch(body: APIModel.UpdateFdAdminUserDto[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdAdminUser/batch', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 批量创建新记录 根据提供的数据批量创建新记录。 POST /api/admin/FdAdminUser/batch */
export async function postAdminFdAdminUserBatch(body: APIModel.CreateFdAdminUserDto[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdAdminUser/batch', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 批量删除记录 根据提供的ID列表，批量删除多条记录。 DELETE /api/admin/FdAdminUser/batch */
export async function deleteAdminFdAdminUserBatch(body: string[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdAdminUser/batch', {
		method: 'DELETE',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 根据条件批量更新实体属性（部分字段更新） 根据条件批量更新实体属性（部分字段更新） PUT /api/admin/FdAdminUser/batch/updatebycondition */
export async function putAdminFdAdminUserBatchUpdatebycondition(
	body: APIModel.UpdateFdAdminUserDtoBatchUpdateByConditionDto,
	options?: { [key: string]: any }
) {
	return request<number>('/api/admin/FdAdminUser/batch/updatebycondition', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 此处后端没有提供注释 GET /api/admin/FdAdminUser/getUserInfo */
export async function getAdminFdAdminUserGetUserInfo(options?: { [key: string]: any }) {
	return request<APIModel.FdAdminUserDto>('/api/admin/FdAdminUser/getUserInfo', {
		method: 'GET',
		...(options || {}),
	});
}

/** 分页获取记录 根据页码和页面大小，分页检索记录。 GET /api/admin/FdAdminUser/page */
export async function getAdminFdAdminUserPage(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAdminFdAdminUserPageParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.PageInfo>('/api/admin/FdAdminUser/page', {
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

/** 根据条件分页获取记录 根据提供的查询条件和分页参数，分页检索记录。 POST /api/admin/FdAdminUser/page/search */
export async function postAdminFdAdminUserPageSearch(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/admin/FdAdminUser/page/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}

/** 获取回收站数据 检索并返回已软删除的记录（回收站数据）。 GET /api/admin/FdAdminUser/recyclebin */
export async function getAdminFdAdminUserRecyclebin(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAdminFdAdminUserRecyclebinParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.PageInfo>('/api/admin/FdAdminUser/recyclebin', {
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

/** 永久删除回收站中的记录 根据提供的ID，将已软删除的记录从数据库中永久移除。 DELETE /api/admin/FdAdminUser/recyclebin/${param0}/permanent */
export async function deleteAdminFdAdminUserRecyclebinIdPermanent(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteAdminFdAdminUserRecyclebinIdPermanentParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<boolean>(`/api/admin/FdAdminUser/recyclebin/${param0}/permanent`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 恢复回收站中的记录 根据提供的ID，将已软删除的记录恢复到正常状态。 PUT /api/admin/FdAdminUser/recyclebin/${param0}/restore */
export async function putAdminFdAdminUserRecyclebinIdRestore(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putAdminFdAdminUserRecyclebinIdRestoreParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<boolean>(`/api/admin/FdAdminUser/recyclebin/${param0}/restore`, {
		method: 'PUT',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 根据条件永久删除回收站中的记录 根据提供的条件，将回收站中符合条件的记录从数据库中永久移除。 POST /api/admin/FdAdminUser/recyclebin/permanent */
export async function postAdminFdAdminUserRecyclebinPermanent(body: APIModel.FdAdminUserBooleanFuncExpression, options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdAdminUser/recyclebin/permanent', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 批量恢复回收站中的记录 根据提供的条件，批量将回收站中的记录恢复到正常状态。 POST /api/admin/FdAdminUser/recyclebin/restore */
export async function postAdminFdAdminUserRecyclebinRestore(body: APIModel.FdAdminUserBooleanFuncExpression, options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdAdminUser/recyclebin/restore', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 根据条件查询回收站数据 根据提供的查询条件，检索回收站中的记录。 POST /api/admin/FdAdminUser/recyclebin/search */
export async function postAdminFdAdminUserRecyclebinSearch(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/admin/FdAdminUser/recyclebin/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}

/** 解锁屏幕 POST /api/admin/FdAdminUser/unlock */
export async function postAdminFdAdminUserUnlock(body: APIModel.UnlockDto, options?: { [key: string]: any }) {
	return request<boolean>('/api/admin/FdAdminUser/unlock', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
