namespace __PLUGIN_NAME__.Services;

/// <summary>
/// 示例业务服务。控制器只做编排，数据访问放在这里。
/// </summary>
public class MyThingService
{
    private readonly IRepository<MyThing> _repo;
    private readonly ILogger<MyThingService> _logger;

    public MyThingService(IRepository<MyThing> repo, ILogger<MyThingService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<List<MyThingDto>> GetAllAsync(CancellationToken ct = default)
        => (await _repo.GetListAsync(null, ct)).Adapt<List<MyThingDto>>();

    public async Task<MyThingDto> GetAsync(string id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        if (entity is null) throw new BusinessException($"物品 {id} 不存在");
        return entity.Adapt<MyThingDto>();
    }

    public async Task<MyThingDto> CreateAsync(MyThingCreateDto dto, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new BusinessException("名称不能为空");

        var entity = dto.Adapt<MyThing>();
        await _repo.InsertAsync(entity);          // InsertAsync 没有 CancellationToken 重载
        _logger.LogInformation("创建物品 {Id} {Name}", entity.Id, entity.Name);
        return entity.Adapt<MyThingDto>();
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return false;
        return await _repo.DeleteAsync(id);
    }
}
