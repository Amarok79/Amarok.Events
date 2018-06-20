/* Copyright(c) 2018, Olaf Kober
 * Licensed under GNU Lesser General Public License v3.0 (LPGL-3.0).
 * https://github.com/Amarok79/Events
 */

using System;


namespace Amarok.Events
{
	internal sealed class NullSubscription :
		IDisposable
	{
		public static readonly IDisposable Instance = new NullSubscription();


		public void Dispose()
		{
			// Method intentionally left empty.
		}
	}
}
