using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginA.IService
{
    public interface IPluginAMessageService
    {
        Task SendNotificationToClientsAsync(string message);
        Task SendDataToClientsAsync(string data);
    }
}
