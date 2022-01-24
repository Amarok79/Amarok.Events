// Copyright (c) 2022, Olaf Kober <olaf.kober@outlook.com>

using System;
using NFluent;
using NUnit.Framework;


namespace Amarok.Events;


[TestFixture]
public class Test_EventSystem
{
    [Test]
    public void NotifyUnobservedException_ThrowsForNullException()
    {
        Check.ThatCode(() => EventSystem.NotifyUnobservedException(null))
           .Throws<ArgumentNullException>()
           .WithProperty(x => x.ParamName, "exception");
    }

    [Test]
    public void NotifyUnobservedException_RaisesEvent()
    {
        var       called    = 0;
        Exception exception = null;

        void handler(Exception x)
        {
            exception = x;
            called++;
        }

        using var subscription = EventSystem.UnobservedException.Subscribe(handler);

        Check.That(called)
           .IsEqualTo(0);

        var ex = new ApplicationException();
        EventSystem.NotifyUnobservedException(ex);

        Check.That(called)
           .IsEqualTo(1);

        Check.That(exception)
           .IsSameReferenceAs(ex);
    }
}
