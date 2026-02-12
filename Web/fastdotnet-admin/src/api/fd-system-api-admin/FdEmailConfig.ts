// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 更新现有记录 根据提供的ID和更新数据，修改现有记录的信息。 PUT /api/admin/FdEmailConfig/${param0} */
export async function putApiAdminFdEmailConfigId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putApiAdminFdEmailConfigIdParams,
	body: APIModel.FdUpdateEmailConfigDto,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<APIModel.FdEmailConfigDto>(`/api/admin/FdEmailConfig/${param0}`, {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		params: { ...queryParams },
		data: body,
		...(options || {}),
	});
}
/** 根据实体主键批量更新实体信息 根据实体主键批量更新实体信息 PUT /api/admin/FdEmailConfig/batch */
export async function putApiAdminFdEmailConfigBatch(body: APIModel.FdUpdateEmailConfigDto[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdEmailConfig/batch', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 批量创建新记录 根据提供的数据批量创建新记录。 POST /api/admin/FdEmailConfig/batch */
export async function postApiAdminFdEmailConfigBatch(body: APIModel.FdCreateEmailConfigDto[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdEmailConfig/batch', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 批量删除记录 根据提供的ID列表，批量删除多条记录。 DELETE /api/admin/FdEmailConfig/batch */
export async function deleteApiAdminFdEmailConfigBatch(body: string[], options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdEmailConfig/batch', {
		method: 'DELETE',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 根据条件批量更新实体属性（部分字段更新） 根据条件批量更新实体属性（部分字段更新） PUT /api/admin/FdEmailConfig/batch/updatebycondition */
export async function putApiAdminFdEmailConfigBatchUpdatebycondition(
	body: APIModel.FdUpdateEmailConfigDtoBatchUpdateByConditionDto,
	options?: { [key: string]: any }
) {
	return request<number>('/api/admin/FdEmailConfig/batch/updatebycondition', {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 获取唯一的邮件配置 GET /api/admin/FdEmailConfig/GetConfig */
export async function getApiAdminFdEmailConfigGetConfig(options?: { [key: string]: any }) {
	return request<APIModel.FdEmailConfigDto>('/api/admin/FdEmailConfig/GetConfig', {
		method: 'GET',
		...(options || {}),
	});
}
/** 根据自定义条件获取列表(不分页) 根据自定义条件获取列表(不分页) POST /api/admin/FdEmailConfig/list-by-condition */
export async function postApiAdminFdEmailConfigListByCondition(body: APIModel.QueryByConditionDto, options?: { [key: string]: any }) {
	return request<any>('/api/admin/FdEmailConfig/list-by-condition', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 分页获取记录 根据页码和页面大小，分页检索记录。 GET /api/admin/FdEmailConfig/page */
export async function getApiAdminFdEmailConfigPage(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getApiAdminFdEmailConfigPageParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.PageInfo>('/api/admin/FdEmailConfig/page', {
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
/** 根据条件分页获取记录 根据提供的查询条件和分页参数，分页检索记录。 POST /api/admin/FdEmailConfig/page/search */
export async function postApiAdminFdEmailConfigPageSearch(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/admin/FdEmailConfig/page/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}
/** 获取回收站数据 检索并返回已软删除的记录（回收站数据）。 GET /api/admin/FdEmailConfig/recyclebin */
export async function getApiAdminFdEmailConfigRecyclebin(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getApiAdminFdEmailConfigRecyclebinParams,
	options?: { [key: string]: any }
) {
	return request<APIModel.PageInfo>('/api/admin/FdEmailConfig/recyclebin', {
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
/** 永久删除回收站中的记录 根据提供的ID，将已软删除的记录从数据库中永久移除。 DELETE /api/admin/FdEmailConfig/recyclebin/${param0}/permanent */
export async function deleteApiAdminFdEmailConfigRecyclebinIdPermanent(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteApiAdminFdEmailConfigRecyclebinIdPermanentParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/admin/FdEmailConfig/recyclebin/${param0}/permanent`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}
/** 恢复回收站中的记录 根据提供的ID，将已软删除的记录恢复到正常状态。 PUT /api/admin/FdEmailConfig/recyclebin/${param0}/restore */
export async function putApiAdminFdEmailConfigRecyclebinIdRestore(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putApiAdminFdEmailConfigRecyclebinIdRestoreParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<boolean>(`/api/admin/FdEmailConfig/recyclebin/${param0}/restore`, {
		method: 'PUT',
		params: { ...queryParams },
		...(options || {}),
	});
}
/** 根据条件永久删除回收站中的记录 根据提供的条件，将回收站中符合条件的记录从数据库中永久移除。 POST /api/admin/FdEmailConfig/recyclebin/permanent */
export async function postApiAdminFdEmailConfigRecyclebinPermanent(
	body: APIModel.EmailConfigBooleanFuncExpression,
	options?: { [key: string]: any }
) {
	return request<number>('/api/admin/FdEmailConfig/recyclebin/permanent', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 批量恢复回收站中的记录 根据提供的条件，批量将回收站中的记录恢复到正常状态。 POST /api/admin/FdEmailConfig/recyclebin/restore */
export async function postApiAdminFdEmailConfigRecyclebinRestore(body: APIModel.EmailConfigBooleanFuncExpression, options?: { [key: string]: any }) {
	return request<number>('/api/admin/FdEmailConfig/recyclebin/restore', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 根据条件查询回收站数据 根据提供的查询条件，检索回收站中的记录。 POST /api/admin/FdEmailConfig/recyclebin/search */
export async function postApiAdminFdEmailConfigRecyclebinSearch(body: APIModel.PageQueryByConditionDto, options?: { [key: string]: any }) {
	return request<APIModel.PageInfo>('/api/admin/FdEmailConfig/recyclebin/search', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		data: body,
		...(options || {}),
	});
}
/** 更新唯一的邮件配置 POST /api/admin/FdEmailConfig/UpdateConfig */
export async function postApiAdminFdEmailConfigUpdateConfig(body: APIModel.FdUpdateEmailConfigDto, options?: { [key: string]: any }) {
	return request<APIModel.FdEmailConfigDto>('/api/admin/FdEmailConfig/UpdateConfig', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
