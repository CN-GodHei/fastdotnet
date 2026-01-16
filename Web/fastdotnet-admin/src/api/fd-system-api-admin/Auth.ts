// @ts-ignore
/* eslint-disable */
import request from '@/utils/request';

/** 管理员端登录 POST /api/auth/admin/login */
export async function postApiAuthAdminLogin(body: APIModel.LoginDto, options?: { [key: string]: any }) {
	return request<APIModel.LoginResultDto>('/api/auth/admin/login', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
