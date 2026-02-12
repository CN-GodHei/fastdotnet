namespace Fastdotnet.Service
{

    /// <summary>
    /// AutoMapper配置文件
    /// </summary>
    public class FdAppUserRoleProfile : Profile
    {
        public FdAppUserRoleProfile()
        {
            // Source -> Target
            CreateMap<FdAppUserRole, FdAppUserRoleDto>().MaskSensitiveData();
            CreateMap<CreateFdAppUserRoleDto, FdAppUserRole>();
            CreateMap<UpdateFdAppUserRoleDto, FdAppUserRole>();
        }
    }

    /// <summary>
    ///新增传输模型
    /// </summary>
    public class CreateFdAppUserRoleDto
    {

        /// <summary>
        /// 应用用户ID
        /// </summary>
        [StringLength(255, ErrorMessage = "应用用户ID最多255个字符")]
        public string AppUserId { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        [StringLength(255, ErrorMessage = "角色ID最多255个字符")]
        public string RoleId { get; set; }
    }

    /// <summary>
    ///修改传输模型
    /// </summary>
    public class UpdateFdAppUserRoleDto
    {

        /// <summary>
        /// 应用用户ID
        /// </summary>
        [StringLength(255, ErrorMessage = "应用用户ID最多255个字符")]
        public string AppUserId { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        [StringLength(255, ErrorMessage = "角色ID最多255个字符")]
        public string RoleId { get; set; }
    }

    /// <summary>
    ///输出传输模型
    /// </summary>
    public class FdAppUserRoleDto
    {

        /// <summary>
        /// 主键id
        /// </summary>

        public string Id { get; set; }

        /// <summary>
        /// 应用用户ID
        /// </summary>

        public string AppUserId { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>

        public string RoleId { get; set; }
    }
}