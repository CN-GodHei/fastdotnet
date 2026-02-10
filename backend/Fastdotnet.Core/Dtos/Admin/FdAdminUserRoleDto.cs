namespace Fastdotnet.Service
{

    /// <summary>
    /// AutoMapper配置文件
    /// </summary>
    public class FdAdminUserRoleProfile : Profile
    {
        public FdAdminUserRoleProfile()
        {
            // Source -> Target
            CreateMap<FdAdminUserRole, FdAdminUserRoleDto>().MaskSensitiveData();
            CreateMap<CreateFdAdminUserRoleDto, FdAdminUserRole>();
            CreateMap<UpdateFdAdminUserRoleDto, FdAdminUserRole>();
        }
    }

    /// <summary>
    ///新增传输模型
    /// </summary>
    public class CreateFdAdminUserRoleDto
    {

        /// <summary>
        /// 管理员用户ID
        /// </summary>
        [StringLength(255, ErrorMessage = "管理员用户ID最多255个字符")]
        public string AdminUserId { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        [StringLength(255, ErrorMessage = "角色ID最多255个字符")]
        public string RoleId { get; set; }
    }

    /// <summary>
    ///修改传输模型
    /// </summary>
    public class UpdateFdAdminUserRoleDto
    {

        /// <summary>
        /// 管理员用户ID
        /// </summary>
        [StringLength(255, ErrorMessage = "管理员用户ID最多255个字符")]
        public string AdminUserId { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        [StringLength(255, ErrorMessage = "角色ID最多255个字符")]
        public string RoleId { get; set; }
    }

    /// <summary>
    ///输出传输模型
    /// </summary>
    public class FdAdminUserRoleDto
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 管理员用户ID
        /// </summary>

        public string AdminUserId { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>

        public string RoleId { get; set; }
    }
}
