using Fastdotnet.Core.Dtos.Sys;
using Fastdotnet.Core.Entities.Sys;
using Fastdotnet.Service.IService.Sys;
using Microsoft.AspNetCore.Mvc;

namespace Fastdotnet.WebApi.Controllers.Admin
{
    /// <summary>
    /// 工作台卡片库管理 (管理端)
    /// </summary>
    [Route("api/admin/[controller]")]
    public class FdWorkbenchController : GenericDtoControllerBase<FdWorkbenchCard, string, CreateFdWorkbenchCardDto, UpdateFdWorkbenchCardDto, FdWorkbenchCardDto>
    {
        private readonly IFdWorkbenchService _workbenchService;

        public FdWorkbenchController(
            IFdWorkbenchService workbenchService,
            IBaseService<FdWorkbenchCard, string> service,
            IMapper mapper) : base(service, mapper)
        {
            _workbenchService = workbenchService;
        }

        /// <summary>
        /// 获取所有定义的卡片
        /// </summary>
        [HttpGet("all")]
        public async Task<List<FdWorkbenchCardDto>> GetAll()
        {
            return await _workbenchService.GetAllCardsAsync();
        }

        /// <summary>
        /// 为角色分配卡片
        /// </summary>
        [HttpPost("assign/{roleId}")]
        public async Task<bool> Assign(string roleId, [FromBody] List<string> cardIds)
        {
            return await _workbenchService.AssignCardsToRoleAsync(roleId, cardIds);
        }

        /// <summary>
        /// 获取角色已分配的卡片ID列表
        /// </summary>
        [HttpGet("role-cards/{roleId}")]
        public async Task<List<string>> GetRoleCards(string roleId)
        {
            return await _workbenchService.GetRoleCardIdsAsync(roleId);
        }

        /// <summary>
        /// 获取卡片已分配的角色ID列表
        /// </summary>
        [HttpGet("card-roles/{cardId}")]
        public async Task<List<string>> GetCardRoles(string cardId)
        {
            return await _workbenchService.GetCardRoleIdsAsync(cardId);
        }

        /// <summary>
        /// 为卡片分配角色
        /// </summary>
        [HttpPost("assign-roles/{cardId}")]
        public async Task<bool> UpdateCardRoles(string cardId, [FromBody] List<string> roleIds)
        {
            return await _workbenchService.UpdateCardRolesAsync(cardId, roleIds);
        }
    }
}
