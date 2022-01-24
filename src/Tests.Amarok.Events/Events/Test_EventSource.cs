// Copyright (c) 2022, Olaf Kober <olaf.kober@outlook.com>

using System;
using NFluent;
using NUnit.Framework;


namespace Amarok.Events;


public class Test_EventSource
{
    [TestFixture]
    public class Dispose
    {
        [Test]
        public void DisposeTwice()
        {
            var src = new EventSource<Int32>();

            Check.That(src.IsDisposed)
               .IsFalse();

            src.Dispose();

            Check.ThatCode(() => src.Dispose())
               .DoesNotThrow();

            Check.That(src.IsDisposed)
               .IsTrue();
        }

        [Test]
        public void DisposeClearsSubscriptions()
        {
            var src = new EventSource<Int32>();
            var sub = src.Event.Subscribe(x => { });

            Check.That(src.IsDisposed)
               .IsFalse();

            Check.That(src.NumberOfSubscriptions)
               .IsEqualTo(1);

            src.Dispose();

            Check.That(src.IsDisposed)
               .IsTrue();

            Check.That(src.NumberOfSubscriptions)
               .IsEqualTo(0);

            Check.ThatCode(() => sub.Dispose())
               .DoesNotThrow();
        }
    }
}
