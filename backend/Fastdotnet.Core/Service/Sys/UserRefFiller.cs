
namespace Fastdotnet.Core.Service.Sys
{
    public class UserRefFiller : IUserRefFiller
    {
        private readonly IUserDisplayNameService _displayNameService;

        public UserRefFiller(IUserDisplayNameService displayNameService)
        {
            _displayNameService = displayNameService;
        }

        public async Task FillNamesAsync<T>(
            IList<T> dtos,
            SystemCategory systemCategory,
            params Expression<Func<T, UserRefDto>>[] userRefSelectors)
        {
            if (dtos == null || !dtos.Any() || userRefSelectors == null) return;

            var userIds = new HashSet<string>();
            var compiledSelectors = userRefSelectors.Select(s => s.Compile()).ToList();

            // 收集所有用户 ID
            foreach (var dto in dtos)
            {
                foreach (var selector in compiledSelectors)
                {
                    var userRef = selector(dto);
                    if (!string.IsNullOrWhiteSpace(userRef?.Id))
                        userIds.Add(userRef.Id);
                }
            }

            if (userIds.Count == 0) return;

            // 批量查名称
            var nameMap = await _displayNameService.GetDisplayNamesAsync(userIds, systemCategory);

            // 回填
            foreach (var dto in dtos)
            {
                foreach (var selector in compiledSelectors)
                {
                    var userRef = selector(dto);
                    if (userRef != null && nameMap.TryGetValue(userRef.Id, out var name))
                    {
                        userRef.Name = name;
                    }
                }
            }
        }
    }
}
