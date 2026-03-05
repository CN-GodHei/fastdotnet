// @ts-ignore
/* eslint-disable */
import request, { encryptRequest } from '@/utils/request';

/** 此处后端没有提供注释 PUT /api/PluginConfiguration/${param1} */
export async function putApiPluginConfigurationId(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.putApiPluginConfigurationIdParams,
	body: string,
	options?: { [key: string]: any }
) {
	const { Id: param0, id: param1, ...queryParams } = params;

	return request<boolean>(`/api/PluginConfiguration/${param1}`, {
		method: 'PUT',
		headers: {
			'Content-Type': 'application/json-patch+json',
		},
		params: { ...queryParams },
		data: body,
		...(options || {}),
	});
}
/** 使用插件Id获取插件配置信息 GET /api/PluginConfiguration/Get-Plugin-ConfigurationBy/${param0} */
export async function getApiPluginConfigurationGetPluginConfigurationById(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: APIModel.getApiPluginConfigurationGetPluginConfigurationByIdParams,
	options?: { [key: string]: any }
) {
	const { id: param0, ...queryParams } = params;

	return request<string>(`/api/PluginConfiguration/Get-Plugin-ConfigurationBy/${param0}`, {
		method: 'GET',
		params: {
			...queryParams,
		},
		...(options || {}),
	});
}
