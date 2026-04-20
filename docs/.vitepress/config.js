export default {
  title: "Fastdotnet Docs",
  description: "Fastdotnet 快速开发框架官方文档",
  base: "/",
  
  // 忽略外部链接检查（localhost 等开发环境地址）
  ignoreDeadLinks: true,
  
  themeConfig: {
    nav: [
      { text: '首页', link: '/' },
      { text: '快速开始', link: '/01-快速开始/' },
      { text: '核心概念', link: '/02-核心概念/' },
      { text: '后端开发', link: '/03-后端开发/' },
      { text: '前端开发', link: '/04-前端开发/' },
      { text: '插件开发', link: '/05-插件开发/' },
      { text: '高级主题', link: '/06-高级主题/' },
      { text: 'API 参考', link: '/07-API参考/' }
    ],
    
    sidebar: {
      '/01-快速开始/': [
        {
          text: '快速开始',
          items: [
            { text: '介绍', link: '/01-快速开始/index' },
            { text: '环境准备', link: '/01-快速开始/环境准备' },
            { text: '安装部署', link: '/01-快速开始/安装部署' },
            { text: '第一个应用', link: '/01-快速开始/第一个应用' }
          ]
        }
      ],
      
      '/02-核心概念/': [
        {
          text: '核心概念',
          items: [
            { text: '概览', link: '/02-核心概念/index' },
            { text: '架构设计', link: '/02-核心概念/架构设计' },
            { text: '插件系统', link: '/02-核心概念/插件系统' },
            { text: '权限模型', link: '/02-核心概念/权限模型' }
          ]
        }
      ],
      
      '/03-后端开发/': [
        {
          text: '后端开发',
          items: [
            { text: '概览', link: '/03-后端开发/index' },
            { text: '项目结构', link: '/03-后端开发/项目结构' },
            { text: '实体设计', link: '/03-后端开发/实体设计' },
            { text: '服务层开发', link: '/03-后端开发/服务层开发' },
            { text: 'API 开发', link: '/03-后端开发/API开发' },
            { text: '数据库操作', link: '/03-后端开发/数据库操作' }
          ]
        }
      ],
      
      '/04-前端开发/': [
        {
          text: '前端开发',
          items: [
            { text: '概览', link: '/04-前端开发/index' },
            { text: '项目结构', link: '/04-前端开发/项目结构' },
            { text: '页面开发', link: '/04-前端开发/页面开发' },
            { text: '组件开发', link: '/04-前端开发/组件开发' },
            { text: '微应用集成', link: '/04-前端开发/微应用集成' }
          ]
        }
      ],
      
      '/05-插件开发/': [
        {
          text: '插件开发',
          items: [
            { text: '概览', link: '/05-插件开发/index' },
            { text: '插件架构', link: '/05-插件开发/插件架构' },
            { text: '后端插件', link: '/05-插件开发/后端插件' },
            { text: '前端插件', link: '/05-插件开发/前端插件' },
            { text: '插件发布', link: '/05-插件开发/插件发布' }
          ]
        }
      ],
      
      '/06-高级主题/': [
        {
          text: '高级主题',
          items: [
            { text: '概览', link: '/06-高级主题/index' },
            { text: '性能优化', link: '/06-高级主题/性能优化' },
            { text: '安全最佳实践', link: '/06-高级主题/安全最佳实践' },
            { text: '部署运维', link: '/06-高级主题/部署运维' }
          ]
        }
      ],
      
      '/07-API参考/': [
        {
          text: 'API 参考',
          items: [
            { text: '概览', link: '/07-API参考/index' },
            { text: '接口列表', link: '/07-API参考/接口列表' }
          ]
        }
      ]
    },
    
    // 搜索配置
    search: {
      provider: 'local',
      options: {
        locales: {
          root: {
            translations: {
              button: {
                buttonText: '搜索文档',
                buttonAriaLabel: '搜索文档'
              },
              modal: {
                noResultsText: '无法找到相关结果',
                resetButtonTitle: '清除查询条件',
                footer: {
                  selectText: '选择',
                  navigateText: '切换',
                  closeText: '关闭'
                }
              }
            }
          }
        },
        maxResults: 15,
        detailedView: true
      }
    },
    
    socialLinks: [
      { icon: 'github', link: 'https://github.com/CN-GodHei/fastdotnet.git' },
      { icon: 'gitee', link: 'https://gitee.com/CN-GodHei/fastdotnet.git' },
      { 
        icon: { 
          svg: '<img src="/icons/bilibili.ico" alt="B站" width="20" height="20" style="vertical-align: middle;">'
        }, 
        link: 'https://space.bilibili.com/280446602' 
      },
      { 
        icon: { 
          svg: '<img src="/icons/douyin.ico" alt="抖音" width="20" height="20" style="vertical-align: middle;">'
        }, 
        link: 'https://v.douyin.com/2nIDPl5dvMo' 
      }
    ],
    
    footer: {
      message: 'Released under the MIT License.',
      copyright: 'Copyright © 2024-present Fastdotnet Team'
    }
  }
}
