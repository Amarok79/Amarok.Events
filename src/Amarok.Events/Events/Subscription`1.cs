// Copyright (c) 2024, Olaf Kober <olaf.kober@outlook.com>

using System;
using System.Threading.Tasks;


namespace Amarok.Events;


/// <summary>
///     Implementation class that represents a subscription. Concrete derivations implement specific behaviors, for
///     example, support for synchronous and asynchronous handler methods, weak subscriptions, etc.
/// </summary>
internal abstract class Subscription<T> : IDisposable
{
    /// <summary>
    ///     Invokes the subscription's handler in a synchronous way.
    /// </summary>
    public abstract void Invoke(T value);

    /// <summary>
    ///     Invokes the subscription's handler in an asynchronous way.
    /// </summary>
    public abstract ValueTask InvokeAsync(T value);

    /// <summary>
    ///     Disposes the subscription; removes it from the event source.
    /// </summary>
    public abstract void Dispose();
}
