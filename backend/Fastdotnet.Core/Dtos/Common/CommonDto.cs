using System;
using System.Collections.Generic;
using System.Text;

namespace Fastdotnet.Core.Dtos.Common
{
    public record IdNameDto(string Id, string Name);
    public record IdNameDto<TId>(TId Id, string Name);

    public record IdNameStatusDto(string Id, string Name, DataStatus DataStatus);
    public record IdNameStatusDto<TId>(TId Id, string Name, DataStatus DataStatus);
}
