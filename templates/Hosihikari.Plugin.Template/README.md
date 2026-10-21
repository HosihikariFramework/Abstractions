# Hosihikari.Plugin.Templates

A `dotnet new` template for a precompiled Hosihikari plugin. The generated project is a .NET class
library with an `EntryPoint<T>`, a manifest, and pack scripts that use the framework's packer.

For source-compiled plugins, see
[CSharpPluginTemplate](https://github.com/HosihikariFramework/CSharpPluginTemplate),
[FSharpPluginTemplate](https://github.com/HosihikariFramework/FSharpPluginTemplate), and
[RpcPluginTemplate](https://github.com/HosihikariFramework/RpcPluginTemplate).

## Build and try it

```bash
dotnet pack Hosihikari.Plugin.Templates.csproj -c Release -o artifacts
dotnet new install artifacts/Hosihikari.Plugin.Templates.0.1.2.nupkg
dotnet new hosihikari-plugin -n MyPlugin -o MyPlugin
```

Then build and pack the generated plugin with a sibling `Hosihikari.PluginManager` checkout:

```bash
cd MyPlugin
bash ./pack.sh
```

The result is `dist/MyPlugin/`; copy it to `<server>/plugins/MyPlugin/`.

## Template contents

- `Hosihikari.Plugin.Templates.csproj` — net10.0 class library referencing the contract with
  `ExcludeAssets="runtime"`; the shared layer stays out of the package.
- `Plugin.cs` — `IPlugin` implementation and `EntryPoint<T>`.
- `manifest.json` — `loader: assembly`, the core built-in path.
- `pack.ps1` / `pack.sh` — build, then call the core packer's `pack-plugin` command, which strips
  the canonical shared layer.
- Bilingual plugin README.

## Name constraints

The project name becomes the assembly name, namespace, and manifest name. Use letters, digits, and
dots; spaces and hyphens are not valid C# identifiers. The manifest name must equal the package
directory name.

## Licence

MIT-0, see [LICENSE.md](LICENSE.md). The `Hosihikari.Plugin.Abstractions` 0.7.1 contract the generated
project references is MIT; the 0.7.0 package remains LGPL-3.0-or-later.
