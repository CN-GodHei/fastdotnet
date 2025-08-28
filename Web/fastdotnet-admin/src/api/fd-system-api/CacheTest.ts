// @ts-ignore
/* eslint-disable */
import request from '/@/utils/request';

/** 移除特定标签的缓存 DELETE /api/admin/cache-test/clear-by-tag */
export async function deleteAdminCacheTestClearByTag(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.deleteAdminCacheTestClearByTagParams,
	options?: { [key: string]: any }
) {
	return request<any>('/api/admin/cache-test/clear-by-tag', {
		method: 'DELETE',
		params: {
			...params,
		},
		...(options || {}),
	});
}

/** 获取缓存值 GET /api/admin/cache-test/get */
export async function getAdminCacheTestGet(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAdminCacheTestGetParams,
	options?: { [key: string]: any }
) {
	return request<string>('/api/admin/cache-test/get', {
		method: 'GET',
		params: {
			...params,
		},
		...(options || {}),
	});
}

/** 测试带多个参数的缓存功能 GET /api/admin/cache-test/products */
export async function getAdminCacheTestProducts(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAdminCacheTestProductsParams,
	options?: { [key: string]: any }
) {
	return request<string[]>('/api/admin/cache-test/products', {
		method: 'GET',
		params: {
			// page has a default value: 1
			page: '1',
			// pageSize has a default value: 10
			pageSize: '10',
			...params,
		},
		...(options || {}),
	});
}

/** 手动设置缓存 POST /api/admin/cache-test/set */
export async function postAdminCacheTestSet(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.postAdminCacheTestSetParams,
	options?: { [key: string]: any }
) {
	return request<any>('/api/admin/cache-test/set', {
		method: 'POST',
		params: {
			...params,
		},
		...(options || {}),
	});
}

/** 测试基本缓存功能 GET /api/admin/cache-test/user/${param0} */
export async function getAdminCacheTestUserId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getAdminCacheTestUserIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;
	return request<string>(`/api/admin/cache-test/user/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}
