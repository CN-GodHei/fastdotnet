import { defineStore } from 'pinia';

/**
 * 路由缓存列表
 * @methods setCacheKeepAlive 设置要缓存的路由 names（开启 Tagsview）
 * @methods addCachedView 添加要缓存的路由 names（关闭 Tagsview）
 * @methods delCachedView 删除要缓存的路由 names（关闭 Tagsview）
 * @methods delOthersCachedViews 右键菜单`关闭其它`，删除要缓存的路由 names（关闭 Tagsview）
 * @methods delAllCachedViews 右键菜单`全部关闭`，删除要缓存的路由 names（关闭 Tagsview）
 */
export const useKeepALiveNames = defineStore('keepALiveNames', {
	state: (): KeepAliveNamesState => ({
		keepAliveNames: [],
		cachedViews: [],
	}),
	actions: {
		async setCacheKeepAlive(data: Array<string>) {
			console.log("Setting cache keep alive names:", data);
			this.keepAliveNames = data;
		},
		async addCachedView(view: any) {
			console.log("Adding cached view:", view.name, "meta:", view.meta);
			// 使用view.name而不是view.meta.title
			const componentName = view.name;
			const isKeepAlive = view.meta.isKeepAlive || 
				(view.meta.isFdMicroApp && view.meta.menuInfo?.isKeepAlive !== false);
				
			if (isKeepAlive && componentName) {
				// 避免重复添加
				if (!this.cachedViews.includes(componentName)) {
					this.cachedViews?.push(componentName);
				}
			}
		},
		async delCachedView(view: any) {
			console.log("Deleting cached view:", view.name);
			const componentName = view.name;
			const index = this.cachedViews.indexOf(componentName);
			index > -1 && this.cachedViews.splice(index, 1);
		},
		async delOthersCachedViews(view: any) {
			console.log("Deleting other cached views, keeping:", view.name);
			const componentName = view.name;
			const isKeepAlive = view.meta.isKeepAlive || 
				(view.meta.isFdMicroApp && view.meta.menuInfo?.isKeepAlive !== false);
				
			if (isKeepAlive && componentName) this.cachedViews = [componentName];
			else this.cachedViews = [];
		},
		async delAllCachedViews() {
			console.log("Deleting all cached views");
			this.cachedViews = [];
		},
	},
});