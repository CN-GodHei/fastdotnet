// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 访问被拒绝页面 GET /oidc/access-denied */
/** @deprecated 请使用 getOidcLoginAccessDenied，原函数名: getOidcAccessDenied */
export async function getOidcLoginAccessDenied(options?: { [key: string]: any }) {
	return request<any>('/oidc/access-denied', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getOidcLoginAccessDenied */
export const getOidcAccessDenied = getOidcLoginAccessDenied;
/** OIDC 登录页面（GET） GET /oidc/login */
/** @deprecated 请使用 getOidcLoginLogin，原函数名: getOidcLogin */
export async function getOidcLoginLogin(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getOidcLoginLoginParams,
	options?: { [key: string]: any }
) {
	return request<any>('/oidc/login', {
		method: 'GET',
		params: {
			...params,
		},
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getOidcLoginLogin */
export const getOidcLogin = getOidcLoginLogin;
/** OIDC 登录处理（POST） POST /oidc/login */
/** @deprecated 请使用 postOidcLoginLogin，原函数名: postOidcLogin */
export async function postOidcLoginLogin(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.postOidcLoginLoginParams,
	body: {
		Username: string;
		Password: string;
		CaptchaId?: string;
		CaptchaCode?: string;
	},
	options?: { [key: string]: any }
) {
	return request<any>('/oidc/login', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/x-www-form-urlencoded',
		},
		params: {
			...params,
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postOidcLoginLogin */
export const postOidcLogin = postOidcLoginLogin;
/** OIDC 登出 GET /oidc/logout */
/** @deprecated 请使用 getOidcLoginLogout，原函数名: getOidcLogout */
export async function getOidcLoginLogout(options?: { [key: string]: any }) {
	return request<any>('/oidc/logout', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getOidcLoginLogout */
export const getOidcLogout = getOidcLoginLogout;
