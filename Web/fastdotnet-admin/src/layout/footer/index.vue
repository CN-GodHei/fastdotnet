<template>
	<div class="layout-footer pb15">
		<div class="layout-footer-warp">
			<div>{{ copyrightText }}</div>
			<div class="mt5"></div>
		</div>
	</div>
</template>

<script setup lang="ts" name="layoutFooter">
import { ref, onMounted, watch } from 'vue';
import { useThemeConfig } from '@/stores/themeConfig';

// 创建响应式变量来存储版权信息
const copyrightText = ref('fastdotnet © 2025 ');

// 获取主题配置 store
const themeConfigStore = useThemeConfig();

// 更新版权信息的函数
const updateCopyright = () => {
	const copyrightInfo = themeConfigStore.getConfigValue('CopyrightInfo');
	if (copyrightInfo) {
		copyrightText.value = copyrightInfo;
	} else {
		// 如果没有从后端获取到版权信息，使用默认值
		copyrightText.value = 'fastdotnet © 2025 ';
	}
};

// 组件挂载时更新版权信息
onMounted(() => {
	updateCopyright();
});

// 监听配置变化，当配置更新时也更新版权信息
watch(
	() => themeConfigStore.themeConfig.additionalConfig,
	() => {
		updateCopyright();
	},
	{ deep: true }
);
</script>

<style scoped lang="scss">
.layout-footer {
	width: 100%;
	display: flex;
	&-warp {
		margin: auto;
		color: var(--el-text-color-secondary);
		text-align: center;
		animation: error-num 0.3s ease;
	}
}
</style>
