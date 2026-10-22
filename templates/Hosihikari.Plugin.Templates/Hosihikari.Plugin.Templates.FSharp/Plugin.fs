namespace Hosihikari.Plugin.Templates.FSharp

open Hosihikari
open Microsoft.Extensions.Logging

/// <summary>A precompiled plugin loaded by the core's built-in assembly loader.</summary>
type Plugin() =
    interface IPlugin with
        member _.OnLoad(context: IPluginContext) =
            context.Logger.LogInformation("loaded from {Directory}", context.PackageDirectory)

        member _.OnEnable() = PluginHookResult.Allow()

        member _.OnDisable() = PluginHookResult.Allow()

        member _.OnUnload() = ()

/// <summary>
/// Declares the entry type. F# cannot apply the generic EntryPoint&lt;T&gt; attribute,
/// so the plugin supplies the same EntryPointAttributeBase through a non-generic attribute.
/// </summary>
type PluginEntryPointAttribute() =
    inherit EntryPointAttributeBase()

    override _.CreateInstance() = Plugin() :> IPlugin

[<assembly: PluginEntryPoint>]
do ()
