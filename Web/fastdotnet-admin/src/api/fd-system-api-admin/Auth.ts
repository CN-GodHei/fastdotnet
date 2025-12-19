// @ts-ignore
/* eslint-disable */
import request from '@/utils/request';

/** 管理员端登录 POST /api/auth/admin/login */
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

/** 用户端登录 POST /api/auth/app/login */
export async function postAuthAppLogin(body: APIModel.LoginDto, options?: { [key: string]: any }) {
	return request<APIModel.LoginResultDto>('/api/auth/app/login', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** App端用户注册 POST /api/auth/app/register */
export async function postAuthAppRegister(body: APIModel.AppRegisterDto, options?: { [key: string]: any }) {
	return request<string>('/api/auth/app/register', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** 发送App注册验证码 POST /api/auth/app/send-registration-code */
export async function postAuthAppSendRegistrationCode(body: APIModel.SendRegistrationCodeDto, options?: { [key: string]: any }) {
	return request<string>('/api/auth/app/send-registration-code', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
