/**
 * 微应用实例释放（qiankun 3.0 兼容层）
 *
 * 背景 —— 文档与已发布版本不一致，必须按代码事实写：
 *
 *   qiankun 3.0 的文档站（www.qiankunjs.com）宣称 3.0 新增了
 *   `unload()` / `unloadMicroApp()` / `unloadApplication()`，并建议
 *   「keep-alive 场景用 unload 取代 unmount，否则空闲实例会被回收」。
 *
 *   但已发布的 `qiankun@3.0.0-rc.22` 里这些 API **都不存在**：
 *     - `index.js` 只导出 addErrorHandler / isRuntimeCompatible / loadMicroApp /
 *       microApps / moduleSourceInstanceKeyPlaceholder / precompileModuleSource /
 *       prefetchApps / registerMicroApps / removeErrorHandler / runAfterFirstMounted /
 *       setDefaultMountApp / start / started
 *     - `loadMicroApp()` 返回的是 single-spa 的 Parcel 外部表示，字段只有
 *       mount / unmount / getStatus / loadPromise / bootstrapPromise /
 *       mountPromise / unmountPromise / _parcel
 *
 *   直接照抄文档写 `instance.unload()` 会在 RC 上抛 TypeError。
 *
 * 因此这里做能力探测：未来 GA 真的提供 `unload` 时自动走 `unload`，
 * 否则退回 `unmount`。调用方无需关心当前装的是哪个版本。
 */

import type { MicroApp } from 'qiankun';

/**
 * 释放一个微应用实例。
 *
 * - 有 `unload()`（GA 之后）→ 用它，彻底销毁沙箱与缓存，句柄随之失效；
 * - 只有 `unmount()`（rc.22 现状）→ 退回 `unmount()`，仅移除视图。
 */
export async function disposeMicroApp(instance: MicroApp | null | undefined): Promise<void> {
	if (!instance) return;

	const maybeUnloadable = instance as unknown as { unload?: () => Promise<void> };
	if (typeof maybeUnloadable.unload === 'function') {
		await maybeUnloadable.unload();
		return;
	}

	await instance.unmount();
}
