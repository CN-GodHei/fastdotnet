import { getSharedFdRequest } from '../main'; // 导入获取共享实例的函数

// 定义一个函数来发送请求
export async function fetchData() {
    const FdRequest = getSharedFdRequest();
    if (!FdRequest) {
         console.error('[PluginA] Shared FdRequest Axios instance is not available.');
         // 可以选择抛出错误或使用插件自己的默认实例（如果有的话）
         // throw new Error('Shared FdRequest Axios instance is not available.');
         return Promise.reject(new Error('Shared FdRequest Axios instance is not available.'));
    }

    try {
        // 使用共享的 FdRequest Axios 实例发送请求
        const response = await FdRequest.get('/api/some-endpoint');
        return response; // 因为你的拦截器已经处理了 {Data, Code, Msg} 格式，这里直接返回 Data
    } catch (error) {
        // 错误处理已经在主应用的拦截器中完成，这里可以进行一些特定于插件的逻辑（如果需要）
        console.error('[PluginA] API call failed:', error);
        throw error; // 重新抛出错误，让调用者处理
    }
}