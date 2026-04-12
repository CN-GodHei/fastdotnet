<template>
	<div class="layout-padding layout-padding-unset layout-iframe">
		<div class="layout-padding-auto layout-padding-view" style="position: relative; height: 100%;">
			<div class="w100" v-for="v in setIframeList" :key="v.path" v-loading="v.meta.loading" element-loading-background="white" style="height: 100%; position: relative;">
				<transition-group :name="name">
					<iframe
						:src="v.meta.isLink"
						:key="v.path"
						frameborder="0"
						height="100%"
						width="100%"
						style="position: absolute; top: 0; left: 0;"
						:data-url="v.path"
						v-show="getRoutePath === v.path"
						ref="iframeRef"
						@load="onIframeLoad"
					/>
				</transition-group>
			</div>
		</div>
	</div>
</template>

<script setup lang="ts" name="layoutIframeView">
import { computed, watch, ref, nextTick } from 'vue';
import { useRoute } from 'vue-router';
import { Session } from '@/utils/storage';

// 定义父组件传过来的值
const props = defineProps({
	// 刷新 iframe
	refreshKey: {
		type: String,
		default: () => '',
	},
	// 过渡动画 name
	name: {
		type: String,
		default: () => 'slide-right',
	},
	// iframe 列表
	list: {
		type: Array,
		default: () => [],
	},
});

// 定义变量内容
const iframeRef = ref();
const route = useRoute();

// 处理 list 列表，当打开时，才进行加载
const setIframeList = computed(() => {
	return (<RouteItems>props.list).filter((v: RouteItem) => v.meta?.isIframeOpen);
});
// 获取 iframe 当前路由 path
const getRoutePath = computed(() => {
	return route.path;
});
// iframe 加载完成回调
const onIframeLoad = (event: Event) => {
	// iframe 加载完成
};
// 关闭 iframe loading
const closeIframeLoading = (val: string, item: RouteItem) => {
	nextTick(() => {
		if (!iframeRef.value) return false;
		iframeRef.value.forEach((v: HTMLIFrameElement) => {
			if (v.dataset.url === val) {
				v.onload = () => {
					if (item.meta?.isIframeOpen && item.meta.loading) item.meta.loading = false;
					
					// 关键：iframe 加载完成后，通过 postMessage 发送 FastAdmin token
					sendTokenToIframe(v);
				};
			}
		});
	});
};

// 向 iframe 发送 token
const sendTokenToIframe = (iframe: HTMLIFrameElement) => {
	try {
		const token = Session.get('token');
		if (!token || !iframe.contentWindow) {
			console.warn('[FastAdmin] No token found or iframe not ready');
			return;
		}
		
		// 获取 iframe 的源（origin）
		const iframeOrigin = new URL(iframe.src).origin;
		
		// 发送 token 到 iframe
		iframe.contentWindow.postMessage(
			{
				type: 'FASTDOTNET_TOKEN',
				data: {
					token: token,
					timestamp: Date.now()
				}
			},
			iframeOrigin // 指定目标 origin，提高安全性
		);
	} catch (error) {
		console.error('[FastAdmin] Failed to send token to iframe:', error);
	}
};
// 监听路由变化，初始化 iframe 数据，防止多个 iframe 时，切换不生效
watch(
	() => route.fullPath,
	(val) => {
		const item: any = props.list.find((v: any) => v.path === val);
		if (!item) return false;
		// 确保设置 isIframeOpen 为 true
		if (!item.meta.isIframeOpen) {
			item.meta.isIframeOpen = true;
		}
		closeIframeLoading(val, item);
	},
	{
		immediate: true,
	}
);
// 监听 iframe refreshKey 变化，用于 tagsview 右键菜单刷新
watch(
	() => props.refreshKey,
	() => {
		const item: any = props.list.find((v: any) => v.path === route.path);
		if (!item) return false;
		if (item.meta.isIframeOpen) item.meta.isIframeOpen = false;
		setTimeout(() => {
			item.meta.isIframeOpen = true;
			item.meta.loading = true;
			closeIframeLoading(route.fullPath, item);
		});
	},
	{
		deep: true,
	}
);
</script>
