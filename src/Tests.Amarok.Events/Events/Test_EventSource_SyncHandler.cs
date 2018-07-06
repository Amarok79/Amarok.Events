/* Copyright(c) 2018, Olaf Kober
 * Licensed under GNU Lesser General Public License v3.0 (LPGL-3.0).
 * https://github.com/Amarok79/Events
 */

using System;
using System.Threading.Tasks;
using NFluent;
using NUnit.Framework;


namespace Amarok.Events
{
	public class Test_EventSource_SyncHandler
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

			public Boolean Do(Func<String> func)
			{
				return ChangedSource.Invoke(func);
			}

			public Boolean Do(Func<Int32, String> func, Int32 arg)
			{
				return ChangedSource.Invoke(func, arg);
			}

			public Boolean Do(Func<Int32, Double, String> func, Int32 arg1, Double arg2)
			{
				return ChangedSource.Invoke(func, arg1, arg2);
			}

			public Boolean Do(Func<Int32, Double, Char, String> func, Int32 arg1, Double arg2, Char arg3)
			{
				return ChangedSource.Invoke(func, arg1, arg2, arg3);
			}

			public ValueTask<Boolean> DoAsync(String value)
			{
				return ChangedSource.InvokeAsync(value);
			}

			public ValueTask<Boolean> DoAsync(Func<String> func)
			{
				return ChangedSource.InvokeAsync(func);
			}

			public ValueTask<Boolean> DoAsync(Func<Int32, String> func, Int32 arg)
			{
				return ChangedSource.InvokeAsync(func, arg);
			}

			public ValueTask<Boolean> DoAsync(Func<Int32, Double, String> func, Int32 arg1, Double arg2)
			{
				return ChangedSource.InvokeAsync(func, arg1, arg2);
			}

			public ValueTask<Boolean> DoAsync(Func<Int32, Double, Char, String> func, Int32 arg1, Double arg2, Char arg3)
			{
				return ChangedSource.InvokeAsync(func, arg1, arg2, arg3);
			}
		}


		// --- TESTS ---


		[TestFixture]
		public class Invoke
		{
			[Test]
			public void Invoke_Without_Handler()
			{
				var service = new FooService();

				var flag = service.Do("abc");

				Check.That(flag)
					.IsFalse();

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(0);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public void Invoke_With_SingleHandler()
			{
				var service = new FooService();

				Int32 called = 0;
				String arg = null;

				var subscription = service.Changed.Subscribe(x => {
					arg = x;
					called++;
				});

				Check.That(subscription)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription).TestingGetPreviousSubscription())
					.IsNull();

				var flag1 = service.Do("abc");

				Check.That(flag1)
					.IsTrue();
				Check.That(called)
					.IsEqualTo(1);
				Check.That(arg)
					.IsEqualTo("abc");

				called = 0;
				var flag2 = service.Do("def");

				Check.That(flag2)
					.IsTrue();
				Check.That(called)
					.IsEqualTo(1);
				Check.That(arg)
					.IsEqualTo("def");

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(1);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public void Invoke_With_MultipleHandler()
			{
				var service = new FooService();

				Int32 called1 = 0;
				String arg1 = null;

				var subscription1 = service.Changed.Subscribe(x => {
					arg1 = x;
					called1++;
				});

				Int32 called2 = 0;
				String arg2 = null;

				var subscription2 = service.Changed.Subscribe(x => {
					arg2 = x;
					called2++;
				});

				Check.That(subscription1)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription1).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription2).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.Not.IsSameReferenceAs(subscription1);

				var flag1 = service.Do("abc");

				Check.That(flag1)
					.IsTrue();
				Check.That(called1)
					.IsEqualTo(1);
				Check.That(arg1)
					.IsEqualTo("abc");
				Check.That(called2)
					.IsEqualTo(1);
				Check.That(arg2)
					.IsEqualTo("abc");

				called1 = 0;
				called2 = 0;
				var flag2 = service.Do("def");

				Check.That(flag2)
					.IsTrue();
				Check.That(called1)
					.IsEqualTo(1);
				Check.That(arg1)
					.IsEqualTo("def");
				Check.That(called2)
					.IsEqualTo(1);
				Check.That(arg2)
					.IsEqualTo("def");

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(2);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public void Invoke_With_MultipleHandler_OneThrowingExceptionButAllInvoked()
			{
				var service = new FooService();

				Int32 called1 = 0;
				String arg1 = null;

				var subscription1 = service.Changed.Subscribe(new Action<String>(x => {
					arg1 = x;
					called1++;
					throw new ApplicationException("1");
				}));

				Int32 called2 = 0;
				String arg2 = null;

				var subscription2 = service.Changed.Subscribe(x => {
					arg2 = x;
					called2++;
				});

				Check.That(subscription1)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription1).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription2).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.Not.IsSameReferenceAs(subscription1);

				Exception exception = null;
				EventSystem.UnobservedException.SubscribeWeak(x => exception = x);

				var flag1 = service.Do("abc");

				Check.That(flag1)
					.IsTrue();
				Check.That(called1)
					.IsEqualTo(1);
				Check.That(arg1)
					.IsEqualTo("abc");
				Check.That(called2)
					.IsEqualTo(1);
				Check.That(arg2)
					.IsEqualTo("abc");

				Check.That(exception)
					.IsInstanceOf<ApplicationException>();
				Check.That(exception.Message)
					.IsEqualTo("1");

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(2);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public void Invoke_After_SubscriptionDisposed()
			{
				var service = new FooService();

				Int32 called = 0;
				String arg = null;

				var subscription = service.Changed.Subscribe(x => {
					arg = x;
					called++;
				});

				Check.That(subscription)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription).TestingGetPreviousSubscription())
					.IsNull();

				subscription.Dispose();

				var flag1 = service.Do("abc");

				Check.That(flag1)
					.IsFalse();
				Check.That(called)
					.IsEqualTo(0);

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(0);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public void Invoke_After_EventSourceDisposed()
			{
				var service = new FooService();

				Int32 called = 0;
				String arg = null;

				var subscription = service.Changed.Subscribe(x => {
					arg = x;
					called++;
				});

				Check.That(subscription)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription).TestingGetPreviousSubscription())
					.IsNull();

				service.ChangedSource.Dispose();

				var flag1 = service.Do("abc");

				Check.That(flag1)
					.IsFalse();
				Check.That(called)
					.IsEqualTo(0);

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(0);
				Check.That(service.ChangedSource.IsDisposed)
					.IsTrue();
			}
		}

		[TestFixture]
		public class Invoke_ValueFactory
		{
			[Test]
			public void Invoke_Without_Handler()
			{
				var service = new FooService();

				var flag = service.Do(() => { Assert.Fail("MUST NOT be called"); return "abc"; });

				Check.That(flag)
					.IsFalse();

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(0);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public void Invoke_With_SingleHandler()
			{
				var service = new FooService();

				Int32 called = 0;
				String arg = null;

				var subscription = service.Changed.Subscribe(x => {
					arg = x;
					called++;
				});

				Check.That(subscription)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription).TestingGetPreviousSubscription())
					.IsNull();

				Int32 factoryCalled = 0;
				var flag1 = service.Do(() => { factoryCalled++; return "abc"; });

				Check.That(flag1)
					.IsTrue();
				Check.That(called)
					.IsEqualTo(1);
				Check.That(arg)
					.IsEqualTo("abc");
				Check.That(factoryCalled)
					.IsEqualTo(1);

				called = 0;
				factoryCalled = 0;
				var flag2 = service.Do(() => { factoryCalled++; return "def"; });

				Check.That(flag2)
					.IsTrue();
				Check.That(called)
					.IsEqualTo(1);
				Check.That(arg)
					.IsEqualTo("def");
				Check.That(factoryCalled)
					.IsEqualTo(1);

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(1);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public void Invoke_With_MultipleHandler()
			{
				var service = new FooService();

				Int32 called1 = 0;
				String arg1 = null;

				var subscription1 = service.Changed.Subscribe(x => {
					arg1 = x;
					called1++;
				});

				Int32 called2 = 0;
				String arg2 = null;

				var subscription2 = service.Changed.Subscribe(x => {
					arg2 = x;
					called2++;
				});

				Check.That(subscription1)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription1).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription2).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.Not.IsSameReferenceAs(subscription1);

				Int32 factoryCalled = 0;
				var flag1 = service.Do(() => { factoryCalled++; return "abc"; });

				Check.That(flag1)
					.IsTrue();
				Check.That(called1)
					.IsEqualTo(1);
				Check.That(arg1)
					.IsEqualTo("abc");
				Check.That(called2)
					.IsEqualTo(1);
				Check.That(arg2)
					.IsEqualTo("abc");
				Check.That(factoryCalled)
					.IsEqualTo(1);

				called1 = 0;
				called2 = 0;
				factoryCalled = 0;
				var flag2 = service.Do(() => { factoryCalled++; return "def"; });

				Check.That(flag2)
					.IsTrue();
				Check.That(called1)
					.IsEqualTo(1);
				Check.That(arg1)
					.IsEqualTo("def");
				Check.That(called2)
					.IsEqualTo(1);
				Check.That(arg2)
					.IsEqualTo("def");
				Check.That(factoryCalled)
					.IsEqualTo(1);

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(2);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public void Invoke_With_MultipleHandler_OneThrowingExceptionButAllInvoked()
			{
				var service = new FooService();

				Int32 called1 = 0;
				String arg1 = null;

				var subscription1 = service.Changed.Subscribe(new Action<String>(x => {
					arg1 = x;
					called1++;
					throw new ApplicationException("1");
				}));

				Int32 called2 = 0;
				String arg2 = null;

				var subscription2 = service.Changed.Subscribe(x => {
					arg2 = x;
					called2++;
				});

				Check.That(subscription1)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription1).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription2).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.Not.IsSameReferenceAs(subscription1);

				Exception exception = null;
				EventSystem.UnobservedException.SubscribeWeak(x => exception = x);

				Int32 factoryCalled = 0;
				var flag1 = service.Do(() => { factoryCalled++; return "abc"; });

				Check.That(flag1)
					.IsTrue();
				Check.That(called1)
					.IsEqualTo(1);
				Check.That(arg1)
					.IsEqualTo("abc");
				Check.That(called2)
					.IsEqualTo(1);
				Check.That(arg2)
					.IsEqualTo("abc");
				Check.That(factoryCalled)
					.IsEqualTo(1);

				Check.That(exception)
					.IsInstanceOf<ApplicationException>();
				Check.That(exception.Message)
					.IsEqualTo("1");

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(2);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public void Invoke_After_SubscriptionDisposed()
			{
				var service = new FooService();

				Int32 called = 0;
				String arg = null;

				var subscription = service.Changed.Subscribe(x => {
					arg = x;
					called++;
				});

				Check.That(subscription)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription).TestingGetPreviousSubscription())
					.IsNull();

				subscription.Dispose();

				Int32 factoryCalled = 0;
				var flag1 = service.Do(() => { factoryCalled++; return "abc"; });

				Check.That(flag1)
					.IsFalse();
				Check.That(called)
					.IsEqualTo(0);
				Check.That(factoryCalled)
					.IsEqualTo(0);

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(0);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public void Invoke_After_EventSourceDisposed()
			{
				var service = new FooService();

				Int32 called = 0;
				String arg = null;

				var subscription = service.Changed.Subscribe(x => {
					arg = x;
					called++;
				});

				Check.That(subscription)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription).TestingGetPreviousSubscription())
					.IsNull();

				service.ChangedSource.Dispose();

				Int32 factoryCalled = 0;
				var flag1 = service.Do(() => { factoryCalled++; return "abc"; });

				Check.That(flag1)
					.IsFalse();
				Check.That(called)
					.IsEqualTo(0);
				Check.That(factoryCalled)
					.IsEqualTo(0);

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(0);
				Check.That(service.ChangedSource.IsDisposed)
					.IsTrue();
			}

			[Test]
			public void Exception_For_NullFactory()
			{
				var service = new FooService();

				Check.ThatCode(() => service.Do((Func<String>)null))
					.Throws<ArgumentNullException>()
					.WithProperty(x => x.ParamName, "valueFactory");
			}
		}

		[TestFixture]
		public class Invoke_ValueFactory_Arg1
		{
			[Test]
			public void Invoke_Without_Handler()
			{
				var service = new FooService();

				var flag = service.Do((a) => { Assert.Fail("MUST NOT be called"); return "abc"; }, 123);

				Check.That(flag)
					.IsFalse();

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(0);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public void Invoke_With_SingleHandler()
			{
				var service = new FooService();

				Int32 called = 0;
				String arg = null;

				var subscription = service.Changed.Subscribe(x => {
					arg = x;
					called++;
				});

				Check.That(subscription)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription).TestingGetPreviousSubscription())
					.IsNull();

				Int32 factoryCalled = 0;
				Int32 fa = 0;
				var flag1 = service.Do((a) => { fa = a; factoryCalled++; return "abc"; }, 123);

				Check.That(flag1)
					.IsTrue();
				Check.That(called)
					.IsEqualTo(1);
				Check.That(arg)
					.IsEqualTo("abc");
				Check.That(factoryCalled)
					.IsEqualTo(1);
				Check.That(fa)
					.IsEqualTo(123);

				called = 0;
				factoryCalled = 0;
				fa = 0;
				var flag2 = service.Do((a) => { fa = a; factoryCalled++; return "def"; }, 456);

				Check.That(flag2)
					.IsTrue();
				Check.That(called)
					.IsEqualTo(1);
				Check.That(arg)
					.IsEqualTo("def");
				Check.That(factoryCalled)
					.IsEqualTo(1);
				Check.That(fa)
					.IsEqualTo(456);

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(1);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public void Invoke_With_MultipleHandler()
			{
				var service = new FooService();

				Int32 called1 = 0;
				String arg1 = null;

				var subscription1 = service.Changed.Subscribe(x => {
					arg1 = x;
					called1++;
				});

				Int32 called2 = 0;
				String arg2 = null;

				var subscription2 = service.Changed.Subscribe(x => {
					arg2 = x;
					called2++;
				});

				Check.That(subscription1)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription1).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription2).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.Not.IsSameReferenceAs(subscription1);

				Int32 factoryCalled = 0;
				Int32 fa = 0;
				var flag1 = service.Do((a) => { fa = a; factoryCalled++; return "abc"; }, 123);

				Check.That(flag1)
					.IsTrue();
				Check.That(called1)
					.IsEqualTo(1);
				Check.That(arg1)
					.IsEqualTo("abc");
				Check.That(called2)
					.IsEqualTo(1);
				Check.That(arg2)
					.IsEqualTo("abc");
				Check.That(factoryCalled)
					.IsEqualTo(1);
				Check.That(fa)
					.IsEqualTo(123);

				called1 = 0;
				called2 = 0;
				factoryCalled = 0;
				fa = 0;
				var flag2 = service.Do((a) => { fa = a; factoryCalled++; return "def"; }, 456);

				Check.That(flag2)
					.IsTrue();
				Check.That(called1)
					.IsEqualTo(1);
				Check.That(arg1)
					.IsEqualTo("def");
				Check.That(called2)
					.IsEqualTo(1);
				Check.That(arg2)
					.IsEqualTo("def");
				Check.That(factoryCalled)
					.IsEqualTo(1);
				Check.That(fa)
					.IsEqualTo(456);

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(2);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public void Invoke_With_MultipleHandler_OneThrowingExceptionButAllInvoked()
			{
				var service = new FooService();

				Int32 called1 = 0;
				String arg1 = null;

				var subscription1 = service.Changed.Subscribe(new Action<String>(x => {
					arg1 = x;
					called1++;
					throw new ApplicationException("1");
				}));

				Int32 called2 = 0;
				String arg2 = null;

				var subscription2 = service.Changed.Subscribe(x => {
					arg2 = x;
					called2++;
				});

				Check.That(subscription1)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription1).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription2).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.Not.IsSameReferenceAs(subscription1);

				Exception exception = null;
				EventSystem.UnobservedException.SubscribeWeak(x => exception = x);

				Int32 factoryCalled = 0;
				Int32 fa = 0;
				var flag1 = service.Do((a) => { fa = a; factoryCalled++; return "abc"; }, 123);

				Check.That(flag1)
					.IsTrue();
				Check.That(called1)
					.IsEqualTo(1);
				Check.That(arg1)
					.IsEqualTo("abc");
				Check.That(called2)
					.IsEqualTo(1);
				Check.That(arg2)
					.IsEqualTo("abc");
				Check.That(factoryCalled)
					.IsEqualTo(1);
				Check.That(fa)
					.IsEqualTo(123);

				Check.That(exception)
					.IsInstanceOf<ApplicationException>();
				Check.That(exception.Message)
					.IsEqualTo("1");

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(2);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public void Invoke_After_SubscriptionDisposed()
			{
				var service = new FooService();

				Int32 called = 0;
				String arg = null;

				var subscription = service.Changed.Subscribe(x => {
					arg = x;
					called++;
				});

				Check.That(subscription)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription).TestingGetPreviousSubscription())
					.IsNull();

				subscription.Dispose();

				Int32 factoryCalled = 0;
				var flag1 = service.Do((a) => { factoryCalled++; return "abc"; }, 123);

				Check.That(flag1)
					.IsFalse();
				Check.That(called)
					.IsEqualTo(0);
				Check.That(factoryCalled)
					.IsEqualTo(0);

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(0);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public void Invoke_After_EventSourceDisposed()
			{
				var service = new FooService();

				Int32 called = 0;
				String arg = null;

				var subscription = service.Changed.Subscribe(x => {
					arg = x;
					called++;
				});

				Check.That(subscription)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription).TestingGetPreviousSubscription())
					.IsNull();

				service.ChangedSource.Dispose();

				Int32 factoryCalled = 0;
				var flag1 = service.Do((a) => { factoryCalled++; return "abc"; }, 123);

				Check.That(flag1)
					.IsFalse();
				Check.That(called)
					.IsEqualTo(0);
				Check.That(factoryCalled)
					.IsEqualTo(0);

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(0);
				Check.That(service.ChangedSource.IsDisposed)
					.IsTrue();
			}

			[Test]
			public void Exception_For_NullFactory()
			{
				var service = new FooService();

				Check.ThatCode(() => service.Do((Func<Int32, String>)null, 123))
					.Throws<ArgumentNullException>()
					.WithProperty(x => x.ParamName, "valueFactory");
			}
		}

		[TestFixture]
		public class Invoke_ValueFactory_Arg2
		{
			[Test]
			public void Invoke_Without_Handler()
			{
				var service = new FooService();

				var flag = service.Do((a, b) => { Assert.Fail("MUST NOT be called"); return "abc"; }, 123, 1.2);

				Check.That(flag)
					.IsFalse();

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(0);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public void Invoke_With_SingleHandler()
			{
				var service = new FooService();

				Int32 called = 0;
				String arg = null;

				var subscription = service.Changed.Subscribe(x => {
					arg = x;
					called++;
				});

				Check.That(subscription)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription).TestingGetPreviousSubscription())
					.IsNull();

				Int32 factoryCalled = 0;
				Int32 fa = 0;
				Double fb = 0.0;
				var flag1 = service.Do((a, b) => { fa = a; fb = b; factoryCalled++; return "abc"; }, 123, 1.2);

				Check.That(flag1)
					.IsTrue();
				Check.That(called)
					.IsEqualTo(1);
				Check.That(arg)
					.IsEqualTo("abc");
				Check.That(factoryCalled)
					.IsEqualTo(1);
				Check.That(fa)
					.IsEqualTo(123);
				Check.That(fb)
					.IsEqualTo(1.2);

				called = 0;
				factoryCalled = 0;
				fa = 0;
				fb = 0.0;
				var flag2 = service.Do((a, b) => { fa = a; fb = b; factoryCalled++; return "def"; }, 456, 3.4);

				Check.That(flag2)
					.IsTrue();
				Check.That(called)
					.IsEqualTo(1);
				Check.That(arg)
					.IsEqualTo("def");
				Check.That(factoryCalled)
					.IsEqualTo(1);
				Check.That(fa)
					.IsEqualTo(456);
				Check.That(fb)
					.IsEqualTo(3.4);

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(1);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public void Invoke_With_MultipleHandler()
			{
				var service = new FooService();

				Int32 called1 = 0;
				String arg1 = null;

				var subscription1 = service.Changed.Subscribe(x => {
					arg1 = x;
					called1++;
				});

				Int32 called2 = 0;
				String arg2 = null;

				var subscription2 = service.Changed.Subscribe(x => {
					arg2 = x;
					called2++;
				});

				Check.That(subscription1)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription1).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription2).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.Not.IsSameReferenceAs(subscription1);

				Int32 factoryCalled = 0;
				Int32 fa = 0;
				Double fb = 0.0;
				var flag1 = service.Do((a, b) => { fa = a; fb = b; factoryCalled++; return "abc"; }, 123, 1.2);

				Check.That(flag1)
					.IsTrue();
				Check.That(called1)
					.IsEqualTo(1);
				Check.That(arg1)
					.IsEqualTo("abc");
				Check.That(called2)
					.IsEqualTo(1);
				Check.That(arg2)
					.IsEqualTo("abc");
				Check.That(factoryCalled)
					.IsEqualTo(1);
				Check.That(fa)
					.IsEqualTo(123);
				Check.That(fb)
					.IsEqualTo(1.2);

				called1 = 0;
				called2 = 0;
				factoryCalled = 0;
				fa = 0;
				fb = 0.0;
				var flag2 = service.Do((a, b) => { fa = a; fb = b; factoryCalled++; return "def"; }, 456, 3.4);

				Check.That(flag2)
					.IsTrue();
				Check.That(called1)
					.IsEqualTo(1);
				Check.That(arg1)
					.IsEqualTo("def");
				Check.That(called2)
					.IsEqualTo(1);
				Check.That(arg2)
					.IsEqualTo("def");
				Check.That(factoryCalled)
					.IsEqualTo(1);
				Check.That(fa)
					.IsEqualTo(456);
				Check.That(fb)
					.IsEqualTo(3.4);

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(2);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public void Invoke_With_MultipleHandler_OneThrowingExceptionButAllInvoked()
			{
				var service = new FooService();

				Int32 called1 = 0;
				String arg1 = null;

				var subscription1 = service.Changed.Subscribe(new Action<String>(x => {
					arg1 = x;
					called1++;
					throw new ApplicationException("1");
				}));

				Int32 called2 = 0;
				String arg2 = null;

				var subscription2 = service.Changed.Subscribe(x => {
					arg2 = x;
					called2++;
				});

				Check.That(subscription1)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription1).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription2).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.Not.IsSameReferenceAs(subscription1);

				Exception exception = null;
				EventSystem.UnobservedException.SubscribeWeak(x => exception = x);

				Int32 factoryCalled = 0;
				Int32 fa = 0;
				Double fb = 0.0;
				var flag1 = service.Do((a, b) => { fa = a; fb = b; factoryCalled++; return "abc"; }, 123, 1.2);

				Check.That(flag1)
					.IsTrue();
				Check.That(called1)
					.IsEqualTo(1);
				Check.That(arg1)
					.IsEqualTo("abc");
				Check.That(called2)
					.IsEqualTo(1);
				Check.That(arg2)
					.IsEqualTo("abc");
				Check.That(factoryCalled)
					.IsEqualTo(1);
				Check.That(fa)
					.IsEqualTo(123);
				Check.That(fb)
					.IsEqualTo(1.2);

				Check.That(exception)
					.IsInstanceOf<ApplicationException>();
				Check.That(exception.Message)
					.IsEqualTo("1");

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(2);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public void Invoke_After_SubscriptionDisposed()
			{
				var service = new FooService();

				Int32 called = 0;
				String arg = null;

				var subscription = service.Changed.Subscribe(x => {
					arg = x;
					called++;
				});

				Check.That(subscription)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription).TestingGetPreviousSubscription())
					.IsNull();

				subscription.Dispose();

				Int32 factoryCalled = 0;
				var flag1 = service.Do((a, b) => { factoryCalled++; return "abc"; }, 123, 1.2);

				Check.That(flag1)
					.IsFalse();
				Check.That(called)
					.IsEqualTo(0);
				Check.That(factoryCalled)
					.IsEqualTo(0);

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(0);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public void Invoke_After_EventSourceDisposed()
			{
				var service = new FooService();

				Int32 called = 0;
				String arg = null;

				var subscription = service.Changed.Subscribe(x => {
					arg = x;
					called++;
				});

				Check.That(subscription)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription).TestingGetPreviousSubscription())
					.IsNull();

				service.ChangedSource.Dispose();

				Int32 factoryCalled = 0;
				var flag1 = service.Do((a, b) => { factoryCalled++; return "abc"; }, 123, 1.2);

				Check.That(flag1)
					.IsFalse();
				Check.That(called)
					.IsEqualTo(0);
				Check.That(factoryCalled)
					.IsEqualTo(0);

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(0);
				Check.That(service.ChangedSource.IsDisposed)
					.IsTrue();
			}

			[Test]
			public void Exception_For_NullFactory()
			{
				var service = new FooService();

				Check.ThatCode(() => service.Do((Func<Int32, Double, String>)null, 123, 1.2))
					.Throws<ArgumentNullException>()
					.WithProperty(x => x.ParamName, "valueFactory");
			}
		}

		[TestFixture]
		public class Invoke_ValueFactory_Arg3
		{
			[Test]
			public void Invoke_Without_Handler()
			{
				var service = new FooService();

				var flag = service.Do((a, b, c) => { Assert.Fail("MUST NOT be called"); return "abc"; }, 123, 1.2, 'a');

				Check.That(flag)
					.IsFalse();

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(0);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public void Invoke_With_SingleHandler()
			{
				var service = new FooService();

				Int32 called = 0;
				String arg = null;

				var subscription = service.Changed.Subscribe(x => {
					arg = x;
					called++;
				});

				Check.That(subscription)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription).TestingGetPreviousSubscription())
					.IsNull();

				Int32 factoryCalled = 0;
				Int32 fa = 0;
				Double fb = 0.0;
				Char fc = ' ';
				var flag1 = service.Do((a, b, c) => { fa = a; fb = b; fc = c; factoryCalled++; return "abc"; }, 123, 1.2, 'a');

				Check.That(flag1)
					.IsTrue();
				Check.That(called)
					.IsEqualTo(1);
				Check.That(arg)
					.IsEqualTo("abc");
				Check.That(factoryCalled)
					.IsEqualTo(1);
				Check.That(fa)
					.IsEqualTo(123);
				Check.That(fb)
					.IsEqualTo(1.2);
				Check.That(fc)
					.IsEqualTo('a');

				called = 0;
				factoryCalled = 0;
				fa = 0;
				fb = 0.0;
				fc = ' ';
				var flag2 = service.Do((a, b, c) => { fa = a; fb = b; fc = c; factoryCalled++; return "def"; }, 456, 3.4, 'b');

				Check.That(flag2)
					.IsTrue();
				Check.That(called)
					.IsEqualTo(1);
				Check.That(arg)
					.IsEqualTo("def");
				Check.That(factoryCalled)
					.IsEqualTo(1);
				Check.That(fa)
					.IsEqualTo(456);
				Check.That(fb)
					.IsEqualTo(3.4);
				Check.That(fc)
					.IsEqualTo('b');

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(1);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public void Invoke_With_MultipleHandler()
			{
				var service = new FooService();

				Int32 called1 = 0;
				String arg1 = null;

				var subscription1 = service.Changed.Subscribe(x => {
					arg1 = x;
					called1++;
				});

				Int32 called2 = 0;
				String arg2 = null;

				var subscription2 = service.Changed.Subscribe(x => {
					arg2 = x;
					called2++;
				});

				Check.That(subscription1)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription1).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription2).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.Not.IsSameReferenceAs(subscription1);

				Int32 factoryCalled = 0;
				Int32 fa = 0;
				Double fb = 0.0;
				Char fc = ' ';
				var flag1 = service.Do((a, b, c) => { fa = a; fb = b; fc = c; factoryCalled++; return "abc"; }, 123, 1.2, 'a');

				Check.That(flag1)
					.IsTrue();
				Check.That(called1)
					.IsEqualTo(1);
				Check.That(arg1)
					.IsEqualTo("abc");
				Check.That(called2)
					.IsEqualTo(1);
				Check.That(arg2)
					.IsEqualTo("abc");
				Check.That(factoryCalled)
					.IsEqualTo(1);
				Check.That(fa)
					.IsEqualTo(123);
				Check.That(fb)
					.IsEqualTo(1.2);
				Check.That(fc)
					.IsEqualTo('a');

				called1 = 0;
				called2 = 0;
				factoryCalled = 0;
				fa = 0;
				fb = 0.0;
				fc = ' ';
				var flag2 = service.Do((a, b, c) => { fa = a; fb = b; fc = c; factoryCalled++; return "def"; }, 456, 3.4, 'b');

				Check.That(flag2)
					.IsTrue();
				Check.That(called1)
					.IsEqualTo(1);
				Check.That(arg1)
					.IsEqualTo("def");
				Check.That(called2)
					.IsEqualTo(1);
				Check.That(arg2)
					.IsEqualTo("def");
				Check.That(factoryCalled)
					.IsEqualTo(1);
				Check.That(fa)
					.IsEqualTo(456);
				Check.That(fb)
					.IsEqualTo(3.4);
				Check.That(fc)
					.IsEqualTo('b');

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(2);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public void Invoke_With_MultipleHandler_OneThrowingExceptionButAllInvoked()
			{
				var service = new FooService();

				Int32 called1 = 0;
				String arg1 = null;

				var subscription1 = service.Changed.Subscribe(new Action<String>(x => {
					arg1 = x;
					called1++;
					throw new ApplicationException("1");
				}));

				Int32 called2 = 0;
				String arg2 = null;

				var subscription2 = service.Changed.Subscribe(x => {
					arg2 = x;
					called2++;
				});

				Check.That(subscription1)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription1).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription2).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.Not.IsSameReferenceAs(subscription1);

				Exception exception = null;
				EventSystem.UnobservedException.SubscribeWeak(x => exception = x);

				Int32 factoryCalled = 0;
				Int32 fa = 0;
				Double fb = 0.0;
				Char fc = ' ';
				var flag1 = service.Do((a, b, c) => { fa = a; fb = b; fc = c; factoryCalled++; return "abc"; }, 123, 1.2, 'a');

				Check.That(flag1)
					.IsTrue();
				Check.That(called1)
					.IsEqualTo(1);
				Check.That(arg1)
					.IsEqualTo("abc");
				Check.That(called2)
					.IsEqualTo(1);
				Check.That(arg2)
					.IsEqualTo("abc");
				Check.That(factoryCalled)
					.IsEqualTo(1);
				Check.That(fa)
					.IsEqualTo(123);
				Check.That(fb)
					.IsEqualTo(1.2);
				Check.That(fc)
					.IsEqualTo('a');

				Check.That(exception)
					.IsInstanceOf<ApplicationException>();
				Check.That(exception.Message)
					.IsEqualTo("1");

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(2);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public void Invoke_After_SubscriptionDisposed()
			{
				var service = new FooService();

				Int32 called = 0;
				String arg = null;

				var subscription = service.Changed.Subscribe(x => {
					arg = x;
					called++;
				});

				Check.That(subscription)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription).TestingGetPreviousSubscription())
					.IsNull();

				subscription.Dispose();

				Int32 factoryCalled = 0;
				var flag1 = service.Do((a, b, c) => { factoryCalled++; return "abc"; }, 123, 1.2, 'a');

				Check.That(flag1)
					.IsFalse();
				Check.That(called)
					.IsEqualTo(0);
				Check.That(factoryCalled)
					.IsEqualTo(0);

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(0);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public void Invoke_After_EventSourceDisposed()
			{
				var service = new FooService();

				Int32 called = 0;
				String arg = null;

				var subscription = service.Changed.Subscribe(x => {
					arg = x;
					called++;
				});

				Check.That(subscription)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription).TestingGetPreviousSubscription())
					.IsNull();

				service.ChangedSource.Dispose();

				Int32 factoryCalled = 0;
				var flag1 = service.Do((a, b, c) => { factoryCalled++; return "abc"; }, 123, 1.2, 'a');

				Check.That(flag1)
					.IsFalse();
				Check.That(called)
					.IsEqualTo(0);
				Check.That(factoryCalled)
					.IsEqualTo(0);

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(0);
				Check.That(service.ChangedSource.IsDisposed)
					.IsTrue();
			}

			[Test]
			public void Exception_For_NullFactory()
			{
				var service = new FooService();

				Check.ThatCode(() => service.Do((Func<Int32, Double, Char, String>)null, 123, 1.2, 'a'))
					.Throws<ArgumentNullException>()
					.WithProperty(x => x.ParamName, "valueFactory");
			}
		}


		[TestFixture]
		public class InvokeAsync
		{
			[Test]
			public async Task InvokeAsync_Without_Handler()
			{
				var service = new FooService();

				var flag = await service.DoAsync("abc");

				Check.That(flag)
					.IsFalse();

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(0);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public async Task InvokeAsync_With_SingleHandler()
			{
				var service = new FooService();

				Int32 called = 0;
				String arg = null;

				var subscription = service.Changed.Subscribe(x => {
					arg = x;
					called++;
				});

				Check.That(subscription)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription).TestingGetPreviousSubscription())
					.IsNull();

				var flag1 = await service.DoAsync("abc");

				Check.That(flag1)
					.IsTrue();
				Check.That(called)
					.IsEqualTo(1);
				Check.That(arg)
					.IsEqualTo("abc");

				called = 0;
				var flag2 = await service.DoAsync("def");

				Check.That(flag2)
					.IsTrue();
				Check.That(called)
					.IsEqualTo(1);
				Check.That(arg)
					.IsEqualTo("def");

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(1);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public async Task InvokeAsync_With_MultipleHandler()
			{
				var service = new FooService();

				Int32 called1 = 0;
				String arg1 = null;

				var subscription1 = service.Changed.Subscribe(x => {
					arg1 = x;
					called1++;
				});

				Int32 called2 = 0;
				String arg2 = null;

				var subscription2 = service.Changed.Subscribe(x => {
					arg2 = x;
					called2++;
				});

				Check.That(subscription1)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription1).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription2).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.Not.IsSameReferenceAs(subscription1);

				var flag1 = await service.DoAsync("abc");

				Check.That(flag1)
					.IsTrue();
				Check.That(called1)
					.IsEqualTo(1);
				Check.That(arg1)
					.IsEqualTo("abc");
				Check.That(called2)
					.IsEqualTo(1);
				Check.That(arg2)
					.IsEqualTo("abc");

				called1 = 0;
				called2 = 0;
				var flag2 = await service.DoAsync("def");

				Check.That(flag2)
					.IsTrue();
				Check.That(called1)
					.IsEqualTo(1);
				Check.That(arg1)
					.IsEqualTo("def");
				Check.That(called2)
					.IsEqualTo(1);
				Check.That(arg2)
					.IsEqualTo("def");

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(2);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public async Task InvokeAsync_With_MultipleHandler_OneThrowingExceptionAllInvoked()
			{
				var service = new FooService();

				Int32 called1 = 0;
				String arg1 = null;

				var subscription1 = service.Changed.Subscribe(new Action<String>(x => {
					arg1 = x;
					called1++;
					throw new ApplicationException("1");
				}));

				Int32 called2 = 0;
				String arg2 = null;

				var subscription2 = service.Changed.Subscribe(x => {
					arg2 = x;
					called2++;
				});

				Check.That(subscription1)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription1).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription2).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.Not.IsSameReferenceAs(subscription1);

				Exception exception = null;
				EventSystem.UnobservedException.SubscribeWeak(x => exception = x);

				var flag1 = await service.DoAsync("abc");

				Check.That(flag1)
					.IsTrue();
				Check.That(called1)
					.IsEqualTo(1);
				Check.That(arg1)
					.IsEqualTo("abc");
				Check.That(called2)
					.IsEqualTo(1);
				Check.That(arg2)
					.IsEqualTo("abc");

				Check.That(exception)
					.IsInstanceOf<ApplicationException>();
				Check.That(exception.Message)
					.IsEqualTo("1");

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(2);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public async Task InvokeAsync_After_SubscriptionDisposed()
			{
				var service = new FooService();

				Int32 called = 0;
				String arg = null;

				var subscription = service.Changed.Subscribe(x => {
					arg = x;
					called++;
				});

				Check.That(subscription)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription).TestingGetPreviousSubscription())
					.IsNull();

				subscription.Dispose();

				var flag1 = await service.DoAsync("abc");

				Check.That(flag1)
					.IsFalse();
				Check.That(called)
					.IsEqualTo(0);

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(0);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public async Task InvokeAsync_After_EventSourceDisposed()
			{
				var service = new FooService();

				Int32 called = 0;
				String arg = null;

				var subscription = service.Changed.Subscribe(x => {
					arg = x;
					called++;
				});

				Check.That(subscription)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription).TestingGetPreviousSubscription())
					.IsNull();

				service.ChangedSource.Dispose();

				var flag1 = await service.DoAsync("abc");

				Check.That(flag1)
					.IsFalse();
				Check.That(called)
					.IsEqualTo(0);

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(0);
				Check.That(service.ChangedSource.IsDisposed)
					.IsTrue();
			}
		}

		[TestFixture]
		public class InvokeAsync_ValueFactory
		{
			[Test]
			public async Task InvokeAsync_Without_Handler()
			{
				var service = new FooService();

				var flag = await service.DoAsync(() => { Assert.Fail("MUST NOT be called"); return "abc"; });

				Check.That(flag)
					.IsFalse();

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(0);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public async Task InvokeAsync_With_SingleHandler()
			{
				var service = new FooService();

				Int32 called = 0;
				String arg = null;

				var subscription = service.Changed.Subscribe(x => {
					arg = x;
					called++;
				});

				Check.That(subscription)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription).TestingGetPreviousSubscription())
					.IsNull();

				Int32 factoryCalled = 0;
				var flag1 = await service.DoAsync(() => { factoryCalled++; return "abc"; });

				Check.That(flag1)
					.IsTrue();
				Check.That(called)
					.IsEqualTo(1);
				Check.That(arg)
					.IsEqualTo("abc");
				Check.That(factoryCalled)
					.IsEqualTo(1);

				called = 0;
				factoryCalled = 0;
				var flag2 = await service.DoAsync(() => { factoryCalled++; return "def"; });

				Check.That(flag2)
					.IsTrue();
				Check.That(called)
					.IsEqualTo(1);
				Check.That(arg)
					.IsEqualTo("def");
				Check.That(factoryCalled)
					.IsEqualTo(1);

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(1);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public async Task InvokeAsync_With_MultipleHandler()
			{
				var service = new FooService();

				Int32 called1 = 0;
				String arg1 = null;

				var subscription1 = service.Changed.Subscribe(x => {
					arg1 = x;
					called1++;
				});

				Int32 called2 = 0;
				String arg2 = null;

				var subscription2 = service.Changed.Subscribe(x => {
					arg2 = x;
					called2++;
				});

				Check.That(subscription1)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription1).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription2).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.Not.IsSameReferenceAs(subscription1);

				Int32 factoryCalled = 0;
				var flag1 = await service.DoAsync(() => { factoryCalled++; return "abc"; });

				Check.That(flag1)
					.IsTrue();
				Check.That(called1)
					.IsEqualTo(1);
				Check.That(arg1)
					.IsEqualTo("abc");
				Check.That(called2)
					.IsEqualTo(1);
				Check.That(arg2)
					.IsEqualTo("abc");
				Check.That(factoryCalled)
					.IsEqualTo(1);

				called1 = 0;
				called2 = 0;
				factoryCalled = 0;
				var flag2 = await service.DoAsync(() => { factoryCalled++; return "def"; });

				Check.That(flag2)
					.IsTrue();
				Check.That(called1)
					.IsEqualTo(1);
				Check.That(arg1)
					.IsEqualTo("def");
				Check.That(called2)
					.IsEqualTo(1);
				Check.That(arg2)
					.IsEqualTo("def");
				Check.That(factoryCalled)
					.IsEqualTo(1);

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(2);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public async Task InvokeAsync_With_MultipleHandler_OneThrowingExceptionButAllInvoked()
			{
				var service = new FooService();

				Int32 called1 = 0;
				String arg1 = null;

				var subscription1 = service.Changed.Subscribe(new Action<String>(x => {
					arg1 = x;
					called1++;
					throw new ApplicationException("1");
				}));

				Int32 called2 = 0;
				String arg2 = null;

				var subscription2 = service.Changed.Subscribe(x => {
					arg2 = x;
					called2++;
				});

				Check.That(subscription1)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription1).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription2).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.Not.IsSameReferenceAs(subscription1);

				Exception exception = null;
				EventSystem.UnobservedException.SubscribeWeak(x => exception = x);

				Int32 factoryCalled = 0;
				var flag1 = await service.DoAsync(() => { factoryCalled++; return "abc"; });

				Check.That(flag1)
					.IsTrue();
				Check.That(called1)
					.IsEqualTo(1);
				Check.That(arg1)
					.IsEqualTo("abc");
				Check.That(called2)
					.IsEqualTo(1);
				Check.That(arg2)
					.IsEqualTo("abc");
				Check.That(factoryCalled)
					.IsEqualTo(1);

				Check.That(exception)
					.IsInstanceOf<ApplicationException>();
				Check.That(exception.Message)
					.IsEqualTo("1");

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(2);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public async Task InvokeAsync_After_SubscriptionDisposed()
			{
				var service = new FooService();

				Int32 called = 0;
				String arg = null;

				var subscription = service.Changed.Subscribe(x => {
					arg = x;
					called++;
				});

				Check.That(subscription)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription).TestingGetPreviousSubscription())
					.IsNull();

				subscription.Dispose();

				Int32 factoryCalled = 0;
				var flag1 = await service.DoAsync(() => { factoryCalled++; return "abc"; });

				Check.That(flag1)
					.IsFalse();
				Check.That(called)
					.IsEqualTo(0);
				Check.That(factoryCalled)
					.IsEqualTo(0);

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(0);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public async Task InvokeAsync_After_EventSourceDisposed()
			{
				var service = new FooService();

				Int32 called = 0;
				String arg = null;

				var subscription = service.Changed.Subscribe(x => {
					arg = x;
					called++;
				});

				Check.That(subscription)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription).TestingGetPreviousSubscription())
					.IsNull();

				service.ChangedSource.Dispose();

				Int32 factoryCalled = 0;
				var flag1 = await service.DoAsync(() => { factoryCalled++; return "abc"; });

				Check.That(flag1)
					.IsFalse();
				Check.That(called)
					.IsEqualTo(0);
				Check.That(factoryCalled)
					.IsEqualTo(0);

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(0);
				Check.That(service.ChangedSource.IsDisposed)
					.IsTrue();
			}

			[Test]
			public void Exception_For_NullFactory()
			{
				var service = new FooService();

				Check.ThatAsyncCode(async () => await service.DoAsync((Func<String>)null))
					.Throws<ArgumentNullException>()
					.WithProperty(x => x.ParamName, "valueFactory");
			}
		}

		[TestFixture]
		public class InvokeAsync_ValueFactory_Arg1
		{
			[Test]
			public async Task InvokeAsync_Without_Handler()
			{
				var service = new FooService();

				var flag = await service.DoAsync((a) => { Assert.Fail("MUST NOT be called"); return "abc"; }, 123);

				Check.That(flag)
					.IsFalse();

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(0);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public async Task InvokeAsync_With_SingleHandler()
			{
				var service = new FooService();

				Int32 called = 0;
				String arg = null;

				var subscription = service.Changed.Subscribe(x => {
					arg = x;
					called++;
				});

				Check.That(subscription)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription).TestingGetPreviousSubscription())
					.IsNull();

				Int32 factoryCalled = 0;
				Int32 fa = 0;
				var flag1 = await service.DoAsync((a) => { fa = a; factoryCalled++; return "abc"; }, 123);

				Check.That(flag1)
					.IsTrue();
				Check.That(called)
					.IsEqualTo(1);
				Check.That(arg)
					.IsEqualTo("abc");
				Check.That(factoryCalled)
					.IsEqualTo(1);
				Check.That(fa)
					.IsEqualTo(123);

				called = 0;
				factoryCalled = 0;
				fa = 0;
				var flag2 = await service.DoAsync((a) => { fa = a; factoryCalled++; return "def"; }, 456);

				Check.That(flag2)
					.IsTrue();
				Check.That(called)
					.IsEqualTo(1);
				Check.That(arg)
					.IsEqualTo("def");
				Check.That(factoryCalled)
					.IsEqualTo(1);
				Check.That(fa)
					.IsEqualTo(456);

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(1);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public async Task InvokeAsync_With_MultipleHandler()
			{
				var service = new FooService();

				Int32 called1 = 0;
				String arg1 = null;

				var subscription1 = service.Changed.Subscribe(x => {
					arg1 = x;
					called1++;
				});

				Int32 called2 = 0;
				String arg2 = null;

				var subscription2 = service.Changed.Subscribe(x => {
					arg2 = x;
					called2++;
				});

				Check.That(subscription1)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription1).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription2).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.Not.IsSameReferenceAs(subscription1);

				Int32 factoryCalled = 0;
				Int32 fa = 0;
				var flag1 = await service.DoAsync((a) => { fa = a; factoryCalled++; return "abc"; }, 123);

				Check.That(flag1)
					.IsTrue();
				Check.That(called1)
					.IsEqualTo(1);
				Check.That(arg1)
					.IsEqualTo("abc");
				Check.That(called2)
					.IsEqualTo(1);
				Check.That(arg2)
					.IsEqualTo("abc");
				Check.That(factoryCalled)
					.IsEqualTo(1);
				Check.That(fa)
					.IsEqualTo(123);

				called1 = 0;
				called2 = 0;
				factoryCalled = 0;
				fa = 0;
				var flag2 = await service.DoAsync((a) => { fa = a; factoryCalled++; return "def"; }, 456);

				Check.That(flag2)
					.IsTrue();
				Check.That(called1)
					.IsEqualTo(1);
				Check.That(arg1)
					.IsEqualTo("def");
				Check.That(called2)
					.IsEqualTo(1);
				Check.That(arg2)
					.IsEqualTo("def");
				Check.That(factoryCalled)
					.IsEqualTo(1);
				Check.That(fa)
					.IsEqualTo(456);

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(2);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public async Task InvokeAsync_With_MultipleHandler_OneThrowingExceptionButAllInvoked()
			{
				var service = new FooService();

				Int32 called1 = 0;
				String arg1 = null;

				var subscription1 = service.Changed.Subscribe(new Action<String>(x => {
					arg1 = x;
					called1++;
					throw new ApplicationException("1");
				}));

				Int32 called2 = 0;
				String arg2 = null;

				var subscription2 = service.Changed.Subscribe(x => {
					arg2 = x;
					called2++;
				});

				Check.That(subscription1)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription1).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription2).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.Not.IsSameReferenceAs(subscription1);

				Exception exception = null;
				EventSystem.UnobservedException.SubscribeWeak(x => exception = x);

				Int32 factoryCalled = 0;
				Int32 fa = 0;
				var flag1 = await service.DoAsync((a) => { fa = a; factoryCalled++; return "abc"; }, 123);

				Check.That(flag1)
					.IsTrue();
				Check.That(called1)
					.IsEqualTo(1);
				Check.That(arg1)
					.IsEqualTo("abc");
				Check.That(called2)
					.IsEqualTo(1);
				Check.That(arg2)
					.IsEqualTo("abc");
				Check.That(factoryCalled)
					.IsEqualTo(1);
				Check.That(fa)
					.IsEqualTo(123);

				Check.That(exception)
					.IsInstanceOf<ApplicationException>();
				Check.That(exception.Message)
					.IsEqualTo("1");

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(2);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public async Task InvokeAsync_After_SubscriptionDisposed()
			{
				var service = new FooService();

				Int32 called = 0;
				String arg = null;

				var subscription = service.Changed.Subscribe(x => {
					arg = x;
					called++;
				});

				Check.That(subscription)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription).TestingGetPreviousSubscription())
					.IsNull();

				subscription.Dispose();

				Int32 factoryCalled = 0;
				var flag1 = await service.DoAsync((a) => { factoryCalled++; return "abc"; }, 123);

				Check.That(flag1)
					.IsFalse();
				Check.That(called)
					.IsEqualTo(0);
				Check.That(factoryCalled)
					.IsEqualTo(0);

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(0);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public async Task InvokeAsync_After_EventSourceDisposed()
			{
				var service = new FooService();

				Int32 called = 0;
				String arg = null;

				var subscription = service.Changed.Subscribe(x => {
					arg = x;
					called++;
				});

				Check.That(subscription)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription).TestingGetPreviousSubscription())
					.IsNull();

				service.ChangedSource.Dispose();

				Int32 factoryCalled = 0;
				var flag1 = await service.DoAsync((a) => { factoryCalled++; return "abc"; }, 123);

				Check.That(flag1)
					.IsFalse();
				Check.That(called)
					.IsEqualTo(0);
				Check.That(factoryCalled)
					.IsEqualTo(0);

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(0);
				Check.That(service.ChangedSource.IsDisposed)
					.IsTrue();
			}

			[Test]
			public void Exception_For_NullFactory()
			{
				var service = new FooService();

				Check.ThatAsyncCode(async () => await service.DoAsync((Func<Int32, String>)null, 123))
					.Throws<ArgumentNullException>()
					.WithProperty(x => x.ParamName, "valueFactory");
			}
		}

		[TestFixture]
		public class InvokeAsync_ValueFactory_Arg2
		{
			[Test]
			public async Task InvokeAsync_Without_Handler()
			{
				var service = new FooService();

				var flag = await service.DoAsync((a, b) => { Assert.Fail("MUST NOT be called"); return "abc"; }, 123, 1.2);

				Check.That(flag)
					.IsFalse();

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(0);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public async Task InvokeAsync_With_SingleHandler()
			{
				var service = new FooService();

				Int32 called = 0;
				String arg = null;

				var subscription = service.Changed.Subscribe(x => {
					arg = x;
					called++;
				});

				Check.That(subscription)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription).TestingGetPreviousSubscription())
					.IsNull();

				Int32 factoryCalled = 0;
				Int32 fa = 0;
				Double fb = 0.0;
				var flag1 = await service.DoAsync((a, b) => { fa = a; fb = b; factoryCalled++; return "abc"; }, 123, 1.2);

				Check.That(flag1)
					.IsTrue();
				Check.That(called)
					.IsEqualTo(1);
				Check.That(arg)
					.IsEqualTo("abc");
				Check.That(factoryCalled)
					.IsEqualTo(1);
				Check.That(fa)
					.IsEqualTo(123);
				Check.That(fb)
					.IsEqualTo(1.2);

				called = 0;
				factoryCalled = 0;
				fa = 0;
				fb = 0.0;
				var flag2 = await service.DoAsync((a, b) => { fa = a; fb = b; factoryCalled++; return "def"; }, 456, 3.4);

				Check.That(flag2)
					.IsTrue();
				Check.That(called)
					.IsEqualTo(1);
				Check.That(arg)
					.IsEqualTo("def");
				Check.That(factoryCalled)
					.IsEqualTo(1);
				Check.That(fa)
					.IsEqualTo(456);
				Check.That(fb)
					.IsEqualTo(3.4);

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(1);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public async Task InvokeAsync_With_MultipleHandler()
			{
				var service = new FooService();

				Int32 called1 = 0;
				String arg1 = null;

				var subscription1 = service.Changed.Subscribe(x => {
					arg1 = x;
					called1++;
				});

				Int32 called2 = 0;
				String arg2 = null;

				var subscription2 = service.Changed.Subscribe(x => {
					arg2 = x;
					called2++;
				});

				Check.That(subscription1)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription1).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription2).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.Not.IsSameReferenceAs(subscription1);

				Int32 factoryCalled = 0;
				Int32 fa = 0;
				Double fb = 0.0;
				var flag1 = await service.DoAsync((a, b) => { fa = a; fb = b; factoryCalled++; return "abc"; }, 123, 1.2);

				Check.That(flag1)
					.IsTrue();
				Check.That(called1)
					.IsEqualTo(1);
				Check.That(arg1)
					.IsEqualTo("abc");
				Check.That(called2)
					.IsEqualTo(1);
				Check.That(arg2)
					.IsEqualTo("abc");
				Check.That(factoryCalled)
					.IsEqualTo(1);
				Check.That(fa)
					.IsEqualTo(123);
				Check.That(fb)
					.IsEqualTo(1.2);

				called1 = 0;
				called2 = 0;
				factoryCalled = 0;
				fa = 0;
				fb = 0.0;
				var flag2 = await service.DoAsync((a, b) => { fa = a; fb = b; factoryCalled++; return "def"; }, 456, 3.4);

				Check.That(flag2)
					.IsTrue();
				Check.That(called1)
					.IsEqualTo(1);
				Check.That(arg1)
					.IsEqualTo("def");
				Check.That(called2)
					.IsEqualTo(1);
				Check.That(arg2)
					.IsEqualTo("def");
				Check.That(factoryCalled)
					.IsEqualTo(1);
				Check.That(fa)
					.IsEqualTo(456);
				Check.That(fb)
					.IsEqualTo(3.4);

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(2);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public async Task InvokeAsync_With_MultipleHandler_OneThrowingExceptionButAllInvoked()
			{
				var service = new FooService();

				Int32 called1 = 0;
				String arg1 = null;

				var subscription1 = service.Changed.Subscribe(new Action<String>(x => {
					arg1 = x;
					called1++;
					throw new ApplicationException("1");
				}));

				Int32 called2 = 0;
				String arg2 = null;

				var subscription2 = service.Changed.Subscribe(x => {
					arg2 = x;
					called2++;
				});

				Check.That(subscription1)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription1).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription2).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.Not.IsSameReferenceAs(subscription1);

				Exception exception = null;
				EventSystem.UnobservedException.SubscribeWeak(x => exception = x);

				Int32 factoryCalled = 0;
				Int32 fa = 0;
				Double fb = 0.0;
				var flag1 = await service.DoAsync((a, b) => { fa = a; fb = b; factoryCalled++; return "abc"; }, 123, 1.2);

				Check.That(flag1)
					.IsTrue();
				Check.That(called1)
					.IsEqualTo(1);
				Check.That(arg1)
					.IsEqualTo("abc");
				Check.That(called2)
					.IsEqualTo(1);
				Check.That(arg2)
					.IsEqualTo("abc");
				Check.That(factoryCalled)
					.IsEqualTo(1);
				Check.That(fa)
					.IsEqualTo(123);
				Check.That(fb)
					.IsEqualTo(1.2);

				Check.That(exception)
					.IsInstanceOf<ApplicationException>();
				Check.That(exception.Message)
					.IsEqualTo("1");

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(2);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public async Task InvokeAsync_After_SubscriptionDisposed()
			{
				var service = new FooService();

				Int32 called = 0;
				String arg = null;

				var subscription = service.Changed.Subscribe(x => {
					arg = x;
					called++;
				});

				Check.That(subscription)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription).TestingGetPreviousSubscription())
					.IsNull();

				subscription.Dispose();

				Int32 factoryCalled = 0;
				var flag1 = await service.DoAsync((a, b) => { factoryCalled++; return "abc"; }, 123, 1.2);

				Check.That(flag1)
					.IsFalse();
				Check.That(called)
					.IsEqualTo(0);
				Check.That(factoryCalled)
					.IsEqualTo(0);

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(0);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public async Task InvokeAsync_After_EventSourceDisposed()
			{
				var service = new FooService();

				Int32 called = 0;
				String arg = null;

				var subscription = service.Changed.Subscribe(x => {
					arg = x;
					called++;
				});

				Check.That(subscription)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription).TestingGetPreviousSubscription())
					.IsNull();

				service.ChangedSource.Dispose();

				Int32 factoryCalled = 0;
				var flag1 = await service.DoAsync((a, b) => { factoryCalled++; return "abc"; }, 123, 1.2);

				Check.That(flag1)
					.IsFalse();
				Check.That(called)
					.IsEqualTo(0);
				Check.That(factoryCalled)
					.IsEqualTo(0);

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(0);
				Check.That(service.ChangedSource.IsDisposed)
					.IsTrue();
			}

			[Test]
			public void Exception_For_NullFactory()
			{
				var service = new FooService();

				Check.ThatAsyncCode(async () => await service.DoAsync((Func<Int32, Double, String>)null, 123, 1.2))
					.Throws<ArgumentNullException>()
					.WithProperty(x => x.ParamName, "valueFactory");
			}
		}

		[TestFixture]
		public class InvokeAsync_ValueFactory_Arg3
		{
			[Test]
			public async Task InvokeAsync_Without_Handler()
			{
				var service = new FooService();

				var flag = await service.DoAsync((a, b, c) => { Assert.Fail("MUST NOT be called"); return "abc"; }, 123, 1.2, 'a');

				Check.That(flag)
					.IsFalse();

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(0);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public async Task InvokeAsync_With_SingleHandler()
			{
				var service = new FooService();

				Int32 called = 0;
				String arg = null;

				var subscription = service.Changed.Subscribe(x => {
					arg = x;
					called++;
				});

				Check.That(subscription)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription).TestingGetPreviousSubscription())
					.IsNull();

				Int32 factoryCalled = 0;
				Int32 fa = 0;
				Double fb = 0.0;
				Char fc = ' ';
				var flag1 = await service.DoAsync((a, b, c) => { fa = a; fb = b; fc = c; factoryCalled++; return "abc"; }, 123, 1.2, 'a');

				Check.That(flag1)
					.IsTrue();
				Check.That(called)
					.IsEqualTo(1);
				Check.That(arg)
					.IsEqualTo("abc");
				Check.That(factoryCalled)
					.IsEqualTo(1);
				Check.That(fa)
					.IsEqualTo(123);
				Check.That(fb)
					.IsEqualTo(1.2);
				Check.That(fc)
					.IsEqualTo('a');

				called = 0;
				factoryCalled = 0;
				fa = 0;
				fb = 0.0;
				fc = ' ';
				var flag2 = await service.DoAsync((a, b, c) => { fa = a; fb = b; fc = c; factoryCalled++; return "def"; }, 456, 3.4, 'b');

				Check.That(flag2)
					.IsTrue();
				Check.That(called)
					.IsEqualTo(1);
				Check.That(arg)
					.IsEqualTo("def");
				Check.That(factoryCalled)
					.IsEqualTo(1);
				Check.That(fa)
					.IsEqualTo(456);
				Check.That(fb)
					.IsEqualTo(3.4);
				Check.That(fc)
					.IsEqualTo('b');

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(1);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public async Task InvokeAsync_With_MultipleHandler()
			{
				var service = new FooService();

				Int32 called1 = 0;
				String arg1 = null;

				var subscription1 = service.Changed.Subscribe(x => {
					arg1 = x;
					called1++;
				});

				Int32 called2 = 0;
				String arg2 = null;

				var subscription2 = service.Changed.Subscribe(x => {
					arg2 = x;
					called2++;
				});

				Check.That(subscription1)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription1).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription2).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.Not.IsSameReferenceAs(subscription1);

				Int32 factoryCalled = 0;
				Int32 fa = 0;
				Double fb = 0.0;
				Char fc = ' ';
				var flag1 = await service.DoAsync((a, b, c) => { fa = a; fb = b; fc = c; factoryCalled++; return "abc"; }, 123, 1.2, 'a');

				Check.That(flag1)
					.IsTrue();
				Check.That(called1)
					.IsEqualTo(1);
				Check.That(arg1)
					.IsEqualTo("abc");
				Check.That(called2)
					.IsEqualTo(1);
				Check.That(arg2)
					.IsEqualTo("abc");
				Check.That(factoryCalled)
					.IsEqualTo(1);
				Check.That(fa)
					.IsEqualTo(123);
				Check.That(fb)
					.IsEqualTo(1.2);
				Check.That(fc)
					.IsEqualTo('a');

				called1 = 0;
				called2 = 0;
				factoryCalled = 0;
				fa = 0;
				fb = 0.0;
				fc = ' ';
				var flag2 = await service.DoAsync((a, b, c) => { fa = a; fb = b; fc = c; factoryCalled++; return "def"; }, 456, 3.4, 'b');

				Check.That(flag2)
					.IsTrue();
				Check.That(called1)
					.IsEqualTo(1);
				Check.That(arg1)
					.IsEqualTo("def");
				Check.That(called2)
					.IsEqualTo(1);
				Check.That(arg2)
					.IsEqualTo("def");
				Check.That(factoryCalled)
					.IsEqualTo(1);
				Check.That(fa)
					.IsEqualTo(456);
				Check.That(fb)
					.IsEqualTo(3.4);
				Check.That(fc)
					.IsEqualTo('b');

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(2);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public async Task InvokeAsync_With_MultipleHandler_OneThrowingExceptionButAllInvoked()
			{
				var service = new FooService();

				Int32 called1 = 0;
				String arg1 = null;

				var subscription1 = service.Changed.Subscribe(new Action<String>(x => {
					arg1 = x;
					called1++;
					throw new ApplicationException("1");
				}));

				Int32 called2 = 0;
				String arg2 = null;

				var subscription2 = service.Changed.Subscribe(x => {
					arg2 = x;
					called2++;
				});

				Check.That(subscription1)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription1).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription2).TestingGetPreviousSubscription())
					.IsNull();

				Check.That(subscription2)
					.Not.IsSameReferenceAs(subscription1);

				Exception exception = null;
				EventSystem.UnobservedException.SubscribeWeak(x => exception = x);

				Int32 factoryCalled = 0;
				Int32 fa = 0;
				Double fb = 0.0;
				Char fc = ' ';
				var flag1 = await service.DoAsync((a, b, c) => { fa = a; fb = b; fc = c; factoryCalled++; return "abc"; }, 123, 1.2, 'a');

				Check.That(flag1)
					.IsTrue();
				Check.That(called1)
					.IsEqualTo(1);
				Check.That(arg1)
					.IsEqualTo("abc");
				Check.That(called2)
					.IsEqualTo(1);
				Check.That(arg2)
					.IsEqualTo("abc");
				Check.That(factoryCalled)
					.IsEqualTo(1);
				Check.That(fa)
					.IsEqualTo(123);
				Check.That(fb)
					.IsEqualTo(1.2);
				Check.That(fc)
					.IsEqualTo('a');

				Check.That(exception)
					.IsInstanceOf<ApplicationException>();
				Check.That(exception.Message)
					.IsEqualTo("1");

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(2);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public async Task InvokeAsync_After_SubscriptionDisposed()
			{
				var service = new FooService();

				Int32 called = 0;
				String arg = null;

				var subscription = service.Changed.Subscribe(x => {
					arg = x;
					called++;
				});

				Check.That(subscription)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription).TestingGetPreviousSubscription())
					.IsNull();

				subscription.Dispose();

				Int32 factoryCalled = 0;
				var flag1 = await service.DoAsync((a, b, c) => { factoryCalled++; return "abc"; }, 123, 1.2, 'a');

				Check.That(flag1)
					.IsFalse();
				Check.That(called)
					.IsEqualTo(0);
				Check.That(factoryCalled)
					.IsEqualTo(0);

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(0);
				Check.That(service.ChangedSource.IsDisposed)
					.IsFalse();
			}

			[Test]
			public async Task InvokeAsync_After_EventSourceDisposed()
			{
				var service = new FooService();

				Int32 called = 0;
				String arg = null;

				var subscription = service.Changed.Subscribe(x => {
					arg = x;
					called++;
				});

				Check.That(subscription)
					.IsInstanceOf<ActionSubscription<String>>();
				Check.That(((ActionSubscription<String>)subscription).TestingGetPreviousSubscription())
					.IsNull();

				service.ChangedSource.Dispose();

				Int32 factoryCalled = 0;
				var flag1 = await service.DoAsync((a, b, c) => { factoryCalled++; return "abc"; }, 123, 1.2, 'a');

				Check.That(flag1)
					.IsFalse();
				Check.That(called)
					.IsEqualTo(0);
				Check.That(factoryCalled)
					.IsEqualTo(0);

				Check.That(service.ChangedSource.NumberOfSubscriptions)
					.IsEqualTo(0);
				Check.That(service.ChangedSource.IsDisposed)
					.IsTrue();
			}

			[Test]
			public void Exception_For_NullFactory()
			{
				var service = new FooService();

				Check.ThatAsyncCode(async () => await service.DoAsync((Func<Int32, Double, Char, String>)null, 123, 1.2, 'a'))
					.Throws<ArgumentNullException>()
					.WithProperty(x => x.ParamName, "valueFactory");
			}
		}
	}
}
