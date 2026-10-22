using Hosihikari;
using Microsoft.Extensions.Logging;

[assembly: EntryPoint<Hosihikari.Plugin.Templates.Plugin>]

namespace Hosihikari.Plugin.Templates;

/// <summary>A precompiled plugin loaded by the core's built-in assembly loader.</summary>
public sealed class Plugin : IPlugin
{
    public void OnLoad(IPluginContext context)
    {
        context.Logger.LogInformation(
            "{Name} loaded from {Directory}",
            context.Name,
            context.PackageDirectory);
    }

    public PluginHookResult OnEnable() => PluginHookResult.Allow();

    public PluginHookResult OnDisable() => PluginHookResult.Allow();

    public void OnUnload()
    {
    }
}
