import { getSharedFdRequest } from '../main'

/**
 * 把 openapi2ts 生成的 request(url, options) 适配到宿主注入的 axios 实例。
 * 宿主 axios 的响应拦截器已经返回 {Code,Msg,Data} 中的 Data。
 */
export default async function fdRequestAdapter(url: string, options: any = {}) {
  const FdRequest: any = getSharedFdRequest()
  if (!FdRequest) {
    throw new Error('[FdRequestAdapter] 宿主未注入 FdRequest（独立运行或 props 名称不对）')
  }

  const { method = 'GET', params, data, headers, ...rest } = options
  const config: any = { url, method, params, headers, ...rest }
  if (['POST', 'PUT', 'PATCH', 'DELETE'].includes(String(method).toUpperCase())) {
    config.data = data
  }
  return FdRequest(config)
}
