// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 管理端专用接口 GET /api/test-scope/admin-only */
/** @deprecated 请使用 getTestScopeAdminOnly，原函数名: getApiTestScopeAdminOnly */
export async function getTestScopeAdminOnly(options?: { [key: string]: any }) {
	return request<any>('/api/test-scope/admin-only', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getTestScopeAdminOnly */
export const getApiTestScopeAdminOnly = getTestScopeAdminOnly;
/** 两端通用接口 GET /api/test-scope/both */
/** @deprecated 请使用 getTestScopeBoth，原函数名: getApiTestScopeBoth */
export async function getTestScopeBoth(options?: { [key: string]: any }) {
	return request<any>('/api/test-scope/both', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getTestScopeBoth */
export const getApiTestScopeBoth = getTestScopeBoth;
/** 加密特性使用示例 - 使用加密特性，请求参数解密固定使用RSA算法，响应加密使用RSA算法 

**请求加密**: 该接口的请求参数需要使用 RSA 算法进行加密。

**响应加密**: 该接口的响应数据使用 RSA 算法进行加密。 [请求加密] [响应加密] POST /api/test-scope/encrypt/default */
/** @deprecated 请使用 postTestScopeDefaultEncryption，原函数名: postApiTestScopeEncryptOpenApiDefault */
export async function postTestScopeDefaultEncryption(body: APIModel.ExampleRequest, options?: { [key: string]: any }) {
	// 请求加密
	let processedBody = body;
	if (body) {
		processedBody = await encryptRequest(body, 'RSA');
	}

	return request<any>('/api/test-scope/encrypt/default', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: processedBody,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postTestScopeDefaultEncryption */
export const postApiTestScopeEncryptOpenApiDefault = postTestScopeDefaultEncryption;
/** 加密特性使用示例 - 请求参数解密固定使用RSA算法，响应加密使用RSA算法 

**请求加密**: 该接口的请求参数需要使用 RSA 算法进行加密。

**响应加密**: 该接口的响应数据使用 RSA 算法进行加密。 [请求加密] [响应加密] POST /api/test-scope/encrypt/rsa */
/** @deprecated 请使用 postTestScopeRsaEncryption，原函数名: postApiTestScopeEncryptRsa */
export async function postTestScopeRsaEncryption(body: APIModel.ExampleRequest, options?: { [key: string]: any }) {
	// 请求加密
	let processedBody = body;
	if (body) {
		processedBody = await encryptRequest(body, 'RSA');
	}

	return request<string>('/api/test-scope/encrypt/rsa', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: processedBody,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postTestScopeRsaEncryption */
export const postApiTestScopeEncryptRsa = postTestScopeRsaEncryption;
