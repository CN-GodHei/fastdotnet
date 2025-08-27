<template>
	<el-form size="large" class="login-content-form">
		<el-form-item class="login-animation1">
			<el-input text :placeholder="$t('message.account.accountPlaceholder1')" v-model="state.ruleForm.userName" clearable autocomplete="off">
				<template #prefix>
					<el-icon class="el-input__icon"><ele-User /></el-icon>
				</template>
			</el-input>
		</el-form-item>
		<el-form-item class="login-animation2">
			<el-input
				:type="state.isShowPassword ? 'text' : 'password'"
				:placeholder="$t('message.account.accountPlaceholder2')"
				v-model="state.ruleForm.password"
				autocomplete="off"
			>
				<template #prefix>
					<el-icon class="el-input__icon"><ele-Unlock /></el-icon>
				</template>
				<template #suffix>
					<i
						class="iconfont el-input__icon login-content-password"
						:class="state.isShowPassword ? 'icon-yincangmima' : 'icon-xianshimima'"
						@click="state.isShowPassword = !state.isShowPassword"
					>
					</i>
				</template>
			</el-input>
		</el-form-item>
		<el-form-item class="login-animation3">
			<el-col :span="15">
				<el-input
					text
					maxlength="4"
					:placeholder="$t('message.account.accountPlaceholder3')"
					v-model="state.ruleForm.code"
					clearable
					autocomplete="off"
				>
					<template #prefix>
						<el-icon class="el-input__icon"><ele-Position /></el-icon>
					</template>
				</el-input>
			</el-col>
			<el-col :span="1"></el-col>
			<el-col :span="8">
				<el-button class="login-content-code" v-waves>1234</el-button>
			</el-col>
		</el-form-item>
		<el-form-item class="login-animation4">
			<el-button type="primary" class="login-content-submit" round v-waves @click="onSignIn" :loading="state.loading.signIn">
				<span>{{ $t('message.account.accountBtnText') }}</span>
			</el-button>
		</el-form-item>
	</el-form>
</template>

<script setup lang="ts" name="loginAccount">
import { reactive, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { ElMessage } from 'element-plus';
import { useI18n } from 'vue-i18n';
import Cookies from 'js-cookie';
import { storeToRefs } from 'pinia';
import { useThemeConfig } from '/@/stores/themeConfig';
import { initFrontEndControlRoutes } from '/@/router/frontEnd';
import { initBackEndControlRoutes } from '/@/router/backEnd';
import { Session } from '/@/utils/storage';
import { formatAxis } from '/@/utils/formatTime';
import { NextLoading } from '/@/utils/loading';
import { useAuthApi } from '/@/api/auth/index'; // 引入适配的登录 API
import { startQiankun } from '/@/main'; // 导入 startQiankun 函数

// 定义变量内容
const { t } = useI18n();
const storesThemeConfig = useThemeConfig();
const { themeConfig } = storeToRefs(storesThemeConfig);
const route = useRoute();
const router = useRouter();
const authApi = useAuthApi(); // 实例化登录 API

const state = reactive({
	isShowPassword: false,
	ruleForm: {
		userName: 'superadmin', // 默认用户名
		password: '123456', // 默认密码
		code: '1234', // 默认验证码
	},
	loading: {
		signIn: false,
	},
});

// 时间获取
const currentTime = computed(() => {
	return formatAxis(new Date());
});
// 登录 (适配 Fastdotnet 后端)
const onSignIn = async () => {
	state.loading.signIn = true;
	try {
		// 1. 调用后端登录接口
		const res = await authApi.adminLogin({
			Username: state.ruleForm.userName,
			Password: state.ruleForm.password,
		});

		// 2. 检查响应并存储 token
		// 由于 request.ts 响应拦截器已修改为直接返回 res.Data,
		// 这里的 res 就是 login API 返回的 Data 部分，即 { Token: "..." }
		if (res && res.Token) {
			const token = res.Token;
			Session.set('token', token); // 存储从后端获取的真实 token 到 Cookies
			// 添加调试日志
			console.log('Login Success - Token Set to Cookie:', token);
			// 存储用户名用于模拟用户信息获取（实际项目中可能由后端返回或通过 token 解析）
			Cookies.set('userName', state.ruleForm.userName);

			// 3. 初始化路由权限
			if (!themeConfig.value.isRequestRoutes) {
				// 前端控制路由，2、请注意执行顺序
				const isNoPower = await initFrontEndControlRoutes();
				signInSuccess(isNoPower);
			} else {
				// 后端控制路由，isRequestRoutes 为 true，则开启后端控制路由
				// 添加完动态路由，再进行 router 跳转，否则可能报错 No match found for location with path "/"
				const isNoPower = await initBackEndControlRoutes();
				// 执行完 initBackEndControlRoutes，再执行 signInSuccess
				signInSuccess(isNoPower);
			}
		} else {
			// 如果 res 存在但没有 Token，或者 res 本身就是 undefined (由 request.ts 的错误处理部分 reject 导致这里 catch)
			// 错误处理已在 request.ts 的响应拦截器中通过 ElMessage 显示，这里可以简单提示或不做额外处理
			// 除非你需要特殊的登录错误提示，否则可以省略这个 else 块
			// 为了保持逻辑完整，这里保留一个通用提示
			ElMessage.error('登录失败：响应数据异常');
			state.loading.signIn = false;
		}
	} catch (error: any) {
		// 4. 处理登录错误
		console.error('登录请求失败:', error);
		ElMessage.error(error.message || '登录请求失败，请检查网络或联系管理员');
		state.loading.signIn = false;
	}
};
// 登录成功后的跳转
const signInSuccess = (isNoPower: boolean | undefined) => {
	if (isNoPower) {
		ElMessage.warning('抱歉，您没有登录权限');
		Session.clear();
	} else {
		// 初始化登录成功时间问候语
		let currentTimeInfo = currentTime.value;
		// 登录成功，跳到转首页
		// 如果是复制粘贴的路径，非首页/登录页，那么登录成功后重定向到对应的路径中
		if (route.query?.redirect) {
			router.push({
				path: <string>route.query?.redirect,
				query: Object.keys(<string>route.query?.params).length > 0 ? JSON.parse(<string>route.query?.params) : '',
			});
		} else {
			router.push('/');
		}
		// 登录成功提示
		const signInText = t('message.signInText');
		ElMessage.success(`${currentTimeInfo}，${signInText}`);
		// 添加 loading，防止第一次进入界面时出现短暂空白
		NextLoading.start();
		
		// 启动 qiankun
		startQiankun();
	}
	state.loading.signIn = false;
};
</script>

<style scoped lang="scss">
.login-content-form {
	margin-top: 20px;
	@for $i from 1 through 4 {
		.login-animation#{$i} {
			opacity: 0;
			animation-name: error-num;
			animation-duration: 0.5s;
			animation-fill-mode: forwards;
			animation-delay: calc($i/10) + s;
		}
	}
	.login-content-password {
		display: inline-block;
		width: 20px;
		cursor: pointer;
		&:hover {
			color: #909399;
		}
	}
	.login-content-code {
		width: 100%;
		padding: 0;
		font-weight: bold;
		letter-spacing: 5px;
	}
	.login-content-submit {
		width: 100%;
		letter-spacing: 2px;
		font-weight: 300;
		margin-top: 15px;
	}
}
</style>
