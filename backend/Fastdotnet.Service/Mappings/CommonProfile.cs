using AutoMapper;
using Fastdotnet.Core.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fastdotnet.Service.Mappings
{
    /// <summary>
    /// 通用 AutoMapper 配置（类型转换、全局规则等）
    /// </summary>
    public class CommonProfile : Profile
    {
        public CommonProfile()
        {
            // ✅ 全局：string → UserRefDto
            CreateMap<string, UserRefDto>()
                .ConvertUsing(id => new UserRefDto { Id = id ?? string.Empty });

            // 未来可扩展其他通用映射，例如：
            // CreateMap<DateTime, string>().ConvertUsing(...);
        }
    }
}
