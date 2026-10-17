using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Fastdotnet.WebApi.Tests.Infrastructure;

/// <summary>
/// 宿主集成测试工厂。
/// <para>
/// 默认使用 Development 环境（SQLite，零外部依赖），并允许显式打开防重放，
/// 用于验证「本地关闭、线上开启」这种曾经靠 <c>#if DEBUG</c> 隐式分叉的行为。
/// </para>
/// </summary>
public sealed class FastdotnetWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly bool _antiReplayEnabled;

    public FastdotnetWebApplicationFactory(bool antiReplayEnabled = false)
    {
        _antiReplayEnabled = antiReplayEnabled;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        // 启动自检本身也是被测对象：测试环境必须开启，否则漏注册无法被发现
        builder.UseSetting("SelfCheck:Enabled", "true");

        // 显式控制防重放，避免依赖编译配置（#if DEBUG）
        builder.UseSetting("Security:AntiReplay:Enabled", _antiReplayEnabled ? "true" : "false");
    }
}
