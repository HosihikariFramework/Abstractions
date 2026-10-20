using System.Text.Json;
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

    /// <summary>
    /// 父级的路由键，恒为 <see cref="PluginContract.ShimModName"/> —— 父级靠它把包交给我们（ADR-0006）。
    /// </summary>
    public string Type { get; set; } = "";

    /// <summary>插件自身版本。</summary>
    public string? Version { get; set; }

    /// <summary>依赖的其它插件。加载按拓扑序、卸载按反序（ADR-0004）。</summary>
    public List<ManifestDependency> Dependencies
    {
        get => _dependencies;
        set => _dependencies = value ?? [];
    }

    /// <summary>
    /// 框架自己的字段都放这里 —— 清单里留给第三方的扩展位。
    /// 值保持原始 JSON：字符串、数组（例如进程外插件的 <c>command</c>）都放得下。
    /// </summary>
    public Dictionary<string, JsonElement> ExtraInfo
    {
        get => _extraInfo;
        set => _extraInfo = value ?? [];
    }

    private List<ManifestDependency> _dependencies = [];
    private Dictionary<string, JsonElement> _extraInfo = [];

    /// <summary>处理这个包的 Provider（ADR-0002）。不写就是 <c>assembly</c>。</summary>
    [JsonIgnore]
    public string Provider =>
        ExtraInfo.TryGetValue("provider", out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString() ?? "assembly"
            : "assembly";

    /// <summary>插件依赖的契约主版本（ADR-0012）。</summary>
    [JsonIgnore]
    public string? Api =>
        ExtraInfo.TryGetValue("api", out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;

    /// <summary>清单 schema 版本（ADR-0012）。</summary>
    [JsonIgnore]
    public string? FormatVersion =>
        ExtraInfo.TryGetValue("format_version", out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;
}

/// <summary>一条依赖声明。</summary>
public sealed class ManifestDependency
{
    /// <summary>被依赖的插件名。</summary>
    public string Name { get; set; } = "";

    /// <summary>版本约束。当前不参与解析，留给后续的依赖治理。</summary>
    public string? Version { get; set; }
}
