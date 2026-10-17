/* eslint-disable @typescript-eslint/no-explicit-any */
import * as axios from 'axios';

// 扩展 axios 数据返回类型，可自行扩展
declare module 'axios' {
	export interface AxiosResponse<T = any> {
		Code: number;
		Data: T;
		Msg: string;
		type?: string;
		[key: string]: any;
	}

	/**
	 * openapi2ts 生成的客户端会在请求配置里带上 requestType（用于区分普通/加密/上传等请求），
	 * 但 axios 本身没有这个字段。此前每个生成的 api 文件都会因此报 TS2353；
	 * 声明放在公共层后，一处修复覆盖所有生成代码。
	 */
	export interface AxiosRequestConfig {
		requestType?: string;
	}

	// 分页数据类型
	export interface PageInfo {
		Total: number;
		TotalPages: number;
		HasPreviousPage: boolean;
		HasNextPage: boolean;
		Page: number;
		PageSize: number;
	}

	// 分页响应类型
	export interface PagedResponse<T> {
		Data: {
			PageInfo: PageInfo;
			Items: T[];
		};
		Code: number;
		Msg: string;
		[key: string]: any;
	}
}