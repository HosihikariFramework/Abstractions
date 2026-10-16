namespace Hosihikari;

/// <summary>框架交给插件的一切（ADR-0007）。能力面止于此，BDS 能力不在其中（ADR-0001）。</summary>
public interface IPluginContext
{
    string Name { get; }

    /// <summary>插件包目录。</summary>
    string PackageDirectory { get; }

    /// <summary>插件自己写的运行期数据（ADR-0018）。</summary>
    string DataDirectory { get; }

    /// <summary>服务解析。载体对象与载体事件是自愿取用的逃生舱（ADR-0019）。</summary>
    IServiceProvider Services { get; }
}
