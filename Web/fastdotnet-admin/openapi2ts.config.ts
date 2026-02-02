export default [
    {
  schemaPath: 'http://localhost:18889/swagger/main-admin/swagger.json',
  serversPath: './src/api',
  projectName:'fd-system-api-admin',//项目名称
  namespace:"APIModel",//默认API
  enumStyle:"string-literal",//枚举样式	string-literal | enum
  requestImportStatement:"import request, { encryptRequest } from '@/utils/request'",
  isCamelCase:false,
  dataFields: ['Data', 'PageInfo', 'Items'], // 添加分页数据字段支持
}
]