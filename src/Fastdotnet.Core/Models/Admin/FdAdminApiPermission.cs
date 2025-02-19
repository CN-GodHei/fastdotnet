using Fastdotnet.Core.Models.Base;

namespace Fastdotnet.Core.Models.Admin
{
    /// <summary>
    /// 接口权限表
    /// </summary>
    public class FdAdminApiPermission : BaseEntity
    {
        /// <summary>
        /// 接口名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 接口地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 请求方法（GET、POST、PUT、DELETE等）
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// 所属模块
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        /// 权限编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 状态（0：禁用，1：启用）
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 是否需要认证（0：不需要，1：需要）
        /// </summary>
        public bool RequireAuth { get; set; }

        /// <summary>
        /// 是否需要权限验证（0：不需要，1：需要）
        /// </summary>
        public bool RequirePermission { get; set; }
    }
}