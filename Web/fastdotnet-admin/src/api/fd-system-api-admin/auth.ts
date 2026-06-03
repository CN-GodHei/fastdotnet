// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 管理员端登录 

**请求加密**: 该接口的请求参数需要使用 RSA + AES 混合加密 进行加密。

**响应加密**: 该接口的响应数据使用 RSA + AES 混合加密 进行加密。 [请求加密] [响应加密] POST /api/auth/admin/login */
export async function postAuthAdminLogin(body: APIModel.LoginDto, options?: { [key: string]: any }) {
	// 请求加密
	let processedBody = body;
	if (body) {
		// 混合加密（RSA + AES）
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
