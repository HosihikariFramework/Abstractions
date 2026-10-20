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

    /// <summary>
    /// 把一个 Provider 登记给框架（ADR-0002）。Provider 本身就是一个插件包，装载时用它把自己
    /// 交出去；**此后**装载的包才会被路由到它 —— 所以需要它的插件要在清单里声明依赖它（ADR-0006）。
    ///
    /// 登记之后不能撤销：它造出来的实例的类型住在它的装载上下文里。
    /// </summary>
    /// <param name="provider">要接管的插件类型。</param>
    void RegisterProvider(IPluginProvider provider);

    /// <summary>
    /// 登记 Provider，并把“同名已被占用”作为返回值交出来。
    /// 默认实现保持旧行为并返回 <see langword="true"/>；新 Provider 应优先用它，
    /// 这样重复登记不会在包以为成功时静默落空。
    /// </summary>
    /// <param name="provider">要接管的插件类型。</param>
    /// <returns>登记成功为 <see langword="true"/>；同名已存在为 <see langword="false"/>。</returns>
    bool TryRegisterProvider(IPluginProvider provider)
    {
        RegisterProvider(provider);
        return true;
    }

    /// <summary>
    /// 报告一个在钩子之外发生的故障（进程外环境死亡、协议错误等，ADR-0030）。
    /// 宿主按故障处理：卸载该插件，成功后通知载体。进程内 Provider 通常用不到。
    /// </summary>
    /// <param name="reason">可读的原因，进日志与父级的账。</param>
    void ReportFault(string reason);
}
