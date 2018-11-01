/* Copyright(c) 2018, Olaf Kober
 * Licensed under GNU Lesser General Public License v3.0 (LPGL-3.0).
 * https://github.com/Amarok79/Amarok.Events
 */

using System;
using NFluent;
using NUnit.Framework;


namespace Amarok.Events
{
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
}
