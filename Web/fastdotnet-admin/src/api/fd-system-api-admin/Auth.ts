// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 管理员端登录 POST /api/auth/admin/login */
/** @deprecated 请使用 postAuthAdminLogin，原函数名: postApiAuthAdminLogin */
export async function postAuthAdminLogin(body: APIModel.LoginDto, options?: { [key: string]: any }) {
	return request<APIModel.LoginResultDto>('/api/auth/admin/login', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postAuthAdminLogin */
export const postApiAuthAdminLogin = postAuthAdminLogin;
