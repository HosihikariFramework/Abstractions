namespace Hosihikari;

/// <summary>契约自身的版本（ADR-0012）。清单 <c>extraInfo.api</c> 必须等于它，否则拒载。</summary>
public static class PluginContract
{
    public const string ApiVersion = "1";
}

/// <summary>插件作者实现的契约（ADR-0008）。钩子只承诺顺序与幂等，不承诺与 BDS 阶段的对应（ADR-0013）。</summary>
public interface IPlugin
{
    void OnLoad(IPluginContext context);

    void OnEnable();

    void OnDisable();

    void OnUnload();
}
