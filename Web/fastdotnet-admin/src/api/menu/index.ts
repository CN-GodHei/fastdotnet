import request from '/@/utils/request';

/**
 * 菜单api接口集合 (适配 Fastdotnet 后端)
 * @method getUserMenuTree 获取当前用户菜单树
 */
export function useMenuApi() {
	return {
		// 获取当前用户菜单树 (Admin端)
		getUserMenuTree: () => {
			return request({
				url: '/api/admin/menus/tree',
				method: 'get',
				// get 请求通常不带 data, 参数放在 params 或 url 中
				// 如果后端需要特定参数，可以在这里添加
				// params: { ... }
			});
		},
	};
}