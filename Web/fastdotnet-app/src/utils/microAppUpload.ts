/**
 * 微应用上传工具
 * 用于在微应用环境中使用主应用的上传服务
 
 */

interface MicroAppProps {
  uploadService?: any;
  [key: string]: any;
}

let microAppProps: MicroAppProps | null = null;

/**
 * 设置微应用属性
 */
export function setMicroAppProps(props: MicroAppProps) {
  microAppProps = props;
}

/**
 * 获取上传服务实例
 */
function getUploadService() {
  // 优先使用微应用传递的上传服务
  if (microAppProps?.uploadService) {
    return microAppProps.uploadService;
  }
  
  // 其次尝试从全局获取
  if ((window as any).__UPLOAD_SERVICE__) {
    return (window as any).__UPLOAD_SERVICE__;
  }
  
  throw new Error('未找到上传服务，请确保主应用已正确初始化上传服务');
}

/**
 * 上传文件（微应用环境）
 */
export async function microAppUploadFile(file: File, options?: {
  bucketName?: string;
  onProgress?: (percent: number) => void;
  timeout?: number;
}) {
  try {
    const uploadService = getUploadService();
    return await uploadService.uploadFile(file, options);
  } catch (error) {
    console.error('微应用上传文件失败:', error);
    throw error;
  }
}

/**
 * 批量上传文件（微应用环境）
 */
export async function microAppUploadFiles(files: File[], options?: {
  bucketName?: string;
  onProgress?: (percent: number, index: number) => void;
  timeout?: number;
}) {
  try {
    const uploadService = getUploadService();
    return await uploadService.uploadFiles(files, options);
  } catch (error) {
    console.error('微应用批量上传文件失败:', error);
    throw error;
  }
}

/**
 * 获取当前存储配置（微应用环境）
 */
export async function microAppGetCurrentStorageConfig() {
  try {
    const uploadService = getUploadService();
    return await uploadService.getCurrentStorageConfig();
  } catch (error) {
    console.error('获取存储配置失败:', error);
    throw error;
  }
}

/**
 * 验证文件（微应用环境）
 */
export function microAppValidateFile(file: File, options?: {
  maxSize?: number; // MB
  accept?: string; // 例如: '.jpg,.png,image/*'
}) {
  try {
    const uploadService = getUploadService();
    return uploadService.validateFile(file, options);
  } catch (error) {
    console.error('验证文件失败:', error);
    throw error;
  }
}

/**
 * 检查上传服务是否可用
 */
export function isUploadServiceAvailable(): boolean {
  try {
    getUploadService();
    return true;
  } catch {
    return false;
  }
}