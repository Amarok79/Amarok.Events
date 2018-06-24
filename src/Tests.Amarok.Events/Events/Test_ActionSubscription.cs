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
	[TestFixture, Ignore("")]
	public class Test_ActionSubscription
	{
		private void DummyHandler(String x)
		{
			// dummy
		}


		[Test]
		public void Invoke_Calls_Handler()
		{
			// arrange
			var source = new EventSource<String>();

			Int32 called = 0;
			String arg = null;

			var s = new ActionSubscription<String>(source, x => { called++; arg = x; });

			// act
			s.Invoke("abc");

			// assert
			Check.That(called)
				.IsEqualTo(1);
			Check.That(arg)
				.IsEqualTo("abc");
		}

		[Test]
		public async Task InvokeAsync_Calls_Handler()
		{
			// arrange
			var source = new EventSource<String>();

			Int32 called = 0;
			String arg = null;

			var s = new ActionSubscription<String>(source, x => { called++; arg = x; });

			// act
			await s.InvokeAsync("abc");

			// assert
			Check.That(called)
				.IsEqualTo(1);
			Check.That(arg)
				.IsEqualTo("abc");
		}

		[Test]
		public void ToString_Returns_HandlerName()
		{
			var source = new EventSource<String>();
			var s = new ActionSubscription<String>(source, DummyHandler);

			Check.That(s.ToString())
				.IsEqualTo("=> Amarok.Events.Test_ActionSubscription.DummyHandler()");
		}
	}
}
