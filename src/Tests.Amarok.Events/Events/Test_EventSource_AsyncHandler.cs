// Copyright (c) 2024, Olaf Kober <olaf.kober@outlook.com>

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

using System;
using System.Threading;
using System.Threading.Tasks;
using NFluent;
using NUnit.Framework;


namespace Amarok.Events;


public class Test_EventSource_AsyncHandler
{
    public interface IFooService
    {
        Event<String> Changed { get; }
    }

    public sealed class FooService : IFooService
    {
        public readonly EventSource<String> ChangedSource = new();

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

            var subscription = service.Changed.Subscribe(
                async x => {
                    arg = x;
                    called++;
                }
            );

            Check.That(subscription).IsInstanceOf<FuncSubscription<String>>();

            Check.That(((FuncSubscription<String>)subscription).TestingGetPreviousSubscription()).IsNull();

            Check.That(((FuncSubscription<String>)subscription).Target).IsNotNull();

            Check.That(((FuncSubscription<String>)subscription).Method).IsNotNull();

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

            var subscription = service.Changed.Subscribe(
                async x => {
                    await Task.Delay(100);
                    arg = x;
                    called++;
                }
            );

            Check.That(subscription).IsInstanceOf<FuncSubscription<String>>();

            Check.That(((FuncSubscription<String>)subscription).TestingGetPreviousSubscription()).IsNull();

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
        public void Invoke_With_AsyncCompletingHandler_NoArg()
        {
            var service = new FooService();

            var called = 0;

            var subscription = service.Changed.Subscribe(
                async () => {
                    await Task.Delay(100);
                    called++;
                }
            );

            Check.That(subscription).IsInstanceOf<FuncSubscription<String>>();

            Check.That(((FuncSubscription<String>)subscription).TestingGetPreviousSubscription()).IsNull();

            var flag1 = service.Do("abc");

            Check.That(flag1).IsTrue();

            Check.That(called).IsEqualTo(0);

            SpinWait.SpinUntil(() => called == 1, 2000);

            Check.That(called).IsEqualTo(1);

            Check.That(service.ChangedSource.NumberOfSubscriptions).IsEqualTo(1);

            Check.That(service.ChangedSource.IsDisposed).IsFalse();
        }

        [Test]
        public void Invoke_With_AlreadyCompletedHandler()
        {
            var service = new FooService();

            var    called = 0;
            String arg    = null;

            var subscription = service.Changed.Subscribe(
                x => {
                    arg = x;
                    called++;

                    return Task.CompletedTask;
                }
            );

            Check.That(subscription).IsInstanceOf<FuncSubscription<String>>();

            Check.That(((FuncSubscription<String>)subscription).TestingGetPreviousSubscription()).IsNull();

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

            var subscription = service.Changed.Subscribe(
                x => {
                    arg = x;
                    called++;
                    var cts = new CancellationTokenSource();
                    cts.Cancel();

                    return Task.FromCanceled(cts.Token);
                }
            );

            Check.That(subscription).IsInstanceOf<FuncSubscription<String>>();

            Check.That(((FuncSubscription<String>)subscription).TestingGetPreviousSubscription()).IsNull();

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

            var subscription = service.Changed.Subscribe(
                async x => {
                    arg = x;
                    called++;

                    if (called == 1)
                        throw new ApplicationException();

                    await Task.Yield();
                }
            );

            Check.That(subscription).IsInstanceOf<FuncSubscription<String>>();

            Check.That(((FuncSubscription<String>)subscription).TestingGetPreviousSubscription()).IsNull();

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

            var subscription = service.Changed.Subscribe(
                async x => {
                    arg = x;
                    called++;
                    await Task.Delay(10);

                    throw new ApplicationException();
                }
            );

            Check.That(subscription).IsInstanceOf<FuncSubscription<String>>();

            Check.That(((FuncSubscription<String>)subscription).TestingGetPreviousSubscription()).IsNull();

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

            var subscription = service.Changed.Subscribe(
                async x => {
                    arg = x;
                    called++;
                }
            );

            Check.That(subscription).IsInstanceOf<FuncSubscription<String>>();

            Check.That(((FuncSubscription<String>)subscription).TestingGetPreviousSubscription()).IsNull();

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

            var subscription = service.Changed.Subscribe(
                async x => {
                    await Task.Delay(100);
                    arg = x;
                    called++;
                }
            );

            Check.That(subscription).IsInstanceOf<FuncSubscription<String>>();

            Check.That(((FuncSubscription<String>)subscription).TestingGetPreviousSubscription()).IsNull();

            var flag1 = await service.DoAsync("abc");

            Check.That(flag1).IsTrue();

            Check.That(called).IsEqualTo(1);

            Check.That(arg).IsEqualTo("abc");

            Check.That(service.ChangedSource.NumberOfSubscriptions).IsEqualTo(1);

            Check.That(service.ChangedSource.IsDisposed).IsFalse();
        }

        [Test]
        public async Task Invoke_With_AsyncCompletingHandler_NoArg()
        {
            var service = new FooService();

            var called = 0;

            var subscription = service.Changed.Subscribe(
                async () => {
                    await Task.Delay(100);
                    called++;
                }
            );

            Check.That(subscription).IsInstanceOf<FuncSubscription<String>>();

            Check.That(((FuncSubscription<String>)subscription).TestingGetPreviousSubscription()).IsNull();

            var flag1 = await service.DoAsync("abc");

            Check.That(flag1).IsTrue();

            Check.That(called).IsEqualTo(1);

            Check.That(service.ChangedSource.NumberOfSubscriptions).IsEqualTo(1);

            Check.That(service.ChangedSource.IsDisposed).IsFalse();
        }

        [Test]
        public async Task Invoke_With_AlreadyCompletedHandler()
        {
            var service = new FooService();

            var    called = 0;
            String arg    = null;

            var subscription = service.Changed.Subscribe(
                x => {
                    arg = x;
                    called++;

                    return Task.CompletedTask;
                }
            );

            Check.That(subscription).IsInstanceOf<FuncSubscription<String>>();

            Check.That(((FuncSubscription<String>)subscription).TestingGetPreviousSubscription()).IsNull();

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

            var subscription = service.Changed.Subscribe(
                x => {
                    arg = x;
                    called++;
                    var cts = new CancellationTokenSource();
                    cts.Cancel();

                    return Task.FromCanceled(cts.Token);
                }
            );

            Check.That(subscription).IsInstanceOf<FuncSubscription<String>>();

            Check.That(((FuncSubscription<String>)subscription).TestingGetPreviousSubscription()).IsNull();

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

            var subscription = service.Changed.Subscribe(
                async x => {
                    arg = x;
                    called++;

                    if (called == 1)
                        throw new ApplicationException();

                    await Task.Yield();
                }
            );

            Check.That(subscription).IsInstanceOf<FuncSubscription<String>>();

            Check.That(((FuncSubscription<String>)subscription).TestingGetPreviousSubscription()).IsNull();

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

            var subscription = service.Changed.Subscribe(
                async x => {
                    arg = x;
                    called++;
                    await Task.Delay(10);

                    throw new ApplicationException();
                }
            );

            Check.That(subscription).IsInstanceOf<FuncSubscription<String>>();

            Check.That(((FuncSubscription<String>)subscription).TestingGetPreviousSubscription()).IsNull();

            Exception exception = null;

            using (EventSystem.UnobservedException.SubscribeWeak(x => Volatile.Write(ref exception, x)))
            {
                var flag1 = await service.DoAsync("abc");

                Check.That(flag1).IsTrue();

                Check.That(called).IsEqualTo(1);

                Check.That(arg).IsEqualTo("abc");

                SpinWait.SpinUntil(() => Volatile.Read(ref exception) != null, 2000);

                Check.That(Volatile.Read(ref exception)).IsInstanceOf<ApplicationException>();

                Check.That(service.ChangedSource.NumberOfSubscriptions).IsEqualTo(1);

                Check.That(service.ChangedSource.IsDisposed).IsFalse();
            }
        }
    }
}
