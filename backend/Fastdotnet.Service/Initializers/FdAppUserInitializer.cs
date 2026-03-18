
using Fastdotnet.Core.Entities.Sys;
using Fastdotnet.Service.IService.Sys;

namespace Fastdotnet.Service.Initializers
{
    public class FdAppUserInitializer : IApplicationInitializer
    {
        private readonly IRepository<FdAppUser> _Repository;
        private readonly IRepository<FdAppUserRole> _UserRoleRepository;
        private readonly IRepository<FdRole> _fdroleRepository;
        private readonly IRepository<FdRoleMenu> _fdroleMenuRepository;
        private readonly IFdRoleInitializerService _fdRoleInitializer;

        public FdAppUserInitializer(IRepository<FdAppUser> Repository, IRepository<FdAppUserRole> userRoleRepository,
            IRepository<FdRole> fdroleRepository, IRepository<FdRoleMenu> fdroleMenuRepository
            ,IFdRoleInitializerService fdRoleInitializer

            )
        {
            _Repository = Repository;
            _UserRoleRepository = userRoleRepository;
            _fdroleRepository = fdroleRepository;
            _fdroleMenuRepository = fdroleMenuRepository;
            _fdRoleInitializer = fdRoleInitializer;
        }

        public async Task InitializeAsync()
        {
            //if (await _Repository.ExistsAsync(a => a.Id != null))
            //{
            //    return;
            //}
            var entitys = new List<FdAppUser>
            {
                new FdAppUser { Id = "12055423131517957", Username = "admintest", Nickname = "AdminTest", Email = "fastdotnet@test.com", Password = "123456", PhoneNumber = "159****7417" }
            };

            // 直接使用条件查询已存在的项
            var existing = await _Repository.GetListAsync(m => entitys.Select(c => c.Id).Contains(m.Id));
            var existingIds = existing.Select(m => m.Id).ToHashSet();

            // 只插入不存在的项
            var ToInsert = entitys.Where(c => !existingIds.Contains(c.Id)).ToList();

            if (ToInsert.Any())
            {
                await _Repository.InsertRangeAsync(ToInsert);
            }

            //默认角色菜单不做硬编码:系统初始化完成后，会自动创建默认角色，从后台配置默认角色能访问的菜单
            
            //const string APP_DEFAULT_ROLECODE = "APP_ROLE_001";
            //await _fdRoleInitializer.RoleInitializer();
            //var userrole = await _UserRoleRepository.GetFirstAsync(x => x.AppUserId == entitys.FirstOrDefault().Id);
            //var temp = _fdroleRepository.GetFirstAsync(x => x.Code == APP_DEFAULT_ROLECODE).GetAwaiter().GetResult();
            //if (userrole == null)
            //{
            //    await _UserRoleRepository.InsertAsync(new FdAppUserRole { RoleId = temp.Id, AppUserId = entitys.FirstOrDefault().Id });
            //}
            //string appUserDefaultMenuId = "12262382148846597";
            ////APP端默认角色可用菜单
            //var appUserDefaultMenu = await _fdroleMenuRepository.GetFirstAsync(w => w.RoleId == temp.Id && w.MenuId == appUserDefaultMenuId);
            //if (appUserDefaultMenu == null)
            //{
            //    await _fdroleMenuRepository.InsertAsync(new FdRoleMenu { RoleId = temp.Id, MenuId = appUserDefaultMenuId });
            //}
        }
    }
}
