// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 生成加密算法的密钥对 POST /api/EncryptionKey/generate */
export async function postEncryptionKeyGenerateKeyPair(body: string, options?: { [key: string]: any }) {
	return request<any>('/api/EncryptionKey/generate', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 获取 RSA 公钥（用于混合加密） GET /api/EncryptionKey/public-key */
export async function getEncryptionKeyGetPublicKey(options?: { [key: string]: any }) {
	return request<any>('/api/EncryptionKey/public-key', {
		method: 'GET',
		...(options || {}),
	});
}
