/* Copyright(c) 2018, Olaf Kober
 * Licensed under GNU Lesser General Public License v3.0 (LPGL-3.0).
 * https://github.com/Amarok79/Events
 */

using System;
using NCrunch.Framework;
using NFluent;
using NUnit.Framework;


namespace Amarok.Events
{
	[TestFixture]
	public class Test_EventSystem
	{
		[Test]
		public void NotifyUnobservedException_ThrowsForNullException()
		{
			Check.ThatCode(() => EventSystem.NotifyUnobservedException(null))
				.Throws<ArgumentNullException>()
				.WithProperty(x => x.ParamName, "exception");
		}

		[Test, Serial]
		public void NotifyUnobservedException_RaisesEvent()
		{
			Int32 called = 0;
			Exception exception = null;

			Action<Exception> handler = x => {
				exception = x;
				called++;
			};

			using (var subscription = EventSystem.UnobservedException.Subscribe(handler))
			{
				Check.That(called)
					.IsEqualTo(0);

				var ex = new ApplicationException();
				EventSystem.NotifyUnobservedException(ex);

				Check.That(called)
					.IsEqualTo(1);
				Check.That(exception)
					.IsSameReferenceAs(ex);
			}
		}
	}
}
