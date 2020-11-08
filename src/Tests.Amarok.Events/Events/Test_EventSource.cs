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
using NFluent;
using NUnit.Framework;


namespace Amarok.Events
{
    public class Test_EventSource
    {
        [TestFixture]
        public class Dispose
        {
            [Test]
            public void DisposeTwice()
            {
                var src = new EventSource<Int32>();

                Check.That(src.IsDisposed).IsFalse();

                src.Dispose();

                Check.ThatCode(() => src.Dispose()).DoesNotThrow();

                Check.That(src.IsDisposed).IsTrue();
            }

            [Test]
            public void DisposeClearsSubscriptions()
            {
                var src = new EventSource<Int32>();
                var sub = src.Event.Subscribe(x => { });

                Check.That(src.IsDisposed).IsFalse();
                Check.That(src.NumberOfSubscriptions).IsEqualTo(1);

                src.Dispose();

                Check.That(src.IsDisposed).IsTrue();
                Check.That(src.NumberOfSubscriptions).IsEqualTo(0);

                Check.ThatCode(() => sub.Dispose()).DoesNotThrow();
            }
        }
    }
}
