// @ts-ignore
/* eslint-disable */
import request from '/@/utils/request';

/** 获取所有记录 检索并返回系统中该类型的所有记录。 GET /api/admin/menu-buttons */
export async function getAdminMenuButtons(options?: { [key: string]: any }) {
	return request<APIModel.MenuButtonDto[]>('/api/admin/menu-buttons', {
		method: 'GET',
		...(options || {}),
	});
}

/** 创建新记录 根据提供的数据创建一条新记录。 POST /api/admin/menu-buttons */
export async function postAdminMenuButtons(body: APIModel.CreateMenuButtonDto, options?: { [key: string]: any }) {
	return request<APIModel.MenuButtonDto>('/api/admin/menu-buttons', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 根据ID获取记录 根据提供的唯一标识符(ID)检索特定记录的详细信息。 GET /api/admin/menu-buttons/${param0} */
export async function getAdminMenuButtonsId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAdminMenuButtonsIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<APIModel.MenuButtonDto>(`/api/admin/menu-buttons/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 更新现有记录 根据提供的ID和更新数据，修改现有记录的信息。 PUT /api/admin/menu-buttons/${param0} */
export async function putAdminMenuButtonsId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putAdminMenuButtonsIdParams,
	body: APIModel.UpdateMenuButtonDto,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<APIModel.MenuButtonDto>(`/api/admin/menu-buttons/${param0}`, {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		params: { ...queryParams },
		data: body,
		...(options || {}),
	});
}

/** 删除记录 根据提供的ID，从系统中移除指定的记录。 DELETE /api/admin/menu-buttons/${param0} */
export async function deleteAdminMenuButtonsId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteAdminMenuButtonsIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<boolean>(`/api/admin/menu-buttons/${param0}`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 批量删除记录 根据提供的ID列表，批量删除多条记录。 DELETE /api/admin/menu-buttons/batch */
export async function deleteAdminMenuButtonsBatch(body: string[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/menu-buttons/batch', {
		method: 'DELETE',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 分页获取记录 根据页码和页面大小，分页检索记录。 GET /api/admin/menu-buttons/page */
export async function getAdminMenuButtonsPage(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAdminMenuButtonsPageParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.MenuButtonDtoPageResult>('/api/admin/menu-buttons/page', {
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

/** 根据条件分页获取记录 根据提供的查询条件和分页参数，分页检索记录。 POST /api/admin/menu-buttons/page/search */
export async function postAdminMenuButtonsPageSearch(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.MenuButtonDtoPageResult>('/api/admin/menu-buttons/page/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}

/** 获取回收站数据 检索并返回已软删除的记录（回收站数据）。 GET /api/admin/menu-buttons/recyclebin */
export async function getAdminMenuButtonsRecyclebin(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAdminMenuButtonsRecyclebinParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.MenuButtonDtoPageResult>('/api/admin/menu-buttons/recyclebin', {
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

/** 永久删除回收站中的记录 根据提供的ID，将已软删除的记录从数据库中永久移除。 DELETE /api/admin/menu-buttons/recyclebin/${param0}/permanent */
export async function deleteAdminMenuButtonsRecyclebinIdPermanent(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteAdminMenuButtonsRecyclebinIdPermanentParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<boolean>(`/api/admin/menu-buttons/recyclebin/${param0}/permanent`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 恢复回收站中的记录 根据提供的ID，将已软删除的记录恢复到正常状态。 PUT /api/admin/menu-buttons/recyclebin/${param0}/restore */
export async function putAdminMenuButtonsRecyclebinIdRestore(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putAdminMenuButtonsRecyclebinIdRestoreParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<boolean>(`/api/admin/menu-buttons/recyclebin/${param0}/restore`, {
		method: 'PUT',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 根据条件永久删除回收站中的记录 根据提供的条件，将回收站中符合条件的记录从数据库中永久移除。 POST /api/admin/menu-buttons/recyclebin/permanent */
export async function postAdminMenuButtonsRecyclebinPermanent(body: APIModel.FdMenuButtonBooleanFuncExpression, options?: { [key: string]: any }) {
	return request<number>('/api/admin/menu-buttons/recyclebin/permanent', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 批量恢复回收站中的记录 根据提供的条件，批量将回收站中的记录恢复到正常状态。 POST /api/admin/menu-buttons/recyclebin/restore */
export async function postAdminMenuButtonsRecyclebinRestore(body: APIModel.FdMenuButtonBooleanFuncExpression, options?: { [key: string]: any }) {
	return request<number>('/api/admin/menu-buttons/recyclebin/restore', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 根据条件查询回收站数据 根据提供的查询条件，检索回收站中的记录。 POST /api/admin/menu-buttons/recyclebin/search */
export async function postAdminMenuButtonsRecyclebinSearch(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.MenuButtonDtoPageResult>('/api/admin/menu-buttons/recyclebin/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}
