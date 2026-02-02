

namespace Fastdotnet.Core.Dtos
{
    /// <summary>
    /// 分页信息实体
    /// </summary>
    public record PageInfo : InputPage
    {
        /// <summary>
        /// 总记录数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages
        {
            get
            {
                if (PageSize == 0) return 0;
                var totalPages = Total / PageSize;
                if (Total % PageSize > 0)
                    totalPages++;
                return totalPages;
            }
        }

        /// <summary>
        /// 是否有上一页
        /// </summary>
        public bool HasPreviousPage => Page > 1;

        /// <summary>
        /// 是否有下一页
        /// </summary>
        public bool HasNextPage => Page < TotalPages;
    }

    /// <summary>
    /// 分页结果实体
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public record PageResult<T>
    {
        /// <summary>
        /// 分页信息
        /// </summary>
        public PageInfo PageInfo { get; set; }

        /// <summary>
        /// 数据列表
        /// </summary>
        public IList<T> Items { get; set; }

        public PageResult()
        {
            PageInfo = new PageInfo();
            Items = new List<T>();
        }

        public PageResult(IList<T> items, int total, int page, int pageSize)
        {
            Items = items;
            PageInfo = new PageInfo
            {
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }
    }

    public record InputPage
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// 每页大小
        /// </summary>
        public int PageSize { get; set; } = 10;
    }
}
