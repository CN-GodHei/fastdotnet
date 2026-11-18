namespace __PLUGIN_NAME__.Controllers;

/// <summary>
/// 示例控制器。
/// 路由会被宿主重写为 /api/plugins/{admin|app|shared}/p{PluginId}/MyThing；
/// 缺少 [ApiUsageScope] 会导致整个控制器被移除（404）。
/// </summary>
[ApiController]
[Route("api/[controller]")]
[ApiUsageScope(ApiUsageScopeEnum.AdminOnly)]
public class MyThingController : ControllerBase
{
    private readonly MyThingService _service;

    public MyThingController(MyThingService service) => _service = service;

    [HttpGet]
    [Authorize(Policy = __PLUGIN_NAME__Permissions.ThingView)]
    public Task<List<MyThingDto>> GetAll(CancellationToken ct) => _service.GetAllAsync(ct);

    [HttpGet("{id}")]
    [Authorize(Policy = __PLUGIN_NAME__Permissions.ThingView)]
    public Task<MyThingDto> Get(string id, CancellationToken ct) => _service.GetAsync(id, ct);

    [HttpPost]
    [Authorize(Policy = __PLUGIN_NAME__Permissions.ThingCreate)]
    public Task<MyThingDto> Create([FromBody] MyThingCreateDto dto, CancellationToken ct)
        => _service.CreateAsync(dto, ct);

    [HttpDelete("{id}")]
    [Authorize(Policy = __PLUGIN_NAME__Permissions.ThingDelete)]
    public Task<bool> Delete(string id) => _service.DeleteAsync(id);

    /// <summary>匿名健康检查，便于联调。</summary>
    [HttpGet("ping")]
    [AllowAnonymous]
    public object Ping() => new { ok = true, plugin = "__PLUGIN_NAME__", at = DateTime.Now };
}
