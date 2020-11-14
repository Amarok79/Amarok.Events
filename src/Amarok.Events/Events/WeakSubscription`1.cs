/* MIT License
 * 
 * Copyright (c) 2020, Olaf Kober
 * https://github.com/Amarok79/Amarok.Events
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Diagnostics;
using System.Threading.Tasks;


namespace Amarok.Events
{
    /// <summary>
    ///     Implementation class that represents a weak subscription. This weak subscription usually refers via weak reference
    ///     to another subscription, which again refers back to this subscription again via weak reference.
    /// </summary>
    [DebuggerStepThrough]
    internal sealed class WeakSubscription<T> : Subscription<T>
    {
        /// <summary>
        ///     a reference to the event source; necessary for disposal
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly EventSource<T> mSource;

        /// <summary>
        ///     a weak reference to another subscription referring to the handler
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly WeakReference<Subscription<T>> mNextSubscription;


        /// <summary>
        ///     For better debugging experience.
        /// </summary>
        public WeakReference<Subscription<T>> Subscription => mNextSubscription;


        /// <summary>
        ///     Initializes a new instance.
        /// </summary>
        public WeakSubscription(EventSource<T> source, Subscription<T> subscription)
        {
            mSource           = source;
            mNextSubscription = new WeakReference<Subscription<T>>(subscription);
        }


        /// <summary>
        ///     Invokes the subscription's handler in a synchronous way.
        /// </summary>
        public override void Invoke(T value)
        {
            if (mNextSubscription.TryGetTarget(out var subscription))
            {
                // forward invocation to next subscription
                subscription.Invoke(value);
            }
            else
            {
                // otherwise, remove ourself from event source
                mSource.Remove(this);
            }
        }

        /// <summary>
        ///     Invokes the subscription's handler in an asynchronous way.
        /// </summary>
        public override ValueTask InvokeAsync(T value)
        {
            if (mNextSubscription.TryGetTarget(out var subscription))
            {
                // forward invocation to next subscription
                return subscription.InvokeAsync(value);
            }

            // otherwise, remove ourself from event source
            mSource.Remove(this);

            return new ValueTask(Task.CompletedTask);
        }

        /// <summary>
        ///     Disposes the subscription; removes it from the event source.
        /// </summary>
        public override void Dispose()
        {
            // simply, remove ourself from event source
            mSource.Remove(this);
        }

        /// <summary>
        ///     Returns a string that represents the current instance.
        /// </summary>
        public override String ToString()
        {
            if (mNextSubscription.TryGetTarget(out var subscription))
                return $"⇒ weak {subscription}";

            return "⇒ weak ⇒ <null>";
        }


        internal void TestingClearNextSubscription()
        {
            mNextSubscription.SetTarget(null!);
        }
    }
}
