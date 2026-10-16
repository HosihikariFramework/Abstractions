# Hosihikari.Abstractions

Hosihikari 框架的**插件契约**：插件实现的那几个类型，以及它从框架手里拿到的东西。

框架本体与其余包见 [HosihikariFramework](https://github.com/HosihikariFramework)。

## 引用

```xml
<PackageReference Include="Hosihikari.Abstractions" Version="[0.1.0]" ExcludeAssets="runtime" />
```

**`ExcludeAssets="runtime"` 不能省。** 契约驻留在**共享层** —— 宿主已经载入过这一份；插件包里若再带一份，就是同一个类型名下的第二个身份，插件转型会在运行期失败（而且报错点离病因很远）。

同理，插件引用任何"框架侧也有"的程序集（载体绑定等）时都要带上这一条。

## 一个插件

```csharp
using Hosihikari;

public sealed class MyPlugin : IPlugin
{
    public void OnLoad(IPluginContext context) =>
        context.Logger.LogInformation("载入：{Dir}", context.PackageDirectory);

    public void OnEnable() { }
    public void OnDisable() { }
    public void OnUnload() { }
}
```

入口类型写在插件清单的 `extraInfo.entryType` 里（如 `My.MyPlugin`），由框架反射构造 —— 框架不去程序集里"找唯一的那个实现"。

## 清单

插件包是一个目录，目录名就是插件名，里面必须有 `manifest.json`（格式沿用 LeviLamina 的 mod 清单，框架自己的字段住在 `extraInfo`）：

```json
{
  "name": "My",
  "entry": "My.dll",
  "type": "assembly",
  "version": "1.0.0",
  "dependencies": [{ "name": "hosihikari" }],
  "extraInfo": {
    "api": "1",
    "format_version": "1",
    "entryType": "My.MyPlugin"
  }
}
```

`type` 决定由哪个 Provider 处理；`api` 必须等于 `PluginContract.ApiVersion`，不匹配的包在**载入之前**就会被剔除并给出可读报告。

## 作者要知道的几条

- **钩子只承诺顺序与幂等**，不承诺它们落在服务器的哪个阶段 —— `OnEnable` 时世界未必已加载。
- **契约只覆盖通用宿主服务**：日志、配置、数据目录、调度、事件总线、服务解析。世界、实体、命令、游戏事件不在其中。
- **卸载要真能回收**：不要留长期静态状态，不要裸起线程。事件订阅与定时器由框架在卸载时替你收回，但那只是兜底。
- 插件之间的直接引用是允许的；跨插件的事件类型必须放在共享层，否则两边看到的是两个类型。

## 许可

LGPL-3.0-or-later，见 [LICENSE.md](LICENSE.md)。
