export default [
    {
  schemaPath: 'http://localhost:18889/swagger/main-app/swagger.json',
  serversPath: './src/api',
  projectName:'fd-system-api-app',//项目名称
  namespace:"APIModel",//默认API
  enumStyle:"enum",//枚举样式	string-literal | enum
  requestImportStatement:"import request, { encryptRequest } from '@/utils/request'",
  isCamelCase:true,
  noLegacyFunctionNames:false, // 过渡期兼容旧函数名
  dataFields: ['Data', 'PageInfo', 'Items'], // 添加分页数据字段支持
}
]