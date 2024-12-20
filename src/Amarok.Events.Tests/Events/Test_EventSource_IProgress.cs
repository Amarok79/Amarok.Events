﻿// Copyright (c) 2024, Olaf Kober <olaf.kober@outlook.com>

using System;
using System.Collections.Generic;
using NFluent;
using NUnit.Framework;


namespace Amarok.Events;


public class Test_EventSource_IProgress
{
    public static void FakeMethodWithIProgress(IProgress<Int32> progress)
    {
        for (var i = 0; i < 10; i++)
        {
            progress.Report(i);
        }
    }


    [Test]
    public void EventSource_CanBeUsedAs_IProgress()
    {
        using var src = new EventSource<Int32>();

        var events = new List<Int32>();
        src.Event.Subscribe(x => events.Add(x));

        FakeMethodWithIProgress(src);

        Check.That(events)
        .ContainsExactly(
            0,
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9
        );
    }

    [Test]
    public void Events_CanBeForwardedTo_IProgress_Subscribe()
    {
        using var src = new EventSource<Int32>();
        var       evt = src.Event;

        var progress = new EventSource<Int32>();
        var events   = new List<Int32>();
        progress.Event.Subscribe(x => events.Add(x));

        evt.Subscribe(progress);

        for (var i = 0; i < 10; i++)
        {
            src.Invoke(i);
        }

        Check.That(events)
        .ContainsExactly(
            0,
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9
        );
    }

    [Test]
    public void Events_CanBeForwardedTo_IProgress_SubscribeWeak()
    {
        using var src = new EventSource<Int32>();
        var       evt = src.Event;

        var progress = new EventSource<Int32>();
        var events   = new List<Int32>();
        progress.Event.Subscribe(x => events.Add(x));

        var subscription = evt.SubscribeWeak(progress);

        for (var i = 0; i < 10; i++)
        {
            src.Invoke(i);
        }

        Check.That(events)
        .ContainsExactly(
            0,
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9
        );

        GC.KeepAlive(subscription);
    }
}
