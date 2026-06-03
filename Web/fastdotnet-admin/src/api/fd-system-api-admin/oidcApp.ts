// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 获取所有 OIDC 应用列表 GET /api/OidcApp */
export async function getOidcAppGetApplications(options?: { [key: string]: any }) {
	return request<any>('/api/OidcApp', {
		method: 'GET',
		...(options || {}),
	});
}
/** 创建 OIDC 应用 POST /api/OidcApp */
export async function postOidcAppCreateApplication(body: APIModel.CreateOidcApplicationRequest, options?: { [key: string]: any }) {
	return request<any>('/api/OidcApp', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 根据 ClientId 获取应用详情 GET /api/OidcApp/${param0} */
export async function getOidcAppGetApplication(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getOidcAppGetApplicationParams,
	options?: { [key: string]: any }
) {
	const { clientId: param0, ...queryParams } = params;

	return request<any>(`/api/OidcApp/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}
/** 删除应用 DELETE /api/OidcApp/${param0} */
export async function deleteOidcAppDeleteApplication(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteOidcAppDeleteApplicationParams,
	options?: { [key: string]: any }
) {
	const { clientId: param0, ...queryParams } = params;

	return request<boolean>(`/api/OidcApp/${param0}`, {
		method: 'DELETE',
		params: { ...queryParams },
		...(options || {}),
	});
}
/** 重置应用密钥 POST /api/OidcApp/${param0}/reset-secret */
export async function postOidcAppResetSecret(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.postOidcAppResetSecretParams,
	options?: { [key: string]: any }
) {
	const { clientId: param0, ...queryParams } = params;

	return request<any>(`/api/OidcApp/${param0}/reset-secret`, {
		method: 'POST',
		params: { ...queryParams },
		...(options || {}),
	});
}
