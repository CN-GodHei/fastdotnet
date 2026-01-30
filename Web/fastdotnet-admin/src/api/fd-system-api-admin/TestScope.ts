// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 管理端专用接口 GET /api/test-scope/admin-only */
export async function getApiTestScopeAdminOnly(options?: { [key: string]: any }) {
	return request<any>('/api/test-scope/admin-only', {
		method: 'GET',
		...(options || {}),
	});
}
/** 两端通用接口 GET /api/test-scope/both */
export async function getApiTestScopeBoth(options?: { [key: string]: any }) {
	return request<any>('/api/test-scope/both', {
		method: 'GET',
		...(options || {}),
	});
}
/** 将参数使用RSA加密后返回 POST /api/test-scope/encrypt-with-config-key */
export async function postApiTestScopeEncryptWithConfigKey(body: APIModel.ExampleRequest, options?: { [key: string]: any }) {
	return request<any>('/api/test-scope/encrypt-with-config-key', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 加密特性使用示例 - 使用加密特性，请求参数解密固定使用RSA算法，响应加密使用RSA算法 

**请求加密**: 该接口的请求参数需要使用 RSA 算法进行加密。

**响应加密**: 该接口的响应数据使用 RSA 算法进行加密。 [请求加密] [响应加密] POST /api/test-scope/encrypt/default */
export async function postApiTestScopeEncrypt__openAPI__default(body: APIModel.ExampleRequest, options?: { [key: string]: any }) {
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
/** 加密特性使用示例 - 请求参数解密固定使用RSA算法，响应加密使用RSA算法 

**请求加密**: 该接口的请求参数需要使用 RSA 算法进行加密。

**响应加密**: 该接口的响应数据使用 RSA 算法进行加密。 [请求加密] [响应加密] POST /api/test-scope/encrypt/rsa */
export async function postApiTestScopeEncryptRsa(body: APIModel.ExampleRequest, options?: { [key: string]: any }) {
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
/** 无作用域限制接口 GET /api/test-scope/no-scope */
export async function getApiTestScopeNoScope(options?: { [key: string]: any }) {
	return request<any>('/api/test-scope/no-scope', {
		method: 'GET',
		...(options || {}),
	});
}
