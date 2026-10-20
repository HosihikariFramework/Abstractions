namespace Hosihikari;

/// <summary>某一插件类型的加载器（ADR-0002）。核心只认这个契约，不认识 ALC 与脚本引擎（ADR-0010）。</summary>
public interface IPluginProvider
{
    /// <summary>清单 <c>type</c> 字段的取值。</summary>
    string Type { get; }

    /// <summary>能不能处理这个包。核心据此在多个 Provider 之间选型。</summary>
    /// <param name="manifest">包内清单。</param>
    /// <returns>能处理返回 <see langword="true"/>。</returns>
    bool CanLoad(PluginManifest manifest);

    /// <summary>把插件包变成可运行的实例。失败返回 <see langword="null"/>，不抛（ADR-0009）。</summary>
    /// <param name="manifest">包内清单。</param>
    /// <param name="packageDirectory">插件包目录。</param>
    /// <param name="context">交给插件的上下文。</param>
    /// <returns>实例与释放句柄；加载失败返回 <see langword="null"/>。</returns>
    PluginInstance? Load(PluginManifest manifest, string packageDirectory, IPluginContext context);
}

/// <summary>Provider 交出的实例与它的释放句柄。逻辑停用与真正卸载是两件事（ADR-0010）。</summary>
public abstract class PluginInstance(IPlugin plugin)
{
    /// <summary>插件实例。核心只经这个接口驱动它。</summary>
    public IPlugin Plugin { get; } = plugin;

    /// <summary>
    /// 释放句柄：卸掉 ALC / 关掉脚本引擎，并交出**回收验证器**。
    /// 交出验证器而不是布尔值，是因为回收要求调用方先丢掉自己持有的引用再验（ADR-0004）——
    /// 谁拥有搬运单元谁提供验证方式，核心因此不必认识 ALC。
    /// </summary>
    /// <returns>验证器：回收完成返回 <see langword="true"/>。没有搬运单元的 Provider 交一个恒为真的即可。</returns>
    public abstract Func<bool> Release();
}

/// <summary>一次卸载的结果。失败要如实回传，不假装成功（ADR-0004 / ADR-0009）。</summary>
/// <param name="Released">
/// 是否真的回收掉了。失败可能是未加载、仍有依赖者、停用被拒绝（插件仍在册且可能活跃），
/// 也可能是已经放下但回收验证没通过（状态 Detached，再调一次卸载会重新验证）。
/// </param>
/// <param name="Error">失败原因；成功时为 <see langword="null"/>。</param>
public readonly record struct UnloadResult(bool Released, string? Error)
{
    /// <summary>成功：句柄已释放且回收已验证。</summary>
    public static UnloadResult Ok() => new(true, null);

    /// <summary>失败：如实带上原因。</summary>
    public static UnloadResult Failed(string error) => new(false, error);
}
