// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 此处后端没有提供注释 POST /api/auth/app/checkregistrusername */
export async function postApiAuthAppCheckregistrusername(body: APIModel.CheckRegistrUserNameDto, options?: { [key: string]: any }) {
	return request<boolean>('/api/auth/app/checkregistrusername', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 用户端登录 POST /api/auth/app/login */
export async function postApiAuthAppLogin(body: APIModel.LoginDto, options?: { [key: string]: any }) {
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
export async function postApiAuthAppRegister(body: APIModel.AppRegisterDto, options?: { [key: string]: any }) {
	return request<boolean>('/api/auth/app/register', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 发送App注册验证码 POST /api/auth/app/send-registration-code */
export async function postApiAuthAppSendRegistrationCode(body: APIModel.SendRegistrationCodeDto, options?: { [key: string]: any }) {
	return request<boolean>('/api/auth/app/send-registration-code', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
