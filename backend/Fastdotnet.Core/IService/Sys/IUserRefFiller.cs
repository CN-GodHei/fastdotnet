
namespace Fastdotnet.Core.IService.Sys
{
    public interface IUserRefFiller
    {
        Task FillNamesAsync<T>(
            IList<T> dtos,
            SystemCategory systemCategory,
            params Expression<Func<T, UserRefDto>>[] userRefSelectors);
    }
}
