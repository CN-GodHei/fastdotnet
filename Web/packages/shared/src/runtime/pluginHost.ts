/**
 * 插件运行时（微前端）的宿主环境注入缝
 *
 * `@fastdotnet/shared/plugins` 是管理端与应用端共用的插件运行时，
 * 但它需要三样「只有各应用自己才知道」的东西：
 *
 *   1. 插件列表扫描接口 —— 来自各应用自己生成的 openapi2ts 客户端
 *      （`api/fd-system-api-admin/plugin` 与 `api/fd-system-api-app/plugin`）
 *   2. 主应用的 axios 实例 —— 用于通过 props 传给插件子应用（子应用按 `FdRequest` 取用）
 *   3. 少量环境差异 —— 插件前端资源路径段（admin / app）、qiankun 严格样式隔离开关
 *
 * 若运行时直接 `import '@/api/...'`，就会把应用专属路径带进公共层，
 * 「一份代码」的前提立刻被破坏。因此这里只暴露注册点，由各应用在入口注入。
 *
 * 依赖方向始终是「应用 → 公共层」。
 */

/** 插件列表扫描接口的签名（无参、返回插件数组） */
export type PluginScanApi = () => Promise<any>;

export interface PluginHostConfig {
	/** 插件前端资源路径段：管理端为 `admin`，应用端为 `app` */
	frontendSegment: string;
	/**
	 * qiankun 3.0 的样式隔离开关（`sandbox.styleIsolation`，基于原生 CSS `@scope`）。
	 * 2.x 的 `strictStyleIsolation`（Shadow DOM）与 `experimentalStyleIsolation`（选择器改写）
	 * 在 3.0 已合并为这一个布尔项，且不再提供 Shadow DOM 模式。
	 * 默认 true，保持与 2.x 下 `experimentalStyleIsolation: true` 的行为对等。
	 */
	styleIsolation: boolean;
}

let _config: PluginHostConfig = {
	frontendSegment: 'admin',
	styleIsolation: true,
};

let _scanApi: PluginScanApi | undefined;
let _request: unknown;

/** 由应用入口配置插件运行时 */
export function configurePluginHost(config: Partial<PluginHostConfig>): void {
	_config = { ..._config, ...config };
}

export function getPluginHostConfig(): Readonly<PluginHostConfig> {
	return _config;
}

/** 由应用入口注入「扫描插件列表」的实现 */
export function registerPluginScanApi(api: PluginScanApi): void {
	_scanApi = api;
}

export function getPluginScanApi(): PluginScanApi {
	if (!_scanApi) {
		throw new Error(
			'[@fastdotnet/shared] 尚未注册插件扫描接口。请在应用入口（main.ts）中调用：\n' +
				"  import { registerPluginScanApi } from '@fastdotnet/shared/runtime/pluginHost';\n" +
				"  import { getPluginScanPlugins } from '@/api/.../plugin';\n" +
				'  registerPluginScanApi(getPluginScanPlugins);'
		);
	}
	return _scanApi;
}

/** 由应用入口注入主应用的 axios 实例（会被传给插件子应用，属性名为 FdRequest） */
export function registerPluginRequest(request: unknown): void {
	_request = request;
}

export function getPluginRequest<T = any>(): T {
	if (!_request) {
		throw new Error(
			'[@fastdotnet/shared] 尚未注册主应用 request 实例。请在应用入口（main.ts）中调用：\n' +
				"  import { registerPluginRequest } from '@fastdotnet/shared/runtime/pluginHost';\n" +
				"  import request from '@/utils/request';\n" +
				'  registerPluginRequest(request);'
		);
	}
	return _request as T;
}
