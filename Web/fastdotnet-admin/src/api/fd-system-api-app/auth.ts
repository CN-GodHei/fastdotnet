// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 此处后端没有提供注释 POST /api/auth/app/checkregistrusername */
/** @deprecated 请使用 postAuthCheckRegistrUserName，原函数名: postApiAuthAppCheckregistrusername */
export async function postAuthCheckRegistrUserName(body: APIModel.CheckRegistrUserNameDto, options?: { [key: string]: any }) {
	return request<boolean>('/api/auth/app/checkregistrusername', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postAuthCheckRegistrUserName */
export const postApiAuthAppCheckregistrusername = postAuthCheckRegistrUserName;
/** 用户端登录 

**请求加密**: 该接口的请求参数需要使用 RSA + AES 混合加密 进行加密。

**响应加密**: 该接口的响应数据使用 RSA + AES 混合加密 进行加密。 [请求加密] [响应加密] POST /api/auth/app/login */
/** @deprecated 请使用 postAuthAppLogin，原函数名: postApiAuthAppLogin */
export async function postAuthAppLogin(body: APIModel.LoginDto, options?: { [key: string]: any }) {
	// 请求加密
	let processedBody = body;
	if (body) {
		processedBody = await encryptRequest(body, 'HYBRID');
	}

	return request<APIModel.LoginResultDto>('/api/auth/app/login', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: processedBody,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postAuthAppLogin */
export const postApiAuthAppLogin = postAuthAppLogin;
/** App端用户注册 POST /api/auth/app/register */
/** @deprecated 请使用 postAuthAppRegister，原函数名: postApiAuthAppRegister */
export async function postAuthAppRegister(body: APIModel.AppRegisterDto, options?: { [key: string]: any }) {
	return request<boolean>('/api/auth/app/register', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postAuthAppRegister */
export const postApiAuthAppRegister = postAuthAppRegister;
/** 发送App注册验证码 POST /api/auth/app/send-registration-code */
/** @deprecated 请使用 postAuthSendRegistrationCode，原函数名: postApiAuthAppSendRegistrationCode */
export async function postAuthSendRegistrationCode(body: APIModel.SendRegistrationCodeDto, options?: { [key: string]: any }) {
	return request<boolean>('/api/auth/app/send-registration-code', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postAuthSendRegistrationCode */
export const postApiAuthAppSendRegistrationCode = postAuthSendRegistrationCode;
