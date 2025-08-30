// @ts-ignore
/* eslint-disable */
import request from "../../utils/fdRequestAdapter";

/** 获取所有记录 检索并返回系统中该类型的所有记录。 GET /api/plugins/11375910391972869/PluginATestDto */
export async function getApiPluginsPinyin_11375910391972869PluginATestDto(options?: {
  [key: string]: any;
}) {
  return request<APIModel.PluginATestDto[]>(
    "/api/plugins/11375910391972869/PluginATestDto",
    {
      method: "GET",
      ...(options || {}),
    }
  );
}

/** 创建新记录 根据提供的数据创建一条新记录。 POST /api/plugins/11375910391972869/PluginATestDto */
export async function postApiPluginsPinyin_11375910391972869PluginATestDto(
  body: APIModel.PluginATestCreateDto,
  options?: { [key: string]: any }
) {
  return request<APIModel.PluginATestDto>(
    "/api/plugins/11375910391972869/PluginATestDto",
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

/** 根据ID获取记录 根据提供的唯一标识符(ID)检索特定记录的详细信息。 GET /api/plugins/11375910391972869/PluginATestDto/${param0} */
export async function getId(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: APIModel.getIdParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<APIModel.PluginATestDto>(
    `/api/plugins/11375910391972869/PluginATestDto/${param0}`,
    {
      method: "GET",
      params: { ...queryParams },
      ...(options || {}),
    }
  );
}

/** 更新现有记录 根据提供的ID和更新数据，修改现有记录的信息。 PUT /api/plugins/11375910391972869/PluginATestDto/${param0} */
export async function putId(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: APIModel.putIdParams,
  body: APIModel.PluginATestUpdateDto,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<APIModel.PluginATestDto>(
    `/api/plugins/11375910391972869/PluginATestDto/${param0}`,
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

/** 删除记录 根据提供的ID，从系统中移除指定的记录。 DELETE /api/plugins/11375910391972869/PluginATestDto/${param0} */
export async function deleteId(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: APIModel.deleteIdParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<boolean>(
    `/api/plugins/11375910391972869/PluginATestDto/${param0}`,
    {
      method: "DELETE",
      params: { ...queryParams },
      ...(options || {}),
    }
  );
}

/** 批量删除记录 根据提供的ID列表，批量删除多条记录。 DELETE /api/plugins/11375910391972869/PluginATestDto/batch */
export async function deleteBatch(
  body: string[],
  options?: { [key: string]: any }
) {
  return request<number>(
    "/api/plugins/11375910391972869/PluginATestDto/batch",
    {
      method: "DELETE",
      headers: {
        "Content-Type": "application/json-patch+json",
      },
      data: body,
      ...(options || {}),
    }
  );
}

/** 分页获取记录 根据页码和页面大小，分页检索记录。 GET /api/plugins/11375910391972869/PluginATestDto/page */
export async function getPage(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: APIModel.getPageParams,
  options?: { [key: string]: any }
) {
  return request<APIModel.PluginATestDtoPageResult>(
    "/api/plugins/11375910391972869/PluginATestDto/page",
    {
      method: "GET",
      params: {
        // pageIndex has a default value: 1
        pageIndex: "1",
        // pageSize has a default value: 10
        pageSize: "10",
        ...params,
      },
      ...(options || {}),
    }
  );
}

/** 根据条件分页获取记录 根据提供的查询条件和分页参数，分页检索记录。 POST /api/plugins/11375910391972869/PluginATestDto/page/search */
export async function postPageSearch(
  body: APIModel.PageQueryByConditionDto,
  options?: { [key: string]: any }
) {
  return request<APIModel.PluginATestDtoPageResult>(
    "/api/plugins/11375910391972869/PluginATestDto/page/search",
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      data: body,
      ...(options || {}),
    }
  );
}

/** 获取回收站数据 检索并返回已软删除的记录（回收站数据）。 GET /api/plugins/11375910391972869/PluginATestDto/recyclebin */
export async function getRecyclebin(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: APIModel.getRecyclebinParams,
  options?: { [key: string]: any }
) {
  return request<APIModel.PluginATestDtoPageResult>(
    "/api/plugins/11375910391972869/PluginATestDto/recyclebin",
    {
      method: "GET",
      params: {
        // pageIndex has a default value: 1
        pageIndex: "1",
        // pageSize has a default value: 10
        pageSize: "10",
        ...params,
      },
      ...(options || {}),
    }
  );
}

/** 永久删除回收站中的记录 根据提供的ID，将已软删除的记录从数据库中永久移除。 DELETE /api/plugins/11375910391972869/PluginATestDto/recyclebin/${param0}/permanent */
export async function deleteRecyclebinIdPermanent(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: APIModel.deleteRecyclebinIdPermanentParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<boolean>(
    `/api/plugins/11375910391972869/PluginATestDto/recyclebin/${param0}/permanent`,
    {
      method: "DELETE",
      params: { ...queryParams },
      ...(options || {}),
    }
  );
}

/** 恢复回收站中的记录 根据提供的ID，将已软删除的记录恢复到正常状态。 PUT /api/plugins/11375910391972869/PluginATestDto/recyclebin/${param0}/restore */
export async function putRecyclebinIdRestore(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: APIModel.putRecyclebinIdRestoreParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<boolean>(
    `/api/plugins/11375910391972869/PluginATestDto/recyclebin/${param0}/restore`,
    {
      method: "PUT",
      params: { ...queryParams },
      ...(options || {}),
    }
  );
}

/** 根据条件永久删除回收站中的记录 根据提供的条件，将回收站中符合条件的记录从数据库中永久移除。 POST /api/plugins/11375910391972869/PluginATestDto/recyclebin/permanent */
export async function postRecyclebinPermanent(
  body: APIModel.PluginATestBooleanFuncExpression,
  options?: { [key: string]: any }
) {
  return request<number>(
    "/api/plugins/11375910391972869/PluginATestDto/recyclebin/permanent",
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

/** 批量恢复回收站中的记录 根据提供的条件，批量将回收站中的记录恢复到正常状态。 POST /api/plugins/11375910391972869/PluginATestDto/recyclebin/restore */
export async function postRecyclebinRestore(
  body: APIModel.PluginATestBooleanFuncExpression,
  options?: { [key: string]: any }
) {
  return request<number>(
    "/api/plugins/11375910391972869/PluginATestDto/recyclebin/restore",
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

/** 根据条件查询回收站数据 根据提供的查询条件，检索回收站中的记录。 POST /api/plugins/11375910391972869/PluginATestDto/recyclebin/search */
export async function postRecyclebinSearch(
  body: APIModel.PageQueryByConditionDto,
  options?: { [key: string]: any }
) {
  return request<APIModel.PluginATestDtoPageResult>(
    "/api/plugins/11375910391972869/PluginATestDto/recyclebin/search",
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      data: body,
      ...(options || {}),
    }
  );
}

/** 此处后端没有提供注释 GET /api/plugins/11375910391972869/PluginATestDto/test */
export async function getTest(options?: { [key: string]: any }) {
  return request<any>("/api/plugins/11375910391972869/PluginATestDto/test", {
    method: "GET",
    ...(options || {}),
  });
}
