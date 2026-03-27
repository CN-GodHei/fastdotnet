namespace Fastdotnet.Core.Dtos.Sys
{
    /// <summary>
    /// 树形结构 DTO
    /// </summary>
    /// <typeparam name="T">节点数据类型</typeparam>
    public class TreeModel<T>
    {
        /// <summary>
        /// 树形数据
        /// </summary>
        public List<T> TreeData { get; set; } = new();

        /// <summary>
        /// 总数
        /// </summary>
        public int Total { get; set; }
    }
}
