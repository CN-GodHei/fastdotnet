import request from '@/utils/request';
import { getStorageGetCurrentConfig, postStorageGetUploadCredential, postStorageUpload, deleteStorageDelete } from '@/api/fd-system-api-admin/Storage';

/**
 * 上传文件工具函数
 * 根据系统当前配置自动选择上传方式(后端代理或前端直传)
 */
export interface UploadFileOptions {
  file: File;
  bucketName?: string;
  onProgress?: (percent: number) => void;
  timeout?: number;
  /**
   * 是否强制使用后端代理上传
   * - true: 始终走后端代理,即使OSS支持直传
   * - false/undefined: 根据OSS配置自动选择(默认行为)
   * 
   * 适用场景:
   * - 需要后端进行文件内容校验(病毒扫描、内容审核)
   * - 需要严格的权限控制
   * - 敏感文件不希望暴露OSS凭证
   * - 需要在上传前执行其他业务逻辑
   */
  forceProxy?: boolean;
}

export interface UploadResult {
  code: number;
  data: {
    url: string;
    fileName: string;
  };
  message: string;
}

/**
 * 上传单个文件
 */
export const uploadFile = async (options: UploadFileOptions): Promise<UploadResult> => {
  const { file, bucketName, onProgress, timeout = 60000, forceProxy = false } = options;

  try {
    // 如果强制使用代理,直接走后端代理上传
    if (forceProxy) {
      return await uploadFileViaBackend(file, bucketName, onProgress, timeout);
    }

    // 否则获取当前存储配置,自动选择上传方式
    const configResponse = await getStorageGetCurrentConfig();
    
    if (!configResponse) {
      throw new Error('获取存储配置失败');
    }
    
    // 兼容 PascalCase 和 camelCase
    const supportDirectUpload = configResponse.SupportDirectUpload ?? configResponse.supportDirectUpload ?? false;

    if (supportDirectUpload) {
      // 使用前端直传
      return await uploadFileDirectly(file, bucketName, onProgress);
    } else {
      // 使用后端代理上传
      return await uploadFileViaBackend(file, bucketName, onProgress, timeout);
    }
  } catch (error: any) {
    console.error('[Upload] 文件上传失败:', error);
    throw error;
  }
};

/**
 * 通过后端代理上传文件
 */
const uploadFileViaBackend = async (
  file: File,
  bucketName?: string,
  onProgress?: (percent: number) => void,
  timeout: number = 60000
): Promise<UploadResult> => {
  // 使用专门的上传API
  const params = {
    bucketName: bucketName
  };
  const body = {};
  const response: any = await postStorageUpload(params, body, file, {
    headers: {
      'Content-Type': 'multipart/form-data',
    },
    timeout,
    onUploadProgress: (progressEvent: any) => {
      if (onProgress && progressEvent.total) {
        const percentCompleted = Math.round(
          (progressEvent.loaded * 100) / progressEvent.total!
        );
        onProgress(percentCompleted);
      }
    }
  });

  return response;
};

/**
 * 前端直传文件
 */
const uploadFileDirectly = async (
  file: File,
  bucketName?: string,
  onProgress?: (percent: number) => void
): Promise<UploadResult> => {
  // 获取上传凭证
  const credentialResponse = await postStorageGetUploadCredential({
    FileName: file.name,
    FileSize: file.size,
    ContentType: file.type,
    BucketName: bucketName || undefined
  } as any);

  if (!credentialResponse) {
    throw new Error('获取上传凭证失败');
  }

  const credential = credentialResponse;
  // 兼容 PascalCase 和 camelCase
  const uploadUrl = credential.UploadUrl || credential.uploadUrl;
  const uploadParams = credential.UploadParams || credential.uploadParams || {};
  const uploadHeaders = credential.UploadHeaders || credential.uploadHeaders || {};
  const fileUrlTemplate = credential.FileUrlTemplate || credential.fileUrlTemplate;
  const requestMethod = (credential.RequestMethod || credential.requestMethod || 'POST').toUpperCase();
  
  
  if (!uploadUrl) {
    throw new Error('上传凭证中缺少UploadUrl');
  }
  
  let response: Response;
  
  // 根据请求方法选择不同的上传方式
  if (requestMethod === 'PUT') {
    // MinIO/S3 预签名URL: 直接 PUT 文件流
    response = await fetch(uploadUrl, {
      method: 'PUT',
      body: file,
      headers: {
        'Content-Type': file.type || 'application/octet-stream'
      }
    });
  } else {
    // 七牛云等: POST + FormData
    const formData = new FormData();
    
    // 添加直传参数
    for (const [key, value] of Object.entries(uploadParams)) {
      formData.append(key, String(value));
    }
    
    // 添加文件
    formData.append('file', file, file.name);

    response = await fetch(uploadUrl, {
      method: 'POST',
      body: formData,
      headers: uploadHeaders
    });
  }

  if (response.ok) {
    // 直传成功
    const key = uploadParams.Key || uploadParams.key || file.name;
    const url = fileUrlTemplate?.replace('{filename}', key) || key;
    return {
      code: 200,
      data: {
        url,
        fileName: file.name
      },
      message: '上传成功'
    };
  } else {
    throw new Error(`上传失败，HTTP状态码: ${response.status}`);
  }
};

/**
 * 获取当前存储配置
 */
export const getCurrentStorageConfig = async () => {
  const response = await getStorageGetCurrentConfig();
  return response;
};

/**
 * 获取上传凭证
 */
export const getUploadCredential = async (params: {
  fileName: string;
  fileSize: number;
  contentType: string;
  bucketName?: string;
}) => {
  const response = await postStorageGetUploadCredential({
    FileName: params.fileName,
    FileSize: params.fileSize,
    ContentType: params.contentType,
    BucketName: params.bucketName || undefined
  } as any);
  return response;
};

/**
 * 删除文件
 */
export const deleteFile = async (fileName: string, bucketName?: string): Promise<boolean> => {
  try {
    // 从 URL 中提取完整的文件路径(如果传入的是完整URL)
    let actualFileName = fileName;
    if (fileName.includes('://')) {
      // 是完整URL,提取域名后的完整路径
      // 例如: https://domain.com/20260425/xxx.png -> 20260425/xxx.png
      try {
        const url = new URL(fileName);
        // pathname 以 / 开头,需要去掉
        actualFileName = url.pathname.startsWith('/') ? url.pathname.substring(1) : url.pathname;
      } catch (e) {
        // URL解析失败,尝试简单分割
        const urlParts = fileName.split('/');
        // 找到域名后的所有部分
        const domainIndex = urlParts.findIndex(part => part.includes('.'));
        if (domainIndex !== -1 && domainIndex < urlParts.length - 1) {
          actualFileName = urlParts.slice(domainIndex + 1).join('/');
        } else {
          actualFileName = urlParts[urlParts.length - 1];
        }
      }
    }

    if (!actualFileName) {
      throw new Error('无法解析文件名');
    }

    // console.log('[Upload] 删除文件:', actualFileName);

    // 调用 openapi2ts 生成的 API
    const result = await deleteStorageDelete({
      filePath: actualFileName,
      bucketName: bucketName || undefined
    });

    // request 拦截器已经返回 response.data
    return result as unknown as boolean;
  } catch (error: any) {
    // console.error('[Upload] 删除文件失败:', error);
    throw error;
  }
};