// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 生成加密算法的密钥对 POST /api/EncryptionKey/generate */
/** @deprecated 请使用 postEncryptionKeyGenerateKeyPair，原函数名: postApiEncryptionKeyGenerate */
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

/** @deprecated 此函数名已变更，请使用 postEncryptionKeyGenerateKeyPair */
export const postApiEncryptionKeyGenerate = postEncryptionKeyGenerateKeyPair;
/** 获取 RSA 公钥（用于混合加密） GET /api/EncryptionKey/public-key */
/** @deprecated 请使用 getEncryptionKeyGetPublicKey，原函数名: getApiEncryptionKeyPublicKey */
export async function getEncryptionKeyGetPublicKey(options?: { [key: string]: any }) {
	return request<any>('/api/EncryptionKey/public-key', {
		method: 'GET',
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getEncryptionKeyGetPublicKey */
export const getApiEncryptionKeyPublicKey = getEncryptionKeyGetPublicKey;
