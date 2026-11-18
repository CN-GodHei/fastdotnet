export default [
  {
    // 插件启用后才会出现该 Swagger 分组；端口按你的宿主实际地址改
    schemaPath: 'http://localhost:18889/swagger/plugin-__PLUGIN_ID__-admin/swagger.json',
    serversPath: './src/api',
    projectName: '__PLUGIN_NAME__',
    namespace: 'APIModel',
    enumStyle: 'enum',
    requestLibPath: '../../utils/fdRequestAdapter',
    isCamelCase: true
  }
]
