// Copyright (c) 2022, Olaf Kober <olaf.kober@outlook.com>

using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;


namespace Amarok.Events;


/// <summary>
///     Implementation class that represents a subscription to a sync handler method.
/// </summary>
[DebuggerStepThrough]
internal sealed class ActionSubscription<T> : Subscription<T>
{
    // a reference to the event source; necessary for disposal
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly EventSource<T> mSource;

    // a delegate to the handler method
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Action<T> mAction;

    // an optional weak reference back to another subscription holding this subscription
    // also via weak reference; necessary for automatic removal magic of weak subscriptions
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private WeakReference<Subscription<T>>? mPreviousSubscription;


    /// <summary>
    ///     For better debugging experience.
    /// </summary>
    public Object? Target => mAction.Target;

    /// <summary>
    ///     For better debugging experience.
    /// </summary>
    public MethodInfo Method => mAction.Method;


    /// <summary>
    ///     Initializes a new instance.
    /// </summary>
    public ActionSubscription(EventSource<T> source, Action<T> action)
    {
        mSource = source;
        mAction = action;
    }


    /// <summary>
    ///     Invoked to establish a weak reference back to another subscription. Only called for weak
    ///     subscriptions.
    /// </summary>
    public void SetPreviousSubscription(Subscription<T> subscription)
    {
        mPreviousSubscription = new WeakReference<Subscription<T>>(subscription);
    }

    /// <summary>
    ///     Invokes the subscription's handler in a synchronous way.
    /// </summary>
    public override void Invoke(T value)
    {
        mAction(value);
    }

    /// <summary>
    ///     Invokes the subscription's handler in an asynchronous way.
    /// </summary>
    public override ValueTask InvokeAsync(T value)
    {
        mAction(value);

        return new ValueTask(Task.CompletedTask);
    }

    /// <summary>
    ///     Disposes the subscription; removes it from the event source.
    /// </summary>
    public override void Dispose()
    {
        if (mPreviousSubscription != null)
        {
            // dispose the previous subscription, if still reachable
            if (mPreviousSubscription.TryGetTarget(out var subscription))
            {
                subscription.Dispose();
            }
        }
        else
        {
            // remove ourselves from event source
            mSource.Remove(this);
        }
    }

    /// <summary>
    ///     Returns a string that represents the current instance.
    /// </summary>
    public override String ToString()
    {
        return $"⇒ {mAction.Method.DeclaringType?.FullName}.{mAction.Method.Name}()";
    }


    internal Subscription<T>? TestingGetPreviousSubscription()
    {
        if (mPreviousSubscription == null)
        {
            return null;
        }

        if (mPreviousSubscription.TryGetTarget(out var subscription))
        {
            return subscription;
        }

        return null;
    }

    internal void TestingClearNextSubscription()
    {
        mPreviousSubscription?.SetTarget(null!);
    }
}
