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
        public AliPay AliPayInfo { get; set; }
        public AliOss AliOssInfo { get; set; }
    }

    public class AliPay
    {
        public string AppId { get; set; }
        public string Secret { get; set; }
    }

    public class AliOss
    {
        public string AK { get; set; }
        public string SK { get; set; }
    }
}
