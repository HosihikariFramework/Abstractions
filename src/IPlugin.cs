namespace Hosihikari;

/// <summary>契约自身的版本（ADR-0012）。清单 <c>extraInfo.api</c> 必须等于它，否则拒载。</summary>
public static class PluginContract
{
    /// <summary>当前的契约主版本。清单里的 <c>api</c> 与它不匹配的包在载入之前就会被剔除。</summary>
    public const string ApiVersion = "1";
}

/// <summary>插件作者实现的契约（ADR-0008）。钩子只承诺顺序与幂等，不承诺与 BDS 阶段的对应（ADR-0013）。</summary>
public interface IPlugin
{
    /// <summary>载入。此时依赖已就绪；服务器未必已就绪。</summary>
    /// <param name="context">框架交给插件的一切。</param>
    void OnLoad(IPluginContext context);

    /// <summary>启用：可以接活了。</summary>
    void OnEnable();

    /// <summary>停用：停止接活。父级态下由父级驱动；自持态下不会被调用（ADR-0020）。</summary>
    void OnDisable();

    /// <summary>
    /// 卸载：释放自己持有的东西。父级态下由父级驱动；自持态下不会被调用（ADR-0020）——
    /// 所以必须执行完的收尾不要只放在这里。
    /// </summary>
    void OnUnload();
}
