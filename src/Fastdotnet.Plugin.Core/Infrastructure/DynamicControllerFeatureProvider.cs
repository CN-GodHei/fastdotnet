using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Fastdotnet.Plugin.Core.Infrastructure
{
    public class DynamicControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly ILogger<DynamicControllerFeatureProvider> _logger;

        public DynamicControllerFeatureProvider(ILogger<DynamicControllerFeatureProvider> logger = null)
        {
            _logger = logger;
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            if (parts == null)
            {
                throw new ArgumentNullException(nameof(parts));
            }

            if (feature == null)
            {
                throw new ArgumentNullException(nameof(feature));
            }

            try
            {
                foreach (var part in parts)
                {
                    if (part is AssemblyPart assemblyPart)
                    {
                        var assembly = assemblyPart.Assembly;
                        _logger?.LogDebug($"Scanning assembly {assembly.FullName} for controllers");

                        try
                        {
                            var candidates = assembly.GetExportedTypes()
                                .Where(x => !x.IsAbstract && !x.IsInterface)
                                .Where(x => x.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase))
                                .ToList();

                            foreach (var candidate in candidates)
                            {
                                var typeInfo = candidate.GetTypeInfo();
                                // 检查控制器是否已经存在，避免重复注册
                                if (!feature.Controllers.Contains(typeInfo))
                                {
                                    feature.Controllers.Add(typeInfo);
                                    _logger?.LogDebug($"Added controller: {typeInfo.FullName}");
                                }
                                else
                                {
                                    _logger?.LogDebug($"Controller already registered: {typeInfo.FullName}");
                                }
                            }
                        }
                        catch (ReflectionTypeLoadException ex)
                        {
                            _logger?.LogError(ex, $"Error loading types from assembly {assembly.FullName}");

                            // 记录详细的加载器异常信息
                            if (ex.LoaderExceptions != null)
                            {
                                foreach (var loaderEx in ex.LoaderExceptions)
                                {
                                    if (loaderEx != null)
                                    {
                                        _logger?.LogError(loaderEx, "Loader exception details");
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger?.LogError(ex, $"Error processing assembly {assembly.FullName}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in PopulateFeature");
                throw; // 重新抛出异常以便调用者处理
            }
        }
    }
}
