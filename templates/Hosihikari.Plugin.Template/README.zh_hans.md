# Hosihikari.Plugin.Template

预编译星光框架插件的 `dotnet new` 模板。生成的是一个 .NET 类库，带 `EntryPoint<T>`、清单，以及调用框架打包器的打包脚本。

## 构建并试用

```bash
dotnet pack Hosihikari.Plugin.Template.csproj -c Release -o artifacts
dotnet new install artifacts/Hosihikari.Plugin.Template.0.1.0.nupkg
dotnet new hosihikari-plugin -n MyPlugin -o MyPlugin
```

然后在并列的 `Hosihikari.PluginManager` checkout 下构建并打包：

```bash
cd MyPlugin
bash ./pack.sh
```

产物在 `dist/MyPlugin/`，复制到 `<服务端>/plugins/MyPlugin/`。

## 模板内容

- `Hosihikari.Plugin.Template.csproj` —— net10.0 类库，引用契约时 `ExcludeAssets="runtime"`，共享层不会进包。
- `Plugin.cs` —— `IPlugin` 实现与 `EntryPoint<T>`。
- `manifest.json` —— `provider: assembly`，即核心内置路径。
- `pack.ps1` / `pack.sh` —— 构建后调用核心打包器的 `pack-plugin`，按规范剔除共享层。
- 双语插件 README。

## 命名约束

项目名会成为程序集名、命名空间与清单名。请使用字母、数字与点；空格与连字符不是合法的 C# 标识符。
清单里的 `name` 必须与包目录名一致。

## 许可

MIT-0，见 [LICENSE.md](LICENSE.md)。生成的项目引用的 `Hosihikari.Abstractions` 契约仍为
LGPL-3.0-or-later。
