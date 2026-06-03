// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 访问被拒绝页面 GET /oidc/access-denied */
export async function getOidcLoginAccessDenied(options?: { [key: string]: any }) {
	return request<any>('/oidc/access-denied', {
		method: 'GET',
		...(options || {}),
	});
}
/** OIDC 登录页面（GET） GET /oidc/login */
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
/** OIDC 登录处理（POST） POST /oidc/login */
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
/** OIDC 登出 GET /oidc/logout */
export async function getOidcLoginLogout(options?: { [key: string]: any }) {
	return request<any>('/oidc/logout', {
		method: 'GET',
		...(options || {}),
	});
}
