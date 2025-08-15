// 分页查询参数
export interface PageQuery {
  pageIndex?: number
  pageSize?: number
  [key: string]: any // 其他查询参数
}

// 分页结果
export interface PageResult<T> {
  items: T[]
  total: number
  pageIndex: number
  pageSize: number
  totalPages: number
}