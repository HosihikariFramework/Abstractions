namespace Hosihikari;

/// <summary>契约自身的版本（ADR-0012）。清单 <c>extraInfo.api</c> 必须等于它，否则拒载。</summary>
public static class PluginContract
{
    /// <summary>当前的契约主版本。清单里的 <c>api</c> 与它不匹配的包在载入之前就会被剔除。</summary>
    public const string ApiVersion = "2";

    /// <summary>当前的清单 schema 版本。清单里的 <c>format_version</c> 与它不匹配的包同样在载入之前被剔除。</summary>
    public const string FormatVersion = "1";

    /// <summary>
    /// Shim 作为父级插件的名字，也是清单里 <c>type</c> 该写的值（ADR-0006）。
    /// 它是父级命名空间里的一等名字：插件靠 <c>dependencies</c> 声明对它的依赖来保证排序。
    /// </summary>
    public const string ShimModName = "hosihikari";
}

/// <summary>
/// 一次生命周期钩子的结果（ADR-0030）。拒绝带原因；钩子抛异常是故障，不走这里。
/// </summary>
/// <param name="Allowed">是否允许继续。</param>
/// <param name="Reason">拒绝原因；允许时为 <see langword="null"/>。</param>
public readonly record struct PluginHookResult(bool Allowed, string? Reason)
{
    /// <summary>允许：照常继续。</summary>
    public static PluginHookResult Allow() => new(true, null);

    /// <summary>拒绝：本次动作失败，原因交给父级与日志。</summary>
    public static PluginHookResult Refuse(string? reason = null) => new(false, reason);
}

/// <summary>插件作者实现的契约（ADR-0008）。钩子只承诺顺序与幂等，不承诺与 BDS 阶段的对应（ADR-0013）。</summary>
public interface IPlugin
{
    /// <summary>载入。此时依赖已就绪；服务器未必已就绪。</summary>
    /// <param name="context">框架交给插件的一切。</param>
    void OnLoad(IPluginContext context);

    /// <summary>启用：可以接活了。成功后打开作用域闸门；拒绝时留在册、未启用（ADR-0030 / 0035）。</summary>
    PluginHookResult OnEnable();

    /// <summary>
    /// 停用：停止接活。框架先关闭作用域闸门，不再投递事件与定时器回调；已在执行的回调允许跑完。
    /// 拒绝时闸门重开；卸载经停用，同样中止（ADR-0030 / 0035）。
    /// 父级态下由父级驱动；自持态下不会被调用（ADR-0020）。
    /// </summary>
    PluginHookResult OnDisable();

    /// <summary>
    /// 卸载：释放自己持有的东西。父级态下由父级驱动；自持态下不会被调用（ADR-0020）——
    /// 所以必须执行完的收尾不要只放在这里。抛出按故障处理（ADR-0030）。
    /// </summary>
    void OnUnload();
}
