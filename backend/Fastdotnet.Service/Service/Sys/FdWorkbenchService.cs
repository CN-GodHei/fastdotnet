using AutoMapper;
using Fastdotnet.Core.Dtos.Sys;
using Fastdotnet.Core.Entities.Sys;
using Fastdotnet.Service.IService.Sys;

namespace Fastdotnet.Service.Service.Sys
{
    public class FdWorkbenchService : IFdWorkbenchService
    {
        private readonly IRepository<FdWorkbenchCard, string> _cardRepository;
        private readonly IRepository<FdRoleCard, string> _roleCardRepository;
        private readonly IMapper _mapper;

        public FdWorkbenchService(
            IRepository<FdWorkbenchCard, string> cardRepository,
            IRepository<FdRoleCard, string> roleCardRepository,
            IMapper mapper)
        {
            _cardRepository = cardRepository;
            _roleCardRepository = roleCardRepository;
            _mapper = mapper;
        }

        public async Task<List<FdWorkbenchCardDto>> GetAllCardsAsync()
        {
            var cards = await _cardRepository.GetAllAsync();
            return _mapper.Map<List<FdWorkbenchCardDto>>(cards.OrderBy(c => c.Name));
        }

        public async Task<string> CreateCardAsync(CreateFdWorkbenchCardDto dto)
        {
            var card = _mapper.Map<FdWorkbenchCard>(dto);
            card.Id = Guid.NewGuid().ToString("N");
            await _cardRepository.InsertAsync(card);
            return card.Id;
        }

        public async Task<bool> UpdateCardAsync(UpdateFdWorkbenchCardDto dto)
        {
            var card = await _cardRepository.GetByIdAsync(dto.Id);
            if (card == null) return false;

            _mapper.Map(dto, card);
            return await _cardRepository.UpdateAsync(card) != null;
        }

        public async Task<bool> DeleteCardAsync(string id)
        {
            return await _cardRepository.DeleteAsync(id);
        }

        public async Task<bool> AssignCardsToRoleAsync(string roleId, List<string> cardIds)
        {
            // 1. 删除旧的分配
            await _roleCardRepository.DeleteAsync(rc => rc.RoleId == roleId);

            // 2. 批量插入新分配
            var roleCards = cardIds.Select(cid => new FdRoleCard
            {
                Id = Guid.NewGuid().ToString("N"),
                RoleId = roleId,
                CardId = cid
            }).ToList();

            await _roleCardRepository.InsertRangeAsync(roleCards);
            return true;
        }

        public async Task<List<string>> GetRoleCardIdsAsync(string roleId)
        {
            var list = await _roleCardRepository.GetListAsync(rc => rc.RoleId == roleId);
            return list.Select(rc => rc.CardId).ToList();
        }

        public async Task<List<string>> GetCardRoleIdsAsync(string cardId)
        {
            var list = await _roleCardRepository.GetListAsync(rc => rc.CardId == cardId);
            return list.Select(rc => rc.RoleId).ToList();
        }

        public async Task<bool> UpdateCardRolesAsync(string cardId, List<string> roleIds)
        {
            // 1. 删除该卡片的所有旧角色分配
            await _roleCardRepository.DeleteAsync(rc => rc.CardId == cardId);

            // 2. 插入新分配
            var roleCards = roleIds.Select(rid => new FdRoleCard
            {
                Id = Guid.NewGuid().ToString("N"),
                CardId = cardId,
                RoleId = rid
            }).ToList();

            await _roleCardRepository.InsertRangeAsync(roleCards);
            return true;
        }
    }
}
