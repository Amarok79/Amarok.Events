/* Copyright(c) 2018, Olaf Kober
 * Licensed under GNU Lesser General Public License v3.0 (LPGL-3.0).
 * https://github.com/Amarok79/Events
 */

#pragma warning disable S1215 // "GC.Collect" should not be called

using System;
using NFluent;
using NUnit.Framework;


namespace Amarok.Events
{
	[TestFixture]
	public class Test_UseCase_SyncWeakHandler
	{
		public sealed class FooArgs
		{
			public Int32 Value { get; }

			public FooArgs(Int32 value)
			{
				this.Value = value;
			}
		}

		public interface IFooService
		{
			Event<FooArgs> Changed { get; }
		}

		public sealed class FooServiceImpl : IFooService
		{
			private readonly EventSource<FooArgs> mChangedEvent = new EventSource<FooArgs>();

			public Event<FooArgs> Changed => mChangedEvent.Event;

			public Boolean Do(Int32 value)
			{
				return mChangedEvent.Invoke(new FooArgs(value));
			}
		}


		// --- TESTS ---


		[Test]
		public void Invoked_Without_Handler()
		{
			var service = new FooServiceImpl();
			var flag = service.Do(123);

			Check.That(flag)
				.IsFalse();
		}

		[Test]
		public void Invoked_With_Handler()
		{
			var service = new FooServiceImpl();

			Int32 called = 0;
			FooArgs arg = null;

			var subscription = service.Changed.SubscribeWeak(x => { arg = x; called++; });

			var flag = service.Do(123);

			Check.That(flag)
				.IsTrue();
			Check.That(called)
				.IsEqualTo(1);
			Check.That(arg.Value)
				.IsEqualTo(123);

			called = 0;
			flag = service.Do(456);

			Check.That(flag)
				.IsTrue();
			Check.That(called)
				.IsEqualTo(1);
			Check.That(arg.Value)
				.IsEqualTo(456);

			subscription.Dispose();

			called = 0;
			flag = service.Do(789);

			Check.That(flag)
				.IsFalse();
			Check.That(called)
				.IsEqualTo(0);
		}

		[Test]
		public void Invoked_WithGCed_Handler()
		{
			var service = new FooServiceImpl();

			Int32 called = 0;
			FooArgs arg = null;

			var subscription = service.Changed.SubscribeWeak(x => { arg = x; called++; });

			var flag = service.Do(123);

			Check.That(flag)
				.IsTrue();
			Check.That(called)
				.IsEqualTo(1);
			Check.That(arg.Value)
				.IsEqualTo(123);

			var weakSubscription = (WeakSubscription<FooArgs>)
				((ActionSubscription<FooArgs>)subscription).TestingGetPreviousSubscription();
			weakSubscription.TestingClearNextSubscription();

			called = 0;
			flag = service.Do(456);

			Check.That(flag)
				.IsTrue();
			Check.That(called)
				.IsEqualTo(0);

			subscription.Dispose();

			called = 0;
			flag = service.Do(789);

			Check.That(flag)
				.IsFalse();
			Check.That(called)
				.IsEqualTo(0);
		}
	}
}
