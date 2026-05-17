// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 管理员端登录 

**请求加密**: 该接口的请求参数需要使用 RSA + AES 混合加密 进行加密。

**响应加密**: 该接口的响应数据使用 RSA + AES 混合加密 进行加密。 [请求加密] [响应加密] POST /api/auth/admin/login */
/** @deprecated 请使用 postAuthAdminLogin，原函数名: postApiAuthAdminLogin */
export async function postAuthAdminLogin(body: APIModel.LoginDto, options?: { [key: string]: any }) {
	// 请求加密
	let processedBody = body;
	if (body) {
		processedBody = await encryptRequest(body, 'HYBRID');
	}

	return request<APIModel.LoginResultDto>('/api/auth/admin/login', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: processedBody,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 postAuthAdminLogin */
export const postApiAuthAdminLogin = postAuthAdminLogin;
