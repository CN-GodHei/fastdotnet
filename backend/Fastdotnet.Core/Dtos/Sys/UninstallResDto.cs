using System;
using System.Collections.Generic;
using System.Text;

namespace Fastdotnet.Core.Dtos.Sys
{
    public class UninstallResDto
    {
        public bool Result { get; set; }
        public bool Offline { get; set; }
        public string UninstallCode { get; set; }
    }
}
