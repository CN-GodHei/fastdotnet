using PluginA.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginA.Services
{
    public class TestService : ITestService
    {
        public string GetTestMessage()
        {
            return $"Test message from PluginA.TestService at {DateTime.Now}";
        }
    }
}
