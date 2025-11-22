import { defineStore } from 'pinia';
import { getAdminFdAdminUserGetUserInfo } from '/@/api/fd-system-api/FdAdminUser';
import { Session } from '/@/utils/storage';

/**
 * 用户信息
 * @methods setUserInfos 设置用户信息
 */
export const useUserInfo = defineStore('userInfo', {
	state: (): UserInfosState => ({
		userInfos: {
			userName: '',
			photo: '',
			time: 0,
			roles: [],
			authBtnList: [],
		},
	}),
	actions: {
		async setUserInfos() {
			// 存储用户信息到浏览器缓存
			if (Session.get('userInfo')) {
				this.userInfos = Session.get('userInfo');
			} else {
				const userInfos = <UserInfos>await this.getApiUserInfo();
				this.userInfos = userInfos;
			}
		},
		// 从后端API获取用户信息
		async getApiUserInfo() {
			try {
				// 调用后端API获取用户信息
				// 由于 request.ts 的响应拦截器已经返回了 res.Data，所以这里直接获取 AdminUserDto 对象
				const apiUserInfo: APIModel.FdAdminUserDto = await getAdminFdAdminUserGetUserInfo();
				// console.log('apiUserInfo', apiUserInfo);
				
				
				
				// 将API返回的用户信息映射到前端UserInfos格式
				const userInfos = {
					userName: apiUserInfo.Username || '',
					photo: apiUserInfo.Avatar || '',
					Name: apiUserInfo.Name || '',
					time: Date.now(),
					// 从后端获取角色信息，现在AdminUserDto已扩展包含RoleIds
					roles: apiUserInfo.RoleIds || [], // 现在AdminUserDto包含RoleIds字段
					// 使用后端返回的按钮权限，不再单独获取按钮权限
					authBtnList: apiUserInfo.Buttons || [], // 使用AdminUserDto中包含的Buttons字段
				};
				
				// console.log('Final userInfos with roles and permissions:', userInfos);
				
				// 存储到会话存储中
				Session.set('userInfo', userInfos);
				
				return userInfos;
			} catch (error) {
				// console.error('获取用户信息失败:', error);
				// 如果获取失败，返回默认用户信息
				const defaultUserInfos = {
					userName: '',
					photo: '',
					time: 0,
					roles: [],
					authBtnList: [],
				};
				Session.set('userInfo', defaultUserInfos);
				return defaultUserInfos;
			}
		},
	},
});
