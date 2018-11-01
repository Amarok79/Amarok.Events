/* Copyright(c) 2018, Olaf Kober
 * Licensed under GNU Lesser General Public License v3.0 (LPGL-3.0).
 * https://github.com/Amarok79/Amarok.Events
 */

#pragma warning disable S1694 // An abstract class should have both abstract and concrete methods
#pragma warning disable S3881 // "IDisposable" should be implemented correctly

using System;
using System.Threading.Tasks;


namespace Amarok.Events
{
	/// <summary>
	/// Implementation class that represents a subscription.
	/// 
	/// Concrete derivations implement specific behaviors, for example, support for synchronous and 
	/// asynchronous handler methods, weak subscriptions, etc.
	/// </summary>
	internal abstract class Subscription<T> :
		IDisposable
	{
		/// <summary>
		/// Invokes the subscription's handler in a synchronous way.
		/// </summary>
		public abstract void Invoke(T value);

		/// <summary>
		/// Invokes the subscription's handler in an asynchronous way.
		/// </summary>
		public abstract ValueTask InvokeAsync(T value);

		/// <summary>
		/// Disposes the subscription; removes it from the event source.
		/// </summary>
		public abstract void Dispose();
	}
}
