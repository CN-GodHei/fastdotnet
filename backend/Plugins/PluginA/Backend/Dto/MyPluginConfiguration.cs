using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginA.Dto
{
    public class MyPluginConfiguration
    {
        public AliPay AliPayInfo { get; set; } = null!;
        public AliOss AliOssInfo { get; set; } = null!;
        public string testinfo { get; set; } = string.Empty;
    }

    public class AliPay
    {
        public string AppId { get; set; } = string.Empty;
        public string Secret { get; set; } = string.Empty;
    }

    public class AliOss
    {
        public string AK { get; set; } = string.Empty;
        public string SK { get; set; } = string.Empty;
    }
}
