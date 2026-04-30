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
/** 获取指定算法的私钥 GET /api/EncryptionKey/private/${param0} */
/** @deprecated 请使用 getEncryptionKeyGetPrivateKey，原函数名: getApiEncryptionKeyPrivateAlgorithm */
export async function getEncryptionKeyGetPrivateKey(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getEncryptionKeyGetPrivateKeyParams,
	options?: { [key: string]: any }
) {
	const { algorithm: param0, ...queryParams } = params;

	return request<any>(`/api/EncryptionKey/private/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getEncryptionKeyGetPrivateKey */
export const getApiEncryptionKeyPrivateAlgorithm = getEncryptionKeyGetPrivateKey;
/** 获取指定算法的公钥 GET /api/EncryptionKey/public/${param0} */
/** @deprecated 请使用 getEncryptionKeyGetPublicKey，原函数名: getApiEncryptionKeyPublicAlgorithm */
export async function getEncryptionKeyGetPublicKey(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getEncryptionKeyGetPublicKeyParams,
	options?: { [key: string]: any }
) {
	const { algorithm: param0, ...queryParams } = params;

	return request<any>(`/api/EncryptionKey/public/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getEncryptionKeyGetPublicKey */
export const getApiEncryptionKeyPublicAlgorithm = getEncryptionKeyGetPublicKey;
