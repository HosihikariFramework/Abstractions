namespace Hosihikari;

/// <summary>
/// 入口声明的运行期句柄。加载器用它在载入之后构造插件实例 —— 泛型约束由编译器保证，
/// 这里只负责把它变成一个可调用的入口。
/// </summary>
public abstract class EntryPointAttributeBase : Attribute
{
    /// <summary>构造入口实例。</summary>
    /// <returns>新的插件实例。</returns>
    public abstract IPlugin CreateInstance();
}

/// <summary>
/// 声明插件的入口类型。程序集上至多一个。
/// <code>[assembly: EntryPoint&lt;MyPlugin&gt;]</code>
/// </summary>
/// <typeparam name="T">入口类型。必须是 <see cref="IPlugin"/>，且有无参构造 —— 两条都由编译器在这里强制。</typeparam>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
public sealed class EntryPointAttribute<T> : EntryPointAttributeBase where T : IPlugin, new()
{
    /// <inheritdoc/>
    public override IPlugin CreateInstance() => new T();
}
