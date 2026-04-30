// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 获取所有 OIDC 应用列表 GET /api/OidcApp */
/** @deprecated 请使用 getOidcAppGetApplications，原函数名: getApiOidcApp */
export async function getOidcAppGetApplications(options?: { [key: string]: any }) {
	return request<any>('/api/OidcApp', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getOidcAppGetApplications */
export const getApiOidcApp = getOidcAppGetApplications;
/** 创建 OIDC 应用 POST /api/OidcApp */
/** @deprecated 请使用 postOidcAppCreateApplication，原函数名: postApiOidcApp */
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

/** @deprecated 此函数名已变更，请使用 postOidcAppCreateApplication */
export const postApiOidcApp = postOidcAppCreateApplication;
/** 根据 ClientId 获取应用详情 GET /api/OidcApp/${param0} */
/** @deprecated 请使用 getOidcAppGetApplication，原函数名: getApiOidcAppClientId */
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

/** @deprecated 此函数名已变更，请使用 getOidcAppGetApplication */
export const getApiOidcAppClientId = getOidcAppGetApplication;
/** 删除应用 DELETE /api/OidcApp/${param0} */
/** @deprecated 请使用 deleteOidcAppDeleteApplication，原函数名: deleteApiOidcAppClientId */
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

/** @deprecated 此函数名已变更，请使用 deleteOidcAppDeleteApplication */
export const deleteApiOidcAppClientId = deleteOidcAppDeleteApplication;
/** 重置应用密钥 POST /api/OidcApp/${param0}/reset-secret */
/** @deprecated 请使用 postOidcAppResetSecret，原函数名: postApiOidcAppClientIdResetSecret */
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

/** @deprecated 此函数名已变更，请使用 postOidcAppResetSecret */
export const postApiOidcAppClientIdResetSecret = postOidcAppResetSecret;
