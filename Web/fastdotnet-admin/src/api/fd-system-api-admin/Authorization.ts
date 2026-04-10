// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** OIDC 授权端点
GET /connect/authorize GET /connect/authorize */
export async function getConnectAuthorize(options?: { [key: string]: any }) {
	return request<any>('/connect/authorize', {
		method: 'GET',
		...(options || {}),
	});
}
/** OIDC 授权端点
GET /connect/authorize POST /connect/authorize */
export async function postConnectAuthorize(options?: { [key: string]: any }) {
	return request<any>('/connect/authorize', {
		method: 'POST',
		...(options || {}),
	});
}
/** 用户登录端点（供前端调用）
POST /connect/login POST /connect/login */
export async function postConnectLogin(body: APIModel.LoginRequest, options?: { [key: string]: any }) {
	return request<any>('/connect/login', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** OIDC 登出端点
GET /connect/logout GET /connect/logout */
export async function getConnectLogout(options?: { [key: string]: any }) {
	return request<any>('/connect/logout', {
		method: 'GET',
		...(options || {}),
	});
}
/** OIDC 登出端点
GET /connect/logout POST /connect/logout */
export async function postConnectLogout(options?: { [key: string]: any }) {
	return request<any>('/connect/logout', {
		method: 'POST',
		...(options || {}),
	});
}
/** OIDC Token 端点
POST /connect/token POST /connect/token */
export async function postConnectToken(options?: { [key: string]: any }) {
	return request<any>('/connect/token', {
		method: 'POST',
		...(options || {}),
	});
}
