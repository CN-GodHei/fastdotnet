namespace Fastdotnet.Core.Models.Interfaces;

/// <summary>
/// 软删除接口
/// </summary>
public interface ISoftDelete
{
    /// <summary>
    /// 是否删除
    /// </summary>
    bool IsDeleted { get; set; }

    /// <summary>
    /// 删除时间
    /// </summary>
    DateTime? DeletedAt { get; set; }
}