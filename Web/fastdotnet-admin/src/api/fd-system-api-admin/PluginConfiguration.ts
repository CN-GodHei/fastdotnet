// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 此处后端没有提供注释 PUT /api/PluginConfiguration/${param0} */
/** @deprecated 请使用 putPluginConfigurationUpdate，原函数名: putApiPluginConfigurationPluginId */
export async function putPluginConfigurationUpdate(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putPluginConfigurationUpdateParams,
	body: string,
	options?: { [key: string]: any }
) {
	const { PluginId: param0, ...queryParams } = params;

	return request<boolean>(`/api/PluginConfiguration/${param0}`, {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		params: { ...queryParams },
		data: body,
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 putPluginConfigurationUpdate */
export const putApiPluginConfigurationPluginId = putPluginConfigurationUpdate;
/** 使用插件Id获取插件配置信息 GET /api/PluginConfiguration/Get-Plugin-ConfigurationBy/${param0} */
/** @deprecated 请使用 getPluginConfigurationGetPluginConfigurationById，原函数名: getApiPluginConfigurationGetPluginConfigurationByPluginId */
export async function getPluginConfigurationGetPluginConfigurationById(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getPluginConfigurationGetPluginConfigurationByIdParams,
	options?: { [key: string]: any }
) {
	const { PluginId: param0, ...queryParams } = params;

	return request<APIModel.PluginConfigurationGetRawJsonDto>(`/api/PluginConfiguration/Get-Plugin-ConfigurationBy/${param0}`, {
		method: 'GET',
		params: { ...queryParams },
		...(options || {}),
	});
}

/** @deprecated 此函数名已变更，请使用 getPluginConfigurationGetPluginConfigurationById */
export const getApiPluginConfigurationGetPluginConfigurationByPluginId = getPluginConfigurationGetPluginConfigurationById;
