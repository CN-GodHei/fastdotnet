// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 获取所有记录 检索并返回系统中该类型的所有记录。 GET /api/admin/FdMenu */
export async function getFdMenuGetAll(options?: { [key: string]: any }) {
	return request<APIModel.FdMenuDto[]>('/api/admin/FdMenu', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getFdMenuGetAll */
export const getApiAdminFdMenu = getFdMenuGetAll;
/** 创建新记录 根据提供的数据创建一条新记录。 POST /api/admin/FdMenu */
export async function postGenericDtoControllerBase5Create(body: APIModel.CreateFdMenuDto, options?: { [key: string]: any }) {
	return request<APIModel.FdMenuDto>('/api/admin/FdMenu', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5Create */
export const postApiAdminFdMenu = postGenericDtoControllerBase5Create;
/** 根据ID获取记录 根据提供的唯一标识符(ID)检索特定记录的详细信息。 GET /api/admin/FdMenu/${param0} */
export async function getFdMenuGetById(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getFdMenuGetByIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<APIModel.FdMenuDto>(`/api/admin/FdMenu/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getFdMenuGetById */
export const getApiAdminFdMenuId = getFdMenuGetById;
/** 更新现有记录 根据提供的ID和更新数据，修改现有记录的信息。 PUT /api/admin/FdMenu/${param0} */
export async function putFdMenuUpdate(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putFdMenuUpdateParams,
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

/** @deprecated 此函数名已变更，请使用 putFdMenuUpdate */
export const putApiAdminFdMenuId = putFdMenuUpdate;
/** 删除记录 根据提供的ID，从系统中移除指定的记录。 DELETE /api/admin/FdMenu/${param0} */
export async function deleteFdMenuDelete(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteFdMenuDeleteParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/admin/FdMenu/${param0}`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteFdMenuDelete */
export const deleteApiAdminFdMenuId = deleteFdMenuDelete;
/** 根据实体主键批量更新实体信息 根据实体主键批量更新实体信息 PUT /api/admin/FdMenu/batch */
export async function putGenericDtoControllerBase5UpdateMany(body: APIModel.UpdateFdMenuDto[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdMenu/batch', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putGenericDtoControllerBase5UpdateMany */
export const putApiAdminFdMenuBatch = putGenericDtoControllerBase5UpdateMany;
/** 批量创建新记录 根据提供的数据批量创建新记录。 POST /api/admin/FdMenu/batch */
export async function postGenericDtoControllerBase5CreateMany(body: APIModel.CreateFdMenuDto[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdMenu/batch', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5CreateMany */
export const postApiAdminFdMenuBatch = postGenericDtoControllerBase5CreateMany;
/** 批量删除记录 根据提供的ID列表，批量删除多条记录。 DELETE /api/admin/FdMenu/batch */
export async function deleteGenericDtoControllerBase5BatchDelete(body: string[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdMenu/batch', {
		method: 'DELETE',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteGenericDtoControllerBase5BatchDelete */
export const deleteApiAdminFdMenuBatch = deleteGenericDtoControllerBase5BatchDelete;
/** 根据条件批量更新实体属性（部分字段更新） 根据条件批量更新实体属性（部分字段更新） PUT /api/admin/FdMenu/batch/updatebycondition */
export async function putGenericDtoControllerBase5UpdateManyByCondition(
	body: APIModel.BatchUpdateByConditionDto1UpdateFdMenuDto,
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

/** @deprecated 此函数名已变更，请使用 putGenericDtoControllerBase5UpdateManyByCondition */
export const putApiAdminFdMenuBatchUpdatebycondition = putGenericDtoControllerBase5UpdateManyByCondition;
/** 根据自定义条件获取列表(不分页) 根据自定义条件获取列表(不分页) POST /api/admin/FdMenu/list-by-condition */
export async function postGenericDtoControllerBase5GetListByCondition(body: APIModel.QueryByConditionDto, options?: { [key: string]: any }) {
	return request<any>('/api/admin/FdMenu/list-by-condition', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5GetListByCondition */
export const postApiAdminFdMenuListByCondition = postGenericDtoControllerBase5GetListByCondition;
/** 此处后端没有提供注释 GET /api/admin/FdMenu/menu-btns */
export async function getFdMenuGetMenuBtn(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getFdMenuGetMenuBtnParams,
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

/** @deprecated 此函数名已变更，请使用 getFdMenuGetMenuBtn */
export const getApiAdminFdMenuMenuBtns = getFdMenuGetMenuBtn;
/** 分页获取记录 根据页码和页面大小，分页检索记录。 GET /api/admin/FdMenu/page */
export async function getFdMenuGetPage(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getFdMenuGetPageParams,
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

/** @deprecated 此函数名已变更，请使用 getFdMenuGetPage */
export const getApiAdminFdMenuPage = getFdMenuGetPage;
/** 根据条件分页获取记录 根据提供的查询条件和分页参数，分页检索记录。 POST /api/admin/FdMenu/page/search */
export async function postGenericDtoControllerBase5GetPageByCondition(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/admin/FdMenu/page/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5GetPageByCondition */
export const postApiAdminFdMenuPageSearch = postGenericDtoControllerBase5GetPageByCondition;
/** 获取回收站数据 检索并返回已软删除的记录（回收站数据）。 GET /api/admin/FdMenu/recyclebin */
export async function getGenericDtoControllerBase5GetRecycleBin(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getGenericDtoControllerBase5GetRecycleBinParams,
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

/** @deprecated 此函数名已变更，请使用 getGenericDtoControllerBase5GetRecycleBin */
export const getApiAdminFdMenuRecyclebin = getGenericDtoControllerBase5GetRecycleBin;
/** 永久删除回收站中的记录 根据提供的ID，将已软删除的记录从数据库中永久移除。 DELETE /api/admin/FdMenu/recyclebin/${param0}/permanent */
export async function deleteGenericDtoControllerBase5PermanentDelete(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteGenericDtoControllerBase5PermanentDeleteParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/admin/FdMenu/recyclebin/${param0}/permanent`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 deleteGenericDtoControllerBase5PermanentDelete */
export const deleteApiAdminFdMenuRecyclebinIdPermanent = deleteGenericDtoControllerBase5PermanentDelete;
/** 恢复回收站中的记录 根据提供的ID，将已软删除的记录恢复到正常状态。 PUT /api/admin/FdMenu/recyclebin/${param0}/restore */
export async function putGenericDtoControllerBase5Restore(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putGenericDtoControllerBase5RestoreParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/admin/FdMenu/recyclebin/${param0}/restore`, {
		method: 'PUT',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putGenericDtoControllerBase5Restore */
export const putApiAdminFdMenuRecyclebinIdRestore = putGenericDtoControllerBase5Restore;
/** 根据条件永久删除回收站中的记录 根据提供的条件，将回收站中符合条件的记录从数据库中永久移除。 POST /api/admin/FdMenu/recyclebin/permanent */
export async function postGenericDtoControllerBase5PermanentDeleteBatch(
	body: APIModel.Expression1Func2FdMenu_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral,
	options?: { [key: string]: any }
) {
	return request<number>('/api/admin/FdMenu/recyclebin/permanent', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5PermanentDeleteBatch */
export const postApiAdminFdMenuRecyclebinPermanent = postGenericDtoControllerBase5PermanentDeleteBatch;
/** 批量恢复回收站中的记录 根据提供的条件，批量将回收站中的记录恢复到正常状态。 POST /api/admin/FdMenu/recyclebin/restore */
export async function postGenericDtoControllerBase5RestoreBatch(
	body: APIModel.Expression1Func2FdMenu_SystemBooleanSystemPrivateCoreLibVersion10000Cultureneutral,
	options?: { [key: string]: any }
) {
	return request<number>('/api/admin/FdMenu/recyclebin/restore', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5RestoreBatch */
export const postApiAdminFdMenuRecyclebinRestore = postGenericDtoControllerBase5RestoreBatch;
/** 根据条件查询回收站数据 根据提供的查询条件，检索回收站中的记录。 POST /api/admin/FdMenu/recyclebin/search */
export async function postGenericDtoControllerBase5SearchRecycleBin(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/admin/FdMenu/recyclebin/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postGenericDtoControllerBase5SearchRecycleBin */
export const postApiAdminFdMenuRecyclebinSearch = postGenericDtoControllerBase5SearchRecycleBin;
/** 此处后端没有提供注释 GET /api/admin/FdMenu/tree */
export async function getFdMenuGetUserMenuTree(options?: { [key: string]: any }) {
	return request<APIModel.FdMenuDto[]>('/api/admin/FdMenu/tree', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getFdMenuGetUserMenuTree */
export const getApiAdminFdMenuTree = getFdMenuGetUserMenuTree;
