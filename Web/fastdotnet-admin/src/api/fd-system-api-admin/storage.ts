// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 获取当前存储配置 GET /api/Storage/config */
export async function getStorageGetCurrentConfig(options?: { [key: string]: any }) {
	return request<APIModel.StorageConfigResponse>('/api/Storage/config', {
		method: 'GET',
		...(options || {}),
	});
}
/** 删除文件 DELETE /api/Storage/delete */
export async function deleteStorageDelete(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteStorageDeleteParams,
	options?: { [key: string]: any }
) {
	return request<boolean>('/api/Storage/delete', {
		method: 'DELETE',
		params: {
			...params,
		},
		...(options || {}),
	});
}
/** 下载文件 GET /api/Storage/download/${param0} */
export async function getStorageDownload(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getStorageDownloadParams,
	options?: { [key: string]: any }
) {
	const { filePath: param0, ...queryParams } = params;

	return request<any>(`/api/Storage/download/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}
/** 获取上传凭证 POST /api/Storage/get-upload-credential */
export async function postStorageGetUploadCredential(body: APIModel.UploadCredentialRequest, options?: { [key: string]: any }) {
	return request<APIModel.UploadCredentialResponse>('/api/Storage/get-upload-credential', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		data: body,
		...(options || {}),
	});
}
/** 上传文件 POST /api/Storage/upload */
export async function postStorageUpload(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.postStorageUploadParams,
	body: {},
	file?: File,
	options?: { [key: string]: any }
) {
	const formData = new FormData();

	if (file) {
		formData.append('file', file);
	}

	Object.keys(body).forEach((ele) => {
		const item = (body as any)[ele];

		if (item !== undefined && item !== null) {
			if (typeof item === 'object' && !(item instanceof File)) {
				if (item instanceof Array) {
					item.forEach((f) => formData.append(ele, f || ''));
				} else {
					formData.append(ele, new Blob([JSON.stringify(item)], { type: 'application/json' }));
				}
			} else {
				formData.append(ele, item);
			}
		}
	});

	return request<string>('/api/Storage/upload', {
		method: 'POST',
		params: {
			...params,
		},
		data: formData,
		requestType: 'form',
		...(options || {}),
	});
}
/** 获取文件URL GET /api/Storage/url/${param0} */
export async function getStorageGetFileUrl(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getStorageGetFileUrlParams,
	options?: { [key: string]: any }
) {
	const { filePath: param0, ...queryParams } = params;

	return request<string>(`/api/Storage/url/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}
/** 通过URL直接访问上传的文件（公共访问接口） GET /uploads/${param0} */
export async function getStorageGetFileByPath(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getStorageGetFileByPathParams,
	options?: { [key: string]: any }
) {
	const { relativePath: param0, ...queryParams } = params;

	return request<any>(`/uploads/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}
