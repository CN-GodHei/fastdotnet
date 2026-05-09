export default [
  {
    schemaPath: 'http://localhost:18889/swagger/main-admin/swagger.json',
    serversPath: './src/api',
    projectName: 'fd-system-api-admin',
    namespace: "APIModel",
    enumStyle: "string-literal",
    requestImportStatement: "import request, { encryptRequest } from '@/utils/request'",
    isCamelCase: true,
    dataFields: ['Data', 'PageInfo', 'Items'],
  }
]