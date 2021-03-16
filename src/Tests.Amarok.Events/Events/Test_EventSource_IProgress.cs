/* MIT License
 * 
 * Copyright (c) 2020, Olaf Kober
 * https://github.com/Amarok79/Amarok.Events
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Collections.Generic;
using NFluent;
using NUnit.Framework;


namespace Amarok.Events
{
    public class Test_EventSource_IProgress
    {
        public static void FakeMethodWithIProgress(IProgress<Int32> progress)
        {
            for (var i = 0; i < 10; i++)
                progress.Report(i);
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
            using var    src = new EventSource<Int32>();
            Event<Int32> evt = src.Event;

            var progress = new EventSource<Int32>();
            var events   = new List<Int32>();
            progress.Event.Subscribe(x => events.Add(x));

            evt.Subscribe(progress);

            for (var i = 0; i < 10; i++)
                src.Invoke(i);

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
            using var    src = new EventSource<Int32>();
            Event<Int32> evt = src.Event;

            var progress = new EventSource<Int32>();
            var events   = new List<Int32>();
            progress.Event.Subscribe(x => events.Add(x));

            var subscription = evt.SubscribeWeak(progress);

            for (var i = 0; i < 10; i++)
                src.Invoke(i);

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
}
