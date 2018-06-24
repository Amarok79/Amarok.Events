/* Copyright(c) 2018, Olaf Kober
 * Licensed under GNU Lesser General Public License v3.0 (LPGL-3.0).
 * https://github.com/Amarok79/Events
 */

using System;


namespace Amarok.Events
{
	/// <summary>
	/// Implementation class that represents a null subscription.
	/// </summary>
	internal sealed class NullSubscription :
		IDisposable
	{
		public static readonly IDisposable Instance = new NullSubscription();


		private NullSubscription()
		{
			// shouldn't be construct-able; use Instance instead
		}


		public void Dispose()
		{
			// intentionally left empty
		}

		public override String ToString()
		{
			return "=> <null>";
		}
	}
}
