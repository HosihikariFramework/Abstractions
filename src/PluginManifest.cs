using System.Text.Json.Serialization;

namespace Hosihikari;

/// <summary>
/// 插件清单（ADR-0006）。格式取 LeviLamina 的 <c>manifest.json</c>，我们自己的字段住在 <c>extraInfo</c> ——
/// <c>entry</c> 是它的文件路径语义，我们的入口类型是 <c>extraInfo.entryType</c>。
/// </summary>
public sealed class PluginManifest
{
    public string Name { get; set; } = "";

    /// <summary>包内入口文件的相对路径。</summary>
    public string Entry { get; set; } = "";

    public string Type { get; set; } = "";

    public string? Version { get; set; }

    public List<ManifestDependency> Dependencies { get; set; } = [];

    public Dictionary<string, string> ExtraInfo { get; set; } = [];

    /// <summary>插件依赖的契约主版本（ADR-0012）。</summary>
    [JsonIgnore]
    public string? Api => ExtraInfo.GetValueOrDefault("api");

    /// <summary>清单 schema 版本（ADR-0012）。</summary>
    [JsonIgnore]
    public string? FormatVersion => ExtraInfo.GetValueOrDefault("format_version");

    /// <summary>入口类型，如 <c>Foo.FooPlugin</c>（ADR-0008）。</summary>
    [JsonIgnore]
    public string? EntryType => ExtraInfo.GetValueOrDefault("entryType");
}

public sealed class ManifestDependency
{
    public string Name { get; set; } = "";

    public string? Version { get; set; }
}
