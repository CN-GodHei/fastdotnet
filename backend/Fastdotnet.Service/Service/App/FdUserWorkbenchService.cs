using Fastdotnet.Core.Dtos.App;
using Fastdotnet.Core.Dtos.Sys;
using Fastdotnet.Core.Entities.App;
using Fastdotnet.Core.Entities.Sys;
using Fastdotnet.Core.Entities.Admin; // 引入 Admin 实体
using Fastdotnet.Service.IService.App;
using Fastdotnet.Service.IService.Admin;
using Fastdotnet.Core.IService;

namespace Fastdotnet.Service.Service.App
{
    public class FdUserWorkbenchService : IFdUserWorkbenchService
    {
        private readonly IRepository<FdWorkbenchCard, string> _cardRepository;
        private readonly IRepository<FdRoleCard, string> _roleCardRepository;
        private readonly IRepository<FdUserLayout, string> _layoutRepository;
        private readonly IAppUserService _appUserService;
        private readonly IAdminUserService _adminUserService;
        private readonly ICurrentUser _currentUser;

        public FdUserWorkbenchService(
            IRepository<FdWorkbenchCard, string> cardRepository,
            IRepository<FdRoleCard, string> roleCardRepository,
            IRepository<FdUserLayout, string> layoutRepository,
            IAppUserService appUserService,
            IAdminUserService adminUserService,
            ICurrentUser currentUser)
        {
            _cardRepository = cardRepository;
            _roleCardRepository = roleCardRepository;
            _layoutRepository = layoutRepository;
            _appUserService = appUserService;
            _adminUserService = adminUserService;
            _currentUser = currentUser;
        }

        public async Task<List<FdWorkbenchCardDto>> GetAvailableCardsAsync()
        {
            var userId = _currentUser.Id;
            if (string.IsNullOrEmpty(userId)) return new List<FdWorkbenchCardDto>();

            // 1. 获取用户角色列表 (根据用户类型选择不同的表)
            List<string> roleIds;
            if (_currentUser.UserType == "Admin")
            {
                roleIds = (await _adminUserService.GetUserRoleRelationsAsync(userId))
                          .Select(ur => ur.RoleId).ToList();
            }
            else
            {
                roleIds = (await _appUserService.GetUserRoleRelationsAsync(userId))
                          .Select(ur => ur.RoleId).ToList();
            }

            if (!roleIds.Any()) return new List<FdWorkbenchCardDto>();

            // 2. 获取角色关联的卡片ID
            var cardIds = (await _roleCardRepository.GetListAsync(rc => roleIds.Contains(rc.RoleId)))
                          .Select(rc => rc.CardId).Distinct().ToList();

            if (!cardIds.Any()) return new List<FdWorkbenchCardDto>();

            // 3. 获取卡片详情
            var cards = await _cardRepository.GetListAsync(c => cardIds.Contains(c.Id));
            return cards.Adapt<List<FdWorkbenchCardDto>>();
        }

        public async Task<FdUserLayoutDto?> GetMyLayoutAsync(string name = "Default")
        {
            var userId = _currentUser.Id;
            if (string.IsNullOrEmpty(userId)) return null;

            var layout = await _layoutRepository.GetFirstAsync(l => l.UserId == userId && l.Name == name);
            return layout.Adapt<FdUserLayoutDto>();
        }

        public async Task<bool> SaveMyLayoutAsync(SaveFdUserLayoutDto dto)
        {
            var userId = _currentUser.Id;
            if (string.IsNullOrEmpty(userId)) return false;

            var layout = await _layoutRepository.GetFirstAsync(l => l.UserId == userId && l.Name == dto.Name);

            if (layout == null)
            {
                layout = new FdUserLayout
                {
                    Id = Guid.NewGuid().ToString("N"),
                    UserId = userId,
                    Name = dto.Name,
                    LayoutData = dto.LayoutData,
                    IsActive = 1,
                    CreatedAt = DateTime.Now
                };
                return await _layoutRepository.InsertAsync(layout) != null;
            }
            else
            {
                layout.LayoutData = dto.LayoutData;
                layout.UpdatedAt = DateTime.Now;
                return await _layoutRepository.UpdateAsync(layout) != null;
            }
        }
    }
}
