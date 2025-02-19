using Fastdotnet.Core.Plugin;

namespace Fastdotnet.Demo
{
    public class DemoPlugin : IPlugin
    {
        private int _counter = 0;
        private bool _isRunning = false;

        public string Name => "Demo Plugin";
        public string Description => "A simple demo plugin for Fastdotnet framework";
        public string Version => "1.0.0";
        public string Id => "demo-plugin";
        public string Author => "Fastdotnet Team";
        public string RequiredFrameworkVersion => "1.0.0";

        public void Initialize()
        {
            _counter = 0;
            _isRunning = true;
        }

        public void Start()
        {
            if (!_isRunning)
            {
                _isRunning = true;
            }
        }

        public void Stop()
        {
            if (_isRunning)
            {
                _isRunning = false;
            }
        }

        public int IncrementCounter()
        {
            if (_isRunning)
            {
                return ++_counter;
            }
            throw new InvalidOperationException("Plugin is not running");
        }

        public int GetCurrentCount()
        {
            return _counter;
        }
    }
}