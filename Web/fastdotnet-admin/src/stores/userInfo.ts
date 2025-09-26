import { defineStore } from 'pinia';
import { getAdminUsersGetUserInfo } from '/@/api/fd-system-api/AdminUsers';
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
				const apiUserInfo: APIModel.AdminUserDto = await getAdminUsersGetUserInfo();
				console.log('apiUserInfo', apiUserInfo);
				// 将API返回的用户信息映射到前端UserInfos格式
				const userInfos = {
					userName: apiUserInfo.Username,
					photo: apiUserInfo.Avatar,
					Name: apiUserInfo.Name,
					time: Date.now(),
					// roles: apiUserInfo.roles || [],
					// authBtnList: apiUserInfo.permissions || apiUserInfo.authBtnList || [],
				};
				
				// 存储到会话存储中
				Session.set('userInfo', userInfos);
				
				return userInfos;
			} catch (error) {
				console.error('获取用户信息失败:', error);
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
