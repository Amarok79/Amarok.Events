/* Copyright(c) 2018, Olaf Kober
 * Licensed under GNU Lesser General Public License v3.0 (LPGL-3.0).
 * https://github.com/Amarok79/Events
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
			for (Int32 i = 0; i < 10; i++)
				progress.Report(i);
		}


		[Test]
		public void EventSource_CanBeUsedAs_IProgress()
		{
			var source = new EventSource<Int32>();

			var events = new List<Int32>();
			source.Event.Subscribe(x => events.Add(x));

			FakeMethodWithIProgress(source);

			Check.That(events)
				.ContainsExactly(0, 1, 2, 3, 4, 5, 6, 7, 8, 9);
		}
	}
}
