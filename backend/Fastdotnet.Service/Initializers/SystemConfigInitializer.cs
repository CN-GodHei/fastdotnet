using System.Collections.Generic;
using System.Threading.Tasks;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Initializers;
using Fastdotnet.Core.IService;
using Microsoft.Extensions.Logging;

namespace Fastdotnet.Service.Initializers
{
    public class SystemConfigInitializer : IApplicationInitializer
    {
        private readonly IRepository<SystemInfoConfig> _systemConfigRepository;
        private readonly ILogger<SystemConfigInitializer> _logger;

        public SystemConfigInitializer(IRepository<SystemInfoConfig> systemConfigRepository, ILogger<SystemConfigInitializer> logger)
        {
            _systemConfigRepository = systemConfigRepository;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            _logger.LogInformation("Start: Initializing System Config...");

            if (await _systemConfigRepository.ExistsAsync(a => a.Id != null))
            {
                _logger.LogInformation("System config already seeded. Skipping initialization.");
                return;
            }

            //var configs = new List<SystemInfoConfig>
            //{
            //    new SystemInfoConfig { Name = "App版本号", Code = "AppVersion", Value = "1.0.0", Description = "当前应用版本号", IsSystem = true },
            //    new SystemInfoConfig { Name = "水印内容", Code = "Watermark", Value = "Fastdotnet", Description = "系统水印内容。在“启用水印”开启后生效。", IsSystem = true },
            //    new SystemInfoConfig { Name = "启用验证码", Code = "EnableCaptcha", Value = false, Description = "控制登录、注册等功能是否开启图片或行为验证码", IsSystem = true },
            //    new SystemInfoConfig { Name = "验证码类型", Code = "CaptchaType", Value = "normal", Description = "验证码类型，可选值：normal (图形验证码), behavioral (行为验证)", IsSystem = true },
            //    new SystemInfoConfig { Name = "注册邮箱验证", Code = "EnableRegisterEmailVerification", Value = false, Description = "控制用户注册时是否必须通过邮箱验证", IsSystem = true },

            //    // 新增配置项
            //    new SystemInfoConfig { Name = "系统名称", Code = "SystemName", Value = "Fastdotnet", Description = "显示在浏览器标签和登录页的系统名称", IsSystem = true },
            //    new SystemInfoConfig { Name = "系统Logo", Code = "SystemLogo", Value = "", Description = "显示在登录页和侧边栏顶部的Logo图片URL", IsSystem = true },
            //    new SystemInfoConfig { Name = "启用水印", Code = "EnableWatermark", Value = true, Description = "是否在系统页面上显示水印", IsSystem = true },
            //    new SystemInfoConfig { Name = "版权信息", Code = "CopyrightInfo", Value = $"Copyright © 2025 Fastdotnet. All Rights Reserved.", Description = "显示在登录页和布局页脚的版权信息", IsSystem = true }
            //};
            var configs = new List<SystemInfoConfig>
{
                new SystemInfoConfig { Name = "版权信息", Code = "CopyrightInfo", Value = "© 2025 Fastdotnet 开源系统 · 商业插件需授权", Description="版权信息"},
                new SystemInfoConfig { Name = "App版本号", Code = "AppVersion", Value = "1.0.0", Description = "当前应用版本号", IsSystem = true },
                new SystemInfoConfig { Name = "启用验证码", Code = "EnableCaptcha", Value = false, Description = "控制登录、注册等功能是否开启图片或行为验证码", IsSystem = true },
                new SystemInfoConfig { Name = "验证码类型", Code = "CaptchaType", Value = "normal", Description = "验证码类型，可选值：normal (图形验证码), behavioral (行为验证:暂未实现)", IsSystem = true },
                new SystemInfoConfig { Name = "注册邮箱验证", Code = "EnableRegisterEmailVerification", Value = false, Description = "控制用户注册时是否必须通过邮箱验证", IsSystem = true },

    // ========== 布局配置抽屉 ==========
    new SystemInfoConfig { Name = "是否开启布局配置抽屉", Code = "isDrawer", Value = false, Description = "控制是否显示主题配置抽屉面板", IsSystem = true },

    // ========== 全局主题 ==========
    new SystemInfoConfig { Name = "默认 primary 主题颜色", Code = "primary", Value = "#0F59A4", Description = "系统主色调，影响按钮、选中态等UI元素", IsSystem = true },
    new SystemInfoConfig { Name = "是否开启深色模式", Code = "isIsDark", Value = false, Description = "启用深色主题模式", IsSystem = true },

    // ========== 顶栏设置 ==========
    new SystemInfoConfig { Name = "默认顶栏导航背景颜色", Code = "topBar", Value = "#ffffff", Description = "顶部导航栏背景色", IsSystem = true },
    new SystemInfoConfig { Name = "默认顶栏导航字体颜色", Code = "topBarColor", Value = "#606266", Description = "顶部导航栏文字颜色", IsSystem = true },
    new SystemInfoConfig { Name = "是否开启顶栏背景颜色渐变", Code = "isTopBarColorGradual", Value = false, Description = "顶栏背景是否使用渐变色", IsSystem = true },

    // ========== 菜单设置 ==========
    new SystemInfoConfig { Name = "默认菜单导航背景颜色", Code = "menuBar", Value = "#545c64", Description = "侧边菜单栏背景色", IsSystem = true },
    new SystemInfoConfig { Name = "默认菜单导航字体颜色", Code = "menuBarColor", Value = "#eaeaea", Description = "侧边菜单栏文字颜色", IsSystem = true },
    new SystemInfoConfig { Name = "默认菜单高亮背景色", Code = "menuBarActiveColor", Value = "rgba(0, 0, 0, 0.2)", Description = "菜单项激活/选中时的背景色", IsSystem = true },
    new SystemInfoConfig { Name = "是否开启菜单背景颜色渐变", Code = "isMenuBarColorGradual", Value = false, Description = "菜单栏背景是否使用渐变色", IsSystem = true },

    // ========== 分栏设置 ==========
    new SystemInfoConfig { Name = "默认分栏菜单背景颜色", Code = "columnsMenuBar", Value = "#545c64", Description = "分栏布局下菜单背景色", IsSystem = true },
    new SystemInfoConfig { Name = "默认分栏菜单字体颜色", Code = "columnsMenuBarColor", Value = "#e6e6e6", Description = "分栏布局下菜单文字颜色", IsSystem = true },
    new SystemInfoConfig { Name = "是否开启分栏菜单背景颜色渐变", Code = "isColumnsMenuBarColorGradual", Value = false, Description = "分栏菜单背景是否使用渐变色", IsSystem = true },
    new SystemInfoConfig { Name = "是否开启分栏菜单鼠标悬停预加载", Code = "isColumnsMenuHoverPreload", Value = false, Description = "分栏菜单悬停时是否预加载子菜单", IsSystem = true },

    // ========== 界面设置 ==========
    new SystemInfoConfig { Name = "是否开启菜单水平折叠效果", Code = "isCollapse", Value = false, Description = "菜单是否默认折叠", IsSystem = true },
    new SystemInfoConfig { Name = "是否开启菜单手风琴效果", Code = "isUniqueOpened", Value = true, Description = "是否只允许同时展开一个菜单项", IsSystem = true },
    new SystemInfoConfig { Name = "是否开启固定 Header", Code = "isFixedHeader", Value = true, Description = "页面滚动时是否固定顶部导航栏", IsSystem = true },
    new SystemInfoConfig { Name = "初始化变量（勿删）", Code = "isFixedHeaderChange", Value = false, Description = "用于更新菜单 el-scrollbar 高度，请勿删除", IsSystem = true },
    new SystemInfoConfig { Name = "是否开启经典布局分割菜单", Code = "isClassicSplitMenu", Value = false, Description = "仅经典布局生效，是否分割菜单显示", IsSystem = true },
    new SystemInfoConfig { Name = "是否开启自动锁屏", Code = "isLockScreen", Value = true, Description = "用户无操作时是否自动锁屏", IsSystem = true },
    new SystemInfoConfig { Name = "自动锁屏倒计时(秒)", Code = "lockScreenTime", Value = 300, Description = "自动锁屏等待时间，单位：秒", IsSystem = true },

    // ========== 界面显示 ==========
    new SystemInfoConfig { Name = "是否开启侧边栏 Logo", Code = "isShowLogo", Value = false, Description = "侧边栏顶部是否显示 Logo", IsSystem = true },
    new SystemInfoConfig { Name = "初始化变量（勿删）", Code = "isShowLogoChange", Value = false, Description = "用于 el-scrollbar 高度更新，请勿删除", IsSystem = true },
    new SystemInfoConfig { Name = "是否开启 Breadcrumb", Code = "isBreadcrumb", Value = true, Description = "是否显示顶部面包屑导航", IsSystem = true },
    new SystemInfoConfig { Name = "是否开启 Tagsview", Code = "isTagsview", Value = true, Description = "是否启用顶部标签页导航", IsSystem = true },
    new SystemInfoConfig { Name = "是否开启 Breadcrumb 图标", Code = "isBreadcrumbIcon", Value = false, Description = "面包屑是否显示图标", IsSystem = true },
    new SystemInfoConfig { Name = "是否开启 Tagsview 图标", Code = "isTagsviewIcon", Value = false, Description = "标签页是否显示图标", IsSystem = true },
    new SystemInfoConfig { Name = "是否开启 TagsView 缓存", Code = "isCacheTagsView", Value = true, Description = "标签页是否缓存已打开页面", IsSystem = true },
    new SystemInfoConfig { Name = "是否开启 TagsView 拖拽", Code = "isSortableTagsView", Value = true, Description = "标签页是否支持拖拽排序", IsSystem = true },
    new SystemInfoConfig { Name = "是否开启 TagsView 共用", Code = "isShareTagsView", Value = false, Description = "多标签页是否跨路由共享", IsSystem = true },
    new SystemInfoConfig { Name = "是否开启 Footer 底部版权信息", Code = "isFooter", Value = true, Description = "页面底部是否显示版权信息栏", IsSystem = true },
    new SystemInfoConfig { Name = "是否开启灰色模式", Code = "isGrayscale", Value = false, Description = "是否启用灰度显示模式", IsSystem = true },
    new SystemInfoConfig { Name = "是否开启色弱模式", Code = "isInvert", Value = false, Description = "是否启用色弱辅助模式", IsSystem = true },
    new SystemInfoConfig { Name = "是否开启水印", Code = "isWartermark", Value = true, Description = "是否在页面显示水印", IsSystem = true },
    new SystemInfoConfig { Name = "水印文案", Code = "wartermarkText", Value = "Fastdotnet", Description = "水印显示的文字内容", IsSystem = true },

    // ========== 其它设置 ==========
    new SystemInfoConfig { Name = "Tagsview 风格", Code = "tagsStyle", Value = "tags-style-five", Description = "标签页样式，可选值：tags-style-one / four / five", IsSystem = true },
    new SystemInfoConfig { Name = "主页面切换动画", Code = "animation", Value = "slide-right", Description = "页面切换动画，可选值：slide-right / slide-left / opacitys", IsSystem = true },
    new SystemInfoConfig { Name = "分栏高亮风格", Code = "columnsAsideStyle", Value = "columns-round", Description = "分栏菜单高亮样式，可选值：columns-round / columns-card", IsSystem = true },
    new SystemInfoConfig { Name = "分栏布局风格", Code = "columnsAsideLayout", Value = "columns-vertical", Description = "分栏菜单布局方向，可选值：columns-horizontal / vertical", IsSystem = true },

    // ========== 布局切换 ==========
    new SystemInfoConfig { Name = "布局切换", Code = "layout", Value = "defaults", Description = "系统布局模式，可选值：defaults / classic / transverse / columns", IsSystem = true },

    // ========== 后端控制路由 ==========
    new SystemInfoConfig { Name = "是否开启后端控制路由", Code = "isRequestRoutes", Value = true, Description = "是否由后端返回动态路由菜单结构", IsSystem = true },

    // ========== 全局网站标题 / 副标题 ==========
    new SystemInfoConfig { Name = "网站主标题", Code = "globalTitle", Value = "Fastdotnet", Description = "显示在浏览器标签和菜单导航的主标题", IsSystem = true },
    new SystemInfoConfig { Name = "网站副标题", Code = "globalViceTitle", Value = "Fastdotnet", Description = "登录页顶部显示的副标题", IsSystem = true },
    new SystemInfoConfig { Name = "网站副标题描述", Code = "globalViceTitleMsg", Value = "不是又一个 CRUD 框架，而是.NET生态可扩展的企业级插件平台", Description = "登录页副标题下方的描述文案", IsSystem = true },
    new SystemInfoConfig { Name = "默认初始语言", Code = "globalI18n", Value = "zh-cn", Description = "系统默认语言，可选值：zh-cn / en / zh-tw", IsSystem = true },
    new SystemInfoConfig { Name = "默认全局组件大小", Code = "globalComponentSize", Value = "small", Description = "组件默认尺寸，可选值：large / default / small", IsSystem = true }
};
            await _systemConfigRepository.InsertRangeAsync(configs);

            _logger.LogInformation("Finish: System Config initialization complete.");
        }
    }
}