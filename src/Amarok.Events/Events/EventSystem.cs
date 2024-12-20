﻿// Copyright (c) 2024, Olaf Kober <olaf.kober@outlook.com>

using System;


namespace Amarok.Events;


/// <summary>
///     This type provides static members that affect the entire app domain-wide event system. This type is
///     thread-safe.
/// </summary>
public static class EventSystem
{
    // static data
    private static readonly EventSource<Exception> sUnobservedExceptionEventSource = new();


    #region ++ Public Interface ++

    /// <summary>
    ///     An event that is raised every time an exception is thrown by one of the event subscribers. All exceptions
    ///     thrown by event subscribers of all event sources in the current app domain are forwarded to this single
    ///     global event. Applications can subscribe on this event and thus log otherwise unobserved exceptions
    ///     occurring in the application's event subscribers.
    /// </summary>
    public static Event<Exception> UnobservedException => sUnobservedExceptionEventSource.Event;


    /// <summary>
    ///     Notifies that an unobserved exception was thrown by one of the application's event subscribers.
    /// </summary>
    /// 
    /// <param name="exception">
    ///     The exception that was caught.
    /// </param>
    /// 
    /// <exception cref="ArgumentNullException">
    ///     A null reference was passed to a method that did not accept it as a valid argument.
    /// </exception>
    public static void NotifyUnobservedException(Exception? exception)
    {
        if (exception == null)
            ThrowHelper.ThrowArgumentNullException(nameof(exception));

        sUnobservedExceptionEventSource.Invoke(exception!);
    }

    #endregion
}
