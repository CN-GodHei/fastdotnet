export default [
    {
  schemaPath: 'http://localhost:18889/swagger/main/swagger.json',
  serversPath: './src/api',
  projectName:'admin-base',//项目名称
  namespace:"APIModel",//默认API
  enumStyle:"enum",//枚举样式	string-literal | enum
  requestImportStatement:"import request from '/@/utils/request'"
}
]