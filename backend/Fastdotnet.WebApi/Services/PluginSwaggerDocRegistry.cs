using System.Collections.Concurrent;
using System.IO;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Fastdotnet.WebApi.Services;

/// <summary>
/// 运行时插件 Swagger 文档注册器（单例）
/// 在插件启用/禁用时动态注册/注销 SwaggerDoc 和 SwaggerUI 端点，
/// 安装新插件后无需重启即可在 Swagger "Select a definition" 中看到新文档
/// </summary>
public class PluginSwaggerDocRegistry
{
    private readonly ConcurrentDictionary<string, (string pluginName, string description, string entryPoint)> _plugins = new();
    private readonly IOptions<SwaggerGeneratorOptions> _swaggerGenOptions;
    private SwaggerUIOptions? _swaggerUIOptions;

    public PluginSwaggerDocRegistry(IOptions<SwaggerGeneratorOptions> swaggerGenOptions)
    {
        _swaggerGenOptions = swaggerGenOptions;
    }

    /// <summary>
    /// 设置 SwaggerUIOptions 引用（由 UseCustomSwagger 在配置完成后传入）
    /// </summary>
    public void SetSwaggerUIOptions(SwaggerUIOptions options)
    {
        _swaggerUIOptions = options;
    }

    /// <summary>
    /// 批量注册插件（启动时同步用，插件已在 AddCustomSwagger/UseCustomSwagger 中注册）
    /// </summary>
    public void BulkRegister(IEnumerable<(string id, string name, string description)> plugins)
    {
        foreach (var (id, name, description) in plugins)
        {
            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(name))
            {
                _plugins.TryAdd(id.ToLower(), (name, description ?? "", id));
            }
        }
    }

    /// <summary>
    /// 运行时注册单条插件文档（添加 SwaggerDoc + SwaggerUI 端点 + XML 注释）
    /// </summary>
    public bool RegisterPlugin(string pluginId, string pluginName, string description, string entryPoint = null)
    {
        var key = pluginId.ToLower();
        if (!_plugins.TryAdd(key, (pluginName, description ?? "", entryPoint ?? pluginId)))
            return false; // 已存在，不做重复注册

        var swaggerOptions = _swaggerGenOptions.Value;

        // 1. 注册 SwaggerDoc（使 swagger.json 可访问）
        swaggerOptions.SwaggerDocs[$"plugin-{key}-admin"] = new OpenApiInfo
        {
            Title = $"{pluginName} 插件 API (Admin)",
            Version = "v1",
            Description = $"Fastdotnet {description} - Admin端"
        };
        swaggerOptions.SwaggerDocs[$"plugin-{key}-app"] = new OpenApiInfo
        {
            Title = $"{pluginName} 插件 API (App)",
            Version = "v1",
            Description = $"Fastdotnet {description} - App端"
        };

        // 2. 添加 SwaggerUI 端点（刷新 Swagger 页面时生效）
        if (_swaggerUIOptions != null)
        {
            var config = _swaggerUIOptions.ConfigObject;
            var urls = config.Urls?.ToList() ?? new List<UrlDescriptor>();

            // 移除可能残留的旧条目
            urls.RemoveAll(u => u.Url != null && u.Url.Contains($"/plugin-{key}-admin"));
            urls.RemoveAll(u => u.Url != null && u.Url.Contains($"/plugin-{key}-app"));

            urls.Add(new UrlDescriptor
            {
                Url = $"/swagger/plugin-{key}-admin/swagger.json",
                Name = $"{pluginName} 插件 API (Admin) v1"
            });
            urls.Add(new UrlDescriptor
            {
                Url = $"/swagger/plugin-{key}-app/swagger.json",
                Name = $"{pluginName} 插件 API (App) v1"
            });

            config.Urls = urls;
        }

        // 3. 加载插件 XML 注释到运行时过滤器
        try
        {
            var assemblyName = Path.GetFileNameWithoutExtension(entryPoint ?? pluginId);
            var pluginXmlPath = Path.Combine(AppContext.BaseDirectory, "plugins", pluginId, $"{assemblyName}.xml");
            PluginXmlCommentFilter.AddPluginXml(pluginId, assemblyName, pluginXmlPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"加载插件XML注释到运行时过滤器失败 [{pluginId}]: {ex.Message}");
        }

        return true;
    }

    /// <summary>
    /// 运行时注销插件文档（移除 SwaggerDoc + SwaggerUI 端点）
    /// </summary>
    public bool UnregisterPlugin(string pluginId)
    {
        var key = pluginId.ToLower();
        if (!_plugins.TryRemove(key, out _))
            return false; // 不存在

        var swaggerOptions = _swaggerGenOptions.Value;

        // 1. 移除 SwaggerDoc
        swaggerOptions.SwaggerDocs.Remove($"plugin-{key}-admin");
        swaggerOptions.SwaggerDocs.Remove($"plugin-{key}-app");

        // 2. 移除 SwaggerUI 端点
        if (_swaggerUIOptions != null)
        {
            var config = _swaggerUIOptions.ConfigObject;
            var urls = config.Urls?.ToList() ?? new List<UrlDescriptor>();
            urls.RemoveAll(u => u.Url != null && u.Url.Contains($"/plugin-{key}-admin"));
            urls.RemoveAll(u => u.Url != null && u.Url.Contains($"/plugin-{key}-app"));
            config.Urls = urls;
        }

        return true;
    }

    /// <summary>
    /// 获取当前所有活跃的插件文档信息
    /// </summary>
    public List<(string pluginId, string pluginName, string description)> GetActivePlugins()
    {
        return _plugins.Select(kvp => (kvp.Key, kvp.Value.pluginName, kvp.Value.description)).ToList();
    }
}
