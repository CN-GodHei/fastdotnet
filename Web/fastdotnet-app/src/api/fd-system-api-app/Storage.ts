// @ts-ignore
/* eslint-disable */
import request from '@/utils/request';

/** 获取当前存储配置 GET /api/Storage/config */
export async function getApiStorageConfig(options?: { [key: string]: any }) {
	return request<APIModel.StorageConfigResponse>('/api/Storage/config', {
		method: 'GET',
		...(options || {}),
	});
}

/** 删除文件 DELETE /api/Storage/delete/${param0} */
export async function deleteApiStorage__openAPI__deleteFileName(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteApiStorage_openAPI_deleteFileNameParams,
	options?: { [key: string]: any }
) {
	const { fileName: param0, ...queryParams } = params;
	return request<boolean>(`/api/Storage/delete/${param0}`, {
		method: 'DELETE',
		params: {
			...queryParams,
		},
		...(options || {}),
	});
}

/** 下载文件 GET /api/Storage/download/${param0} */
export async function getApiStorageDownloadFileName(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getApiStorageDownloadFileNameParams,
	options?: { [key: string]: any }
) {
	const { fileName: param0, ...queryParams } = params;
	return request<any>(`/api/Storage/download/${param0}`, {
		method: 'GET',
		params: {
			...queryParams,
		},
		...(options || {}),
	});
}

/** 获取上传凭证 POST /api/Storage/get-upload-credential */
export async function postApiStorageGetUploadCredential(body: APIModel.UploadCredentialRequest, options?: { [key: string]: any }) {
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
export async function postApiStorageUpload(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.postApiStorageUploadParams,
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
export async function getApiStorageUrlFileName(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getApiStorageUrlFileNameParams,
	options?: { [key: string]: any }
) {
	const { fileName: param0, ...queryParams } = params;
	return request<string>(`/api/Storage/url/${param0}`, {
		method: 'GET',
		params: {
			...queryParams,
		},
		...(options || {}),
	});
}

/** 通过URL直接访问上传的文件（公共访问接口） GET /uploads/${param0} */
export async function getUploadsRelativePath(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getUploadsRelativePathParams,
	options?: { [key: string]: any }
) {
	const { relativePath: param0, ...queryParams } = params;
	return request<any>(`/uploads/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}
