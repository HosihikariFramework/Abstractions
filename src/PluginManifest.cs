using System.Text.Json.Serialization;

namespace Hosihikari;

/// <summary>
/// 插件清单（ADR-0006）。格式取 LeviLamina 的 <c>manifest.json</c>，我们自己的字段住在 <c>extraInfo</c>。
/// </summary>
public sealed class PluginManifest
{
    /// <summary>插件名。必须等于插件包目录名。</summary>
    public string Name { get; set; } = "";

    /// <summary>包内入口文件的相对路径。</summary>
    public string Entry { get; set; } = "";

    /// <summary>插件类型，决定由哪个 Provider 处理。</summary>
    public string Type { get; set; } = "";

    /// <summary>插件自身版本。</summary>
    public string? Version { get; set; }

    /// <summary>依赖的其它插件。加载按拓扑序、卸载按反序（ADR-0004）。</summary>
    public List<ManifestDependency> Dependencies { get; set; } = [];

    /// <summary>框架自己的字段都放这里 —— 清单里留给第三方的扩展位。</summary>
    public Dictionary<string, string> ExtraInfo { get; set; } = [];

    /// <summary>插件依赖的契约主版本（ADR-0012）。</summary>
    [JsonIgnore]
    public string? Api => ExtraInfo.GetValueOrDefault("api");

    /// <summary>清单 schema 版本（ADR-0012）。</summary>
    [JsonIgnore]
    public string? FormatVersion => ExtraInfo.GetValueOrDefault("format_version");
}

/// <summary>一条依赖声明。</summary>
public sealed class ManifestDependency
{
    /// <summary>被依赖的插件名。</summary>
    public string Name { get; set; } = "";

    /// <summary>版本约束。当前不参与解析，留给后续的依赖治理。</summary>
    public string? Version { get; set; }
}
