// Copyright (c) 2024, Olaf Kober <olaf.kober@outlook.com>

using System;
using System.Threading;
using NFluent;
using NUnit.Framework;


namespace Amarok.Events;


public class Test_EventRecorder
{
    [Test]
    public void From_Event()
    {
        using var src = new EventSource<String>();
        var       rec = EventRecorder.From(src.Event);

        Check.That(rec.IsPaused).IsFalse();

        Check.That(rec.Count).IsEqualTo(0);

        Check.That(rec.Events).IsEmpty();

        Check.That(rec.EventInfos).IsEmpty();
    }

    [Test]
    public void From_EventSource()
    {
        var src = new EventSource<String>();
        var rec = EventRecorder.From(src);

        Check.That(rec.IsPaused).IsFalse();

        Check.That(rec.Count).IsEqualTo(0);

        Check.That(rec.Events).IsEmpty();

        Check.That(rec.EventInfos).IsEmpty();
    }

    [Test]
    public void From_NullEventSource()
    {
        Check.ThatCode(() => EventRecorder.From<Int32>(null)).Throws<ArgumentNullException>();
    }


    [Test]
    public void Construction()
    {
        var rec = EventRecorder.From(new Event<String>());

        Check.That(rec.IsPaused).IsFalse();

        Check.That(rec.Count).IsEqualTo(0);

        Check.That(rec.Events).IsEmpty();

        Check.That(rec.EventInfos).IsEmpty();
    }

    [Test]
    public void Record_SingleEvent()
    {
        using var src = new EventSource<String>();
        var       rec = EventRecorder.From(src.Event);

        src.Invoke("aaa");

        Check.That(rec.IsPaused).IsFalse();

        Check.That(rec.Count).IsEqualTo(1);

        Check.That(rec.Events).ContainsExactly("aaa");

        Check.That(rec.EventInfos).HasSize(1);

        var info1 = rec.EventInfos[0];

        Check.That(info1.Value).IsEqualTo("aaa");

        Check.That(info1.Index).IsEqualTo(0);

        Check.That(info1.Timestamp - DateTimeOffset.Now).IsLessThan(TimeSpan.FromMilliseconds(500));

        Check.That(info1.TimeOffset).IsEqualTo(TimeSpan.Zero);

        Check.That(info1.Thread).IsEqualTo(Thread.CurrentThread);
    }

    [Test]
    public void Record_MultipleEvents()
    {
        using var src = new EventSource<String>();
        var       rec = EventRecorder.From(src.Event);

        src.Invoke("aaa");
        Thread.Sleep(200);
        src.Invoke("bbb");
        Thread.Sleep(200);
        src.Invoke("ccc");

        Check.That(rec.IsPaused).IsFalse();

        Check.That(rec.Count).IsEqualTo(3);

        Check.That(rec.Events).ContainsExactly("aaa", "bbb", "ccc");

        Check.That(rec.EventInfos).HasSize(3);

        var info1 = rec.EventInfos[0];
        var info2 = rec.EventInfos[1];
        var info3 = rec.EventInfos[2];

        Check.That(info1.Value).IsEqualTo("aaa");

        Check.That(info1.Index).IsEqualTo(0);

        Check.That(info1.Timestamp - DateTimeOffset.Now).IsLessThan(TimeSpan.FromMilliseconds(500));

        Check.That(info1.TimeOffset).IsEqualTo(TimeSpan.Zero);

        Check.That(info1.Thread).IsEqualTo(Thread.CurrentThread);

        Check.That(info2.Value).IsEqualTo("bbb");

        Check.That(info2.Index).IsEqualTo(1);

        Check.That(info2.Timestamp - DateTimeOffset.Now).IsLessThan(TimeSpan.FromMilliseconds(500));

        Check.That(info2.TimeOffset)
            .IsLessThan(TimeSpan.FromMilliseconds(500))
            .And.IsGreaterThan(TimeSpan.FromMilliseconds(200));

        Check.That(info2.Thread).IsEqualTo(Thread.CurrentThread);

        Check.That(info3.Value).IsEqualTo("ccc");

        Check.That(info3.Index).IsEqualTo(2);

        Check.That(info3.Timestamp - DateTimeOffset.Now).IsLessThan(TimeSpan.FromMilliseconds(500));

        Check.That(info3.TimeOffset)
            .IsLessThan(TimeSpan.FromMilliseconds(500))
            .And.IsGreaterThan(TimeSpan.FromMilliseconds(200));

        Check.That(info3.Thread).IsEqualTo(Thread.CurrentThread);

        Check.That(info1.Timestamp < info2.Timestamp).IsTrue();

        Check.That(info2.Timestamp < info3.Timestamp).IsTrue();
    }

    [Test]
    public void Events_Returns_CachedResults()
    {
        using var src = new EventSource<String>();
        var       rec = EventRecorder.From(src.Event);

        src.Invoke("aaa");

        var events1 = rec.Events;
        var events2 = rec.Events;

        Check.That(events1).HasSize(1);

        Check.That(events1).IsSameReferenceAs(events2);

        src.Invoke("bbb");

        Check.That(rec.Events).HasSize(2);

        Check.That(rec.Events).Not.IsSameReferenceAs(events1);
    }

    [Test]
    public void EventInfos_Returns_CachedResults()
    {
        using var src = new EventSource<String>();
        var       rec = EventRecorder.From(src.Event);

        src.Invoke("aaa");

        var infos1 = rec.EventInfos;
        var infos2 = rec.EventInfos;

        Check.That(infos1).HasSize(1);

        Check.That(infos1).IsSameReferenceAs(infos2);

        src.Invoke("bbb");

        Check.That(rec.EventInfos).HasSize(2);

        Check.That(rec.EventInfos).Not.IsSameReferenceAs(infos1);
    }

    [Test]
    public void Pause()
    {
        using var src = new EventSource<String>();
        var       rec = EventRecorder.From(src.Event);

        rec.Pause();
        src.Invoke("aaa");

        Check.That(rec.IsPaused).IsTrue();

        Check.That(rec.Count).IsEqualTo(0);

        rec.Resume();

        Check.That(rec.IsPaused).IsFalse();

        Check.That(rec.Count).IsEqualTo(0);
    }

    [Test]
    public void Resume()
    {
        using var src = new EventSource<String>();
        var       rec = EventRecorder.From(src.Event);

        rec.Pause();
        src.Invoke("aaa");

        Check.That(rec.IsPaused).IsTrue();

        Check.That(rec.Count).IsEqualTo(0);

        rec.Resume();
        src.Invoke("bbb");

        Check.That(rec.IsPaused).IsFalse();

        Check.That(rec.Count).IsEqualTo(1);

        Check.That(rec.Events).ContainsExactly("bbb");
    }

    [Test]
    public void Reset_ClearsEvents()
    {
        using var src = new EventSource<String>();
        var       rec = EventRecorder.From(src.Event);

        src.Invoke("aaa");

        Check.That(rec.IsPaused).IsFalse();

        Check.That(rec.Count).IsEqualTo(1);

        rec.Reset();

        Check.That(rec.IsPaused).IsFalse();

        Check.That(rec.Count).IsEqualTo(0);
    }

    [Test]
    public void Reset_Resumes()
    {
        using var src = new EventSource<String>();
        var       rec = EventRecorder.From(src.Event);

        src.Invoke("aaa");
        rec.Pause();

        Check.That(rec.IsPaused).IsTrue();

        Check.That(rec.Count).IsEqualTo(1);

        rec.Reset();

        Check.That(rec.IsPaused).IsFalse();

        Check.That(rec.Count).IsEqualTo(0);
    }

    [Test]
    public void Dispose()
    {
        using var src = new EventSource<String>();
        var       rec = EventRecorder.From(src.Event);

        src.Invoke("aaa");
        rec.Dispose();
        src.Invoke("bbb");

        Check.That(rec.Count).IsEqualTo(1);

        Check.That(rec.Events).ContainsExactly("aaa");

        Check.That(rec.EventInfos).HasSize(1);

        var info1 = rec.EventInfos[0];

        Check.That(info1.Value).IsEqualTo("aaa");

        Check.That(info1.Index).IsEqualTo(0);

        Check.That(info1.Timestamp - DateTimeOffset.Now).IsLessThan(TimeSpan.FromMilliseconds(500));

        Check.That(info1.TimeOffset).IsEqualTo(TimeSpan.Zero);

        Check.That(info1.Thread).IsEqualTo(Thread.CurrentThread);
    }
}
