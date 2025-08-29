export default [
    {
        schemaPath: 'http://localhost:18889/swagger/plugin-11375910391972869/swagger.json',
        serversPath: './src/api',
        projectName: 'plugin-a',
        namespace: "APIModel",
        enumStyle: "enum",
        // --- 修改：使用 requestLibPath 指向你的适配器函数文件（默认导出） ---
        // 路径是相对于生成的 API 文件所在目录 (src/api/...) 的。
        requestLibPath: '../../utils/fdRequestAdapter',
        // --- 修改结束 ---
        isCamelCase: false,
    }
];