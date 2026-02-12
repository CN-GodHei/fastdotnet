// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 获取所有记录 检索并返回系统中该类型的所有记录。 GET /api/admin/FdMenu */
export async function getApiAdminFdMenu(options?: { [key: string]: any }) {
	return request<APIModel.FdMenuDto[]>('/api/admin/FdMenu', {
		method: 'GET',
		...(options || {}),
	});
}
/** 创建新记录 根据提供的数据创建一条新记录。 POST /api/admin/FdMenu */
export async function postApiAdminFdMenu(body: APIModel.CreateFdMenuDto, options?: { [key: string]: any }) {
	return request<APIModel.FdMenuDto>('/api/admin/FdMenu', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 根据ID获取记录 根据提供的唯一标识符(ID)检索特定记录的详细信息。 GET /api/admin/FdMenu/${param0} */
export async function getApiAdminFdMenuId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getApiAdminFdMenuIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<APIModel.FdMenuDto>(`/api/admin/FdMenu/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}
/** 更新现有记录 根据提供的ID和更新数据，修改现有记录的信息。 PUT /api/admin/FdMenu/${param0} */
export async function putApiAdminFdMenuId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putApiAdminFdMenuIdParams,
	body: APIModel.UpdateFdMenuDto,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<APIModel.FdMenuDto>(`/api/admin/FdMenu/${param0}`, {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		params: { ...queryParams },
		data: body,
		...(options || {}),
	});
}
/** 删除记录 根据提供的ID，从系统中移除指定的记录。 DELETE /api/admin/FdMenu/${param0} */
export async function deleteApiAdminFdMenuId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteApiAdminFdMenuIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/admin/FdMenu/${param0}`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}
/** 根据实体主键批量更新实体信息 根据实体主键批量更新实体信息 PUT /api/admin/FdMenu/batch */
export async function putApiAdminFdMenuBatch(body: APIModel.UpdateFdMenuDto[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdMenu/batch', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 批量创建新记录 根据提供的数据批量创建新记录。 POST /api/admin/FdMenu/batch */
export async function postApiAdminFdMenuBatch(body: APIModel.CreateFdMenuDto[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdMenu/batch', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 批量删除记录 根据提供的ID列表，批量删除多条记录。 DELETE /api/admin/FdMenu/batch */
export async function deleteApiAdminFdMenuBatch(body: string[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdMenu/batch', {
		method: 'DELETE',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 根据条件批量更新实体属性（部分字段更新） 根据条件批量更新实体属性（部分字段更新） PUT /api/admin/FdMenu/batch/updatebycondition */
export async function putApiAdminFdMenuBatchUpdatebycondition(
	body: APIModel.UpdateFdMenuDtoBatchUpdateByConditionDto,
	options?: { [key: string]: any }
) {
	return request<number>('/api/admin/FdMenu/batch/updatebycondition', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 根据自定义条件获取列表(不分页) 根据自定义条件获取列表(不分页) POST /api/admin/FdMenu/list-by-condition */
export async function postApiAdminFdMenuListByCondition(body: APIModel.QueryByConditionDto, options?: { [key: string]: any }) {
	return request<any>('/api/admin/FdMenu/list-by-condition', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 此处后端没有提供注释 GET /api/admin/FdMenu/menu-btns */
export async function getApiAdminFdMenuMenuBtns(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getApiAdminFdMenuMenuBtnsParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.MenuBtnRe[]>('/api/admin/FdMenu/menu-btns', {
		method: 'GET',
		params: {
			...params,
		},
		...(options || {}),
	});
}
/** 分页获取记录 根据页码和页面大小，分页检索记录。 GET /api/admin/FdMenu/page */
export async function getApiAdminFdMenuPage(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getApiAdminFdMenuPageParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.PageInfo>('/api/admin/FdMenu/page', {
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
/** 根据条件分页获取记录 根据提供的查询条件和分页参数，分页检索记录。 POST /api/admin/FdMenu/page/search */
export async function postApiAdminFdMenuPageSearch(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/admin/FdMenu/page/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}
/** 获取回收站数据 检索并返回已软删除的记录（回收站数据）。 GET /api/admin/FdMenu/recyclebin */
export async function getApiAdminFdMenuRecyclebin(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getApiAdminFdMenuRecyclebinParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.PageInfo>('/api/admin/FdMenu/recyclebin', {
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
/** 永久删除回收站中的记录 根据提供的ID，将已软删除的记录从数据库中永久移除。 DELETE /api/admin/FdMenu/recyclebin/${param0}/permanent */
export async function deleteApiAdminFdMenuRecyclebinIdPermanent(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteApiAdminFdMenuRecyclebinIdPermanentParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/admin/FdMenu/recyclebin/${param0}/permanent`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}
/** 恢复回收站中的记录 根据提供的ID，将已软删除的记录恢复到正常状态。 PUT /api/admin/FdMenu/recyclebin/${param0}/restore */
export async function putApiAdminFdMenuRecyclebinIdRestore(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putApiAdminFdMenuRecyclebinIdRestoreParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/admin/FdMenu/recyclebin/${param0}/restore`, {
		method: 'PUT',
		params: { ...queryParams },
		...(options || {}),
	});
}
/** 根据条件永久删除回收站中的记录 根据提供的条件，将回收站中符合条件的记录从数据库中永久移除。 POST /api/admin/FdMenu/recyclebin/permanent */
export async function postApiAdminFdMenuRecyclebinPermanent(body: APIModel.FdMenuBooleanFuncExpression, options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdMenu/recyclebin/permanent', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 批量恢复回收站中的记录 根据提供的条件，批量将回收站中的记录恢复到正常状态。 POST /api/admin/FdMenu/recyclebin/restore */
export async function postApiAdminFdMenuRecyclebinRestore(body: APIModel.FdMenuBooleanFuncExpression, options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdMenu/recyclebin/restore', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 根据条件查询回收站数据 根据提供的查询条件，检索回收站中的记录。 POST /api/admin/FdMenu/recyclebin/search */
export async function postApiAdminFdMenuRecyclebinSearch(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/admin/FdMenu/recyclebin/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}
/** 此处后端没有提供注释 GET /api/admin/FdMenu/tree */
export async function getApiAdminFdMenuTree(options?: { [key: string]: any }) {
	return request<APIModel.FdMenuDto[]>('/api/admin/FdMenu/tree', {
		method: 'GET',
		...(options || {}),
	});
}
