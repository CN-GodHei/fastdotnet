<template>
	<div v-if="isShowBreadcrumb" class="layout-navbars-breadcrumb">
		<SvgIcon
			class="layout-navbars-breadcrumb-icon"
			:name="themeConfig.isCollapse ? 'ele-Expand' : 'ele-Fold'"
			:size="16"
			@click="onThemeConfigChange"
		/>
		<el-breadcrumb class="layout-navbars-breadcrumb-hide">
			<transition-group name="breadcrumb">
				<el-breadcrumb-item v-for="(v, k) in state.breadcrumbList" :key="v.path">
					<span v-if="k === state.breadcrumbList.length - 1" class="layout-navbars-breadcrumb-span">
						<SvgIcon :name="v.meta.icon" class="layout-navbars-breadcrumb-iconfont" v-if="themeConfig.isBreadcrumbIcon" />
						<div v-if="!v.meta.tagsViewName">{{ $t(v.meta.title) }}</div>
						<div v-else>{{ v.meta.tagsViewName }}</div>
					</span>
					<a v-else @click.prevent="onBreadcrumbClick(v)">
						<SvgIcon :name="v.meta.icon" class="layout-navbars-breadcrumb-iconfont" v-if="themeConfig.isBreadcrumbIcon" />{{ $t(v.meta.title) }}
					</a>
				</el-breadcrumb-item>
			</transition-group>
		</el-breadcrumb>
	</div>
</template>

<script setup lang="ts" name="layoutBreadcrumb">
import { reactive, computed, onMounted, watch } from 'vue';
import { onBeforeRouteUpdate, useRoute, useRouter, RouteRecordRaw } from 'vue-router';
import { Local } from '@/utils/storage';
import other from '@/utils/other';
import { storeToRefs } from 'pinia';
import { useThemeConfig } from '@/stores/themeConfig';
import { useRoutesList } from '@/stores/routesList';

// 定义变量内容
const stores = useRoutesList();
const storesThemeConfig = useThemeConfig();
const { themeConfig } = storeToRefs(storesThemeConfig);
const { routesList } = storeToRefs(stores);
const route = useRoute();
const router = useRouter();
const state = reactive<BreadcrumbState>({
	breadcrumbList: [],
});

// 动态设置经典、横向布局不显示
const isShowBreadcrumb = computed(() => {
	const { layout, isBreadcrumb } = themeConfig.value;
	if (layout === 'classic' || layout === 'transverse') return false;
	else return isBreadcrumb ? true : false;
});

// 面包屑点击时
const onBreadcrumbClick = (v: RouteItem) => {
	const { redirect, path } = v;
	if (redirect) router.push(redirect);
	else router.push(path);
};

// 展开/收起左侧菜单点击
const onThemeConfigChange = () => {
	themeConfig.value.isCollapse = !themeConfig.value.isCollapse;
	setLocalThemeConfig();
};

// 存储布局配置
const setLocalThemeConfig = () => {
	Local.remove('themeConfig');
	Local.set('themeConfig', themeConfig.value);
};

// --- 全新的面包屑生成逻辑 ---

// 递归函数：根据 code 在路由树中查找对应的路由对象
const findRouteByCode = (code: string, routeItems: RouteItems): RouteItem | null => {
    for (const item of routeItems) {
        if (item.meta?.code === code) {
            return item;
        }
        if (item.children) {
            const found = findRouteByCode(code, item.children);
            if (found) return found;
        }
    }
    return null;
};

// 主函数：设置面包屑列表
const setBreadcrumbList = (currentRoute: typeof route) => {
    // 1. 获取当前路由的 code
    const currentCode = currentRoute.meta?.code as string;
    if (!currentCode) {
        state.breadcrumbList = [];
        return;
    }

    // 2. 在完整的路由列表 (routesList) 中找到当前路由的对象
    const currentRouteItem = findRouteByCode(currentCode, routesList.value);
    if (!currentRouteItem) {
        state.breadcrumbList = [];
        return;
    }

    const breadcrumbs: RouteItems = [];
    let current: RouteItem | null = currentRouteItem;

    // 3. 循环向上查找父级，直到根节点
    while (current) {
        breadcrumbs.unshift(current);
        const parentCode = current.meta?.parentCode as string;
        if (parentCode) {
            current = findRouteByCode(parentCode, routesList.value);
        } else {
            current = null;
        }
    }
    state.breadcrumbList = breadcrumbs;

    // 4. (可选) 如果您希望面包屑不包含最顶级的 "首页"，可以取消下面的注释
    // if (state.breadcrumbList.length > 0 && state.breadcrumbList[0].name === 'home') {
    //     state.breadcrumbList.shift();
    // }
};

// 监听路由变化，重新生成面包屑
watch(
    () => route.path,
    () => {
        setBreadcrumbList(route);
    },
    { immediate: true, deep: true }
);

</script>

<style scoped lang="scss">
.layout-navbars-breadcrumb {
	flex: 1;
	height: inherit;
	display: flex;
	align-items: center;
	.layout-navbars-breadcrumb-icon {
		cursor: pointer;
		font-size: 18px;
		color: var(--next-bg-topBarColor);
		height: 100%;
		width: 40px;
		opacity: 0.8;
		&:hover {
			opacity: 1;
		}
	}
	.layout-navbars-breadcrumb-span {
		display: flex;
		opacity: 0.7;
		color: var(--next-bg-topBarColor);
	}
	.layout-navbars-breadcrumb-iconfont {
		font-size: 14px;
		margin-right: 5px;
	}
	:deep(.el-breadcrumb__separator) {
		opacity: 0.7;
		color: var(--next-bg-topBarColor);
	}
	:deep(.el-breadcrumb__inner a, .el-breadcrumb__inner.is-link) {
		font-weight: unset !important;
		color: var(--next-bg-topBarColor);
		&:hover {
			color: var(--el-color-primary) !important;
		}
	}
}
</style>
