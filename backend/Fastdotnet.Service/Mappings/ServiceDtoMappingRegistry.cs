using Fastdotnet.Core.Dtos.Admin;
using Fastdotnet.Core.Dtos.Admin.Users;
using Fastdotnet.Core.Dtos.App;
using Fastdotnet.Core.Dtos.Common;
using Fastdotnet.Core.Dtos.Sys;
using Fastdotnet.Core.Entities.Admin;
using Fastdotnet.Core.Entities.App;
using Fastdotnet.Core.Entities.Sys;
using Fastdotnet.Core.Extensions;
using Mapster;

namespace Fastdotnet.Service.Mappings
{
    /// <summary>
    /// Service 层 DTO 映射配置（从 AutoMapper Profile 迁移）
    /// </summary>
    public class ServiceDtoMappingRegistry : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            System.Console.WriteLine("[DEBUG] ServiceDtoMappingRegistry.Register 被调用");
            
            // FdAdminUserRole (从 Core 层迁移)
            config.NewConfig<FdAdminUserRole, FdAdminUserRoleDto>()
                .MaskSensitiveData();
            config.NewConfig<CreateFdAdminUserRoleDto, FdAdminUserRole>();
            config.NewConfig<UpdateFdAdminUserRoleDto, FdAdminUserRole>();

            // FdAppUserRole (从 Core 层迁移)
            config.NewConfig<FdAppUserRole, FdAppUserRoleDto>()
                .MaskSensitiveData();
            config.NewConfig<CreateFdAppUserRoleDto, FdAppUserRole>();
            config.NewConfig<UpdateFdAppUserRoleDto, FdAppUserRole>();

            // FdNationalStandardItem (从 Core 层迁移)
            config.NewConfig<FdNationalStandardItem, FdNationalStandardItemDto>()
                .MaskSensitiveData();
            config.NewConfig<CreateFdNationalStandardItemDto, FdNationalStandardItem>();
            config.NewConfig<UpdateFdNationalStandardItemDto, FdNationalStandardItem>();

            // FdMenuButton (从 Core 层迁移)
            config.NewConfig<FdMenuButton, FdMenuButtonDto>()
                .MaskSensitiveData();
            config.NewConfig<CreateFdFdMenuButtonDto, FdMenuButton>();
            config.NewConfig<UpdateFdFdMenuButtonDto, FdMenuButton>();

            // AdminUserProfile
            config.NewConfig<FdAdminUser, FdAdminUserDto>()
                .MaskSensitiveData();
            config.NewConfig<CreateFdAdminUserDto, FdAdminUser>();
            config.NewConfig<UpdateFdAdminUserDto, FdAdminUser>();

            // RoleProfile
            config.NewConfig<FdRole, FdRoleDto>()
                .MaskSensitiveData();
            config.NewConfig<CreateFdRoleDto, FdRole>();
            config.NewConfig<UpdateFdRoleDto, FdRole>();

            // PermissionProfile
            config.NewConfig<FdPermission, FdPermissionDto>()
                .MaskSensitiveData();

            // CommonProfile - string → UserRefDto
            config.NewConfig<string, UserRefDto>()
                .MapWith(id => new UserRefDto { Id = id ?? string.Empty });

            // FdTodoTaskProfile - Sys (管理端)
            config.NewConfig<Fastdotnet.Core.Entities.Sys.FdTodoTask, Fastdotnet.Core.Dtos.Sys.FdTodoTaskDto>();
            config.NewConfig<Fastdotnet.Core.Dtos.Sys.CreateFdTodoTaskDto, Fastdotnet.Core.Entities.Sys.FdTodoTask>();
            config.NewConfig<Fastdotnet.Core.Dtos.Sys.UpdateFdTodoTaskDto, Fastdotnet.Core.Entities.Sys.FdTodoTask>();

            // FdTodoTaskProfile - App (应用端)
            config.NewConfig<Fastdotnet.Core.Entities.App.FdTodoTask, Fastdotnet.Core.Dtos.App.FdTodoTaskDto>();
            config.NewConfig<Fastdotnet.Core.Dtos.App.CreateFdTodoTaskDto, Fastdotnet.Core.Entities.App.FdTodoTask>();
            config.NewConfig<Fastdotnet.Core.Dtos.App.UpdateFdTodoTaskDto, Fastdotnet.Core.Entities.App.FdTodoTask>();

            // FdNoticeProfile - Sys (管理端)
            config.NewConfig<Fastdotnet.Core.Entities.Sys.FdNotice, Fastdotnet.Core.Dtos.Sys.FdNoticeDto>();
            config.NewConfig<Fastdotnet.Core.Dtos.Sys.CreateFdNoticeDto, Fastdotnet.Core.Entities.Sys.FdNotice>();
            config.NewConfig<Fastdotnet.Core.Dtos.Sys.UpdateFdNoticeDto, Fastdotnet.Core.Entities.Sys.FdNotice>();

            // FdNoticeProfile - App (应用端)
            config.NewConfig<Fastdotnet.Core.Entities.App.FdNotice, Fastdotnet.Core.Dtos.App.FdNoticeDto>();
            config.NewConfig<Fastdotnet.Core.Dtos.App.CreateFdNoticeDto, Fastdotnet.Core.Entities.App.FdNotice>();
            config.NewConfig<Fastdotnet.Core.Dtos.App.UpdateFdNoticeDto, Fastdotnet.Core.Entities.App.FdNotice>();

            // FdWorkbenchProfile
            config.NewConfig<FdWorkbenchCard, FdWorkbenchCardDto>()
                .MaskSensitiveData();
            config.NewConfig<CreateFdWorkbenchCardDto, FdWorkbenchCard>();
            config.NewConfig<UpdateFdWorkbenchCardDto, FdWorkbenchCard>();

            // FdUserLayout (应用端布局)
            config.NewConfig<FdUserLayout, FdUserLayoutDto>();
            config.NewConfig<SaveFdUserLayoutDto, FdUserLayout>();

            // FdAppUserProfile
            config.NewConfig<FdAppUser, FdAppUserDto>()
                .MaskSensitiveData();
            config.NewConfig<CreateFdAppUserDto, FdAppUser>();
            config.NewConfig<UpdateFdAppUserDto, FdAppUser>();

            // FdBlacklistProfile
            config.NewConfig<FdBlacklist, FdBlacklistDto>()
                .MaskSensitiveData();
            config.NewConfig<CreateFdBlacklistDto, FdBlacklist>();
            config.NewConfig<UpdateFdBlacklistDto, FdBlacklist>();

            // FdDictDataProfile
            config.NewConfig<FdDictData, FdDictDataDto>()
                .Map(dest => dest.ValueType, src => (int)src.ValueType)
                .MaskSensitiveData();
            config.NewConfig<CreateFdDictDataDto, FdDictData>()
                .Map(dest => dest.ValueType, src => (DictValueType)src.ValueType);
            config.NewConfig<UpdateFdDictDataDto, FdDictData>()
                .Map(dest => dest.ValueType, src => (DictValueType)src.ValueType);

            // FdDictTypeProfile
            config.NewConfig<FdDictType, FdDictTypeDto>()
                .MaskSensitiveData();
            config.NewConfig<CreateFdDictTypeDto, FdDictType>();
            config.NewConfig<UpdateFdDictTypeDto, FdDictType>();

            // FdMenuProfile
            config.NewConfig<FdMenu, FdMenuDto>()
                .MaskSensitiveData()
                .Map(dest => dest.Creator, src => new UserRefDto { Id = src.CreatedBy ?? string.Empty })
                .Map(dest => dest.Updater, src => new UserRefDto { Id = src.UpdatedBy ?? string.Empty })
                .Map(dest => dest.Deleter, src => new UserRefDto { Id = src.DeletedBy ?? string.Empty });
            config.NewConfig<CreateFdMenuDto, FdMenu>();
            config.NewConfig<UpdateFdMenuDto, FdMenu>();

            // FdRateLimitRuleProfile
            config.NewConfig<FdRateLimitRule, FdRateLimitRuleDto>()
                .MaskSensitiveData();
            config.NewConfig<CreateFdRateLimitRuleDto, FdRateLimitRule>();
            config.NewConfig<UpdateFdRateLimitRuleDto, FdRateLimitRule>();

            // EmailConfigMappingProfile
            config.NewConfig<EmailConfig, FdEmailConfigDto>()
                .MaskSensitiveData();
            config.NewConfig<FdCreateEmailConfigDto, EmailConfig>();
            config.NewConfig<FdUpdateEmailConfigDto, EmailConfig>();

            // SystemConfigMappingProfile
            config.NewConfig<SystemInfoConfig, FdSystemInfoConfigDto>()
                .MaskSensitiveData();
            config.NewConfig<CreateFdSystemInfoConfigDto, SystemInfoConfig>();
            config.NewConfig<UpdateFdSystemInfoConfigDto, SystemInfoConfig>();

            // 注意：CodeGenProfile 和 CodeGenConfigProfile 有复杂的自定义映射逻辑（JSON序列化/反序列化）
            // 需要单独创建 IRegister 来处理这些复杂映射
        }
    }
}
