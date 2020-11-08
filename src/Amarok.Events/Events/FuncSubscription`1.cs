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
using System.Reflection;
using System.Threading.Tasks;


namespace Amarok.Events
{
    /// <summary>
    ///     Implementation class that represents a subscription to an async handler method.
    /// </summary>
    [DebuggerStepThrough]
    internal sealed class FuncSubscription<T> : Subscription<T>
    {
        // a reference to the event source; necessary for disposal
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly EventSource<T> mSource;

        // a delegate to the handler method
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Func<T, Task> mFunc;

        // an optional weak reference back to another subscription holding this subscription
        // also via weak reference; necessary for automatic removal magic of weak subscriptions
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private WeakReference<Subscription<T>>? mPreviousSubscription;


        /// <summary>
        ///     For better debugging experience.
        /// </summary>
        public Object Target => mFunc.Target;

        /// <summary>
        ///     For better debugging experience.
        /// </summary>
        public MethodInfo Method => mFunc.Method;


        /// <summary>
        ///     Initializes a new instance.
        /// </summary>
        public FuncSubscription(EventSource<T> source, Func<T, Task> func)
        {
            mSource = source;
            mFunc   = func;
        }


        /// <summary>
        ///     Invoked to establish a weak reference back to another subscription. Only called for weak subscriptions.
        /// </summary>
        public void SetPreviousSubscription(Subscription<T> subscription)
        {
            mPreviousSubscription = new WeakReference<Subscription<T>>(subscription);
        }

        /// <summary>
        ///     Invokes the subscription's handler in a synchronous way.
        /// </summary>
        public override void Invoke(T value)
        {
            var task = mFunc(value);

            if (task.IsCompleted && !task.IsFaulted)
                return;

            task.ContinueWith(
                x => EventSystem.NotifyUnobservedException(x.Exception.InnerException),
                TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted
            );
        }

        /// <summary>
        ///     Invokes the subscription's handler in an asynchronous way.
        /// </summary>
        public override ValueTask InvokeAsync(T value)
        {
            var task = mFunc(value);

            return new ValueTask(task);
        }

        /// <summary>
        ///     Disposes the subscription; removes it from the event source.
        /// </summary>
        public override void Dispose()
        {
            if (mPreviousSubscription != null)
            {
                // dispose the previous subscription, if still reachable
                if (mPreviousSubscription.TryGetTarget(out var subscription))
                    subscription.Dispose();
            }
            else
            {
                // remove ourself from event source
                mSource.Remove(this);
            }
        }

        /// <summary>
        ///     Returns a string that represents the current instance.
        /// </summary>
        public override String ToString()
        {
            return $"⇒ async {mFunc.Method.DeclaringType.FullName}.{mFunc.Method.Name}()";
        }


        internal Subscription<T>? TestingGetPreviousSubscription()
        {
            if (mPreviousSubscription == null)
                return null;

            if (mPreviousSubscription.TryGetTarget(out var subscription))
                return subscription;

            return null;
        }

        internal void TestingClearNextSubscription()
        {
            mPreviousSubscription?.SetTarget(null!);
        }
    }
}
