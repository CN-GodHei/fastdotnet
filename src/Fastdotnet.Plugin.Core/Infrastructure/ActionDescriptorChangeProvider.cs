
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;
using System.Threading;

namespace Fastdotnet.Plugin.Core.Infrastructure
{
    /// <summary>
    /// 一个单例服务，提供一个变更令牌以通知 MVC 框架
    /// 可用操作（即控制器）的集合已更改。
    /// </summary>
    public class ActionDescriptorChangeProvider : IActionDescriptorChangeProvider
    {
        public static ActionDescriptorChangeProvider Instance { get; } = new ActionDescriptorChangeProvider();

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public IChangeToken GetChangeToken() => new CancellationChangeToken(_cancellationTokenSource.Token);

        /// <summary>
        /// 通过取消旧的令牌并创建一个新的令牌来触发变更令牌，
        /// 通知框架它需要重新发现操作。
        /// </summary>
        public void NotifyChanges()
        {
            var oldTokenSource = Interlocked.Exchange(ref _cancellationTokenSource, new CancellationTokenSource());
            oldTokenSource.Cancel();
            oldTokenSource.Dispose();
        }
    }
}
