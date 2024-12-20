﻿// Copyright (c) 2024, Olaf Kober <olaf.kober@outlook.com>

using System;
using System.Diagnostics;
using System.Threading.Tasks;


namespace Amarok.Events;


/// <summary>
///     This type represents an Event that allows consumers to subscribe on. This type is thread-safe.
/// </summary>
[DebuggerStepThrough]
public readonly struct Event<T> : IEquatable<Event<T>>
{
    // a reference to the owning event source; can be null
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly EventSource<T>? mSource;


    #region ++ Public Interface ++

    /// <summary>
    ///     Gets a reference to the owning <see cref="EventSource{T}"/>, or null if this <see cref="Event{T}"/> isn't
    ///     associated with an <see cref="EventSource{T}"/>. See also <see cref="HasSource"/>.
    /// </summary>
    public readonly EventSource<T>? Source => mSource;

    /// <summary>
    ///     Gets a boolean value indicating whether this <see cref="Event{T}"/> is associated with an
    ///     <see cref="EventSource{T}"/>. See also <see cref="Source"/>.
    /// </summary>
    public readonly Boolean HasSource => mSource != null;


    /// <summary>
    ///     Initializes a new instance.
    /// </summary>
    internal Event(EventSource<T> eventSource)
    {
        mSource = eventSource;
    }


    /// <summary>
    ///     Subscribes the given callback on the event. The callback will be invoked every time the event is raised.
    ///     This method establishes a strong reference between the event source and the object holding the supplied
    ///     callback, aka subscriber. That means as long as the event source is kept in memory, it will also keep the
    ///     subscriber in memory. To break this strong reference, you can dispose the returned subscription.
    /// </summary>
    /// 
    /// <param name="action">
    ///     The callback to subscribe on the event.
    /// </param>
    /// 
    /// <returns>
    ///     An object that represents the newly created subscription. Disposing this object will cancel the
    ///     subscription and remove the callback from the event source's subscription list.
    /// </returns>
    /// 
    /// <exception cref="ArgumentNullException">
    ///     A null reference was passed to a method that did not accept it as a valid argument.
    /// </exception>
    public readonly IDisposable Subscribe(Action<T> action)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (action == null)
            ThrowHelper.ThrowArgumentNullException(nameof(action));

        return mSource == null ? NullSubscription.Instance : mSource.Add(action!);
    }

    /// <summary>
    ///     Subscribes the given callback on the event. The callback will be invoked every time the event is raised.
    ///     This method establishes a strong reference between the event source and the object holding the supplied
    ///     callback, aka subscriber. That means as long as the event source is kept in memory, it will also keep the
    ///     subscriber in memory. To break this strong reference, you can dispose the returned subscription.
    /// </summary>
    /// 
    /// <param name="action">
    ///     The callback to subscribe on the event.
    /// </param>
    /// 
    /// <returns>
    ///     An object that represents the newly created subscription. Disposing this object will cancel the
    ///     subscription and remove the callback from the event source's subscription list.
    /// </returns>
    /// 
    /// <exception cref="ArgumentNullException">
    ///     A null reference was passed to a method that did not accept it as a valid argument.
    /// </exception>
    public readonly IDisposable Subscribe(Action action)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (action == null)
            ThrowHelper.ThrowArgumentNullException(nameof(action));

        return Subscribe(_ => action!());
    }


    /// <summary>
    ///     Subscribes the given callback on the event. The callback will be invoked every time the event is raised.
    ///     This method establishes a strong reference between the event source and the object holding the supplied
    ///     callback, aka subscriber. That means as long as the event source is kept in memory, it will also keep the
    ///     subscriber in memory. To break this strong reference, you can dispose the returned subscription.
    /// </summary>
    /// 
    /// <param name="func">
    ///     The callback to subscribe on the event.
    /// </param>
    /// 
    /// <returns>
    ///     An object that represents the newly created subscription. Disposing this object will cancel the
    ///     subscription and remove the callback from the event source's subscription list.
    /// </returns>
    /// 
    /// <exception cref="ArgumentNullException">
    ///     A null reference was passed to a method that did not accept it as a valid argument.
    /// </exception>
    public readonly IDisposable Subscribe(Func<T, Task> func)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (func == null)
            ThrowHelper.ThrowArgumentNullException(nameof(func));

        return mSource == null ? NullSubscription.Instance : mSource.Add(func!);
    }

    /// <summary>
    ///     Subscribes the given callback on the event. The callback will be invoked every time the event is raised.
    ///     This method establishes a strong reference between the event source and the object holding the supplied
    ///     callback, aka subscriber. That means as long as the event source is kept in memory, it will also keep the
    ///     subscriber in memory. To break this strong reference, you can dispose the returned subscription.
    /// </summary>
    /// 
    /// <param name="func">
    ///     The callback to subscribe on the event.
    /// </param>
    /// 
    /// <returns>
    ///     An object that represents the newly created subscription. Disposing this object will cancel the
    ///     subscription and remove the callback from the event source's subscription list.
    /// </returns>
    /// 
    /// <exception cref="ArgumentNullException">
    ///     A null reference was passed to a method that did not accept it as a valid argument.
    /// </exception>
    public readonly IDisposable Subscribe(Func<Task> func)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (func == null)
            ThrowHelper.ThrowArgumentNullException(nameof(func));

        return Subscribe(_ => func!());
    }


    /// <summary>
    ///     Subscribes the given progress object on the event. The progress object will be invoked every time the
    ///     event is raised. This method establishes a strong reference between the event source and the progress
    ///     object. That means as long as the event source is kept in memory, it will also keep the progress object
    ///     in memory. To break this strong reference, you can dispose the returned subscription.
    /// </summary>
    /// 
    /// <param name="progress">
    ///     The progress object to subscribe on the event.
    /// </param>
    /// 
    /// <returns>
    ///     An object that represents the newly created subscription. Disposing this object will cancel the
    ///     subscription and remove the progress object from the event source's subscription list.
    /// </returns>
    /// 
    /// <exception cref="ArgumentNullException">
    ///     A null reference was passed to a method that did not accept it as a valid argument.
    /// </exception>
    public readonly IDisposable Subscribe(IProgress<T> progress)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (progress == null)
            ThrowHelper.ThrowArgumentNullException(nameof(progress));

        return Subscribe(x => progress!.Report(x));
    }


    /// <summary>
    ///     Subscribes the given callback on the event. The callback will be invoked every time the event is raised.
    ///     This method establishes a weak reference between the event source and the object holding the supplied
    ///     callback, aka subscriber. That means that the subscription is kept alive only as long as both event
    ///     source and subscriber are kept in memory via strong references from other objects. The event source alone
    ///     doesn't keep the subscriber in memory. You have to keep a strong reference to the returned subscription
    ///     object to achieve this. The subscription can be canceled at any time by disposing the returned
    ///     subscription object. Otherwise, the subscription is automatically canceled if the subscriber is being
    ///     garbage collected. For this to happen no other strong reference to the returned subscription must exist.
    /// </summary>
    /// 
    /// <param name="action">
    ///     The callback to subscribe on the event.
    /// </param>
    /// 
    /// <returns>
    ///     An object that represents the newly created subscription. Disposing this object will cancel the
    ///     subscription and remove the callback from the event source's subscription list.
    /// </returns>
    /// 
    /// <exception cref="ArgumentNullException">
    ///     A null reference was passed to a method that did not accept it as a valid argument.
    /// </exception>
    public readonly IDisposable SubscribeWeak(Action<T> action)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (action == null)
            ThrowHelper.ThrowArgumentNullException(nameof(action));

        return mSource == null ? NullSubscription.Instance : mSource.AddWeak(action!);
    }

    /// <summary>
    ///     Subscribes the given callback on the event. The callback will be invoked every time the event is raised.
    ///     This method establishes a weak reference between the event source and the object holding the supplied
    ///     callback, aka subscriber. That means that the subscription is kept alive only as long as both event
    ///     source and subscriber are kept in memory via strong references from other objects. The event source alone
    ///     doesn't keep the subscriber in memory. You have to keep a strong reference to the returned subscription
    ///     object to achieve this. The subscription can be canceled at any time by disposing the returned
    ///     subscription object. Otherwise, the subscription is automatically canceled if the subscriber is being
    ///     garbage collected. For this to happen no other strong reference to the returned subscription must exist.
    /// </summary>
    /// 
    /// <param name="action">
    ///     The callback to subscribe on the event.
    /// </param>
    /// 
    /// <returns>
    ///     An object that represents the newly created subscription. Disposing this object will cancel the
    ///     subscription and remove the callback from the event source's subscription list.
    /// </returns>
    /// 
    /// <exception cref="ArgumentNullException">
    ///     A null reference was passed to a method that did not accept it as a valid argument.
    /// </exception>
    public readonly IDisposable SubscribeWeak(Action action)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (action == null)
            ThrowHelper.ThrowArgumentNullException(nameof(action));

        return SubscribeWeak(_ => action!());
    }


    /// <summary>
    ///     Subscribes the given callback on the event. The callback will be invoked every time the event is raised.
    ///     This method establishes a weak reference between the event source and the object holding the supplied
    ///     callback, aka subscriber. That means that the subscription is kept alive only as long as both event
    ///     source and subscriber are kept in memory via strong references from other objects. The event source alone
    ///     doesn't keep the subscriber in memory. You have to keep a strong reference to the returned subscription
    ///     object to achieve this. The subscription can be canceled at any time by disposing the returned
    ///     subscription object. Otherwise, the subscription is automatically canceled if the subscriber is being
    ///     garbage collected. For this to happen no other strong reference to the returned subscription must exist.
    /// </summary>
    /// 
    /// <param name="func">
    ///     The callback to subscribe on the event.
    /// </param>
    /// 
    /// <returns>
    ///     An object that represents the newly created subscription. Disposing this object will cancel the
    ///     subscription and remove the callback from the event source's subscription list.
    /// </returns>
    /// 
    /// <exception cref="ArgumentNullException">
    ///     A null reference was passed to a method that did not accept it as a valid argument.
    /// </exception>
    public readonly IDisposable SubscribeWeak(Func<T, Task> func)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (func == null)
            ThrowHelper.ThrowArgumentNullException(nameof(func));

        return mSource == null ? NullSubscription.Instance : mSource.AddWeak(func!);
    }

    /// <summary>
    ///     Subscribes the given callback on the event. The callback will be invoked every time the event is raised.
    ///     This method establishes a weak reference between the event source and the object holding the supplied
    ///     callback, aka subscriber. That means that the subscription is kept alive only as long as both event
    ///     source and subscriber are kept in memory via strong references from other objects. The event source alone
    ///     doesn't keep the subscriber in memory. You have to keep a strong reference to the returned subscription
    ///     object to achieve this. The subscription can be canceled at any time by disposing the returned
    ///     subscription object. Otherwise, the subscription is automatically canceled if the subscriber is being
    ///     garbage collected. For this to happen no other strong reference to the returned subscription must exist.
    /// </summary>
    /// 
    /// <param name="func">
    ///     The callback to subscribe on the event.
    /// </param>
    /// 
    /// <returns>
    ///     An object that represents the newly created subscription. Disposing this object will cancel the
    ///     subscription and remove the callback from the event source's subscription list.
    /// </returns>
    /// 
    /// <exception cref="ArgumentNullException">
    ///     A null reference was passed to a method that did not accept it as a valid argument.
    /// </exception>
    public readonly IDisposable SubscribeWeak(Func<Task> func)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (func == null)
            ThrowHelper.ThrowArgumentNullException(nameof(func));

        return SubscribeWeak(_ => func!());
    }


    /// <summary>
    ///     Subscribes the given progress object on the event. The progress object will be invoked every time the
    ///     event is raised. This method establishes a weak reference between the event source and the progress
    ///     object. That means that the subscription is kept alive only as long as both event source and progress
    ///     object are kept in memory via strong references from other objects. The event source alone doesn't keep
    ///     the progress object in memory. You have to keep a strong reference to the returned subscription object to
    ///     achieve this. The subscription can be canceled at any time by disposing the returned subscription object.
    ///     Otherwise, the subscription is automatically canceled if the progress object is being garbage collected.
    ///     For this to happen no other strong reference to the returned subscription must exist.
    /// </summary>
    /// 
    /// <param name="progress">
    ///     The progress object to subscribe on the event.
    /// </param>
    /// 
    /// <returns>
    ///     An object that represents the newly created subscription. Disposing this object will cancel the
    ///     subscription and remove the progress object from the event source's subscription list.
    /// </returns>
    /// 
    /// <exception cref="ArgumentNullException">
    ///     A null reference was passed to a method that did not accept it as a valid argument.
    /// </exception>
    public readonly IDisposable SubscribeWeak(IProgress<T> progress)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (progress == null)
            ThrowHelper.ThrowArgumentNullException(nameof(progress));

        return SubscribeWeak(x => progress!.Report(x));
    }


    /// <summary>
    ///     Returns a string that represents the current instance.
    /// </summary>
    public readonly override String ToString()
    {
        return HasSource ? $"Event<{typeof(T).Name}> ⇔ {mSource}" : $"Event<{typeof(T).Name}> ⇔ <null>";
    }

    #endregion

    #region ++ Public Interface (Equality) ++

    /// <summary>
    ///     Returns the hash code for the current instance.
    /// </summary>
    /// 
    /// <returns>
    ///     A 32-bit signed integer hash code.
    /// </returns>
    public readonly override Int32 GetHashCode()
    {
        return mSource?.GetHashCode() ?? 0;
    }


    /// <summary>
    ///     Determines whether the specified instance is equal to the current instance.
    /// </summary>
    /// 
    /// <param name="obj">
    ///     The instance to compare with the current instance.
    /// </param>
    /// 
    /// <returns>
    ///     True, if the specified instance is equal to the current instance; otherwise, False.
    /// </returns>
    public readonly override Boolean Equals(Object? obj)
    {
        return obj is Event<T> other && Equals(other);
    }

    /// <summary>
    ///     Determines whether the specified instance is equal to the current instance.
    /// </summary>
    /// 
    /// <param name="other">
    ///     The instance to compare with the current instance.
    /// </param>
    /// 
    /// <returns>
    ///     True, if the specified instance is equal to the current instance; otherwise, False.
    /// </returns>
    public readonly Boolean Equals(Event<T> other)
    {
        return ReferenceEquals(mSource, other.mSource);
    }


    /// <summary>
    ///     Determines whether the specified instances are equal.
    /// </summary>
    /// 
    /// <param name="left">
    ///     The first instance to compare.
    /// </param>
    /// <param name="right">
    ///     The second instance to compare.
    /// </param>
    /// <returns>
    ///     True, if the specified instances are equal; otherwise, False.
    /// </returns>
    public static Boolean operator ==(Event<T> left, Event<T> right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Determines whether the specified instances are unequal.
    /// </summary>
    /// 
    /// <param name="left">
    ///     The first instance to compare.
    /// </param>
    /// <param name="right">
    ///     The second instance to compare.
    /// </param>
    /// <returns>
    ///     True, if the specified instances are unequal; otherwise, False.
    /// </returns>
    public static Boolean operator !=(Event<T> left, Event<T> right)
    {
        return !left.Equals(right);
    }

    #endregion
}
