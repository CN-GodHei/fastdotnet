<template>
	<div class="social-login-callback">
		<div v-if="loading" class="loading-container">
			<el-icon class="is-loading" :size="50"><Loading /></el-icon>
			<p>登录处理中...</p>
		</div>
		<div v-else-if="error" class="error-container">
			<el-icon :size="50" color="#f56c6c"><CircleClose /></el-icon>
			<p>{{ errorMessage }}</p>
			<el-button type="primary" @click="goToLogin">返回登录页</el-button>
		</div>
		<div v-else class="success-container">
			<el-icon :size="50" color="#67c23a"><SuccessFilled /></el-icon>
			<p>登录成功！正在跳转...</p>
		</div>
	</div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { ElMessage } from 'element-plus';
import { Loading, CircleClose, SuccessFilled } from '@element-plus/icons-vue';
import { useAuthStore } from '@/stores/modules/auth';

const router = useRouter();
const authStore = useAuthStore();

const loading = ref(true);
const error = ref(false);
const errorMessage = ref('');

onMounted(async () => {
	try {
		// 从 URL 参数获取 Token
		const urlParams = new URLSearchParams(window.location.search);
		const token = urlParams.get('token');
		const provider = urlParams.get('provider');

		if (!token) {
			throw new Error('未收到登录令牌');
		}

		console.log(`[SocialLoginCallback] Received token from ${provider}`);

		// 保存 Token 到 store
		authStore.setToken(token);

		// 获取用户信息（可选，根据项目需求）
		// await authStore.getUserInfo();

		ElMessage.success('登录成功！');

		// 延迟跳转，让用户看到成功提示
		setTimeout(() => {
			// 跳转到首页或之前的页面
			const redirect = urlParams.get('redirect') || '/';
			router.push(redirect);
		}, 1000);
	} catch (err: any) {
		console.error('[SocialLoginCallback] Login failed:', err);
		error.value = true;
		errorMessage.value = err.message || '登录失败，请重试';
		ElMessage.error(errorMessage.value);
	} finally {
		loading.value = false;
	}
});

const goToLogin = () => {
	router.push('/login');
};
</script>

<style scoped>
.social-login-callback {
	display: flex;
	justify-content: center;
	align-items: center;
	min-height: 100vh;
	background: #f5f7fa;
}

.loading-container,
.error-container,
.success-container {
	text-align: center;
	padding: 40px;
	background: white;
	border-radius: 12px;
	box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
	max-width: 400px;
	width: 90%;
}

.loading-container p,
.error-container p,
.success-container p {
	margin-top: 20px;
	font-size: 16px;
	color: #606266;
}

.error-container p {
	color: #f56c6c;
}

.success-container p {
	color: #67c23a;
}
</style>
