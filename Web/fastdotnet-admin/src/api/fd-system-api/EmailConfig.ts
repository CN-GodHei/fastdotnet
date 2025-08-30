// @ts-ignore
/* eslint-disable */
import request from '/@/utils/request';

/** 更新现有记录 根据提供的ID和更新数据，修改现有记录的信息。 PUT /api/admin/EmailConfig/${param0} */
export async function putAdminEmailConfigId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putAdminEmailConfigIdParams,
	body: APIModel.UpdateEmailConfigDto,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<APIModel.EmailConfigDto>(`/api/admin/EmailConfig/${param0}`, {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		params: { ...queryParams },
		data: body,
		...(options || {}),
	});
}

/** 批量删除记录 根据提供的ID列表，批量删除多条记录。 DELETE /api/admin/EmailConfig/batch */
export async function deleteAdminEmailConfigBatch(body: string[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/EmailConfig/batch', {
		method: 'DELETE',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 获取唯一的邮件配置 GET /api/admin/EmailConfig/GetConfig */
export async function getAdminEmailConfigGetConfig(options?: { [key: string]: any }) {
	return request<APIModel.EmailConfigDto>('/api/admin/EmailConfig/GetConfig', {
		method: 'GET',
		...(options || {}),
	});
}

/** 分页获取记录 根据页码和页面大小，分页检索记录。 GET /api/admin/EmailConfig/page */
export async function getAdminEmailConfigPage(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAdminEmailConfigPageParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.EmailConfigDtoPageResult>('/api/admin/EmailConfig/page', {
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

/** 根据条件分页获取记录 根据提供的查询条件和分页参数，分页检索记录。 POST /api/admin/EmailConfig/page/search */
export async function postAdminEmailConfigPageSearch(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.EmailConfigDtoPageResult>('/api/admin/EmailConfig/page/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}

/** 获取回收站数据 检索并返回已软删除的记录（回收站数据）。 GET /api/admin/EmailConfig/recyclebin */
export async function getAdminEmailConfigRecyclebin(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAdminEmailConfigRecyclebinParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.EmailConfigDtoPageResult>('/api/admin/EmailConfig/recyclebin', {
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

/** 永久删除回收站中的记录 根据提供的ID，将已软删除的记录从数据库中永久移除。 DELETE /api/admin/EmailConfig/recyclebin/${param0}/permanent */
export async function deleteAdminEmailConfigRecyclebinIdPermanent(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteAdminEmailConfigRecyclebinIdPermanentParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<boolean>(`/api/admin/EmailConfig/recyclebin/${param0}/permanent`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 恢复回收站中的记录 根据提供的ID，将已软删除的记录恢复到正常状态。 PUT /api/admin/EmailConfig/recyclebin/${param0}/restore */
export async function putAdminEmailConfigRecyclebinIdRestore(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putAdminEmailConfigRecyclebinIdRestoreParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<boolean>(`/api/admin/EmailConfig/recyclebin/${param0}/restore`, {
		method: 'PUT',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** 根据条件永久删除回收站中的记录 根据提供的条件，将回收站中符合条件的记录从数据库中永久移除。 POST /api/admin/EmailConfig/recyclebin/permanent */
export async function postAdminEmailConfigRecyclebinPermanent(body: APIModel.EmailConfigBooleanFuncExpression, options?: { [key: string]: any }) {
	return request<number>('/api/admin/EmailConfig/recyclebin/permanent', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 批量恢复回收站中的记录 根据提供的条件，批量将回收站中的记录恢复到正常状态。 POST /api/admin/EmailConfig/recyclebin/restore */
export async function postAdminEmailConfigRecyclebinRestore(body: APIModel.EmailConfigBooleanFuncExpression, options?: { [key: string]: any }) {
	return request<number>('/api/admin/EmailConfig/recyclebin/restore', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 根据条件查询回收站数据 根据提供的查询条件，检索回收站中的记录。 POST /api/admin/EmailConfig/recyclebin/search */
export async function postAdminEmailConfigRecyclebinSearch(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.EmailConfigDtoPageResult>('/api/admin/EmailConfig/recyclebin/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}

/** 更新唯一的邮件配置 PUT /api/admin/EmailConfig/UpdateConfig */
export async function putAdminEmailConfigUpdateConfig(body: APIModel.UpdateEmailConfigDto, options?: { [key: string]: any }) {
	return request<any>('/api/admin/EmailConfig/UpdateConfig', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
