// 导入 JSEncrypt
import { JSEncrypt } from 'jsencrypt';
import CryptoJS from 'crypto-js';
import { Session } from '@/utils/storage';

/**
 * AES 加密函数（CBC 模式 + PKCS7 填充）
 * @param data 要加密的数据（对象或字符串）
 * @param key Base64 编码的 AES 密钥
 * @param iv Base64 编码的初始化向量
 */
export async function aesEncrypt(
  data: any,
  key: string,
  iv: string
): Promise<string> {
  try {
    // 1. 序列化数据
    const plaintext = typeof data === 'string' ? data : JSON.stringify(data);

    // 2. 解码密钥和 IV
    const keyBytes = CryptoJS.enc.Base64.parse(key);
    const ivBytes = CryptoJS.enc.Base64.parse(iv);

    // 3. AES 加密（CBC 模式 + PKCS7 填充）
    const encrypted = CryptoJS.AES.encrypt(plaintext, keyBytes, {
      iv: ivBytes,
      mode: CryptoJS.mode.CBC,
      padding: CryptoJS.pad.Pkcs7
    });

    // 4. 返回 Base64 编码的密文
    return encrypted.toString();
  } catch (error) {
    console.error('AES 加密失败:', error);
    throw new Error('AES 加密失败');
  }
}

/**
 * AES 解密函数（CBC 模式 + PKCS7 填充）
 * @param encryptedData Base64 编码的密文
 * @param key Base64 编码的 AES 密钥
 * @param iv Base64 编码的初始化向量
 */
export async function aesDecrypt(
  encryptedData: string,
  key: string,
  iv: string
): Promise<any> {
  try {
    // 1. 解码密钥和 IV
    const keyBytes = CryptoJS.enc.Base64.parse(key);
    const ivBytes = CryptoJS.enc.Base64.parse(iv);

    // 2. AES 解密
    const decrypted = CryptoJS.AES.decrypt(encryptedData, keyBytes, {
      iv: ivBytes,
      mode: CryptoJS.mode.CBC,
      padding: CryptoJS.pad.Pkcs7
    });

    // 3. 转换为字符串
    const plaintext = decrypted.toString(CryptoJS.enc.Utf8);
    if (!plaintext) {
      throw new Error('解密结果为空');
    }

    // 4. 尝试解析为 JSON
    try {
      return JSON.parse(plaintext);
    } catch {
      return plaintext;
    }
  } catch (error) {
    console.error('AES 解密失败:', error);
    throw new Error('AES 解密失败');
  }
}

/**
 * RSA 加密函数（用于加密 AES 密钥或小数据）
 */
export async function rsaEncrypt(data: string, publicKeyBase64?: string): Promise<string> {
  if (!publicKeyBase64) {
    throw new Error('RSA 公钥未提供');
  }

  // 使用 JSEncrypt 进行加密
  const encrypt = new JSEncrypt();
  
  // 将 Base64 编码的公钥转换为 PEM 格式
  let pemKey;
  if (publicKeyBase64.includes('-----BEGIN')) {
    pemKey = publicKeyBase64;
  } else {
    const formattedKey = publicKeyBase64.replace(/(.{64})/g, '$1\n').trim();
    pemKey = `-----BEGIN PUBLIC KEY-----\n${formattedKey}\n-----END PUBLIC KEY-----`;
  }
  
  encrypt.setPublicKey(pemKey);
  
  const encrypted = encrypt.encrypt(data);
  if (!encrypted) {
    throw new Error('RSA 加密失败');
  }
  
  return encrypted as string;
}

/**
 * RSA 解密函数（用于解密 AES 密钥）
 * 注意：私钥由后端通过响应头传递，仅用于解密会话密钥
 */
export async function rsaDecrypt(encryptedData: string, privateKeyBase64?: string): Promise<string> {
  if (!privateKeyBase64) {
    throw new Error('RSA 私钥未提供');
  }
  
  const decrypt = new JSEncrypt();
  
  // 将 Base64 编码的私钥转换为 PEM 格式
  let pemKey;
  if (privateKeyBase64.includes('-----BEGIN')) {
    pemKey = privateKeyBase64;
  } else {
    const formattedKey = privateKeyBase64.replace(/(.{64})/g, '$1\n').trim();
    pemKey = `-----BEGIN RSA PRIVATE KEY-----\n${formattedKey}\n-----END RSA PRIVATE KEY-----`;
  }
  
  decrypt.setPrivateKey(pemKey);

  const decrypted = decrypt.decrypt(encryptedData);
  if (decrypted === false) {
    throw new Error('RSA 解密失败');
  }
  
  return decrypted as string;
}

/**
 * 请求加密工具函数
 * @param data 要加密的数据
 * @param algorithm 加密算法（'RSA' 或 'AES' 或 'HYBRID'）
 * @param publicKey RSA 公钥（用于 RSA 或混合加密）
 */
export async function encryptRequest(
  data: any,
  algorithm: string = 'RSA',
  publicKey?: string
): Promise<string | any> {
  switch (algorithm) {
    case 'AES':
      // AES 需要额外的 key 和 iv 参数，此模式不常用
      console.warn('AES 加密需要提供密钥和 IV，请使用 HYBRID 模式');
      return data;

    case 'HYBRID':
      // 混合加密：RSA + AES
      // 如果没有提供公钥，尝试从 Session 中获取
      let rsaPublicKey = publicKey;
      if (!rsaPublicKey) {
        rsaPublicKey = Session.get('encryptionPublicKey');
        if (!rsaPublicKey) {
          console.warn('[Encryption] 未找到 RSA 公钥，尝试从后端获取...');
          throw new Error('混合加密需要提供 RSA 公钥。请在登录前调用 /api/encryption/public-key 获取公钥并存储到 Session');
        }
      }
      
      try {
        // 1. 生成随机 AES 密钥和 IV
        const aesKey = CryptoJS.lib.WordArray.random(32); // 256-bit
        const aesIV = CryptoJS.lib.WordArray.random(16);  // 128-bit

        // 2. 用 AES 加密数据
        const plaintext = typeof data === 'string' ? data : JSON.stringify(data);
        const encryptedData = CryptoJS.AES.encrypt(plaintext, aesKey, {
          iv: aesIV,
          mode: CryptoJS.mode.CBC,
          padding: CryptoJS.pad.Pkcs7
        }).toString();

        // 3. 用 RSA 加密 AES 密钥和 IV
        const keyAndIV = aesKey.toString(CryptoJS.enc.Base64) + '|' + aesIV.toString(CryptoJS.enc.Base64);
        const encryptedKey = await rsaEncrypt(keyAndIV, rsaPublicKey);

        // 4. 返回组合结果
        return JSON.stringify({
          encryptedData,
          encryptedKey,
          algorithm: 'AES-256-CBC+RSA'
        });
      } catch (error) {
        console.error('[Encryption] 混合加密失败，可能公钥已过期，尝试刷新公钥...');
        
        // 如果是首次尝试且没有强制指定公钥，尝试刷新公钥后重试
        if (!publicKey) {
          try {
            // 动态导入避免循环依赖
            const { getEncryptionPublicKey } = await import('@/utils/encryption');
            const freshPublicKey = await getEncryptionPublicKey(true);
            
            if (freshPublicKey) {
              console.log('[Encryption] 公钥已刷新，重试加密...');
              // 递归调用，使用新公钥
              return encryptRequest(data, 'HYBRID', freshPublicKey);
            }
          } catch (refreshError) {
            console.error('[Encryption] 刷新公钥失败:', refreshError);
          }
        }
        
        // 重试失败或已有公钥，抛出原始错误
        throw error;
      }

    case 'RSA':
    default:
      // 纯 RSA 加密（仅适用于小数据）
      try {
        return await rsaEncrypt(JSON.stringify(data), publicKey);
      } catch (error) {
        console.error('RSA加密失败:', error);
        return data;
      }
  }
}

/**
 * 响应解密工具函数
 * @param data 加密的数据（字符串或对象）
 * @param algorithm 加密算法（'RSA' 或 'AES' 或 'HYBRID'）
 * @param privateKey RSA 私钥（从响应头获取）
 */
export async function decryptResponse(
  data: any,
  algorithm: string = 'RSA',
  privateKey?: string
): Promise<any> {
  if (!data) return data;

  switch (algorithm) {
    case 'HYBRID':
      // 混合加密解密
      if (!privateKey) {
        throw new Error('混合解密需要提供 RSA 私钥');
      }
      try {
        // 1. 解析加密数据
        const encryptedObj = typeof data === 'string' ? JSON.parse(data) : data;
        const { encryptedData, encryptedKey } = encryptedObj;

        // 2. 用 RSA 解密 AES 密钥
        const decryptedKeyAndIV = await rsaDecrypt(encryptedKey, privateKey);
        const [keyBase64, ivBase64] = decryptedKeyAndIV.split('|');

        // 3. 用 AES 解密数据
        return await aesDecrypt(encryptedData, keyBase64, ivBase64);
      } catch (error) {
        console.error('混合解密失败:', error);
        throw error;
      }

    case 'AES':
      // 纯 AES 解密（需要额外参数，不常用）
      console.warn('AES 解密需要提供密钥和 IV，请使用 HYBRID 模式');
      return data;

    case 'RSA':
    default:
      // 纯 RSA 解密
      if (!privateKey) {
        console.warn('RSA 解密需要提供私钥');
        return data;
      }
      try {
        return await rsaDecrypt(data, privateKey);
      } catch (error) {
        console.error('RSA解密失败:', error);
        return data;
      }
  }
}