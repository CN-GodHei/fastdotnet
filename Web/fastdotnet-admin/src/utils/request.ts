import axios, { AxiosInstance, AxiosResponse, InternalAxiosRequestConfig, AxiosError } from 'axios';
import { ElMessage, ElMessageBox } from 'element-plus';
import { Session } from '@/utils/storage';
import qs from 'qs';
import { encryptRequest, decryptResponse } from '@/utils/crypto-utils';
import CryptoJS from 'crypto-js';

// ========== 配置常量 ==========
const REPLAY_WINDOW_SECONDS = 300; // 允许的时间窗口（秒），需与后端一致
const MAX_RETRY_COUNT = 1; // 防重放失败后的最大自动重试次数

// ========== 增强的时间同步服务 ==========
class TimeSyncService {
	private static instance: TimeSyncService;
	private offset: number = 0;
	private lastSyncTime: number = 0;
	private syncPromise: Promise<void> | null = null;
	private isSyncing: boolean = false;

	// 配置：多久没同步才需要从响应头进行“粗略校准” (15分钟)
	private readonly STALE_THRESHOLD = 15 * 60 * 1000;
	// 配置：定期精确同步的间隔 (10分钟)
	private readonly SYNC_INTERVAL = 10 * 60 * 1000;

	private constructor() { }

	public static getInstance(): TimeSyncService {
		if (!TimeSyncService.instance) {
			TimeSyncService.instance = new TimeSyncService();
		}
		return TimeSyncService.instance;
	}

	/**
	 * 精确同步 (考虑 RTT)
	 * 仅在启动、登录、或报错时调用
	 */
	async syncTime(force: boolean = false): Promise<void> {
		if (!force && this.syncPromise) return this.syncPromise;
		if (this.isSyncing && !force) return this.syncPromise || Promise.resolve();

		this.isSyncing = true;
		this.syncPromise = (async () => {
			try {
				const localSendTime = Date.now();
				// 假设这里获取到了 serverTimestamp
				const response = await fetch(`${import.meta.env.VITE_API_URL}/api/admin/FdSystemInfoConfig/GetServiceDateTime`, { cache: 'no-cache' });
				const tsHeader = response.headers.get('X-Server-Timestamp');
				if (tsHeader) {
					const serverTime = parseInt(tsHeader);
					const localRecvTime = Date.now();
					const rtt = localRecvTime - localSendTime;

					// 核心算法：offset = ServerTime - (SendTime + RTT/2)
					this.offset = serverTime - (localSendTime + rtt / 2);
					this.lastSyncTime = Date.now();
					console.log('[TimeSync] 1精确同步完成，Offset:', this.offset, 'ms');
				} else {
					if (!response.ok) {
						throw new Error(`HTTP error! status: ${response.status}`);
					}

					// 2. 关键步骤：使用 .json() 方法解析流
					const data = await response.json();

					// 3. 现在打印出来就是你期望的样子了
					console.log(data);
					const serverTime = parseInt(data.Data);
					const localRecvTime = Date.now();
					const rtt = localRecvTime - localSendTime;

					// 核心算法：offset = ServerTime - (SendTime + RTT/2)
					this.offset = serverTime - (localSendTime + rtt / 2);
					this.lastSyncTime = Date.now();
					console.log('[TimeSync] 2精确同步完成，Offset:', this.offset, 'ms');
				}
			} catch (e) {
				console.warn('[TimeSync] 精确同步失败', e);
			} finally {
				this.isSyncing = false;
				this.syncPromise = null;
			}
		})();
		return this.syncPromise;
	}

	/**
	 * 粗略校准 (不考虑 RTT，仅用于长时间未同步时的纠偏)
	 * 只有在距离上次同步很久时才调用，避免抖动
	 */
	tryCalibrateFromResponse(serverTimestamp: number): void {
		const now = Date.now();

		// 如果刚刚同步过，直接忽略响应头的时间，防止抖动
		if (now - this.lastSyncTime < this.STALE_THRESHOLD) {
			return;
		}

		// 只有当超过阈值，才用响应头时间简单覆盖 offset
		// 注意：这里没有减去 RTT/2，因为是粗略校准，且通常发生在非关键路径
		const newOffset = serverTimestamp - now;

		// 可选：如果新旧 offset 差异过大（如超过 1 分钟），可能是异常值，可以选择忽略或记录警告
		if (Math.abs(newOffset - this.offset) > 60000) {
			console.warn('[TimeSync] 检测到时间偏移量剧烈变化，忽略此次校准:', newOffset);
			return;
		}

		this.offset = newOffset;
		this.lastSyncTime = now;
		console.log('[TimeSync] 触发粗略校准，新 Offset:', this.offset, 'ms');
	}

	getServerTimestampInSeconds(): number {
		// 如果从未同步过，且用户系统时间明显偏差很大（可选高级策略），可以强制触发一次同步
		// 这里简单返回
		return Math.floor((Date.now() + this.offset) / 1000);
	}

	checkAndSync(): void {
		const now = Date.now();
		// 策略 1: 如果从未同步，立即同步
		if (this.lastSyncTime === 0) {
			this.syncTime();
			return;
		}
		// 策略 2: 如果超过定期同步间隔，后台静默同步
		if (now - this.lastSyncTime > this.SYNC_INTERVAL) {
			this.syncTime();
		}
	}

	reset() {
		this.offset = 0;
		this.lastSyncTime = 0;
	}
}

export const timeSyncService = TimeSyncService.getInstance();

// ========== 请求队列管理 (用于自动重试) ==========
interface PendingRequest {
	config: InternalAxiosRequestConfig;
	resolve: (value: any) => void;
	reject: (reason?: any) => void;
	retryCount: number;
}

const pendingQueue: PendingRequest[] = [];

// ========== Axios 实例配置 ==========
const service: AxiosInstance = axios.create({
	baseURL: import.meta.env.VITE_API_URL,
	timeout: 50000,
	headers: { 'Content-Type': 'application/json' },
	paramsSerializer: {
		serialize: (params) => qs.stringify(params, { allowDots: true }),
	},
	validateStatus: (status) => {
		// 200 成功，401 未授权，408/409 重放错误，422 业务验证错误
		return status === 200 || status === 401 || status === 408 || status === 409 || status === 422;
	}
});

// ========== 请求拦截器优化 ==========
service.interceptors.request.use(
	async (config: InternalAxiosRequestConfig) => {
		// 1. Token 处理
		const token = Session.get('token');
		if (token) {
			config.headers!['Authorization'] = `Bearer ${token}`;
		}

		// 2. 系统类别
		if (import.meta.env.VITE_SYSTEM_CATEGORY) {
			config.headers!['System-Category'] = import.meta.env.VITE_SYSTEM_CATEGORY;
		}

		// 3. 防重放核心逻辑
		timeSyncService.checkAndSync();

		const timestamp = timeSyncService.getServerTimestampInSeconds();

		// 优化 Nonce 生成：结合时间戳 + 随机串 + 计数器（防止高并发下 UUID 碰撞或重复）
		// 简单的 UUID v4 通常足够，但为了极致安全，可以加一个毫秒级后缀
		// 使用兼容性更好的方法生成 UUID（避免某些浏览器不支持 crypto.randomUUID）
		const generateUUID = () => {
			if (typeof crypto !== 'undefined' && crypto.randomUUID) {
				return crypto.randomUUID();
			}
			// 降级方案：使用随机数生成 UUID v4
			return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (c) => {
				const r = Math.random() * 16 | 0;
				const v = c === 'x' ? r : (r & 0x3 | 0x8);
				return v.toString(16);
			});
		};
		
		const nonce = `${generateUUID()}-${Date.now()}`;

		const method = config.method?.toUpperCase() || 'GET';
		const path = config.url || '';

		// 确保 Body 序列化一致性后端完全匹配
		let body = '';
		if (config.data && ['POST', 'PUT', 'PATCH'].includes(method)) {
			// 如果已经是字符串（如 qs.stringify 后的），直接用；否则 JSON.stringify
			// 注意：如果后端对 JSON 空格敏感，需确保 JSON.stringify 不带空格或带固定空格
			body = typeof config.data === 'string'
				? config.data
				: JSON.stringify(config.data);
		}
		let normalizedPath = path;
		// 去掉开头的 / （与后端保持一致）
		if (normalizedPath.startsWith('/')) {
			normalizedPath = normalizedPath.substring(1);
		}
				
		// 【关键修复】：使用 UTF-8 兼容的 Base64 编码
		// btoa 只能处理 Latin1 字符，需要先使用 encodeURIComponent 转义为 UTF-8
		const encodeBase64 = (str: string): string => {
			try {
				// 先使用 encodeURIComponent 编码为 UTF-8，再使用 btoa
				return btoa(encodeURIComponent(str).replace(/%([0-9A-F]{2})/g,
					(match, p1) => String.fromCharCode(parseInt(p1, 16))
				));
			} catch (e) {
				console.error('[Base64 编码失败]', str, e);
				return btoa(str); // 降级处理
			}
		};
				
		const pathEncoded = encodeBase64(normalizedPath);
		const bodyEncoded = body ? encodeBase64(body) : '';
		// 签名字符串构建 (必须与后端顺序、分隔符完全一致)
		const signContent = `${timestamp}|${nonce}|${method}|${pathEncoded}|${bodyEncoded}`;

		// 【关键安全优化】：密钥获取策略
		// 方案 A (推荐): 登录后从后端下发动态密钥，存储在 Session 中
		// 方案 B (次选): 硬编码 (极不安全，仅演示)
		// 这里假设我们在 Session 中存储了 'signKey'，如果没有则 fallback 到默认值（生产环境应报错或禁止请求）
		let secretKey = Session.get('signKey');
		if (!secretKey) {
			// 如果没有动态密钥，且不是公开接口，建议阻断请求或记录严重警告
			// 为了演示代码运行，暂时使用默认值，但生产环境请务必移除
			secretKey = import.meta.env.VITE_APP_SIGNATURE_KEY || 'default-secret-key-change-in-production';
			if (secretKey === 'default-secret-key-change-in-production') {
				console.warn('[Security Warning] 使用默认签名密钥，存在严重安全风险！请配置 VITE_APP_SIGNATURE_KEY 或从后端获取动态密钥。');
			}
		}

		const signature = CryptoJS.HmacSHA256(signContent, secretKey).toString(CryptoJS.enc.Base64);

		config.headers!['X-Timestamp'] = timestamp.toString();
		config.headers!['X-Nonce'] = nonce;
		config.headers!['X-Signature'] = signature;

		return config;
	},
	(error) => Promise.reject(error)
);

// ========== 响应拦截器优化 (含自动重试) ==========
service.interceptors.response.use(
	(response: AxiosResponse) => {
		// 1. 更新时间同步
		const serverTsHeader = response.headers['x-server-timestamp'];
		if (serverTsHeader) {
			const serverTime = parseInt(serverTsHeader);
			if (!isNaN(serverTime)) {
				// 只有当内部判断需要校准（如超过15分钟未同步）时，才更新 offset
				timeSyncService.tryCalibrateFromResponse(serverTime);
			}
		}

		const res = response.data;
		const status = response.status;

		// 2. 状态码处理
		if (status === 200) {
			if (res.Code !== 0) {
				// 业务错误
				if (res.Code === 401 || res.Code === 4001) {
					handleLogout();
					return Promise.reject(new Error('登录已过期'));
				}
				ElMessage.error(res.Msg || `Error Code: ${res.Code}`);
				return Promise.reject(new Error(res.Msg));
			}

			// 解密逻辑
			const privateKey = response.headers['x-rsa-privatekey'];
			if (privateKey && res.Data) {
				try {
					return decryptResponse(res.Data, 'RSA', privateKey);
				} catch (e) {
					console.error('解密失败', e);
					// 解密失败是否视为错误？视业务而定，这里暂返回原文或报错
				}
			}
			return res.Data;
		}

		// 3. 防重放错误处理 (408/409)
		if (status === 408 || status === 409) {
			const errorMsg = res.Msg || res.msg || res.message || '请求验证失败';

			// 查找当前请求是否在重试队列中（通过 config 的唯一标识或闭包）
			// 由于 axios 拦截器无法直接获取“当前是哪个请求”，我们需要一种机制标记“这个请求已经重试过一次”
			// 技巧：在 config 上添加自定义属性 __retryCount

			const currentRetryCount = (response.config as any).__retryCount || 0;

			if (currentRetryCount < MAX_RETRY_COUNT) {
				console.log(`[Anti-Replay] 捕获 ${status} 错误，尝试第 ${currentRetryCount + 1} 次自动重试...`);

				// 标记重试次数
				(response.config as any).__retryCount = currentRetryCount + 1;

				// 强制同步时间
				return timeSyncService.syncTime(true).then(() => {
					// 时间同步完成后，重新发起请求
					return service(response.config as InternalAxiosRequestConfig);
				}).catch((syncErr) => {
					console.error('时间同步失败，无法重试', syncErr);
					ElMessage.error('时间同步失败，请检查网络');
					return Promise.reject(response);
				});
			} else {
				// 超过最大重试次数，放弃并提示用户
				ElMessage.error(errorMsg + ' (自动重试失败，请刷新页面)');
				return Promise.reject(response);
			}
		}

		// 4. 401 HTTP 状态码处理
		if (status === 401) {
			// 避免死循环
			if (!window.location.pathname.includes('/login')) {
				handleLogout();
			}
			return Promise.reject(response);
		}

		// 5. 422 处理
		if (status === 422) {
			ElMessage.error(res.msg || res.Msg || '验证错误');
			return Promise.reject({ message: res.msg || res.Msg, code: 422 });
		}

		return response;
	},
	(error: AxiosError) => {
		// 网络错误或未进入 validateStatus 的错误
		if (error.response) {
			// 非预期状态码
			const res = error.response.data as any;
			ElMessage.error(res.Msg || res.msg || '服务器异常');
		} else if (error.request) {
			ElMessage.error('网络断开，请检查连接');
		} else {
			ElMessage.error('请求配置错误');
		}
		return Promise.reject(error);
	}
);

// 登出统一处理
function handleLogout() {
	Session.clear();
	const redirect = encodeURIComponent(window.location.href);
	ElMessageBox.alert('登录已过期，请重新登录', '提示', {
		confirmButtonText: '去登录',
		type: 'warning'
	}).then(() => {
		window.location.href = `/login?redirect=${redirect}`;
	});
}

export default service;
export { encryptRequest, decryptResponse };