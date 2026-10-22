# Hosihikari.Plugin.Templates

预编译星光框架插件的 `dotnet new` 模板。生成的是一个 .NET 类库，带 `EntryPoint<T>`、清单，以及调用框架打包器的打包脚本。

## 构建并试用

```bash
dotnet pack Hosihikari.Plugin.Templates.csproj -c Release -o artifacts
dotnet new install artifacts/Hosihikari.Plugin.Templates.1.0.0.nupkg
dotnet new hosihikari-plugin -n MyPlugin -o MyPlugin
```

然后在并列的 `PluginManager` checkout 下构建并打包：

```bash
cd MyPlugin
bash ./pack.sh
```

产物在 `dist/MyPlugin/`，复制到 `<服务端>/plugins/MyPlugin/`。

## 模板内容

- `Hosihikari.Plugin.Templates.csproj` —— net10.0 类库，引用契约时 `ExcludeAssets="runtime"`，共享层不会进包。
- `Plugin.cs` —— `IPlugin` 实现与 `EntryPoint<T>`。
- `manifest.json` —— `loader: assembly`，即插件管理者内置路径。
- `pack.ps1` / `pack.sh` —— 构建后调用插件管理者打包器的 `pack-plugin`，按规范剔除共享层。
- 双语插件 README。

## 命名约束

项目名会成为程序集名、命名空间与清单名。请使用字母、数字与点；空格与连字符不是合法的 C# 标识符。
清单里的 `name` 必须与包目录名一致。

## 许可

MIT-0，见 [LICENSE.md](LICENSE.md)。生成的项目引用的 `Hosihikari.Plugin.Abstractions` 1.0.0 契约为
MIT；0.7.0 包仍为 LGPL-3.0-or-later。
