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

#pragma warning disable CS1718 // Comparison made to same variable
#pragma warning disable S1764  // Identical expressions should not be used on both sides of a binary operator

using System;
using System.Threading.Tasks;
using NFluent;
using NUnit.Framework;


namespace Amarok.Events
{
    public class Test_Event
    {
        [TestFixture]
        public class Source
        {
            [Test]
            public void NullEvent()
            {
                var evt = new Event<Int32>();

                Check.That(evt.Source).IsNull();
                Check.That(evt.HasSource).IsFalse();
            }

            [Test]
            public void RealEvent()
            {
                var          src = new EventSource<Int32>();
                Event<Int32> evt = src.Event;

                Check.That(evt.Source).IsSameReferenceAs(src);
                Check.That(evt.HasSource).IsTrue();
            }
        }

        [TestFixture]
        public class Subscribe_Action
        {
            [Test]
            public void Subscribe_On_NullEvent()
            {
                var evt = new Event<String>();
                var sub = evt.Subscribe(x => { });

                Check.That(evt.Source).IsNull();
                Check.That(evt.HasSource).IsFalse();
                Check.That(evt.ToString()).IsEqualTo("Event<String> ⇔ <null>");

                Check.That(sub).IsInstanceOf<NullSubscription>();
                Check.That(sub.ToString()).IsEqualTo("⇒ <null>");
                Check.ThatCode(() => sub.Dispose()).DoesNotThrow();
            }

            [Test]
            public void Subscribe_On_Event()
            {
                var           src = new EventSource<String>();
                Event<String> evt = src.Event;
                var           sub = evt.Subscribe(x => { });

                Check.That(src.NumberOfSubscriptions).IsEqualTo(1);
                Check.That(src.IsDisposed).IsFalse();
                Check.That(src.ToString()).IsEqualTo("EventSource<String>(Subscriptions: 1)");

                Check.That(evt.Source).IsSameReferenceAs(src);
                Check.That(evt.HasSource).IsTrue();
                Check.That(evt.ToString()).IsEqualTo("Event<String> ⇔ EventSource<String>(Subscriptions: 1)");

                Check.That(sub).IsInstanceOf<ActionSubscription<String>>();
                Check.That(sub.ToString()).StartsWith("⇒ Amarok.Events.Test_Event");

                sub.Dispose();

                Check.That(src.NumberOfSubscriptions).IsEqualTo(0);
                Check.That(src.IsDisposed).IsFalse();
                Check.That(src.ToString()).IsEqualTo("EventSource<String>(Subscriptions: 0)");
            }

            [Test]
            public void Subscribe_On_DisposedEventSource()
            {
                var src = new EventSource<String>();
                src.Dispose();

                Event<String> evt = src.Event;
                var           sub = evt.Subscribe(x => { });

                Check.That(src.NumberOfSubscriptions).IsEqualTo(0);
                Check.That(src.IsDisposed).IsTrue();
                Check.That(src.ToString()).IsEqualTo("EventSource<String>(Subscriptions: 0)");

                Check.That(evt.Source).IsSameReferenceAs(src);
                Check.That(evt.HasSource).IsTrue();
                Check.That(evt.ToString()).IsEqualTo("Event<String> ⇔ EventSource<String>(Subscriptions: 0)");

                Check.That(sub).IsInstanceOf<NullSubscription>();
                Check.That(sub.ToString()).StartsWith("⇒ <null>");

                sub.Dispose();

                Check.That(src.NumberOfSubscriptions).IsEqualTo(0);
                Check.That(src.IsDisposed).IsTrue();
                Check.That(src.ToString()).IsEqualTo("EventSource<String>(Subscriptions: 0)");
            }

            [Test]
            public void Exception_For_NullCallback()
            {
                using var src = new EventSource<String>();

                Check.ThatCode(() => src.Event.Subscribe((Action<String>) null))
                     .Throws<ArgumentNullException>()
                     .WithProperty(x => x.ParamName, "action");
            }
        }

        [TestFixture]
        public class SubscribeWeak_Action
        {
            [Test]
            public void Subscribe_On_NullEvent()
            {
                var evt = new Event<String>();
                var sub = evt.SubscribeWeak(x => { });

                Check.That(evt.Source).IsNull();
                Check.That(evt.HasSource).IsFalse();
                Check.That(evt.ToString()).IsEqualTo("Event<String> ⇔ <null>");

                Check.That(sub).IsInstanceOf<NullSubscription>();
                Check.That(sub.ToString()).IsEqualTo("⇒ <null>");
                Check.ThatCode(() => sub.Dispose()).DoesNotThrow();
            }

            [Test]
            public void Subscribe_On_Event()
            {
                var           src = new EventSource<String>();
                Event<String> evt = src.Event;
                var           sub = evt.SubscribeWeak(x => { });

                Check.That(src.NumberOfSubscriptions).IsEqualTo(1);
                Check.That(src.IsDisposed).IsFalse();
                Check.That(src.ToString()).IsEqualTo("EventSource<String>(Subscriptions: 1)");

                Check.That(evt.Source).IsSameReferenceAs(src);
                Check.That(evt.HasSource).IsTrue();
                Check.That(evt.ToString()).IsEqualTo("Event<String> ⇔ EventSource<String>(Subscriptions: 1)");

                Check.That(sub).IsInstanceOf<ActionSubscription<String>>();
                Check.That(sub.ToString()).StartsWith("⇒ Amarok.Events.Test_Event");

                sub.Dispose();

                Check.That(src.NumberOfSubscriptions).IsEqualTo(0);
                Check.That(src.IsDisposed).IsFalse();
                Check.That(src.ToString()).IsEqualTo("EventSource<String>(Subscriptions: 0)");
            }

            [Test]
            public void Subscribe_On_DisposedEventSource()
            {
                var src = new EventSource<String>();
                src.Dispose();

                Event<String> evt = src.Event;
                var           sub = evt.SubscribeWeak(x => { });

                Check.That(src.NumberOfSubscriptions).IsEqualTo(0);
                Check.That(src.IsDisposed).IsTrue();
                Check.That(src.ToString()).IsEqualTo("EventSource<String>(Subscriptions: 0)");

                Check.That(evt.Source).IsSameReferenceAs(src);
                Check.That(evt.HasSource).IsTrue();
                Check.That(evt.ToString()).IsEqualTo("Event<String> ⇔ EventSource<String>(Subscriptions: 0)");

                Check.That(sub).IsInstanceOf<NullSubscription>();
                Check.That(sub.ToString()).StartsWith("⇒ <null>");

                sub.Dispose();

                Check.That(src.NumberOfSubscriptions).IsEqualTo(0);
                Check.That(src.IsDisposed).IsTrue();
                Check.That(src.ToString()).IsEqualTo("EventSource<String>(Subscriptions: 0)");
            }

            [Test]
            public void Exception_For_NullCallback()
            {
                using var src = new EventSource<String>();

                Check.ThatCode(() => src.Event.SubscribeWeak((Action<String>) null))
                     .Throws<ArgumentNullException>()
                     .WithProperty(x => x.ParamName, "action");
            }
        }

        [TestFixture]
        public class Subscribe_Func
        {
            [Test]
            public void Subscribe_On_NullEvent()
            {
                var evt = new Event<String>();
                var sub = evt.Subscribe(async x => await Task.Yield());

                Check.That(evt.Source).IsNull();
                Check.That(evt.HasSource).IsFalse();
                Check.That(evt.ToString()).IsEqualTo("Event<String> ⇔ <null>");

                Check.That(sub).IsInstanceOf<NullSubscription>();
                Check.That(sub.ToString()).IsEqualTo("⇒ <null>");
                Check.ThatCode(() => sub.Dispose()).DoesNotThrow();
            }

            [Test]
            public void Subscribe_On_Event()
            {
                var           src = new EventSource<String>();
                Event<String> evt = src.Event;
                var           sub = evt.Subscribe(async x => await Task.Yield());

                Check.That(src.NumberOfSubscriptions).IsEqualTo(1);
                Check.That(src.IsDisposed).IsFalse();
                Check.That(src.ToString()).IsEqualTo("EventSource<String>(Subscriptions: 1)");

                Check.That(evt.Source).IsSameReferenceAs(src);
                Check.That(evt.HasSource).IsTrue();
                Check.That(evt.ToString()).IsEqualTo("Event<String> ⇔ EventSource<String>(Subscriptions: 1)");

                Check.That(sub).IsInstanceOf<FuncSubscription<String>>();
                Check.That(sub.ToString()).StartsWith("⇒ async Amarok.Events.Test_Event");

                sub.Dispose();

                Check.That(src.NumberOfSubscriptions).IsEqualTo(0);
                Check.That(src.IsDisposed).IsFalse();
                Check.That(src.ToString()).IsEqualTo("EventSource<String>(Subscriptions: 0)");
            }

            [Test]
            public void Subscribe_On_DisposedEventSource()
            {
                var src = new EventSource<String>();
                src.Dispose();

                Event<String> evt = src.Event;
                var           sub = evt.Subscribe(async x => await Task.Yield());

                Check.That(src.NumberOfSubscriptions).IsEqualTo(0);
                Check.That(src.IsDisposed).IsTrue();
                Check.That(src.ToString()).IsEqualTo("EventSource<String>(Subscriptions: 0)");

                Check.That(evt.Source).IsSameReferenceAs(src);
                Check.That(evt.HasSource).IsTrue();
                Check.That(evt.ToString()).IsEqualTo("Event<String> ⇔ EventSource<String>(Subscriptions: 0)");

                Check.That(sub).IsInstanceOf<NullSubscription>();
                Check.That(sub.ToString()).StartsWith("⇒ <null>");

                sub.Dispose();

                Check.That(src.NumberOfSubscriptions).IsEqualTo(0);
                Check.That(src.IsDisposed).IsTrue();
                Check.That(src.ToString()).IsEqualTo("EventSource<String>(Subscriptions: 0)");
            }

            [Test]
            public void Exception_For_NullCallback()
            {
                using var src = new EventSource<String>();

                Check.ThatCode(() => src.Event.Subscribe((Func<String, Task>) null))
                     .Throws<ArgumentNullException>()
                     .WithProperty(x => x.ParamName, "func");
            }
        }

        [TestFixture]
        public class SubscribeWeak_Func
        {
            [Test]
            public void Subscribe_On_NullEvent()
            {
                var evt = new Event<String>();
                var sub = evt.SubscribeWeak(async x => await Task.Yield());

                Check.That(evt.Source).IsNull();
                Check.That(evt.HasSource).IsFalse();
                Check.That(evt.ToString()).IsEqualTo("Event<String> ⇔ <null>");

                Check.That(sub).IsInstanceOf<NullSubscription>();
                Check.That(sub.ToString()).IsEqualTo("⇒ <null>");
                Check.ThatCode(() => sub.Dispose()).DoesNotThrow();
            }

            [Test]
            public void Subscribe_On_Event()
            {
                var           src = new EventSource<String>();
                Event<String> evt = src.Event;
                var           sub = evt.SubscribeWeak(async x => await Task.Yield());

                Check.That(src.NumberOfSubscriptions).IsEqualTo(1);
                Check.That(src.IsDisposed).IsFalse();
                Check.That(src.ToString()).IsEqualTo("EventSource<String>(Subscriptions: 1)");

                Check.That(evt.Source).IsSameReferenceAs(src);
                Check.That(evt.HasSource).IsTrue();
                Check.That(evt.ToString()).IsEqualTo("Event<String> ⇔ EventSource<String>(Subscriptions: 1)");

                Check.That(sub).IsInstanceOf<FuncSubscription<String>>();
                Check.That(sub.ToString()).StartsWith("⇒ async Amarok.Events.Test_Event");

                sub.Dispose();

                Check.That(src.NumberOfSubscriptions).IsEqualTo(0);
                Check.That(src.IsDisposed).IsFalse();
                Check.That(src.ToString()).IsEqualTo("EventSource<String>(Subscriptions: 0)");
            }

            [Test]
            public void Subscribe_On_DisposedEventSource()
            {
                var src = new EventSource<String>();
                src.Dispose();

                Event<String> evt = src.Event;
                var           sub = evt.SubscribeWeak(async x => await Task.Yield());

                Check.That(src.NumberOfSubscriptions).IsEqualTo(0);
                Check.That(src.IsDisposed).IsTrue();
                Check.That(src.ToString()).IsEqualTo("EventSource<String>(Subscriptions: 0)");

                Check.That(evt.Source).IsSameReferenceAs(src);
                Check.That(evt.HasSource).IsTrue();
                Check.That(evt.ToString()).IsEqualTo("Event<String> ⇔ EventSource<String>(Subscriptions: 0)");

                Check.That(sub).IsInstanceOf<NullSubscription>();
                Check.That(sub.ToString()).StartsWith("⇒ <null>");

                sub.Dispose();

                Check.That(src.NumberOfSubscriptions).IsEqualTo(0);
                Check.That(src.IsDisposed).IsTrue();
                Check.That(src.ToString()).IsEqualTo("EventSource<String>(Subscriptions: 0)");
            }

            [Test]
            public void Exception_For_NullCallback()
            {
                using var src = new EventSource<String>();

                Check.ThatCode(() => src.Event.SubscribeWeak((Func<String, Task>) null))
                     .Throws<ArgumentNullException>()
                     .WithProperty(x => x.ParamName, "func");
            }
        }

        [TestFixture]
        public class TestGetHashCode
        {
            [Test]
            public void GetHashCode_On_NullEvent()
            {
                var evt = new Event<String>();

                Check.That(evt.GetHashCode()).IsEqualTo(0);
            }

            [Test]
            public void GetHashCode_On_Event()
            {
                using var src1 = new EventSource<String>();
                using var src2 = new EventSource<String>();

                Check.That(src1.Event.GetHashCode()).Not.IsEqualTo(0);
                Check.That(src2.Event.GetHashCode()).Not.IsEqualTo(0);
                Check.That(src1.GetHashCode()).Not.IsEqualTo(src2.GetHashCode());
            }
        }

        [TestFixture]
        public class TestEquals
        {
            [Test]
            public void Equals()
            {
                var           src1 = new EventSource<String>();
                Event<String> evt1 = src1.Event;

                var           src2 = new EventSource<String>();
                Event<String> evt2 = src2.Event;

                Check.That(evt1.Equals(null)).IsFalse();
                Check.That(evt1.Equals(new Version())).IsFalse();
                Check.That(evt1.Equals(new Event<String>())).IsFalse();
                Check.That(evt1.Equals(evt1)).IsTrue();
                Check.That(evt1.Equals(evt2)).IsFalse();
                Check.That(evt1.Equals(new Event<String>(src1))).IsTrue();
                Check.That(evt1.Equals(new Event<String>(src2))).IsFalse();

                Check.That(evt1 == new Event<String>()).IsFalse();
                Check.That(evt1 == evt1).IsTrue();
                Check.That(evt1 == evt2).IsFalse();
                Check.That(evt1 == new Event<String>(src1)).IsTrue();
                Check.That(evt1 == new Event<String>(src2)).IsFalse();

                Check.That(evt1 != new Event<String>()).Not.IsFalse();
                Check.That(evt1 != evt1).Not.IsTrue();
                Check.That(evt1 != evt2).Not.IsFalse();
                Check.That(evt1 != new Event<String>(src1)).Not.IsTrue();
                Check.That(evt1 != new Event<String>(src2)).Not.IsFalse();
            }
        }
    }
}
