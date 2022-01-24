// Copyright (c) 2022, Olaf Kober <olaf.kober@outlook.com>

using System;
using System.Diagnostics;
using System.Threading.Tasks;


namespace Amarok.Events;


/// <summary>
///     Implementation class that represents a weak subscription. This weak subscription usually refers
///     via weak reference to another subscription, which again refers back to this subscription again
///     via weak reference.
/// </summary>
[DebuggerStepThrough]
internal sealed class WeakSubscription<T> : Subscription<T>
{
    /// <summary>
    ///     A reference to the event source; necessary for disposal.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly EventSource<T> mSource;

    /// <summary>
    ///     A weak reference to another subscription referring to the handler.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly WeakReference<Subscription<T>> mNextSubscription;


    /// <summary>
    ///     For better debugging experience.
    /// </summary>
    public WeakReference<Subscription<T>> Subscription => mNextSubscription;


    /// <summary>
    ///     Initializes a new instance.
    /// </summary>
    public WeakSubscription(EventSource<T> source, Subscription<T> subscription)
    {
        mSource           = source;
        mNextSubscription = new WeakReference<Subscription<T>>(subscription);
    }


    /// <summary>
    ///     Invokes the subscription's handler in a synchronous way.
    /// </summary>
    public override void Invoke(T value)
    {
        if (mNextSubscription.TryGetTarget(out var subscription))
        {
            // forward invocation to next subscription
            subscription.Invoke(value);
        }
        else
        {
            // otherwise, remove ourselves from event source
            mSource.Remove(this);
        }
    }

    /// <summary>
    ///     Invokes the subscription's handler in an asynchronous way.
    /// </summary>
    public override ValueTask InvokeAsync(T value)
    {
        if (mNextSubscription.TryGetTarget(out var subscription))
        {
            // forward invocation to next subscription
            return subscription.InvokeAsync(value);
        }

        // otherwise, remove ourselves from event source
        mSource.Remove(this);

        return new ValueTask(Task.CompletedTask);
    }

    /// <summary>
    ///     Disposes the subscription; removes it from the event source.
    /// </summary>
    public override void Dispose()
    {
        // simply, remove ourselves from event source
        mSource.Remove(this);
    }

    /// <summary>
    ///     Returns a string that represents the current instance.
    /// </summary>
    public override String ToString()
    {
        if (mNextSubscription.TryGetTarget(out var subscription))
        {
            return $"⇒ weak {subscription}";
        }

        return "⇒ weak ⇒ <null>";
    }


    internal void TestingClearNextSubscription()
    {
        mNextSubscription.SetTarget(null!);
    }
}
