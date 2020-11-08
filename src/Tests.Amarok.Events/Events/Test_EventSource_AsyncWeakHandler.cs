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

#pragma warning disable S3257  // Declarations and initializations should be as concise as possible
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

using System;
using System.Threading;
using System.Threading.Tasks;
using NFluent;
using NUnit.Framework;


namespace Amarok.Events
{
    public class Test_EventSource_AsyncWeakHandler
    {
        public interface IFooService
        {
            Event<String> Changed { get; }
        }

        public sealed class FooService : IFooService
        {
            public readonly EventSource<String> ChangedSource = new EventSource<String>();

            public Event<String> Changed => ChangedSource.Event;

            public Boolean Do(String value)
            {
                return ChangedSource.Invoke(value);
            }

            public ValueTask<Boolean> DoAsync(String value)
            {
                return ChangedSource.InvokeAsync(value);
            }
        }


        // --- TESTS ---


        [TestFixture]
        public class Invoke
        {
            [Test]
            public void Invoke_With_SyncCompletingHandler()
            {
                var service = new FooService();

                var    called = 0;
                String arg    = null;

                var subscription = service.Changed.SubscribeWeak(
                    async x => {
                        arg = x;
                        called++;
                    }
                );

                Check.That(subscription).IsInstanceOf<FuncSubscription<String>>();

                Check.That(( (FuncSubscription<String>) subscription ).TestingGetPreviousSubscription())
                     .IsInstanceOf<WeakSubscription<String>>();

                var flag1 = service.Do("abc");

                Check.That(flag1).IsTrue();
                Check.That(called).IsEqualTo(1);
                Check.That(arg).IsEqualTo("abc");

                called = 0;
                var flag2 = service.Do("def");

                Check.That(flag2).IsTrue();
                Check.That(called).IsEqualTo(1);
                Check.That(arg).IsEqualTo("def");

                Check.That(service.ChangedSource.NumberOfSubscriptions).IsEqualTo(1);
                Check.That(service.ChangedSource.IsDisposed).IsFalse();
            }

            [Test]
            public void Invoke_With_AsyncCompletingHandler()
            {
                var service = new FooService();

                var    called = 0;
                String arg    = null;

                var subscription = service.Changed.SubscribeWeak(
                    async x => {
                        await Task.Delay(100);
                        arg = x;
                        called++;
                    }
                );

                Check.That(subscription).IsInstanceOf<FuncSubscription<String>>();

                Check.That(( (FuncSubscription<String>) subscription ).TestingGetPreviousSubscription())
                     .IsInstanceOf<WeakSubscription<String>>();

                var flag1 = service.Do("abc");

                Check.That(flag1).IsTrue();
                Check.That(called).IsEqualTo(0);

                SpinWait.SpinUntil(() => called == 1, 2000);

                Check.That(called).IsEqualTo(1);
                Check.That(arg).IsEqualTo("abc");

                Check.That(service.ChangedSource.NumberOfSubscriptions).IsEqualTo(1);
                Check.That(service.ChangedSource.IsDisposed).IsFalse();
            }

            [Test]
            public void Invoke_With_AlreadyCompletedHandler()
            {
                var service = new FooService();

                var    called = 0;
                String arg    = null;

                var subscription = service.Changed.SubscribeWeak(
                    x => {
                        arg = x;
                        called++;

                        return Task.CompletedTask;
                    }
                );

                Check.That(subscription).IsInstanceOf<FuncSubscription<String>>();

                Check.That(( (FuncSubscription<String>) subscription ).TestingGetPreviousSubscription())
                     .IsInstanceOf<WeakSubscription<String>>();

                var flag1 = service.Do("abc");

                Check.That(flag1).IsTrue();
                Check.That(called).IsEqualTo(1);
                Check.That(arg).IsEqualTo("abc");

                Check.That(service.ChangedSource.NumberOfSubscriptions).IsEqualTo(1);
                Check.That(service.ChangedSource.IsDisposed).IsFalse();
            }

            [Test]
            public void Invoke_With_AlreadyCanceledHandler()
            {
                var service = new FooService();

                var    called = 0;
                String arg    = null;

                var subscription = service.Changed.SubscribeWeak(
                    x => {
                        arg = x;
                        called++;
                        var cts = new CancellationTokenSource();
                        cts.Cancel();

                        return Task.FromCanceled(cts.Token);
                    }
                );

                Check.That(subscription).IsInstanceOf<FuncSubscription<String>>();

                Check.That(( (FuncSubscription<String>) subscription ).TestingGetPreviousSubscription())
                     .IsInstanceOf<WeakSubscription<String>>();

                Exception exception = null;

                using (EventSystem.UnobservedException.SubscribeWeak(x => Volatile.Write(ref exception, x)))
                {
                    var flag1 = service.Do("abc");

                    Check.That(flag1).IsTrue();
                    Check.That(called).IsEqualTo(1);
                    Check.That(arg).IsEqualTo("abc");
                    Check.That(Volatile.Read(ref exception)).IsNull();

                    Check.That(service.ChangedSource.NumberOfSubscriptions).IsEqualTo(1);
                    Check.That(service.ChangedSource.IsDisposed).IsFalse();
                }
            }

            [Test]
            public void Invoke_With_HandlerThrowingException_BeforeAwait()
            {
                var service = new FooService();

                var    called = 0;
                String arg    = null;

                var subscription = service.Changed.SubscribeWeak(
                    async x => {
                        arg = x;
                        called++;

                        if (called == 1)
                            throw new ApplicationException();

                        await Task.Yield();
                    }
                );

                Check.That(subscription).IsInstanceOf<FuncSubscription<String>>();

                Check.That(( (FuncSubscription<String>) subscription ).TestingGetPreviousSubscription())
                     .IsInstanceOf<WeakSubscription<String>>();

                Exception exception = null;

                using (EventSystem.UnobservedException.SubscribeWeak(x => Volatile.Write(ref exception, x)))
                {
                    var flag1 = service.Do("abc");

                    Check.That(flag1).IsTrue();
                    Check.That(called).IsEqualTo(1);
                    Check.That(arg).IsEqualTo("abc");
                    Check.That(Volatile.Read(ref exception)).IsInstanceOf<ApplicationException>();

                    Check.That(service.ChangedSource.NumberOfSubscriptions).IsEqualTo(1);
                    Check.That(service.ChangedSource.IsDisposed).IsFalse();
                }
            }

            [Test]
            public void Invoke_With_HandlerThrowingException_AfterAwait()
            {
                var service = new FooService();

                var    called = 0;
                String arg    = null;

                var subscription = service.Changed.SubscribeWeak(
                    async x => {
                        arg = x;
                        called++;
                        await Task.Delay(10);

                        throw new ApplicationException();
                    }
                );

                Check.That(subscription).IsInstanceOf<FuncSubscription<String>>();

                Check.That(( (FuncSubscription<String>) subscription ).TestingGetPreviousSubscription())
                     .IsInstanceOf<WeakSubscription<String>>();

                Exception exception = null;

                using (EventSystem.UnobservedException.SubscribeWeak(x => Volatile.Write(ref exception, x)))
                {
                    var flag1 = service.Do("abc");

                    Check.That(flag1).IsTrue();
                    Check.That(called).IsEqualTo(1);
                    Check.That(arg).IsEqualTo("abc");

                    SpinWait.SpinUntil(() => Volatile.Read(ref exception) != null, 2000);

                    Check.That(Volatile.Read(ref exception)).IsInstanceOf<ApplicationException>();

                    Check.That(service.ChangedSource.NumberOfSubscriptions).IsEqualTo(1);
                    Check.That(service.ChangedSource.IsDisposed).IsFalse();
                }
            }

            [Test]
            public void Invoke_After_SubscriptionGCed()
            {
                var service = new FooService();

                var    called = 0;
                String arg    = null;

                var subscription = service.Changed.SubscribeWeak(
                    async x => {
                        arg = x;
                        called++;
                        await Task.Yield();
                    }
                );

                Check.That(subscription).IsNotNull();
                Check.That(subscription).IsInstanceOf<FuncSubscription<String>>();

                Check.That(( (FuncSubscription<String>) subscription ).TestingGetPreviousSubscription())
                     .IsInstanceOf<WeakSubscription<String>>();

                var strongSub = (FuncSubscription<String>) subscription;
                var weakSub   = (WeakSubscription<String>) strongSub.TestingGetPreviousSubscription();
                strongSub.TestingClearNextSubscription();
                weakSub.TestingClearNextSubscription();

                var flag1 = service.Do("abc");

                Check.That(flag1).IsTrue();
                Check.That(called).IsEqualTo(0);
                Check.That(service.ChangedSource.NumberOfSubscriptions).IsEqualTo(0);
                Check.That(service.ChangedSource.IsDisposed).IsFalse();

                var flag2 = service.Do("abc");

                Check.That(flag2).IsFalse();
                Check.That(called).IsEqualTo(0);
                Check.That(service.ChangedSource.NumberOfSubscriptions).IsEqualTo(0);

                Check.That(subscription).IsNotNull();
                Check.That(subscription).IsInstanceOf<FuncSubscription<String>>();
                Check.That(( (FuncSubscription<String>) subscription ).TestingGetPreviousSubscription()).IsNull();
            }
        }

        [TestFixture]
        public class InvokeAsync
        {
            [Test]
            public async Task InvokeAsync_With_SyncCompletingHandler()
            {
                var service = new FooService();

                var    called = 0;
                String arg    = null;

                var subscription = service.Changed.SubscribeWeak(
                    async x => {
                        arg = x;
                        called++;
                    }
                );

                Check.That(subscription).IsInstanceOf<FuncSubscription<String>>();

                Check.That(( (FuncSubscription<String>) subscription ).TestingGetPreviousSubscription())
                     .IsInstanceOf<WeakSubscription<String>>();

                var flag1 = await service.DoAsync("abc");

                Check.That(flag1).IsTrue();
                Check.That(called).IsEqualTo(1);
                Check.That(arg).IsEqualTo("abc");

                called = 0;
                var flag2 = await service.DoAsync("def");

                Check.That(flag2).IsTrue();
                Check.That(called).IsEqualTo(1);
                Check.That(arg).IsEqualTo("def");

                Check.That(service.ChangedSource.NumberOfSubscriptions).IsEqualTo(1);
                Check.That(service.ChangedSource.IsDisposed).IsFalse();
            }

            [Test]
            public async Task Invoke_With_AsyncCompletingHandler()
            {
                var service = new FooService();

                var    called = 0;
                String arg    = null;

                var subscription = service.Changed.SubscribeWeak(
                    async x => {
                        await Task.Delay(100);
                        arg = x;
                        called++;
                    }
                );

                Check.That(subscription).IsInstanceOf<FuncSubscription<String>>();

                Check.That(( (FuncSubscription<String>) subscription ).TestingGetPreviousSubscription())
                     .IsInstanceOf<WeakSubscription<String>>();

                var flag1 = await service.DoAsync("abc");

                Check.That(flag1).IsTrue();
                Check.That(called).IsEqualTo(1);
                Check.That(arg).IsEqualTo("abc");

                Check.That(service.ChangedSource.NumberOfSubscriptions).IsEqualTo(1);
                Check.That(service.ChangedSource.IsDisposed).IsFalse();
            }

            [Test]
            public async Task Invoke_With_AlreadyCompletedHandler()
            {
                var service = new FooService();

                var    called = 0;
                String arg    = null;

                var subscription = service.Changed.SubscribeWeak(
                    x => {
                        arg = x;
                        called++;

                        return Task.CompletedTask;
                    }
                );

                Check.That(subscription).IsInstanceOf<FuncSubscription<String>>();

                Check.That(( (FuncSubscription<String>) subscription ).TestingGetPreviousSubscription())
                     .IsInstanceOf<WeakSubscription<String>>();

                var flag1 = await service.DoAsync("abc");

                Check.That(flag1).IsTrue();
                Check.That(called).IsEqualTo(1);
                Check.That(arg).IsEqualTo("abc");

                Check.That(service.ChangedSource.NumberOfSubscriptions).IsEqualTo(1);
                Check.That(service.ChangedSource.IsDisposed).IsFalse();
            }

            [Test]
            public async Task Invoke_With_AlreadyCanceledHandler()
            {
                var service = new FooService();

                var    called = 0;
                String arg    = null;

                var subscription = service.Changed.SubscribeWeak(
                    x => {
                        arg = x;
                        called++;
                        var cts = new CancellationTokenSource();
                        cts.Cancel();

                        return Task.FromCanceled(cts.Token);
                    }
                );

                Check.That(subscription).IsInstanceOf<FuncSubscription<String>>();

                Check.That(( (FuncSubscription<String>) subscription ).TestingGetPreviousSubscription())
                     .IsInstanceOf<WeakSubscription<String>>();

                Exception exception = null;

                using (EventSystem.UnobservedException.SubscribeWeak(x => Volatile.Write(ref exception, x)))
                {
                    var flag1 = await service.DoAsync("abc");

                    Check.That(flag1).IsTrue();
                    Check.That(called).IsEqualTo(1);
                    Check.That(arg).IsEqualTo("abc");
                    Check.That(Volatile.Read(ref exception)).IsNull();

                    Check.That(service.ChangedSource.NumberOfSubscriptions).IsEqualTo(1);
                    Check.That(service.ChangedSource.IsDisposed).IsFalse();
                }
            }

            [Test]
            public async Task Invoke_With_HandlerThrowingException_BeforeAwait()
            {
                var service = new FooService();

                var    called = 0;
                String arg    = null;

                var subscription = service.Changed.SubscribeWeak(
                    async x => {
                        arg = x;
                        called++;

                        if (called == 1)
                            throw new ApplicationException();

                        await Task.Yield();
                    }
                );

                Check.That(subscription).IsInstanceOf<FuncSubscription<String>>();

                Check.That(( (FuncSubscription<String>) subscription ).TestingGetPreviousSubscription())
                     .IsInstanceOf<WeakSubscription<String>>();

                Exception exception = null;

                using (EventSystem.UnobservedException.SubscribeWeak(x => Volatile.Write(ref exception, x)))
                {
                    var flag1 = await service.DoAsync("abc");

                    Check.That(flag1).IsTrue();
                    Check.That(called).IsEqualTo(1);
                    Check.That(arg).IsEqualTo("abc");
                    Check.That(Volatile.Read(ref exception)).IsInstanceOf<ApplicationException>();

                    Check.That(service.ChangedSource.NumberOfSubscriptions).IsEqualTo(1);
                    Check.That(service.ChangedSource.IsDisposed).IsFalse();
                }
            }

            [Test]
            public async Task Invoke_With_HandlerThrowingException_AfterAwait()
            {
                var service = new FooService();

                var    called = 0;
                String arg    = null;

                var subscription = service.Changed.SubscribeWeak(
                    async x => {
                        arg = x;
                        called++;
                        await Task.Delay(10);

                        throw new ApplicationException();
                    }
                );

                Check.That(subscription).IsInstanceOf<FuncSubscription<String>>();

                Check.That(( (FuncSubscription<String>) subscription ).TestingGetPreviousSubscription())
                     .IsInstanceOf<WeakSubscription<String>>();

                Exception exception = null;

                using (EventSystem.UnobservedException.SubscribeWeak(x => Volatile.Write(ref exception, x)))
                {
                    var flag1 = await service.DoAsync("abc");

                    Check.That(flag1).IsTrue();
                    Check.That(called).IsEqualTo(1);
                    Check.That(arg).IsEqualTo("abc");

                    SpinWait.SpinUntil(() => Volatile.Read(ref exception) != null, 5000);

                    Check.That(Volatile.Read(ref exception)).IsInstanceOf<ApplicationException>();

                    Check.That(service.ChangedSource.NumberOfSubscriptions).IsEqualTo(1);
                    Check.That(service.ChangedSource.IsDisposed).IsFalse();
                }
            }

            [Test]
            public async Task InvokeAsync_After_SubscriptionGCed()
            {
                var service = new FooService();

                var    called = 0;
                String arg    = null;

                var subscription = service.Changed.SubscribeWeak(
                    async x => {
                        arg = x;
                        called++;
                        await Task.Yield();
                    }
                );

                Check.That(subscription).IsNotNull();
                Check.That(subscription).IsInstanceOf<FuncSubscription<String>>();

                Check.That(( (FuncSubscription<String>) subscription ).TestingGetPreviousSubscription())
                     .IsInstanceOf<WeakSubscription<String>>();

                var strongSub = (FuncSubscription<String>) subscription;
                var weakSub   = (WeakSubscription<String>) strongSub.TestingGetPreviousSubscription();
                strongSub.TestingClearNextSubscription();
                weakSub.TestingClearNextSubscription();

                var flag1 = await service.DoAsync("abc");

                Check.That(flag1).IsTrue();
                Check.That(called).IsEqualTo(0);
                Check.That(service.ChangedSource.NumberOfSubscriptions).IsEqualTo(0);
                Check.That(service.ChangedSource.IsDisposed).IsFalse();

                var flag2 = await service.DoAsync("abc");

                Check.That(flag2).IsFalse();
                Check.That(called).IsEqualTo(0);
                Check.That(service.ChangedSource.NumberOfSubscriptions).IsEqualTo(0);

                Check.That(subscription).IsNotNull();
                Check.That(subscription).IsInstanceOf<FuncSubscription<String>>();
                Check.That(( (FuncSubscription<String>) subscription ).TestingGetPreviousSubscription()).IsNull();
            }
        }
    }
}
