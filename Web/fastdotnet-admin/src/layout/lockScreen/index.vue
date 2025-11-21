<template>
	<div v-show="state.isShowLockScreen">
		<div class="layout-lock-screen-mask"></div>
		<div class="layout-lock-screen-img" :class="{ 'layout-lock-screen-filter': state.isShowLoockLogin }"></div>
		<div class="layout-lock-screen">
			<div
			class="layout-lock-screen-date"
			ref="layoutLockScreenDateRef"
			@mousedown="onDownPc"
			@mousemove="onMovePc"
			@mouseup="onEnd"
			@touchstart.stop="onDownApp"
			@touchmove.stop="onMoveApp"
			@touchend.stop="onEnd"
			@keydown.enter="onEnterPress"
			tabindex="0"
		>
				<div class="layout-lock-screen-date-box">
					<div class="layout-lock-screen-date-box-time">
						{{ state.time.hm }}<span class="layout-lock-screen-date-box-minutes">{{ state.time.s }}</span>
					</div>
					<div class="layout-lock-screen-date-box-info">{{ state.time.mdq }}</div>
				</div>
				<div class="layout-lock-screen-date-top">
					<SvgIcon name="ele-Top" />
					<div class="layout-lock-screen-date-top-text">上滑解锁</div>
				</div>
			</div>
			<transition name="el-zoom-in-center">
				<div v-show="state.isShowLoockLogin" class="layout-lock-screen-login">
					<div class="layout-lock-screen-login-box">
						<div class="layout-lock-screen-login-box-img">
							<template v-if="userInfos.photo">
								<img :src="userInfos.photo" />
							</template>
							<template v-else>
								<div class="layout-lock-screen-login-box-img-avatar" :style="getAvatarStyle()">
									<span class="avatar-text">{{ getInitials(userInfos.Name) }}</span>
								</div>
							</template>
						</div>
						<div class="layout-lock-screen-login-box-name">{{ userInfos.Name || 'Administrator' }}</div>
						<div class="layout-lock-screen-login-box-value">
							<el-input
								placeholder="请输入密码"
								ref="layoutLockScreenInputRef"
								v-model="state.lockScreenPassword"
								type="password"
								show-password
								@keyup.enter.native.stop="onLockScreenSubmit()"
							>
								<template #append>
									<el-button @click="onLockScreenSubmit">
										<el-icon class="el-input__icon">
											<ele-Right />
										</el-icon>
									</el-button>
								</template>
							</el-input>
							<div v-if="state.lockScreenErrorMsg" class="layout-lock-screen-error-msg">
								{{ state.lockScreenErrorMsg }}
							</div>
						</div>
					</div>
					<div class="layout-lock-screen-login-icon">
						<!-- <SvgIcon name="ele-Microphone" :size="20" />
						<SvgIcon name="ele-AlarmClock" :size="20" /> -->
						<SvgIcon name="ele-SwitchButton" :size="20" />
					</div>
				</div>
			</transition>
		</div>
	</div>
</template>

<script setup lang="ts" name="layoutLockScreen">
import { nextTick, onMounted, reactive, ref, onUnmounted, watch } from 'vue';
import { formatDate } from '/@/utils/formatTime';
import { Local } from '/@/utils/storage';
import { storeToRefs } from 'pinia';
import { useThemeConfig } from '/@/stores/themeConfig';
import { useUserInfo } from '/@/stores/userInfo';
import { postAdminUsersUnlock } from '/@/api/fd-system-api/AdminUsers';
import { ElMessage } from 'element-plus';
import { useRouter, useRoute } from 'vue-router';

// 定义变量内容
const layoutLockScreenDateRef = ref<HtmlType>();
const layoutLockScreenInputRef = ref();
const storesThemeConfig = useThemeConfig();
const storesUserInfo = useUserInfo();
const { themeConfig } = storeToRefs(storesThemeConfig);
const { userInfos } = storeToRefs(storesUserInfo);
const router = useRouter();
const route = useRoute();

// 保存后端原始配置的锁屏时间
const originalLockScreenTime = ref(30); // 在 onMounted 中会更新为实际值

const state = reactive({
	transparency: 1,
	downClientY: 0,
	moveDifference: 0,
	isShowLoockLogin: false,
	isFlags: false,
	querySelectorEl: '' as HtmlType,
	time: {
		hm: '',
		s: '',
		mdq: '',
	},
	setIntervalTime: 0,
	isShowLockScreen: false,
	isShowLockScreenIntervalTime: 0,
	lockScreenPassword: '',
	// 锁屏错误信息
	lockScreenErrorMsg: '',
	// 新增状态：是否正在解锁过程中
	isUnlocking: false,
});

// 鼠标按下 pc
const onDownPc = (down: MouseEvent) => {
	state.isFlags = true;
	state.downClientY = down.clientY;
};
// 鼠标按下 app
const onDownApp = (down: TouchEvent) => {
	state.isFlags = true;
	state.downClientY = down.touches[0].clientY;
};
// 鼠标移动 pc
const onMovePc = (move: MouseEvent) => {
	state.moveDifference = move.clientY - state.downClientY;
	onMove();
};
// 鼠标移动 app
const onMoveApp = (move: TouchEvent) => {
	state.moveDifference = move.touches[0].clientY - state.downClientY;
	onMove();
};
// 鼠标移动事件
const onMove = () => {
	if (state.isFlags) {
		const el = <HTMLElement>state.querySelectorEl;
		const opacitys = (state.transparency -= 1 / 200);
		if (state.moveDifference >= 0) return false;
		el.setAttribute('style', `top:${state.moveDifference}px;cursor:pointer;opacity:${opacitys};`);
		
		// 使用范围判断，而不是精确匹配，提高滑动解锁的可靠性
		if (state.moveDifference <= -el.clientHeight || state.moveDifference < -400) {
			el.setAttribute('style', `top:${-el.clientHeight}px;cursor:pointer;transition:all 0.3s ease;opacity:0;`);
			// 确保记录正确的滑动距离
			state.moveDifference = -el.clientHeight;
			
			// 设置解锁标志，防止其他地方重置锁屏
			state.isUnlocking = true;
			
			// 确保只设置一次密码界面显示状态
			if (!state.isShowLoockLogin) {
				state.isShowLoockLogin = true;
				// 添加短暂延迟确保界面状态已更新
				setTimeout(() => {
					layoutLockScreenInputRef.value?.focus();
				}, 100);
			}
			
			// 不删除元素，而是稍后隐藏它（保持DOM结构完整性）
			setTimeout(() => {
				if (el) {
					el.style.display = 'none';
				}
			}, 300);
		}
	}
};
// 鼠标松开
const onEnd = () => {
	state.isFlags = false;
	state.transparency = 1;
	const el = <HTMLElement>state.querySelectorEl;
	if (state.moveDifference <= -el.clientHeight) {
		// 如果已经滑动完成，则保持解锁状态
		state.isUnlocking = true;
	} else {
		// 如果未完全滑动，则重置状态
		if (el) {
			el.setAttribute('style', `top:0px;opacity:1;transition:all 0.3s ease;`);
		}
		// 只有在未完成解锁时才重置解锁标志
		if (state.moveDifference > -el.clientHeight) {
			state.isUnlocking = false;
		}
	}
};

// 回车键事件处理，直接跳转到密码输入框
const onEnterPress = () => {
	// 直接显示密码输入界面，模拟滑动解锁的效果
	if (!state.isShowLoockLogin) {
		state.isShowLoockLogin = true;
		
		// 设置元素状态，使其显示为已滑动状态
		const el = <HTMLElement>state.querySelectorEl;
		if (el) {
			el.setAttribute('style', `top:${-el.clientHeight}px;cursor:pointer;transition:all 0.3s ease;opacity:0;`);
			
			// 在动画完成后隐藏元素
			setTimeout(() => {
				if (el) {
					el.style.display = 'none';
				}
			}, 300);
		}
		
		// 稍后聚焦到密码输入框
		setTimeout(() => {
			layoutLockScreenInputRef.value.focus();
		}, 350); // 略长于动画时间
	} else {
		// 如果密码输入界面已经显示，则直接聚焦
		layoutLockScreenInputRef.value.focus();
	}
};
// 获取要拖拽的初始元素
const initGetElement = () => {
	nextTick(() => {
		state.querySelectorEl = layoutLockScreenDateRef.value;
	});
};
// 时间初始化
const initTime = () => {
	state.time.hm = formatDate(new Date(), 'HH:MM');
	state.time.s = formatDate(new Date(), 'SS');
	state.time.mdq = formatDate(new Date(), 'mm月dd日，WWW');
};
// 时间初始化定时器
const initSetTime = () => {
	initTime();
	state.setIntervalTime = window.setInterval(() => {
		initTime();
	}, 1000);
};
// 锁屏时间定时器
const initLockScreen = () => {
	// 如果当前在登录页面，则不启动锁屏定时器
	if (isLoginPage()) {
		return;
	}

	// 清除之前的定时器，防止重复创建
	if (state.isShowLockScreenIntervalTime) {
		clearInterval(state.isShowLockScreenIntervalTime);
		state.isShowLockScreenIntervalTime = 0;
	}
	
	// 使用后端配置的原始锁屏时间作为当前倒计时
	let currentCountdown = themeConfig.value.lockScreenTime;
	
	if (themeConfig.value.isLockScreen) {
		// 在启动锁屏定时器时添加事件监听器
		addEventListeners();
		state.isShowLockScreenIntervalTime = window.setInterval(() => {
			if (currentCountdown <= 1) {
				// 锁屏时移除事件监听器，停止重置倒计时
				removeEventListeners();
				state.isShowLockScreen = true;
				// 同步更新全局锁屏状态
				themeConfig.value.lockScreenState = true;
				// 重置滑动状态，确保显示滑动解锁界面（而不是直接显示密码输入框）
				state.isShowLoockLogin = false;
				
				// 重置滑动元素的样式和可见性，确保在下次显示时是可见的
				nextTick(() => {
					const el = <HTMLElement>state.querySelectorEl;
					if (el) {
						// 重置元素样式，确保它可见
						el.style.display = 'block';
						el.style.top = '0px';
						el.style.opacity = '1';
						el.style.transition = '';
					}
				});
				
				setLocalThemeConfig();
				// 清除定时器，停止倒计时
				clearInterval(state.isShowLockScreenIntervalTime);
				return;
			}
			currentCountdown--;
		}, 1000);
	} else {
		clearInterval(state.isShowLockScreenIntervalTime);
	}
};
// 存储布局配置
const setLocalThemeConfig = () => {
	themeConfig.value.isDrawer = false;
	// 保存当前锁屏状态到全局状态
	themeConfig.value.lockScreenState = state.isShowLockScreen;
	Local.set('themeConfig', themeConfig.value);
};

// 重置锁屏倒计时
const resetLockScreenTimer = () => {
	// 只有在非锁屏状态下才重置定时器
	// 如果当前处于锁屏状态，则不重置定时器
	if (themeConfig.value.isLockScreen && !state.isShowLockScreen) {
		// 重新初始化锁屏定时器
		if (state.isShowLockScreenIntervalTime) {
			clearInterval(state.isShowLockScreenIntervalTime);
		}
		initLockScreen();
	}
};

// 处理用户活动，避免在锁屏界面交互时重置锁屏计时器
const handleUserActivity = (event: Event) => {
	// 检查事件目标是否在锁屏界面内
	const target = event.target as HTMLElement;
	const isLockScreenElement = target.closest('.layout-lock-screen') !== null;
	
	// 当正在解锁时，不重置锁屏定时器
	if (state.isUnlocking) {
		return;
	}

	// 只有当事件不是发生在锁屏界面内时，才重置锁屏计时器
	if (!isLockScreenElement) {
		resetLockScreenTimer();
	}
};

// 监听锁屏时间配置变化，更新原始锁屏时间
watch(
	() => themeConfig.value.lockScreenTime,
	(newVal) => {
		originalLockScreenTime.value = newVal;
		// 配置改变时，重置锁屏定时器
		resetLockScreenTimer();
	}
);
// 密码输入点击事件
const onLockScreenSubmit = async () => {
	// 在解锁过程中设置标志
	state.isUnlocking = true;
	
	if (!state.lockScreenPassword) {
		// 密码为空，不执行解锁操作
		return;
	}
	
	try {
		// 调用后端解锁接口
		const response = await postAdminUsersUnlock({
			Password: state.lockScreenPassword
		});
		if (response) {
			// 解锁成功
			// 隐藏锁屏界面
			state.isShowLockScreen = false;
			// 同步更新全局锁屏状态
			themeConfig.value.lockScreenState = false;
			// 重置滑动状态，确保下次锁屏时显示滑动解锁界面
			state.isShowLoockLogin = false;
			state.isUnlocking = false; // 重置解锁标志

			// 重置滑动元素的样式和可见性
			const el = <HTMLElement>state.querySelectorEl;
			if (el) {
				el.style.display = 'block';
				el.style.top = '0px';
				el.style.opacity = '1';
				el.style.transition = '';
			}

			// 保存后端配置值不变，只是重新开始定时器
			setLocalThemeConfig();
			// 清空密码和错误消息
			state.lockScreenPassword = '';
			state.lockScreenErrorMsg = '';

			// 重新初始化锁屏定时器，开始新的倒计时
			if (state.isShowLockScreenIntervalTime) {
				clearInterval(state.isShowLockScreenIntervalTime);
			}
			if (themeConfig.value.isLockScreen) {
				// 在解锁后重新添加事件监听器
				addEventListeners();
				initLockScreen();
			}
		} else {
			// 解锁失败，显示错误提示
			state.lockScreenErrorMsg = '密码错误，请重新输入';
			// 清空密码输入框，让用户可以重新输入
			state.lockScreenPassword = '';
			// 使输入框重新获得焦点，方便用户重新输入
			setTimeout(() => {
				layoutLockScreenInputRef.value?.focus();
			}, 100);
			// 不隐藏锁屏界面，保持在解锁界面，让用户可以重新输入密码
			// 确保全局锁屏状态保持为 true
			themeConfig.value.lockScreenState = true;
		}
	} catch (error: any) {
		// 检查是否为401错误（未授权），如果401则跳转到登录页
		if (error.status === 401) {
			// 清除本地存储的认证信息
			Local.remove('token');
			// 跳转到登录页
			router.push('/login');
		} else {
			ElMessage.error('解锁失败，请重试');
			state.isUnlocking = false; // 重置解锁标志
		}
	}
};
// 检查当前是否在登录页面
const isLoginPage = () => {
	return route.path === '/login';
};

// 页面加载时
onMounted(() => {
	// 如果当前在登录页面，则不启动锁屏功能
	if (isLoginPage()) {
		return;
	}
	
	// 初始化原始锁屏时间，使用后端配置的初始值
	originalLockScreenTime.value = themeConfig.value.lockScreenTime;
	
	initGetElement();
	initSetTime();
	
	// 检查是否应该显示锁屏界面
	// 如果锁屏功能开启且当前是锁屏状态，则直接显示锁屏界面
	if (themeConfig.value.isLockScreen && themeConfig.value.lockScreenState) {
		state.isShowLockScreen = true;
		// 如果是持久化的锁屏状态，也需要重置滑动状态，确保下次显示滑动解锁界面
		state.isShowLoockLogin = false;
		
		// 确保滑动元素是可见的
		nextTick(() => {
			const el = <HTMLElement>state.querySelectorEl;
			if (el) {
				el.style.display = 'block';
				el.style.top = '0px';
				el.style.opacity = '1';
				el.style.transition = '';
			}
		});
	} else if (themeConfig.value.isLockScreen) {
		// 如果锁屏功能开启但当前不是锁屏状态，则启动锁屏定时器
		initLockScreen();
	}
	
	// 添加全局用户活动监听器，用于重置锁屏倒计时
	// 注意：只在用户真正使用系统时才重置锁屏计时器，而不是在与锁屏界面交互时
	addEventListeners();
});

// 监听路由变化
watch(
	() => route.path,
	(newPath) => {
		// 如果切换到登录页面，隐藏锁屏界面
		if (newPath === '/login') {
			state.isShowLockScreen = false;
			// 同步更新全局锁屏状态
			themeConfig.value.lockScreenState = false;
			// 停止锁屏定时器
			if (state.isShowLockScreenIntervalTime) {
				clearInterval(state.isShowLockScreenIntervalTime);
			}
			// 移除事件监听器
			removeEventListeners();
		} else if (!state.isShowLockScreen && themeConfig.value.isLockScreen) {
			// 如果从登录页面返回到其他页面，且锁屏功能已启用，重新启动锁屏定时器
			if (!isLoginPage()) {
				initLockScreen();
				addEventListeners();
			}
		}
	}
);

// 添加全局事件监听器
const addEventListeners = () => {
	// 如果当前在登录页面，则不添加锁屏相关的事件监听器
	if (isLoginPage()) {
		return;
	}
	
	window.addEventListener('keydown', resetLockScreenTimer);
	window.addEventListener('mousedown', handleUserActivity);
	window.addEventListener('mousemove', handleUserActivity);
	window.addEventListener('scroll', handleUserActivity);
	window.addEventListener('touchstart', handleUserActivity);
};

// 移除全局事件监听器
const removeEventListeners = () => {
	window.removeEventListener('keydown', resetLockScreenTimer);
	window.removeEventListener('mousedown', handleUserActivity);
	window.removeEventListener('mousemove', handleUserActivity);
	window.removeEventListener('scroll', handleUserActivity);
	window.removeEventListener('touchstart', handleUserActivity);
};

// 页面卸载时
onUnmounted(() => {
	window.clearInterval(state.setIntervalTime);
	window.clearInterval(state.isShowLockScreenIntervalTime);
	
	// 重置解锁标志
	state.isUnlocking = false;
	
	// 移除全局事件监听器
	removeEventListeners();
	
	// 确保锁屏状态被保存
	if (state.isShowLockScreen) {
		themeConfig.value.lockScreenState = true;
	} else {
		themeConfig.value.lockScreenState = false;
	}
	setLocalThemeConfig();
});

// 获取用户名首字母或首字符
const getInitials = (username: string) => {
	if (!username) return '?';
	// 如果是中文名，取第一个字符；如果是英文名，取首字母
	const firstChar = username.trim().charAt(0).toUpperCase();
	return firstChar || '?';
};

// 根据用户名生成头像背景色
const getAvatarStyle = () => {
	const userName = userInfos.userName;
	if (!userName) {
		// 默认背景色
		return {
			backgroundColor: '#409EFF',
		};
	}
	
	// 基于用户名生成一个简单的哈希值来确定颜色
	let hash = 0;
	for (let i = 0; i < userName.length; i++) {
		hash = userName.charCodeAt(i) + ((hash << 5) - hash);
	}
	
	// 使用哈希值生成色调，确保颜色丰富多样
	const hue = hash % 360;
	return {
		backgroundColor: `hsl(${hue}, 70%, 50%)`,
	};
};
</script>

<style scoped lang="scss">
.layout-lock-screen-fixed {
	position: fixed;
	top: 0;
	left: 0;
	width: 100%;
	height: 100%;
}
.layout-lock-screen-filter {
	filter: blur(1px);
}
.layout-lock-screen-mask {
	background: var(--el-color-white);
	@extend .layout-lock-screen-fixed;
	z-index: 9999990;
}
.layout-lock-screen-img {
	@extend .layout-lock-screen-fixed;
	// background-image: url('');
	background-color: rgb(57, 80, 80);
	background-size: 100% 100%;
	z-index: 9999991;
}
.layout-lock-screen {
	@extend .layout-lock-screen-fixed;
	z-index: 9999992;
	&-date {
		position: absolute;
		left: 0;
		top: 0;
		width: 100%;
		height: 100%;
		color: var(--el-color-white);
		z-index: 9999993;
		user-select: none;
		&-box {
			position: absolute;
			left: 30px;
			bottom: 50px;
			&-time {
				font-size: 100px;
				color: var(--el-color-white);
			}
			&-info {
				font-size: 40px;
				color: var(--el-color-white);
			}
			&-minutes {
				font-size: 16px;
			}
		}
		&-top {
			width: 40px;
			height: 40px;
			line-height: 40px;
			border-radius: 100%;
			border: 1px solid var(--el-border-color-light, #ebeef5);
			background: rgba(255, 255, 255, 0.1);
			color: var(--el-color-white);
			opacity: 0.8;
			position: absolute;
			right: 30px;
			bottom: 50px;
			text-align: center;
			overflow: hidden;
			transition: all 0.3s ease;
			i {
				transition: all 0.3s ease;
			}
			&-text {
				opacity: 0;
				position: absolute;
				top: 150%;
				font-size: 12px;
				color: var(--el-color-white);
				left: 50%;
				line-height: 1.2;
				transform: translate(-50%, -50%);
				transition: all 0.3s ease;
				width: 35px;
			}
			&:hover {
				border: 1px solid rgba(255, 255, 255, 0.5);
				background: rgba(255, 255, 255, 0.2);
				box-shadow: 0 0 12px 0 rgba(255, 255, 255, 0.5);
				color: var(--el-color-white);
				opacity: 1;
				transition: all 0.3s ease;
				i {
					transform: translateY(-40px);
					transition: all 0.3s ease;
				}
				.layout-lock-screen-date-top-text {
					opacity: 1;
					top: 50%;
					transition: all 0.3s ease;
				}
			}
		}
	}
	&-login {
		position: relative;
		z-index: 9999994;
		width: 100%;
		height: 100%;
		left: 0;
		top: 0;
		display: flex;
		flex-direction: column;
		justify-content: center;
		color: var(--el-color-white);
		&-box {
			text-align: center;
			margin: auto;
			&-img {
				width: 180px;
				height: 180px;
				margin: auto;
				img {
					width: 100%;
					height: 100%;
					border-radius: 100%;
				}
			}
			&-name {
				font-size: 26px;
				margin: 15px 0 30px;
			}
		}
		&-icon {
			position: absolute;
			right: 30px;
			bottom: 30px;
			i {
				font-size: 20px;
				margin-left: 15px;
				cursor: pointer;
				opacity: 0.8;
				&:hover {
					opacity: 1;
				}
			}
		}
	}
}
:deep(.el-input-group__append) {
	background: var(--el-color-white);
	padding: 0px 15px;
}
:deep(.el-input__inner) {
	border-right-color: var(--el-border-color-extra-light);
	&:hover {
		border-color: var(--el-border-color-extra-light);
	}
}

.layout-lock-screen-error-msg {
	margin-top: 10px;
	color: #f56c6c; /* Element Plus error color */
	text-align: center;
	font-size: 14px;
}

/* 自定义头像样式 */
.layout-lock-screen-login-box-img .layout-lock-screen-login-box-img-avatar {
	border-radius: 100%;
	width: 100%;
	height: 100%;
	display: flex;
	align-items: center;
	justify-content: center;
	color: white;
	font-size: 64px; /* 适合 180px 容器的大字号 */
	line-height: 1;
}
</style>
