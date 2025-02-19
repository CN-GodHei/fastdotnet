# Plugin 目录说明

## 目录结构
```
Plugin/
├── Fastdotnet.Plugin/    # 基础插件项目
├── Fastdotnet.A/         # A插件项目
├── Fastdotnet.B/         # B插件项目
└── Fastdotnet.Test/      # 测试插件项目
```

## 说明
- 此目录用于统一管理所有Fastdotnet框架的插件项目
- 每个插件项目都是独立的类库项目
- 插件编译后的输出将存放在各自的目录中
- 插件需要实现Fastdotnet.Core中定义的插件接口

## 插件开发规范
1. 插件项目命名规范：Fastdotnet.{PluginName}
2. 每个插件项目需要实现IPlugin接口
3. 插件的编译输出路径配置为Plugin目录下对应的子文件夹
4. 插件之间应保持独立，避免相互依赖