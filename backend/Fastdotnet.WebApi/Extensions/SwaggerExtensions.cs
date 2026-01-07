namespace Fastdotnet.WebApi.Extensions;

public static class SwaggerExtensions
{
    private const string PluginsFolderName = "Plugins";

    /// <summary>
    /// 注册 Swagger 生成器（AddSwaggerGen）
    /// </summary>
    public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            // 添加主API文档 - admin 版本
            c.SwaggerDoc("main-admin", new OpenApiInfo
            {
                Title = "主系统 API",
                Version = "v1",
                Description = "Fastdotnet 主系统 API 文档（Admin端）"
            });
            
            // 添加主API文档 - app 版本
            c.SwaggerDoc("main-app", new OpenApiInfo
            {
                Title = "主系统 API",
                Version = "v1",
                Description = "Fastdotnet 主系统 API 文档（App端）"
            });

            // 为插件动态添加API文档定义 - admin 和 app 版本
            var pluginDirs = Directory.GetDirectories(Path.Combine(AppContext.BaseDirectory, "Plugins"));
            foreach (var pluginDir in pluginDirs)
            {
                try
                {
                    var pluginJsonPath = Path.Combine(pluginDir, "plugin.json");
                    if (File.Exists(pluginJsonPath))
                    {
                        var pluginJson = File.ReadAllText(pluginJsonPath);
                        var pluginConfig = System.Text.Json.JsonDocument.Parse(pluginJson);
                        var pluginId = pluginConfig.RootElement.GetProperty("id").GetString();
                        var pluginName = pluginConfig.RootElement.GetProperty("name").GetString();
                        var pluginDescription = pluginConfig.RootElement.GetProperty("description").GetString();

                        if (!string.IsNullOrEmpty(pluginId))
                        {
                            // 添加 admin 版本文档
                            c.SwaggerDoc($"plugin-{pluginId.ToLower()}-admin", new OpenApiInfo
                            {
                                Title = $"{pluginName} 插件 API (Admin)",
                                Version = "v1",
                                Description = $"Fastdotnet {pluginDescription} - Admin端"
                            });
                            
                            // 添加 app 版本文档
                            c.SwaggerDoc($"plugin-{pluginId.ToLower()}-app", new OpenApiInfo
                            {
                                Title = $"{pluginName} 插件 API (App)",
                                Version = "v1",
                                Description = $"Fastdotnet {pluginDescription} - App端"
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 如果读取plugin.json失败，跳过该插件
                    Console.WriteLine($"读取插件配置失败 {pluginDir}: {ex.Message}");
                }
            }

            c.TagActionsBy(apiDesc =>
            {
                if (apiDesc.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                {
                    var pluginName = apiDesc.ActionDescriptor.Properties.ContainsKey("PluginName")
                        ? apiDesc.ActionDescriptor.Properties["PluginName"] as string
                        : null;

                    var controllerName = controllerActionDescriptor.ControllerName;

                    if (!string.IsNullOrEmpty(pluginName))
                    {
                        return new[] { pluginName };
                    }
                    else
                    {
                        // 为AuthController设置特殊标签名，使其排在最前面
                        if (controllerName == "Auth")
                        {
                            //return new[] { "00-认证接口" };
                            return new[] { " Auth" };
                        }
                        return new[] { controllerName };
                    }
                }
                return new[] { "Default" };
            });

            // 添加文档过滤器
            c.DocumentFilter<PluginDocumentFilter>();
            c.DocumentFilter<TagOrderDocumentFilter>();
            // 添加操作过滤器，为继承自GenericDtoControllerBase的控制器生成文档
            c.OperationFilter<InheritedGenericControllerOperationFilter>();

            // 启用 XML 文档注释
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }

            // 为插件中的控制器添加XML注释支持
            foreach (var pluginDir in pluginDirs)
            {
                try
                {
                    var pluginJsonPath = Path.Combine(pluginDir, "plugin.json");
                    if (File.Exists(pluginJsonPath))
                    {
                        var pluginJson = File.ReadAllText(pluginJsonPath);
                        var pluginConfig = System.Text.Json.JsonDocument.Parse(pluginJson);
                        var pluginId = pluginConfig.RootElement.GetProperty("id").GetString();

                        if (!string.IsNullOrEmpty(pluginId))
                        {
                            var pluginXmlPath = Path.Combine(pluginDir, $"{pluginId}.xml");
                            if (File.Exists(pluginXmlPath))
                            {
                                c.IncludeXmlComments(pluginXmlPath);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 如果读取plugin.json失败，跳过该插件
                    Console.WriteLine($"读取插件配置失败 {pluginDir}: {ex.Message}");
                }
            }

            // Add JWT Bearer security definition
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http, // Change type to Http
                Scheme = "bearer", // Must be lowercase
                BearerFormat = "JWT"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
        });
        return services;
    }

    /// <summary>
    /// 启用 Swagger 中间件（UseSwagger + UseSwaggerUI）
    /// </summary>
    public static WebApplication UseCustomSwagger(this WebApplication app)
    {
        //if (!app.Environment.IsDevelopment())
        //    return app;

        app.UseSwagger();

        app.UseSwaggerUI(c =>
        {
            // 主文档 - admin 版本
            c.SwaggerEndpoint("/swagger/main-admin/swagger.json", "主系统 API (Admin) v1");
            
            // 主文档 - app 版本
            c.SwaggerEndpoint("/swagger/main-app/swagger.json", "主系统 API (App) v1");

            // 插件文档
            var pluginsPath = Path.Combine(AppContext.BaseDirectory, PluginsFolderName);
            Directory.CreateDirectory(pluginsPath); // 确保目录存在

            if (Directory.Exists(pluginsPath))
            {
                var pluginDirs = Directory.GetDirectories(pluginsPath);
                foreach (var pluginDir in pluginDirs)
                {
                    try
                    {
                        var pluginJsonPath = Path.Combine(pluginDir, "plugin.json");
                        if (!File.Exists(pluginJsonPath)) continue;

                        var pluginJson = File.ReadAllText(pluginJsonPath);
                        using var doc = JsonDocument.Parse(pluginJson);
                        var root = doc.RootElement;

                        var pluginId = root.GetProperty("id").GetString();
                        var pluginName = root.GetProperty("name").GetString();

                        if (!string.IsNullOrEmpty(pluginId))
                        {
                            // 添加 admin 版本端点
                            c.SwaggerEndpoint(
                                $"/swagger/plugin-{pluginId.ToLower()}-admin/swagger.json",
                                $"{pluginName} 插件 API (Admin) v1"
                            );
                            
                            // 添加 app 版本端点
                            c.SwaggerEndpoint(
                                $"/swagger/plugin-{pluginId.ToLower()}-app/swagger.json",
                                $"{pluginName} 插件 API (App) v1"
                            );
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"读取插件配置失败 {pluginDir}: {ex.Message}");
                    }
                }
            }

            c.RoutePrefix = "swagger";
            c.DefaultModelsExpandDepth(-1); // 隐藏底部 Models
        });

        app.UseCors(); // 如果 CORS 是开发环境专用，也可放这里

        return app;
    }
}