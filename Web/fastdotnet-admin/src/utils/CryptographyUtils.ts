// @ts-ignore - 忽略类型声明缺失
// @ts-ignore - 忽略类型声明缺失
import {  sm4 } from 'sm-crypto';
// const sm44 = require('sm-crypto').sm2
import { SM2 } from 'gm-crypto';

// @ts-ignore - 忽略类型声明缺失
import CryptoJS from 'crypto-js';

/**
 * 前端加密工具类 - 用于请求加密和响应解密
 */
export class CryptographyUtils {
	/**
	 * SM2加密
	 */
	static sm2Encrypt(plainText: string, publicKey: string): string {
		// try {
		//   // 检查输入参数是否有效
		//   if (typeof plainText !== 'string' || typeof publicKey !== 'string') {
		//     throw new Error('明文和公钥必须是字符串类型');
		//   }

		//   if (!plainText || !publicKey) {
		//     throw new Error('明文和公钥不能为空');
		//   }

		//   console.log('原始公钥:', publicKey);

		//   return SM2.doEncrypt(plainText, publicKey, 1); // 1表示C1C3C2格式
		// } catch (error: any) {
		//   console.error('SM2加密失败:', error);
		//   throw new Error(`SM2加密失败: ${error.message || error}`);
		// }
		return '';
	}

	/**
	 * SM2解密
	 */
	static sm2Decrypt(cipherText: string, privateKey: string): string {
		console.log('SM2解密:', cipherText);
		console.log('私钥:', privateKey);
        // const cipherTextHex = Buffer.from(cipherText, 'base64').toString('hex');
		const plainText = SM2.decrypt(cipherText, privateKey, {mode:SM2.constants.C1C2C3, inputEncoding: 'base64', outputEncoding: 'utf-8' }); // 1表示C1C3C2模式

        console.log('解密后的明文:', plainText);
		return plainText;
	}


	/**
	 * AES加密
	 */
	static aesEncrypt(plainText: string, key: string): string {
		try {
			const keyWordArray = CryptoJS.enc.Utf8.parse(this.padKey(key, 32));
			const iv = CryptoJS.lib.WordArray.random(16);

			const encrypted = CryptoJS.AES.encrypt(plainText, keyWordArray, {
				iv: iv,
				mode: CryptoJS.mode.CBC,
				padding: CryptoJS.pad.Pkcs7,
			});

			// 将IV和密文组合返回
			const result = iv.toString() + ':' + encrypted.ciphertext.toString();
			return result;
		} catch (error: any) {
			console.error('AES加密失败:', error);
			throw new Error(`AES加密失败: ${error.message || error}`);
		}
	}

	/**
	 * AES解密
	 */
	static aesDecrypt(combinedCipherText: string, key: string): string {
		try {
			const parts = combinedCipherText.split(':');
			if (parts.length !== 2) {
				throw new Error('AES密文格式错误');
			}

			const iv = CryptoJS.enc.Hex.parse(parts[0]);
			const cipherText = parts[1];

			const keyWordArray = CryptoJS.enc.Utf8.parse(this.padKey(key, 32));

			const decrypted = CryptoJS.AES.decrypt({ ciphertext: CryptoJS.enc.Base64.parse(cipherText) }, keyWordArray, {
				iv: iv,
				mode: CryptoJS.mode.CBC,
				padding: CryptoJS.pad.Pkcs7,
			});

			return decrypted.toString(CryptoJS.enc.Utf8);
		} catch (error: any) {
			console.error('AES解密失败:', error);
			throw new Error(`AES解密失败: ${error.message || error}`);
		}
	}

	/**
	 * SM4加密
	 */
	static sm4Encrypt(plainText: string, key: string): string {
		try {
			return sm4.encrypt(plainText, key);
		} catch (error: any) {
			console.error('SM4加密失败:', error);
			throw new Error(`SM4加密失败: ${error.message || error}`);
		}
	}

	/**
	 * SM4解密
	 */
	static sm4Decrypt(cipherText: string, key: string): string {
		try {
			return sm4.decrypt(cipherText, key);
		} catch (error: any) {
			console.error('SM4解密失败:', error);
			throw new Error(`SM4解密失败: ${error.message || error}`);
		}
	}

	/**
	 * 解密后端API响应
	 */
	static decryptResponse(encryptedData: string, privateKey: string, algorithm: 'SM2' | 'AES' | 'SM4'): string {
		switch (algorithm.toUpperCase()) {
			case 'SM2':
				return this.sm2Decrypt(encryptedData, privateKey);
			case 'AES':
				return this.aesDecrypt(encryptedData, privateKey);
			case 'SM4':
				return this.sm4Decrypt(encryptedData, privateKey);
			default:
				throw new Error(`不支持的解密算法: ${algorithm}`);
		}
	}

	/**
	 * 标准化密钥长度
	 */
	private static padKey(key: string, length: number): string {
		if (key.length > length) {
			return key.substring(0, length);
		}
		return key.padEnd(length, '\0');
	}
}
