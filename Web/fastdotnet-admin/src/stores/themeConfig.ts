import { defineStore } from 'pinia';
import { getAdminSystemConfigPublicAll } from '/@/api/fd-system-api/SystemConfig';

/**
 * 布局配置
 * 修复：https://gitee.com/lyt-top/vue-next-admin/issues/I567R1，感谢@lanbao123
 * 2020.05.28 by lyt 优化。开发时配置不生效问题
 * 修改配置时：
 * 1、需要每次都清理 `window.localStorage` 浏览器永久缓存
 * 2、或者点击布局配置最底部 `一键恢复默认` 按钮即可看到效果
 */
export const useThemeConfig = defineStore('themeConfig', {
	state: (): ThemeConfigState => ({
		themeConfig: {
			// 是否开启布局配置抽屉
			isDrawer: false,

			/**
			 * 全局主题
			 */
			// 默认 primary 主题颜色
			primary: '#0F59A4',
			// 是否开启深色模式
			isIsDark: false,

			/**
			 * 顶栏设置
			 */
			// 默认顶栏导航背景颜色
			topBar: '#ffffff',
			// 默认顶栏导航字体颜色
			topBarColor: '#606266',
			// 是否开启顶栏背景颜色渐变
			isTopBarColorGradual: false,

			/**
			 * 菜单设置
			 */
			// 默认菜单导航背景颜色
			menuBar: '#545c64',
			// 默认菜单导航字体颜色
			menuBarColor: '#eaeaea',
			// 默认菜单高亮背景色
			menuBarActiveColor: 'rgba(0, 0, 0, 0.2)',
			// 是否开启菜单背景颜色渐变
			isMenuBarColorGradual: false,

			/**
			 * 分栏设置
			 */
			// 默认分栏菜单背景颜色
			columnsMenuBar: '#545c64',
			// 默认分栏菜单字体颜色
			columnsMenuBarColor: '#e6e6e6',
			// 是否开启分栏菜单背景颜色渐变
			isColumnsMenuBarColorGradual: false,
			// 是否开启分栏菜单鼠标悬停预加载(预览菜单)
			isColumnsMenuHoverPreload: false,

			/**
			 * 界面设置
			 */
			// 是否开启菜单水平折叠效果
			isCollapse: false,
			// 是否开启菜单手风琴效果
			isUniqueOpened: true,
			// 是否开启固定 Header
			isFixedHeader: true,
			// 初始化变量，用于更新菜单 el-scrollbar 的高度，请勿删除
			isFixedHeaderChange: false,
			// 是否开启经典布局分割菜单（仅经典布局生效）
			isClassicSplitMenu: false,
			// 是否开启自动锁屏
			isLockScreen: false,
			// 开启自动锁屏倒计时(s/秒)
			lockScreenTime: 30,

			/**
			 * 界面显示
			 */
			// 是否开启侧边栏 Logo
			isShowLogo: false,
			// 初始化变量，用于 el-scrollbar 的高度更新，请勿删除
			isShowLogoChange: false,
			// 是否开启 Breadcrumb，强制经典、横向布局不显示
			isBreadcrumb: true,
			// 是否开启 Tagsview
			isTagsview: true,
			// 是否开启 Breadcrumb 图标
			isBreadcrumbIcon: false,
			// 是否开启 Tagsview 图标
			isTagsviewIcon: false,
			// 是否开启 TagsView 缓存
			isCacheTagsView: false,
			// 是否开启 TagsView 拖拽
			isSortableTagsView: true,
			// 是否开启 TagsView 共用
			isShareTagsView: false,
			// 是否开启 Footer 底部版权信息
			isFooter: true,
			// 是否开启灰色模式
			isGrayscale: false,
			// 是否开启色弱模式
			isInvert: false,
			// 是否开启水印
			isWartermark: true,
			// 水印文案
			wartermarkText: 'fastdotnet',

			/**
			 * 其它设置
			 */
			// Tagsview 风格：可选值"<tags-style-one|tags-style-four|tags-style-five>"，默认 tags-style-five
			// 定义的值与 `/src/layout/navBars/tagsView/tagsView.vue` 中的 class 同名
			tagsStyle: 'tags-style-five',
			// 主页面切换动画：可选值"<slide-right|slide-left|opacitys>\"，默认 slide-right
			animation: 'slide-right',
			// 分栏高亮风格：可选值"<columns-round|columns-card>"，默认 columns-round
			columnsAsideStyle: 'columns-round',
			// 分栏布局风格：可选值"<columns-horizontal|columns-vertical>"，默认 columns-horizontal
			columnsAsideLayout: 'columns-vertical',

			/**
			 * 布局切换
			 * 注意：为了演示，切换布局时，颜色会被还原成默认，代码位置：/@/layout/navBars/topBar/setings.vue
			 * 中的 `initSetLayoutChange(设置布局切换，重置主题样式)` 方法
			 */
			// 布局切换：可选值"<defaults|classic|transverse|columns>"，默认 defaults
			layout: 'defaults',

			/**
			 * 后端控制路由
			 */
			// 是否开启后端控制路由 (适配 Fastdotnet 后端权限)
			isRequestRoutes: true,

			/**
			 * 全局网站标题 / 副标题
			 */
			// 网站主标题（菜单导航、浏览器当前网页标题）
			globalTitle: 'Fastdotnet',
			// 网站副标题（登录页顶部文字）
			globalViceTitle: 'Fastdotnet',
			// 网站副标题（登录页顶部文字）
			globalViceTitleMsg: '专注、免费、开源、维护、解疑、插件化',
			// 默认初始语言，可选值"<zh-cn|en|zh-tw>"，默认 zh-cn
			globalI18n: 'zh-cn',
			// 默认全局组件大小，可选值"<large|'default'|small>"，默认 'large'
			globalComponentSize: 'small',

			// 用于存储后端动态配置的通用字段
			// 这将包含所有后端配置，包括前端未预定义的配置项
			additionalConfig: {},
	},
		
	}),
	actions: {
		setThemeConfig(data: ThemeConfigState) {
			this.themeConfig = data.themeConfig;
		},
		
		// 从后端获取配置并更新主题配置
		async setThemeConfigFromBackend() {
			try {
				const response = await getAdminSystemConfigPublicAll();
				const configData = response;
				
				if (configData) {
					// 创建一个临时对象来扩展当前配置
					const updatedConfig = { ...this.themeConfig };
					
					// 存储前端未定义的配置项
					const additionalConfigs: Record<string, any> = {};
					
					// 遍历所有后端配置项
					for (const [key, value] of Object.entries(configData)) {
						// 对于前端已定义的配置项，直接更新
						if (this.themeConfig.hasOwnProperty(key) && key !== 'additionalConfig') {
							(updatedConfig as any)[key] = value;
						}
						// 特殊处理：如果后端返回了 SystemName，更新 globalTitle 和 globalViceTitle
						else if (key === 'SystemName' && value) {
							updatedConfig.globalTitle = value as string;
							updatedConfig.globalViceTitle = value as string;
						}
						// 如果是前端未定义的配置项，存储到 additionalConfigs
						else if (!this.themeConfig.hasOwnProperty(key)) {
							additionalConfigs[key] = value;
						}
					}
					
					// 只将前端未预定义的配置项存储在 additionalConfig 中
					updatedConfig.additionalConfig = additionalConfigs;
					
					// 更新整个配置对象，这将确保所有配置项（包括 additionalConfig）都会被缓存
					this.themeConfig = updatedConfig;
				}
				return true;
			} catch (error) {
				console.error('获取后端配置失败:', error);
				// 如果获取失败，保持前端的默认值不变
				// 前端将继续使用初始化时的默认值
				return true; // 返回 true 表示处理完成，即使失败也继续
			}
		},
		
		// 获取特定配置项的值
		getConfigValue(key: string) {
			// 优先从主题配置中获取（这些是前端常用的配置）
			if (this.themeConfig.hasOwnProperty(key) && key !== 'additionalConfig') {
				return (this.themeConfig as any)[key];
			}
			// 如果没有找到，尝试从额外配置中获取
			if (this.themeConfig.additionalConfig && this.themeConfig.additionalConfig.hasOwnProperty(key)) {
				return this.themeConfig.additionalConfig[key];
			}
			// 如果都没有找到，返回 undefined
			return undefined;
		},
		
		// 获取所有额外配置
		getAllAdditionalConfigs() {
			return this.themeConfig.additionalConfig || {};
		},
		
		// 获取所有配置（包括前端预定义的和后端额外的）
		getAllConfigs() {
			// 合并前端预定义配置和后端额外配置
			const allConfigs: Record<string, any> = { ...this.themeConfig };
			// 移除 additionalConfig 字段自身，避免重复
			delete allConfigs.additionalConfig;
			
			// 合并额外的配置项
			if (this.themeConfig.additionalConfig) {
				Object.assign(allConfigs, this.themeConfig.additionalConfig);
			}
			
			return allConfigs;
		}
	},
}, {
	// 持久化配置
	persist: {
		key: 'themeConfig',
		storage: localStorage,
		// 持久化整个 themeConfig 对象，包括后端动态添加的配置项
		paths: ['themeConfig'],
	},
});

// 在 store 定义后立即初始化
// 将初始化调用封装到一个函数中，以便在应用启动时调用
export async function initializeThemeConfig() {
	const themeConfigStore = useThemeConfig();
	await themeConfigStore.setThemeConfigFromBackend();
}
