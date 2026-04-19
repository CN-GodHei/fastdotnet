// @ts-ignore
/* eslint-disable */
import request from "../../utils/fdRequestAdapter";

/** 此处后端没有提供注释 POST /api/plugins/shared/p11375910391972869/PluginAUser/extension */
export async function postPluginsSharedP11375910391972869PluginAUserExtension(
  body: APIModel.CreateUserWithExtensionRequest,
  options?: { [key: string]: any }
) {
  return request<any>(
    "/api/plugins/shared/p11375910391972869/PluginAUser/extension",
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json-patch+json",
      },
      data: body,
      ...(options || {}),
    }
  );
}
/** 此处后端没有提供注释 GET /api/plugins/shared/p11375910391972869/PluginAUser/extension/${param0} */
export async function getPluginsSharedP11375910391972869PluginAUserExtensionUserId(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: APIModel.getPluginsSharedP11375910391972869PluginAUserExtensionUserIdParams,
  options?: { [key: string]: any }
) {
  const { userId: param0, ...queryParams } = params;

  return request<any>(
    `/api/plugins/shared/p11375910391972869/PluginAUser/extension/${param0}`,
    {
      method: "GET",
      params: { ...queryParams },
      ...(options || {}),
    }
  );
}
/** 此处后端没有提供注释 PUT /api/plugins/shared/p11375910391972869/PluginAUser/extension/${param0} */
export async function putPluginsSharedP11375910391972869PluginAUserExtensionUserId(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: APIModel.putPluginsSharedP11375910391972869PluginAUserExtensionUserIdParams,
  body: APIModel.PluginAUserExtension,
  options?: { [key: string]: any }
) {
  const { userId: param0, ...queryParams } = params;

  return request<any>(
    `/api/plugins/shared/p11375910391972869/PluginAUser/extension/${param0}`,
    {
      method: "PUT",
      headers: {
        "Content-Type": "application/json-patch+json",
      },
      params: { ...queryParams },
      data: body,
      ...(options || {}),
    }
  );
}
