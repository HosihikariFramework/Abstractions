# Hosihikari.Abstractions

[English](README.md) | [简体中文](README.zh_hans.md)

星光框架的**插件契约**：插件实现的那几个类型，以及它从框架手里拿到的东西。

框架本体与其余包见 [HosihikariFramework](https://github.com/HosihikariFramework)。

## 引用

```xml
<PackageReference Include="Hosihikari.Abstractions" Version="[0.7.0]" ExcludeAssets="runtime" />
```

**`ExcludeAssets="runtime"` 不能省。** 契约驻留在**共享层** —— 宿主已经载入过这一份；插件包里若再带一份，就是同一个类型名下的第二个身份，插件转型会在运行期失败（而且报错点离病因很远）。

同理，插件引用任何"框架侧也有"的程序集（载体绑定等）时都要带上这一条。

## 一个插件

```csharp
using Hosihikari;

[assembly: EntryPoint<MyPlugin>]

public sealed class MyPlugin : IPlugin
{
    public void OnLoad(IPluginContext context) =>
        context.Logger.LogInformation("载入：{Dir}", context.PackageDirectory);

    public PluginHookResult OnEnable() => PluginHookResult.Allow();
    public PluginHookResult OnDisable() => PluginHookResult.Allow();
    public void OnUnload() { }
}
```

入口用程序集上的 `[assembly: EntryPoint<T>]` 声明。这个特性的泛型约束是 `where T : IPlugin, new()`，所以**类型实现了契约**与**有无参构造**两条都由编译器保证 —— 写错编译不过。

## 清单

插件包是一个目录，目录名就是插件名，里面必须有 `manifest.json`（格式沿用 LeviLamina 的 mod 清单，框架自己的字段住在 `extraInfo`）：

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

`type` 是父级加载器的路由键，恒为 `hosihikari`；`extraInfo.provider` 决定由哪个 Provider 处理 —— **怎么从包里找到入口，是那个 Provider 自己的事**。`api` 与 `format_version` 必须分别等于 `PluginContract.ApiVersion` 与 `PluginContract.FormatVersion`；`entry` 必须是包内相对路径。不满足的包在**载入之前**就会被剔除并给出可读报告。

## 作者要知道的几条

- **钩子只承诺顺序与幂等**，不承诺它们落在服务器的哪个阶段 —— `OnEnable` 时世界未必已加载。
- **契约只覆盖通用宿主服务**：日志、配置、数据目录、调度、事件总线、服务解析。世界、实体、命令、游戏事件不在其中。
- **卸载要真能回收**：不要留长期静态状态，不要裸起线程。事件订阅与定时器由框架在卸载时替你收回，但那只是兜底。
- **停用会先关回调闸门**：框架在 `OnDisable` 之前停止投递事件与定时器；拒绝停用则恢复。已在执行的回调会跑完，服务与插件自有线程不受管（ADR-0035）。
- **本机库随进程存活**：放在包根或 `runtimes/<当前可移植 RID>/native`（RID 目录优先）。卸载只回收托管 ALC；本机库与它的静态状态跨重载保留，线程与回调要在 `OnUnload` 里自己收尾。
- 插件之间不共享程序集：直接引用拿到的是自己包里的副本。跨插件通信走框架的通道，而通道上的类型必须来自共享层。

## 许可

LGPL-3.0-or-later，见 [LICENSE.md](LICENSE.md)。
