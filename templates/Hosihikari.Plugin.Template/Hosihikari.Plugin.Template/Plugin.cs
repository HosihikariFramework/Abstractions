using Hosihikari;
using Microsoft.Extensions.Logging;

[assembly: EntryPoint<Hosihikari.Plugin.Template.Plugin>]

namespace Hosihikari.Plugin.Template;

/// <summary>A precompiled plugin loaded by the core's built-in assembly provider.</summary>
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
