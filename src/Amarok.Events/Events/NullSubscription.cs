// Copyright (c) 2024, Olaf Kober <olaf.kober@outlook.com>

using System;
using System.Diagnostics;


namespace Amarok.Events;


/// <summary>
///     Implementation class that represents a null subscription.
/// </summary>
[DebuggerStepThrough]
internal sealed class NullSubscription : IDisposable
{
    public static readonly IDisposable Instance = new NullSubscription();


    private NullSubscription()
    {
        // shouldn't be construct-able; use static Instance instead
    }


    public void Dispose()
    {
        // intentionally left empty
    }

    public override String ToString()
    {
        return "⇒ <null>";
    }
}
