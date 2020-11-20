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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;


namespace Amarok.Events
{
    /// <summary>
    ///     This type represents a recorder of events. The recorder subscribes itself on the supplied event and records all
    ///     events that are being raised. In addition to the event argument, the recorder also records information about the
    ///     timestamp, the calling thread, etc. This makes it a useful utility in writing unit tests interacting with events.
    ///     This type is thread-safe.
    /// </summary>
    public sealed class EventRecorder<T> : IDisposable
    {
        // data
        private readonly Object mSyncThis = new Object();
        private readonly List<EventInfo> mEventInfos = new List<EventInfo>();
        private readonly Stopwatch mWatch = new Stopwatch();
        private readonly IDisposable mSubscription;

        // state
        private Boolean mIsPaused;

        // caches
        private EventInfo[]? mCachedEventInfos;
        private T[]? mCachedEvents;


        /// <summary>
        ///     Gets a boolean value indicating whether the event recorder is currently paused.
        /// </summary>
        public Boolean IsPaused
        {
            get
            {
                lock (mSyncThis)
                {
                    return mIsPaused;
                }
            }
        }

        /// <summary>
        ///     Gets the number of events that have been recorded so far.
        /// </summary>
        public Int32 Count
        {
            get
            {
                lock (mSyncThis)
                {
                    return mEventInfos.Count;
                }
            }
        }

        /// <summary>
        ///     Gets the event arguments of the recorded events as read-only list. This property returns a snapshot for the
        ///     recorded events, thus, it is safe to enumerate the returned collection while still recording further events. The
        ///     property caches the returned collection and returns this cached collection for subsequent calls, if possible. That
        ///     means if in the meantime no further events have been recorded, the same list is returned. This is an optimization
        ///     to reduce memory allocations.
        /// </summary>
        public IReadOnlyList<T> Events
        {
            get
            {
                lock (mSyncThis)
                {
                    if (mCachedEvents == null || mCachedEvents.Length != mEventInfos.Count)
                        mCachedEvents = mEventInfos.Select(x => x.Value).ToArray();

                    return mCachedEvents;
                }
            }
        }

        /// <summary>
        ///     Gets a read-only list of info objects for recorded events, where each info object provides information about a
        ///     single recorded event. This property returns a snapshot for the recorded events, thus, it is safe to enumerate the
        ///     returned collection while still recording further events. The property caches the returned collection and returns
        ///     this cached collection for subsequent calls, if possible. That means if in the meantime no further events have been
        ///     recorded, the same list is returned. This is an optimization to reduce memory allocations.
        /// </summary>
        public IReadOnlyList<EventInfo> EventInfos
        {
            get
            {
                lock (mSyncThis)
                {
                    if (mCachedEventInfos == null || mCachedEventInfos.Length != mEventInfos.Count)
                        mCachedEventInfos = mEventInfos.ToArray();

                    return mCachedEventInfos;
                }
            }
        }


        /// <summary>
        ///     Initializes a new instance.
        /// </summary>
        public EventRecorder(Event<T> @event)
        {
            mSubscription = @event.SubscribeWeak(x => _HandleEvent(x));
        }


        private void _HandleEvent(T value)
        {
            lock (mSyncThis)
            {
                if (mIsPaused)
                    return;

                var timeOffset = mEventInfos.Count == 0 ? TimeSpan.Zero : mWatch.Elapsed;

                var info = new EventInfo(
                    value,
                    mEventInfos.Count,
                    DateTimeOffset.Now,
                    timeOffset,
                    Thread.CurrentThread
                );

                mEventInfos.Add(info);
                mWatch.Restart();
            }
        }


        /// <summary>
        ///     Resets the event recorder to its initial state. The list of recorded events is cleared.
        /// </summary>
        public void Reset()
        {
            lock (mSyncThis)
            {
                mEventInfos.Clear();
                mCachedEvents     = null;
                mCachedEventInfos = null;
                mIsPaused         = false;
                mWatch.Reset();
            }
        }

        /// <summary>
        ///     Pauses recording of events, until <see cref="Resume"/> is called.
        /// </summary>
        public void Pause()
        {
            lock (mSyncThis)
            {
                mIsPaused = true;
            }
        }

        /// <summary>
        ///     Resumes recording of events, if <see cref="Pause"/> was called previously.
        /// </summary>
        public void Resume()
        {
            lock (mSyncThis)
            {
                mIsPaused = false;
            }
        }


        /// <summary>
        ///     Disposes this event recorder, meaning the event recorder unregisters from the supplied event. That means no further
        ///     events are recorded, regardless of whether the event recorder is paused or resumed. The list of already recorded
        ///     events remains accessible.
        /// </summary>
        public void Dispose()
        {
            mSubscription.Dispose();
        }


        /// <summary>
        ///     This type provides information about a single recorded event.
        /// </summary>
        public sealed class EventInfo
        {
            /// <summary>
            ///     Gets the event argument of the recorded event.
            /// </summary>
            public T Value { get; }

            /// <summary>
            ///     Gets the index number of the recorded event. The first recorded event gets an index of zero, the second recorded
            ///     event an index of one, the third an index of two.
            /// </summary>
            public Int32 Index { get; }

            /// <summary>
            ///     Gets the timestamp in local time when the event was recorded.
            /// </summary>
            public DateTimeOffset Timestamp { get; }

            /// <summary>
            ///     Gets the time offset from the previously recorded event. The first recorded event always has a time offset of zero.
            ///     The second recorded event usually has a time offset greater than zero.
            /// </summary>
            public TimeSpan TimeOffset { get; }

            /// <summary>
            ///     Gets the thread that was raising the event.
            /// </summary>
            public Thread Thread { get; }


            /// <summary>
            ///     Initializes a new instance.
            /// </summary>
            public EventInfo(T value, Int32 index, DateTimeOffset timestamp, TimeSpan timeOffset, Thread thread)
            {
                Value      = value;
                Index      = index;
                Timestamp  = timestamp;
                TimeOffset = timeOffset;
                Thread     = thread;
            }
        }
    }
}
