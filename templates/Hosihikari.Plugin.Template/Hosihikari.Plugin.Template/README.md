# Hosihikari.Plugin.Template

A precompiled Hosihikari plugin. The core's built-in assembly provider loads it; no provider
package is required.

## Build and pack

Requires the .NET 10 SDK and a sibling `Hosihikari.PluginManager` checkout (or
`HOSIHIKARI_PLUGINMANAGER` pointing at one).

```powershell
pwsh ./pack.ps1
```

```bash
bash ./pack.sh
```

The packer strips the shared layer and writes `dist/Hosihikari.Plugin.Template/` with the plugin
DLL, its private dependencies, and `manifest.json`. Copy that directory to
`<server>/plugins/Hosihikari.Plugin.Template/`.

## Naming

The project name becomes the assembly name, namespace, and manifest name. Use a valid C# name
(letters, digits, dots; no spaces or hyphens). The manifest name must match the package directory
name.

## Lifecycle

`OnLoad` runs after dependencies are ready. `OnEnable` opens the event/timer gate; a refusal keeps
the plugin loaded and disabled. `OnUnload` releases what the plugin owns. An exception from a hook
is a fault and the host unloads the plugin.

For source-compiled C# or F# plugins, see the provider packages and the online authoring guide.
