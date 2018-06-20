/* Copyright(c) 2018, Olaf Kober
 * Licensed under GNU Lesser General Public License v3.0 (LPGL-3.0).
 * https://github.com/Amarok79/Events
 */

#pragma warning disable S1694 // An abstract class should have both abstract and concrete methods
#pragma warning disable S3881 // "IDisposable" should be implemented correctly

using System;


namespace Amarok.Events
{
	internal abstract class Subscription<T> :
		IDisposable
	{
		public abstract void Invoke(T value);

		public abstract void Dispose();
	}
}
