namespace Fastdotnet.Core.IService
{
    public interface ICurrentUser
    {
        bool IsAuthenticated { get; }
        long? Id { get; }
        string UserName { get; }
        string UserType { get; } // 用于区分 Admin/App
    }
}