import { uploadFile, getCurrentStorageConfig, getUploadCredential } from '@/utils/upload';
import { getApiStorageConfig, postApiStorageGetUploadCredential, postApiStorageUpload } from '@/api/fd-system-api-admin/Storage';

/**
 * 上传服务，提供统一的上传接口
 * 可以被微应用或其他模块使用
 */
class UploadService {
  /**
   * 单例模式
   */
  private static instance: UploadService;

  static getInstance(): UploadService {
    if (!UploadService.instance) {
      UploadService.instance = new UploadService();
    }
    return UploadService.instance;
  }

  /**
   * 上传单个文件
   */
  async uploadFile(file: File, options?: {
    bucketName?: string;
    onProgress?: (percent: number) => void;
    timeout?: number;
  }) {
    return await uploadFile({
      file,
      bucketName: options?.bucketName,
      onProgress: options?.onProgress,
      timeout: options?.timeout || 60000
    });
  }

  /**
   * 批量上传文件
   */
  async uploadFiles(files: File[], options?: {
    bucketName?: string;
    onProgress?: (percent: number, index: number) => void;
    timeout?: number;
  }): Promise<Array<{ file: File; result: any; error?: Error }>> {
    const results: Array<{ file: File; result: any; error?: Error }> = [];

    for (let i = 0; i < files.length; i++) {
      const file = files[i];
      try {
        const result = await this.uploadFile(file, {
          bucketName: options?.bucketName,
          onProgress: (percent) => {
            options?.onProgress?.(percent, i);
          },
          timeout: options?.timeout
        });
        
        results.push({
          file,
          result
        });
      } catch (error: any) {
        results.push({
          file,
          result: null,
          error
        });
      }
    }

    return results;
  }

  /**
   * 获取当前存储配置
   */
  async getCurrentStorageConfig() {
    return await getCurrentStorageConfig();
  }

  /**
   * 获取上传凭证
   */
  async getUploadCredential(params: {
    fileName: string;
    fileSize: number;
    contentType: string;
    bucketName?: string;
  }) {
    return await getUploadCredential(params);
  }

  /**
   * 验证文件
   */
  validateFile(file: File, options?: {
    maxSize?: number; // MB
    accept?: string; // 例如: '.jpg,.png,image/*'
  }): { valid: boolean; errors: string[] } {
    const errors: string[] = [];
    const maxSize = options?.maxSize || 10; // 默认10MB
    const accept = options?.accept || '';

    // 检查文件大小
    const fileSizeMB = file.size / 1024 / 1024;
    if (fileSizeMB > maxSize) {
      errors.push(`文件大小不能超过 ${maxSize}MB`);
    }

    // 检查文件类型
    if (accept) {
      const acceptTypes = accept.split(',').map(t => t.trim().toLowerCase());
      const fileType = file.type.toLowerCase();
      const fileExt = file.name.toLowerCase().split('.').pop() || '';
      
      const isValidType = acceptTypes.some(acceptType => {
        if (acceptType.startsWith('.')) {
          // 检查扩展名
          return fileExt === acceptType.substring(1);
        } else {
          // 检查MIME类型
          return fileType.includes(acceptType);
        }
      });
      
      if (!isValidType) {
        errors.push(`不支持的文件类型，仅支持: ${accept}`);
      }
    }

    return {
      valid: errors.length === 0,
      errors
    };
  }
}

// 暴露到全局，供微应用使用
const uploadService = UploadService.getInstance();
(window as any).__UPLOAD_SERVICE__ = uploadService;

export default uploadService;