import axios, { AxiosInstance, AxiosResponse, InternalAxiosRequestConfig } from 'axios';
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
	// 只将200、401、422状态码视为成功，其他状态码走error处理
	// 422是后端返回的一种可控异常，主要是提示消息
	validateStatus: (status) => {
		return status === 200 || status === 401 || status === 422;
	}
});

// 添加请求拦截器
service.interceptors.request.use(
	(config: InternalAxiosRequestConfig) => {
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
	(response: AxiosResponse) => {
		// 对响应数据做点什么
		const res = response.data;
		
		// 根据HTTP状态码进行不同处理
		if (response.status === 200) {
			// HTTP 200 表示成功请求
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
				} 
				else {
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
		} else if (response.status === 401) {
			// HTTP 401 未授权访问
			// 检查当前页面是否已经是登录页，避免无限重定向
			const currentPath = window.location.pathname + window.location.search;
			if (currentPath !== '/login' && !currentPath.startsWith('/login')) {
				Session.clear(); // 清除浏览器全部临时缓存
				// 将当前页面路径作为查询参数传递给登录页
				const redirectUrl = encodeURIComponent(currentPath);
				// 弹窗提示登录已过期，请重新登录
				ElMessageBox.alert('登录已过期，请重新登录', '提示', {
					confirmButtonText: '确定',
					type: 'warning'
				}).then(() => {
					window.location.href = `/login?redirect=${redirectUrl}`; // 去登录页并携带重定向信息
				}).catch(() => {
					window.location.href = `/login?redirect=${redirectUrl}`; // 去登录页并携带重定向信息
				});
			}
			return Promise.reject(new Error('未授权访问'));
		} else if (response.status === 422) {
			ElMessage.error(res.Msg || res.message || '验证错误');
			// HTTP 422 验证错误，是后端返回的一种可控异常，主要是提示消息
			// 422虽然在validateStatus中被视为成功，但仍需要reject以便调用方可以处理错误
        	return Promise.reject(new Error(res.Msg || res.message || '验证错误'));
        	// return Promise.reject(new Error());
		}
	},
	(error) => {
		// 对响应错误做点什么
		console.error('Axios Error:', error); // 添加日志以便调试
		
		// 只有当请求完全失败（例如网络断开、DNS 解析失败、服务器无响应等）时才显示网络错误
		if (error.response) {
			// 请求已发出，但服务器响应的状态码不在 validateStatus 定义的范围内
			const res = error.response.data;
			const errorMsg = res.Msg || res.msg || res.message || error.message || '请求失败';
			ElMessage.error(typeof errorMsg === 'string' ? errorMsg : '请求失败');
			return Promise.reject(error);
		} else if (error.request) {
			// 请求已发出，但没有收到响应
			ElMessage.error('网络错误，请稍后重试');
			return Promise.reject(error);
		} else {
			// 其他未知错误
			ElMessage.error('未知错误，请稍后重试');
			return Promise.reject(error);
		}
	}
);

// 导出 axios 实例
export default service;