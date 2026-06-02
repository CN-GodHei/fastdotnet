// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** App端专用接口 GET /api/test-scope/app-only */
export async function getTestScopeAppOnly(options?: { [key: string]: any }) {
	return request<any>('/api/test-scope/app-only', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getTestScopeAppOnly */
export const getApiTestScopeAppOnly = getTestScopeAppOnly;
/** 两端通用接口 GET /api/test-scope/both */
export async function getTestScopeBoth(options?: { [key: string]: any }) {
	return request<any>('/api/test-scope/both', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getTestScopeBoth */
export const getApiTestScopeBoth = getTestScopeBoth;
