# Hosihikari.Abstractions

[English](README.md) | [简体中文](README.zh_hans.md)

The **plugin contract** of the Hosihikari framework: the few types a plugin implements, and
everything the framework hands to it.

The framework itself and the other packages live under
[HosihikariFramework](https://github.com/HosihikariFramework).

## Referencing it

```xml
<PackageReference Include="Hosihikari.Abstractions" Version="[0.7.1]" ExcludeAssets="runtime" />
```

**`ExcludeAssets="runtime"` is not optional.** The contract lives in the **shared layer** — the
host has already loaded its copy. A second copy inside a plugin package is a second identity under
the same type name, and the plugin's casts will fail at run time, far away from the cause.

The same applies to any assembly the framework also ships (carrier bindings and the like).

## A plugin

```csharp
using Hosihikari;

[assembly: EntryPoint<MyPlugin>]

public sealed class MyPlugin : IPlugin
{
    public void OnLoad(IPluginContext context) =>
        context.Logger.LogInformation("loaded: {Dir}", context.PackageDirectory);

    public PluginHookResult OnEnable() => PluginHookResult.Allow();
    public PluginHookResult OnDisable() => PluginHookResult.Allow();
    public void OnUnload() { }
}
```

The entry point is declared by `[assembly: EntryPoint<T>]` on the assembly. Its generic constraint
is `where T : IPlugin, new()`, so both "the type implements the contract" and "it has a
parameterless constructor" are enforced by the compiler.

## Manifest

A plugin package is a directory; the directory name is the plugin name, and it must contain a
`manifest.json` (the format is LeviLamina's mod manifest; the framework's own fields live under
`extraInfo`):

```json
{
  "name": "My",
  "entry": "My.dll",
  "type": "hosihikari",
  "version": "1.0.0",
  "dependencies": [{ "name": "hosihikari" }],
  "extraInfo": {
    "api": "2",
    "format_version": "1",
    "provider": "assembly"
  }
}
```

`type` is the parent loader's routing key and is always `hosihikari`. `extraInfo.provider` picks
the Provider that handles the package — **how to find the entry point inside the package is that
Provider's business**. `api` and `format_version` must equal `PluginContract.ApiVersion` and
`PluginContract.FormatVersion`; `entry` must stay inside the package. A violation is rejected
**before** anything is loaded, with a readable report.

## Things an author should know

- **Hooks promise order and idempotence only** — not which server phase they land in. The world may
  not be loaded yet when `OnEnable` runs.
- **The contract covers host services only**: logging, configuration, data directory, scheduling,
  the event bus, service resolution and the service registry. Worlds, entities, commands and game
  events are outside it.
- **Unloading must actually reclaim.** No long-lived static state, no bare threads. The framework
  reclaims event subscriptions and timers on unload, but that is a safety net, not a licence.
- **Disable closes the callback gate first.** The framework stops delivering events and timer
  callbacks before `OnDisable`; a refusal reopens it. In-flight callbacks finish; services and
  plugin-owned threads are outside this guarantee (ADR-0035).
- **Native libraries live with the process.** Put them at the package root or under
  `runtimes/<current portable RID>/native` (the RID directory wins). Unloading reclaims the managed
  ALC only; native modules and their static state survive reloads. Stop threads and unregister
  callbacks in `OnUnload`.
- Plugins do not share assemblies with each other: a direct reference gives you your own copy.
  Cross-plugin communication goes through the framework's channel, and the types on that channel
  must come from the shared layer.

## Plugin template

This repository also carries the `dotnet new` template for precompiled assembly plugins, published
as [Hosihikari.Plugin.Template](https://www.nuget.org/packages/Hosihikari.Plugin.Template). See
[`templates/Hosihikari.Plugin.Template`](templates/Hosihikari.Plugin.Template); source C#, F#, and
out-of-process plugins have their own template repositories:

- [CSharpPluginTemplate](https://github.com/HosihikariFramework/CSharpPluginTemplate)
- [FSharpPluginTemplate](https://github.com/HosihikariFramework/FSharpPluginTemplate)
- [RpcPluginTemplate](https://github.com/HosihikariFramework/RpcPluginTemplate)

The template scaffolding is MIT-0; the Abstractions source is MIT. The 0.7.0 package published on
NuGet before the switch remains LGPL-3.0-or-later; 0.7.1 and later are MIT.

## Licence

The Abstractions source is MIT, see [LICENSE.md](LICENSE.md). The 0.7.0 package published on NuGet
before the switch remains LGPL-3.0-or-later; 0.7.1 and later are MIT. The plugin templates under
`templates/` and the three template repositories are MIT-0.
