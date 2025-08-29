// D:\WorkSpace\Code\gitee\fd\Web\plugin-a-admin\src\utils\fdRequestAdapter.ts
// 1. 修正导入路径，使用相对路径
import { getSharedFdRequest } from '../main'; // 导入获取共享实例的函数

/**
 * 适配器函数，用于连接 openapi2ts 生成的代码和主应用共享的 FdRequest (Axios) 实例。
 * 此函数需要兼容 openapi2ts 期望的 `request(url, options)` 签名。
 *
 * @param url 请求的 URL (不包含 baseURL)
 * @param options 请求的配置选项，通常包含 method, params, data, headers 等
 * @returns Promise<any> 返回由主应用拦截器处理后的最终数据 (即 {Data, Code, Msg} 中的 Data)。
 */
async function fdRequestAdapter(url: string, options: any = {}) {
  // 2. 使用 any 类型来避免对 axios 包的直接类型依赖
  const FdRequest: any = getSharedFdRequest();

  if (!FdRequest) {
    const errorMsg = '[FdRequestAdapter] Shared FdRequest instance is not available.';
    console.error(errorMsg);
    return Promise.reject(new Error(errorMsg));
  }

  const {
    method = 'GET',
    params,
    data,
    headers,
    ...restOptions
  } = options;

  const config: any = {
    url,
    method,
    params,
    headers,
    ...restOptions,
  };

  // 处理请求体数据 (data)
  // 对于有请求体的方法 (POST, PUT, PATCH, DELETE)，将 data 放入 config.data
  if (['POST', 'PUT', 'PATCH', 'DELETE'].includes(method.toUpperCase())) {
    config.data = data;
  }
  // 对于 GET/HEAD 等方法，data 通常不用，参数应放在 params 中。

  try {
    // 3. 直接调用 FdRequest 实例 (它是一个 AxiosInstance)
    // 你的主应用拦截器会处理 {Data: ..., Code: ..., Msg: ...} 格式，并返回 Data 部分。
    const response = await FdRequest(config);
    return response; // 直接返回由拦截器处理后的 Data
  } catch (error) {
    console.error(`[FdRequestAdapter] Request failed for ${method} ${url}:`, error);
    throw error;
  }
}

// 4. 默认导出适配器函数，以匹配 openapi2ts 的 requestLibPath 配置
export default fdRequestAdapter;