// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 生成加密算法的密钥对 POST /api/EncryptionKey/generate */
export async function postApiEncryptionKeyGenerate(body: string, options?: { [key: string]: any }) {
	return request<any>('/api/EncryptionKey/generate', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 获取指定算法的私钥 GET /api/EncryptionKey/private/${param0} */
export async function getApiEncryptionKeyPrivateAlgorithm(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getApiEncryptionKeyPrivateAlgorithmParams,
	options?: { [key: string]: any }
) {
	const { algorithm: param0, ...queryParams } = params;

	return request<any>(`/api/EncryptionKey/private/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}
/** 获取指定算法的公钥 GET /api/EncryptionKey/public/${param0} */
export async function getApiEncryptionKeyPublicAlgorithm(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getApiEncryptionKeyPublicAlgorithmParams,
	options?: { [key: string]: any }
) {
	const { algorithm: param0, ...queryParams } = params;

	return request<any>(`/api/EncryptionKey/public/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}
