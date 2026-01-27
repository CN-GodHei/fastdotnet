// 导入 JSEncrypt
import { JSEncrypt } from 'jsencrypt';

/**
 * AES 加密函数
 */
export async function aesEncrypt(data: any): Promise<string> {
  console.log('使用AES算法对数据进行加密:', data);
  // 这里实现 AES 加密逻辑
  return JSON.stringify(data); // TODO: 实际AES加密逻辑
}

/**
 * AES 解密函数
 */
export async function aesDecrypt(encryptedData: string): Promise<any> {
  console.log('使用AES算法对数据进行解密:', encryptedData);
  // 这里实现 AES 解密逻辑
  try {
    return JSON.parse(encryptedData); // TODO: 实际AES解密逻辑
  } catch {
    return encryptedData;
  }
}

/**
 * RSA 加密函数
 * 注意：实际应用中，前端一般不执行敏感数据的RSA加密
 */
export async function rsaEncrypt(data: string, publicKeyBase64?: string): Promise<string> {
  if (!publicKeyBase64) {
    throw new Error('RSA 公钥未提供');
  }

  // 使用 JSEncrypt 进行加密
  const encrypt = new JSEncrypt();
  
  // 将 Base64 编码的公钥转换为 PEM 格式，JSEncrypt 需要 PEM 格式
  let pemKey;
  if (publicKeyBase64.includes('-----BEGIN')) {
    pemKey = publicKeyBase64;
  } else {
    // 对于 Base64 格式的公钥，需要使用正确的 PEM 包装
    const formattedKey = publicKeyBase64.replace(/(.{64})/g, '$1\n').trim();
    pemKey = `-----BEGIN PUBLIC KEY-----\n${formattedKey}\n-----END PUBLIC KEY-----`;
  }
  
  encrypt.setPublicKey(pemKey);
  
  // 加密数据
  const encrypted = encrypt.encrypt(data);
  if (!encrypted) {
    throw new Error('RSA 加密失败');
  }
  
  return encrypted as string;
}

/**
 * RSA 解密函数
 * 注意：在实际应用中，私钥不应出现在前端，此功能仅作演示用途
 */
export async function rsaDecrypt(encryptedData: string, privateKeyBase64?: string): Promise<any> {
  if (!privateKeyBase64) {
    throw new Error('RSA 私钥未提供');
  }
  
  // 创建 JSEncrypt 实例并设置私钥
  const decrypt = new JSEncrypt();
  
  // 将 Base64 编码的私钥转换为 PEM 格式，JSEncrypt 需要 PEM 格式
  let pemKey;
  if (privateKeyBase64.includes('-----BEGIN')) {
    pemKey = privateKeyBase64;
  } else {
    // 对于 Base64 格式的私钥，需要使用正确的 PEM 包装
    const formattedKey = privateKeyBase64.replace(/(.{64})/g, '$1\n').trim();
    pemKey = `-----BEGIN RSA PRIVATE KEY-----\n${formattedKey}\n-----END RSA PRIVATE KEY-----`;
  }
  
  decrypt.setPrivateKey(pemKey);

  // 检查是否为分段加密的数据（使用 |SPLIT| 分隔符）
  if (encryptedData.includes('|SPLIT|')) {
    // 分段解密
    const encryptedBlocks = encryptedData.split('|SPLIT|');
    const decryptedParts: string[] = [];
    
    for (const encryptedBlock of encryptedBlocks) {
      if (encryptedBlock.trim()) {
        console.log('解密数据块:', encryptedBlock)
        const decryptedBlock = decrypt.decrypt(encryptedBlock);
        if (decryptedBlock === false) {
          throw new Error(`分段解密失败: ${encryptedBlock.substring(0, 20)}...`);
        }
        decryptedParts.push(decryptedBlock as string);
      }
    }
    console.log(decryptedParts)
    // 合并解密后的数据
    const result = decryptedParts.join('');
    
    // 尝试解析为JSON，如果不成功则返回原始字符串
    try {
      return JSON.parse(result);
    } catch {
      return result;
    }
  } else {
    // 单次解密
    const decrypted = decrypt.decrypt(encryptedData);
    if (decrypted === false) {
      throw new Error('RSA 解密失败');
    }
    
    // 尝试解析为JSON，如果不成功则返回原始字符串
    try {
      return JSON.parse(decrypted as string);
    } catch {
      return decrypted;
    }
  }
}

/**
 * 请求加密工具函数
 */
export async function encryptRequest(data: any, algorithm: string = 'RSA', publicKey?: string) {
  // 根据算法类型执行相应加密
  switch(algorithm) {
    case 'AES':
      // 这里实现 AES 加密逻辑
      console.log('使用AES算法对请求数据进行加密:', data);
      return await aesEncrypt(data); // TODO: 实际AES加密逻辑
    case 'RSA':
      // 使用 JSEncrypt 进行 RSA 加密
      try {
        // 使用传入的公钥进行加密
        return await rsaEncrypt(JSON.stringify(data), publicKey);
      } catch (error) {
        console.error('RSA加密失败:', error);
        return data; // 加密失败时返回原数据
      }
    default:
      return data; // 默认不加密
  }
}

/**
 * 响应解密工具函数
 */
export async function decryptResponse(data: any, algorithm: string = 'RSA', privateKey?: string) {
  // 根据算法类型执行相应解密
  // 注意：在实际使用中，响应解密通常通过响应头 x-rsa-privateKey 来判断，
  // 这个函数主要用于在响应拦截器之外进行手动解密
  switch(algorithm) {
    case 'AES':
      // 这里实现 AES 解密逻辑
      console.log('使用AES算法对响应数据进行解密:', data);
      return await aesDecrypt(data); // TODO: 实际AES解密逻辑
    case 'RSA':
      // 使用 JSEncrypt 进行 RSA 解密
      // 注意：在实际应用中，私钥不应出现在前端
      console.log('准备RSA解密，加密数据长度:', typeof data === 'string' ? data.length : 'not string');
      console.log('传入的私钥长度:', privateKey ? privateKey.length : 'undefined');
      console.log('传入的私钥前缀:', privateKey ? privateKey.substring(0, 10) : 'undefined');
      
      try {
        // 使用传入的私钥进行解密
        return await rsaDecrypt(data, privateKey);
      } catch (error) {
        console.error('RSA解密失败:', error);
        return data; // 解密失败时返回原数据
      }
    default:
      return data; // 默认不解密
  }
}