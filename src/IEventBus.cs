namespace Hosihikari;

/// <summary>
/// 事件总线（ADR-0014）：在发布者线程上同步分发；单个 handler 抛异常被捕获、上报、不影响其余 handler。
/// 载体事件（ADR-0007 并入）也是原样透传，插件不必分场合记两套语义。
/// </summary>
public interface IEventBus
{
    IDisposable Subscribe<TEvent>(Action<TEvent> handler);

    void Publish<TEvent>(TEvent @event);
}

/// <summary>
/// 调度器（ADR-0007）。<c>RunLater</c> / <c>RunEvery</c> 跑在线程池上；
/// "在游戏主线程上跑"是载体能力，自持态拿不到（ADR-0013 / ADR-0019）。
/// </summary>
public interface IScheduler
{
    IDisposable RunLater(TimeSpan delay, Action work);

    IDisposable RunEvery(TimeSpan interval, Action work);
}
