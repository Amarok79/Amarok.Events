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


namespace Amarok.Events
{
    /// <summary>
    ///     This type provides static members that affect the entire app domain-wide event system. This type is thread-safe.
    /// </summary>
    public static class EventSystem
    {
        // static data
        private static readonly EventSource<Exception> sUnobservedExceptionEventSource = new();


        #region ++ Public Interface ++

        /// <summary>
        ///     An event that is raised every time an exception is thrown by one of the event subscribers. All exceptions thrown by
        ///     event subscribers of all event sources in the current app domain are forwarded to this single global event.
        ///     Applications can subscribe on this event and thus log otherwise unobserved exceptions occurring in the
        ///     application's event subscribers.
        /// </summary>
        public static Event<Exception> UnobservedException => sUnobservedExceptionEventSource.Event;


        /// <summary>
        ///     Notifies that an unobserved exception was thrown by one of the application's event subscribers.
        /// </summary>
        /// 
        /// <param name="exception">
        ///     The exception that was caught.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        ///     A null reference was passed to a method that did not accept it as a valid argument.
        /// </exception>
        public static void NotifyUnobservedException(Exception? exception)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            sUnobservedExceptionEventSource.Invoke(exception);
        }

        #endregion
    }
}
