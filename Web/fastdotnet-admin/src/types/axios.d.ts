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