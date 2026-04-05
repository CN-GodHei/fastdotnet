<template>
	<div class="error layout-padding">
		<div class="layout-padding-auto layout-padding-view">
			<div class="error-flex">
				<div class="left">
					<div class="left-item">
						<div class="left-item-animation left-item-num">404</div>
						<div class="left-item-animation left-item-title">{{ $t('message.notFound.foundTitle') }}</div>
						<div class="left-item-animation left-item-msg">{{ $t('message.notFound.foundMsg') }}</div>
						<div class="left-item-animation left-item-btn">
							<el-button type="primary" size="default" round @click="onGoHome">{{ $t('message.notFound.foundBtn') }}</el-button>
						</div>
					</div>
				</div>
				<div class="right">
					<svg viewBox="0 0 800 600" xmlns="http://www.w3.org/2000/svg" class="error-svg">
						<!-- 背景装饰 -->
						<circle cx="400" cy="300" r="250" fill="var(--el-color-info-light-9)" opacity="0.3"/>
						<circle cx="400" cy="300" r="200" fill="var(--el-color-info-light-8)" opacity="0.4"/>
						
						<!-- 404 数字 -->
						<text x="400" y="280" text-anchor="middle" font-size="120" font-weight="bold" 
						      fill="var(--el-color-info)" opacity="0.8">404</text>
						
						<!-- 装饰线条 -->
						<line x1="250" y1="320" x2="550" y2="320" stroke="var(--el-color-info)" 
						      stroke-width="3" opacity="0.5" stroke-linecap="round"/>
						
						<!-- 小图标 - 放大镜 -->
						<g transform="translate(370, 350)">
							<circle cx="30" cy="30" r="20" fill="none" stroke="var(--el-color-info)" 
							      stroke-width="4" opacity="0.7"/>
							<line x1="45" y1="45" x2="60" y2="60" stroke="var(--el-color-info)" 
							      stroke-width="4" stroke-linecap="round" opacity="0.7"/>
						</g>
						
						<!-- 装饰点 -->
						<circle cx="200" cy="200" r="8" fill="var(--el-color-info)" opacity="0.4"/>
						<circle cx="600" cy="200" r="6" fill="var(--el-color-info)" opacity="0.3"/>
						<circle cx="200" cy="400" r="6" fill="var(--el-color-info)" opacity="0.3"/>
						<circle cx="600" cy="400" r="8" fill="var(--el-color-info)" opacity="0.4"/>
					</svg>
				</div>
			</div>
		</div>
	</div>
</template>

<script setup lang="ts" name="notFound">
import { useRouter } from 'vue-router';

// 定义变量内容
const router = useRouter();

// 返回首页
const onGoHome = () => {
	router.push('/');
};
</script>

<style scoped lang="scss">
.error {
	height: 100%;
	.error-flex {
		margin: auto;
		display: flex;
		height: 350px;
		width: 900px;
		.left {
			flex: 1;
			height: 100%;
			align-items: center;
			display: flex;
			.left-item {
				.left-item-animation {
					opacity: 0;
					animation-name: error-num;
					animation-duration: 0.5s;
					animation-fill-mode: forwards;
				}
				.left-item-num {
					color: var(--el-color-info);
					font-size: 55px;
				}
				.left-item-title {
					font-size: 20px;
					color: var(--el-text-color-primary);
					margin: 15px 0 5px 0;
					animation-delay: 0.1s;
				}
				.left-item-msg {
					color: var(--el-text-color-secondary);
					font-size: 12px;
					margin-bottom: 30px;
					animation-delay: 0.2s;
				}
				.left-item-btn {
					animation-delay: 0.2s;
				}
			}
		}
		.right {
			flex: 1;
			opacity: 0;
			animation-name: error-img;
			animation-duration: 2s;
			animation-fill-mode: forwards;
			.error-svg {
				width: 100%;
				height: 100%;
				display: block;
			}
		}
	}
}
</style>
