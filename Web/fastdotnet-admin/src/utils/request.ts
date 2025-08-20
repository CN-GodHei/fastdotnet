import axios, { AxiosInstance } from 'axios';
import { ElMessage, ElMessageBox } from 'element-plus';
import { Session } from '/@/utils/storage';
import qs from 'qs';

// 配置新建一个 axios 实例
const service: AxiosInstance = axios.create({
	baseURL: import.meta.env.VITE_API_URL,
	timeout: 50000,
	headers: { 'Content-Type': 'application/json' },
	paramsSerializer: {
		serialize(params) {
			return qs.stringify(params, { allowDots: true });
		},
	},
});

// 添加请求拦截器
service.interceptors.request.use(
	(config) => {
		// 在发送请求之前做些什么 token
		const token = Session.get('token');
		// 添加调试日志
		// console.log('Request Interceptor - Token from Cookie:', token);
		if (token) {
			// 标准 JWT 格式通常是 Bearer <token>
			config.headers!['Authorization'] = `Bearer ${token}`;
		} else {
			// console.log('Request Interceptor - No Token Found');
		}
		return config;
	},
	(error) => {
		// 对请求错误做些什么
		console.error('Request Interceptor Error:', error);
		return Promise.reject(error);
	}
);

// 添加响应拦截器
service.interceptors.response.use(
	(response) => {
		// 对响应数据做点什么
		const res = response.data;
		// 根据后端返回的格式 { Data: ..., Code: ..., Msg: ... } 进行处理
		if (res.Code !== 0) {
			// `token` 过期或者账号已在别处登录 (假设后端用 401 或 4001 表示)
			if (res.Code === 401 || res.Code === 4001) {
				Session.clear(); // 清除浏览器全部临时缓存
				window.location.href = '/'; // 去登录页
				ElMessageBox.alert('你已被登出，请重新登录', '提示', {})
					.then(() => {})
					.catch(() => {});
				// 对于登出错误，可以返回一个特定的 rejected promise 或者让其继续以触发全局错误处理
				return Promise.reject(new Error('登录已过期'));
			} else {
				// 其他业务错误
				const errorMsg = res.Msg || `Error Code: ${res.Code}`;
				ElMessage.error(errorMsg);
			}
			// 返回 reject 以便调用方可以处理错误
			return Promise.reject(new Error(res.Msg || `Error Code: ${res.Code}`));
		} else {
			// Code 为 0 表示成功，直接返回 Data 部分
			// 这样业务代码可以直接使用返回的数据，无需再 .Data
			return res.Data;
		}
	},
	(error) => {
		// 对响应错误做点什么
		console.error('Axios Error:', error); // 添加日志以便调试

		// 更安全地检查 error 对象及其属性
		if (error) {
			// 检查是否是超时错误
			if (error.code === 'ECONNABORTED' && typeof error.message === 'string' && error.message.includes('timeout')) {
				ElMessage.error('网络请求超时');
			}
			// 检查是否是网络错误
			else if (typeof error.message === 'string' && error.message === 'Network Error') {
				ElMessage.error('网络连接错误');
			}
			// 尝试从响应中获取更具体的错误信息
			else if (error.response && error.response.data) {
				const res = error.response.data;
				// 尝试获取后端返回的错误信息
				const errorMsg = res.Msg || res.msg || res.message || res.Message || (error.response.statusText || '请求失败');
				ElMessage.error(typeof errorMsg === 'string' ? errorMsg : '请求失败');
			}
			// 其他未知错误
			else {
				ElMessage.error('接口请求失败或未响应');
			}
		} else {
			// 极少数情况下 error 可能是 undefined
			console.error('Unexpected undefined error in axios interceptor');
			ElMessage.error('发生未知错误');
		}
		return Promise.reject(error);
	}
);

// 导出 axios 实例
export default service;
