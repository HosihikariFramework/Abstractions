using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hosihikari;

/// <summary>框架交给插件的一切（ADR-0007）。能力面止于此，BDS 能力不在其中（ADR-0001）。</summary>
public interface IPluginContext
{
    /// <summary>插件名，等于插件包目录名（ADR-0006）。</summary>
    string Name { get; }

    /// <summary>插件包目录。</summary>
    string PackageDirectory { get; }

    /// <summary>插件自己写的运行期数据（ADR-0018）。</summary>
    string DataDirectory { get; }

    /// <summary>分类名为插件名的日志器。</summary>
    ILogger Logger { get; }

    /// <summary>已按「包内默认 → 包外覆盖 → 环境变量」叠好的配置（ADR-0018）。</summary>
    IConfiguration Configuration { get; }

    /// <summary>事件总线。订阅是全局的 —— 插件必须在 <c>OnUnload</c> 里退订（ADR-0004）。</summary>
    IEventBus Events { get; }

    /// <summary>调度器。跑在线程池上，不承诺游戏主线程（ADR-0013）。</summary>
    IScheduler Scheduler { get; }

    /// <summary>
    /// 其他插件提供的服务（ADR-0027）。插件之间共享类型的唯一通道 —— 它们不共享程序集，
    /// 直接引用另一个插件只会拿到自己包里的副本（ADR-0026）。
    /// 你登记的东西在卸载时由框架收回。
    /// </summary>
    IPluginServiceRegistry PluginServices { get; }

    /// <summary>服务解析。载体对象与载体事件是自愿取用的逃生舱（ADR-0019）。</summary>
    IServiceProvider Services { get; }
}
