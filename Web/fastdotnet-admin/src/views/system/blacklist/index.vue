<template>
  <div class="system-blacklist-container layout-padding">
    <div class="system-blacklist-padding layout-padding-auto layout-padding-view">
      <!-- 搜索与操作区域 -->
      <div class="system-blacklist-search mb15">
        <el-row :gutter="10">
          <el-col :xs="24" :sm="12" :md="8" :lg="6" :xl="4">
            <el-select 
              v-model="state.searchForm.Type" 
              placeholder="请选择类型" 
              clearable 
              style="width: 100%;"
            >
              <el-option label="IP" value="IP"></el-option>
              <el-option label="User" value="User"></el-option>
              <el-option label="ApiKey" value="ApiKey"></el-option>
            </el-select>
          </el-col>
          <el-col :xs="24" :sm="12" :md="8" :lg="6" :xl="4">
            <el-input 
              v-model="state.searchForm.Value" 
              placeholder="请输入值" 
              clearable 
              style="width: 100%;"
              @keyup.enter="handleSearch"
            />
          </el-col>
          <el-col :xs="24" :sm="12" :md="8" :lg="6" :xl="4">
            <el-input 
              v-model="state.searchForm.Reason" 
              placeholder="请输入原因" 
              clearable 
              style="width: 100%;"
              @keyup.enter="handleSearch"
            />
          </el-col>
          <el-col :xs="24" :sm="12" :md="8" :lg="6" :xl="4">
            <el-button size="default" type="primary" class="ml10" @click="handleSearch">
              <el-icon><ele-Search /></el-icon>
              查询
            </el-button>
            <el-button size="default" @click="handleReset">
              <el-icon><ele-Refresh /></el-icon>
              重置
            </el-button>
            <el-button size="default" type="success" class="ml10" @click="onOpenAddBlacklist('add')">
              <el-icon><ele-FolderAdd /></el-icon>
              新增黑名单
            </el-button>
          </el-col>
        </el-row>
      </div>

      <!-- 数据表格 -->
      <el-table 
        :data="state.tableData.data" 
        v-loading="state.tableData.loading" 
        style="width: 100%"
        stripe
        border
      >
        <el-table-column type="index" label="序号" width="60" align="center" />
        <el-table-column prop="Type" label="类型" show-overflow-tooltip />
        <el-table-column prop="Value" label="值" show-overflow-tooltip />
        <el-table-column prop="Reason" label="原因" show-overflow-tooltip />
        <el-table-column prop="ExpiredAt" label="过期时间" width="180" align="center">
          <template #default="scope">
            {{ scope.row.ExpiredAt ? formatDate(scope.row.ExpiredAt) : '-' }}
          </template>
        </el-table-column>
        <el-table-column prop="CreateTime" label="创建时间" width="180" align="center">
          <template #default="scope">
            {{ formatDate(scope.row.CreateTime) }}
          </template>
        </el-table-column>
        <el-table-column prop="IsSystem" label="系统内置" width="100" align="center">
          <template #default="scope">
            <el-tag :type="scope.row.IsSystem ? 'success' : 'info'">
              {{ scope.row.IsSystem ? '是' : '否' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="160" align="center" fixed="right">
          <template #default="scope">
            <el-button size="small" text type="primary" @click="onOpenEditBlacklist('edit', scope.row)" :disabled="scope.row.isSystem">
              修改
            </el-button>
            <el-button size="small" text type="primary" @click="onRowDel(scope.row)" :disabled="scope.row.isSystem">
              删除
            </el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分页 -->
      <el-pagination
        @size-change="onHandleSizeChange"
        @current-change="onHandleCurrentChange"
        class="mt15"
        :pager-count="5"
        :page-sizes="[10, 20, 30]"
        v-model:current-page="state.tableData.param.pageNum"
        background
        v-model:page-size="state.tableData.param.pageSize"
        layout="total, sizes, prev, pager, next, jumper"
        :total="state.tableData.total"
      >
      </el-pagination>
    </div>

    <!-- 新增/编辑弹窗 -->
    <BlacklistDialog ref="blacklistDialogRef" @refresh="getTableData()" />
  </div>
</template>

<script setup lang="ts" name="systemBlacklist">
import { defineAsyncComponent, reactive, onMounted, ref } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import {
  postAdminBlacklistsPageSearch,
  deleteAdminBlacklistsId
} from '/@/api/fd-system-api/Blacklists';
import type { FdBlacklistDto } from '/@/api/fd-system-api/typings';
import { buildMixedQuery } from '/@/utils/queryBuilder';

// 引入组件
const BlacklistDialog = defineAsyncComponent(() => import('./dialog.vue'));

// 定义变量内容
const blacklistDialogRef = ref();
const state = reactive<SystemBlacklistState>({
  tableData: {
    data: [],
    total: 0,
    loading: false,
    param: {
      pageNum: 1,
      pageSize: 10,
    },
  },
  searchForm: {
    Type: '',
    Value: '',
    Reason: ''
  }
});

// 初始化表格数据
const getTableData = async () => {
  state.tableData.loading = true;
  try {
    // 构建搜索条件
    const searchBody: APIModel.PageQueryByConditionDto = {
      PageIndex: state.tableData.param.pageNum,
      PageSize: state.tableData.param.pageSize
    };
    
    // 使用共用方法构建混合查询条件
    const queryConfig = {
      equals: {
        Type: state.searchForm.Type
      },
      contains: {
        Value: state.searchForm.Value,
        Reason: state.searchForm.Reason
      }
    };
    
    const queryResult = buildMixedQuery(queryConfig);
    
    if (queryResult.dynamicQuery) {
      searchBody.DynamicQuery = queryResult.dynamicQuery;
      searchBody.QueryParameters = queryResult.queryParameters;
    }
    
    // 调试日志
    console.log('Search request body:', searchBody);
    
    // 调用新的接口
    const response = await postAdminBlacklistsPageSearch(searchBody);
    
    state.tableData.data = response.Items || [];
    state.tableData.total = response.PageInfo?.Total || 0;
  } catch (error) {
    ElMessage.error('获取数据失败');
    console.error(error);
  } finally {
    state.tableData.loading = false;
  }
};

// 打开新增弹窗
const onOpenAddBlacklist = (type: string) => {
  blacklistDialogRef.value.openDialog(type);
};

// 打开编辑弹窗
const onOpenEditBlacklist = (type: string, row: FdBlacklistDto) => {
  blacklistDialogRef.value.openDialog(type, row);
};

// 删除记录
const onRowDel = (row: FdBlacklistDto) => {
  ElMessageBox.confirm(`此操作将永久删除值为：“${row.Value}”的记录，是否继续?`, '提示', {
    confirmButtonText: '确认',
    cancelButtonText: '取消',
    type: 'warning',
  })
  .then(async () => {
    try {
      await deleteAdminBlacklistsId({ params: { id: row.Id! } });
      ElMessage.success('删除成功');
      getTableData();
    } catch (error) {
      ElMessage.error('删除失败');
      console.error(error);
    }
  })
  .catch(() => {});
};

// 处理搜索
const handleSearch = () => {
  state.tableData.param.pageNum = 1;
  getTableData();
};

// 重置搜索
const handleReset = () => {
  state.searchForm.Type = '';
  state.searchForm.Value = '';
  state.searchForm.Reason = '';
  state.tableData.param.pageNum = 1;
  getTableData();
};

// 分页改变
const onHandleSizeChange = (val: number) => {
  state.tableData.param.pageSize = val;
  state.tableData.param.pageNum = 1;
  getTableData();
};

// 分页改变
const onHandleCurrentChange = (val: number) => {
  state.tableData.param.pageNum = val;
  getTableData();
};

// 格式化日期
const formatDate = (dateStr: string | undefined) => {
  if (!dateStr) return '-';
  const date = new Date(dateStr);
  return isNaN(date.getTime()) ? '-' : date.toLocaleString('zh-CN');
};

// 页面加载时
onMounted(() => {
  getTableData();
});
</script>

<style scoped lang="scss">
.system-blacklist-container {
  .system-blacklist-padding {
    padding: 15px;
    .el-table {
      flex: 1;
    }
  }
}
</style>