import { defineStore } from 'pinia';
import { Session } from '@/utils/storage';

/**
 * 用户认证状态管理
 */
export const useAuthStore = defineStore('auth', {
	state: (): AuthState => ({
		token: Session.get('token') || '',
	}),
	actions: {
		// 设置 Token
		setToken(token: string) {
			this.token = token;
			Session.set('token', token);
		},
		// 移除 Token
		removeToken() {
			this.token = '';
			Session.remove('token');
		},
		// 获取 Token
		getToken(): string {
			return this.token || Session.get('token') || '';
		},
	},
});

interface AuthState {
	token: string;
}