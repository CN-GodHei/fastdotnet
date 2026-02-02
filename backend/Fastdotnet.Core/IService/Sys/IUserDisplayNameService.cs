
namespace Fastdotnet.Core.IService.Sys
{
    public interface IUserDisplayNameService
    {
        Task<Dictionary<string, string>> GetDisplayNamesAsync(IEnumerable<string> userIds, SystemCategory systemCategory);
    }
}
