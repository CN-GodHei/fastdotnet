using Fastdotnet.Demo.IService;

namespace Fastdotnet.Demo.Service
{
    public class DemoService : IDemoService
    {
        public string GetMessage()
        {
            return "Hello from Demo Plugin!";
        }
    }
}