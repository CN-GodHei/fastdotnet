// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** App端专用接口 GET /api/test-scope/app-only */
export async function getApiTestScopeAppOnly(options?: { [key: string]: any }) {
	return request<any>('/api/test-scope/app-only', {
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
/** 无作用域限制接口 GET /api/test-scope/no-scope */
export async function getApiTestScopeNoScope(options?: { [key: string]: any }) {
	return request<any>('/api/test-scope/no-scope', {
		method: 'GET',
		...(options || {}),
	});
}
