
namespace Fastdotnet.Core.IService
{
    /// <summary>
    /// 长整型主键仓储接口
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    public interface IBaseRepository<T> : IRepository<T, long> where T : BaseEntity, new()
    {
    }
}