import request from '/@/utils/request';

/**
 * 登录api接口集合
 * @method signIn 用户登录 (适配 Fastdotnet 后端)
 */
export function useAuthApi() {
	return {
		// 管理员登录
		adminLogin: (data: { Username: string; Password: string }) => {
			return request({
				url: '/api/auth/admin/login',
				method: 'post',
				data,
			});
		},
		// 客户端用户登录 (如果需要)
		// appLogin: (data: { Username: string; Password: string }) => {
		// 	return request({
		// 		url: '/api/auth/app/login',
		// 		method: 'post',
		// 		data,
		// 	});
		// },
	};
}