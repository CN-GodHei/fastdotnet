import request from '@/utils/request';
import { getApiStorageConfig, postApiStorageGetUploadCredential, postApiStorageUpload } from '@/api/fd-system-api-app/Storage';

/**
 * 上传文件工具函数
 * 根据系统当前配置自动选择上传方式（后端代理或前端直传）
 */
export interface UploadFileOptions {
  file: File;
  bucketName?: string;
  onProgress?: (percent: number) => void;
  timeout?: number;
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
  const { file, bucketName, onProgress, timeout = 60000 } = options;

  try {
    // 首先获取当前存储配置
    const configResponse = await getApiStorageConfig();
    
    if (!configResponse) {
      throw new Error('获取存储配置失败');
    }

    if (configResponse.supportDirectUpload) {
      // 使用前端直传
      return await uploadFileDirectly(file, bucketName, onProgress);
    } else {
      // 使用后端代理上传
      return await uploadFileViaBackend(file, bucketName, onProgress, timeout);
    }
  } catch (error: any) {
    console.error('文件上传失败:', error);
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
  const response: any = await postApiStorageUpload(params, body, file, {
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
  const credentialResponse = await postApiStorageGetUploadCredential({
    FileName: file.name,
    FileSize: file.size,
    ContentType: file.type,
    BucketName: bucketName || undefined
  } as any);

  if (!credentialResponse) {
    throw new Error('获取上传凭证失败');
  }

  const credential = credentialResponse;
  
  // 构建表单数据
  const formData = new FormData();
  
  // 添加直传参数
  const uploadParams = credential.uploadParams || {};
  for (const [key, value] of Object.entries(uploadParams)) {
    formData.append(key, String(value));
  }
  
  // 添加文件
  formData.append('file', file, file.name);

  // 执行直传
  const response = await fetch(credential.uploadUrl, {
    method: 'POST',
    body: formData,
    headers: credential.uploadHeaders || {}
  });

  if (response.ok) {
    // 直传成功
    return {
      code: 200,
      data: {
        url: credential.fileUrlTemplate?.replace('{filename}', uploadParams.key || file.name),
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
  const response = await getApiStorageConfig();
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
  const response = await postApiStorageGetUploadCredential({
    FileName: params.fileName,
    FileSize: params.fileSize,
    ContentType: params.contentType,
    BucketName: params.bucketName || undefined
  } as any);
  return response;
};