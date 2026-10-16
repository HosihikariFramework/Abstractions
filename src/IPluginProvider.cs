namespace Hosihikari;

/// <summary>某一插件类型的加载器（ADR-0002）。核心只认这个契约，不认识 ALC 与脚本引擎（ADR-0010）。</summary>
public interface IPluginProvider
{
    /// <summary>清单 <c>type</c> 字段的取值。</summary>
    string Type { get; }

    bool CanLoad(PluginManifest manifest);

    /// <summary>把插件包变成可运行的实例。失败返回 null，不抛（ADR-0009）。</summary>
    PluginInstance? Load(PluginManifest manifest, string packageDirectory, IPluginContext context);
}

/// <summary>Provider 交出的实例与它的释放句柄。逻辑停用与真正卸载是两件事（ADR-0010）。</summary>
public abstract class PluginInstance(IPlugin plugin)
{
    public IPlugin Plugin { get; } = plugin;

    /// <summary>
    /// 释放句柄：卸掉 ALC / 关掉脚本引擎，并交出**回收验证器**。
    /// 交出验证器而不是布尔值，是因为回收要求调用方先丢掉自己持有的引用再验（ADR-0004）——
    /// 谁拥有搬运单元谁提供验证方式，核心因此不必认识 ALC。
    /// </summary>
    public abstract Func<bool> Release();
}

/// <summary>一次卸载的结果。失败要如实回传，不假装成功（ADR-0004 / ADR-0009）。</summary>
public readonly record struct UnloadResult(bool Released, string? Error)
{
    public static UnloadResult Ok() => new(true, null);

    public static UnloadResult Failed(string error) => new(false, error);
}
