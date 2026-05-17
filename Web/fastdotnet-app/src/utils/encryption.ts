import { getEncryptionKeyGetPublicKey } from '@/api/fd-system-api-app/encryptionKey';
import { Session } from '@/utils/storage';

/**
 * 获取并缓存 RSA 公钥（用于混合加密）
 * @param forceRefresh 是否强制刷新（忽略缓存）
 */
export async function getEncryptionPublicKey(forceRefresh = false): Promise<string | null> {
  // 如果不强制刷新且 Session 中已有公钥，直接返回
  if (!forceRefresh) {
    const cachedKey = Session.get('encryptionPublicKey');
    if (cachedKey) {
      return cachedKey;
    }
  }

  try {
    const res = await getEncryptionKeyGetPublicKey();

    if (res && res.PublicKey) {
      Session.set('encryptionPublicKey', res.PublicKey);
      return res.PublicKey;
    } else {
      return null;
    }
  } catch (error) {
    console.error('[Encryption] 获取 RSA 公钥失败:', error);
    return null;
  }
}

/**
 * 清除缓存的公钥（密钥轮换时使用）
 */
export function clearEncryptionPublicKey(): void {
  Session.remove('encryptionPublicKey');
}
