// 分页查询参数
export interface PageQuery {
  pageIndex?:string
  pageSize?:string
  [key: string]: any // 其他查询参数
}

// 分页结果
export interface PageResult<T> {
  items: T[]
  total:string
  pageIndex:string
  pageSize:string
  totalPages:string
}