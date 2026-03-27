using System;
using System.Collections.Generic;
using System.Text;

namespace Fastdotnet.Core.Dtos.Sys
{
    public class DictTypeAndData
    {
        public FdDictType fdDictType { get; set; }
        public List<FdDictData> fdDictData { get; set; }
    }
}
