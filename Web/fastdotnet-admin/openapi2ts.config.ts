export default [
  {
    schemaPath: 'http://localhost:18889/swagger/main-admin/swagger.json',
    serversPath: './src/api',
    projectName: 'fd-system-api-admin',
    namespace: "APIModel",
    enumStyle: "string-literal",
    requestImportStatement: "import request, { encryptRequest } from '@/utils/request'",
    isCamelCase: true,
    noLegacyFunctionNames: false,
    dataFields: ['Data', 'PageInfo', 'Items'],
  },
    {
    schemaPath: 'http://localhost:18889/swagger/main-app/swagger.json',
    serversPath: './src/api',
    projectName: 'fd-system-api-app',
    namespace: "APIModel",
    enumStyle: "string-literal",
    requestImportStatement: "import request, { encryptRequest } from '@/utils/request'",
    isCamelCase: true,
    noLegacyFunctionNames: false,
    dataFields: ['Data', 'PageInfo', 'Items'],
  }
]