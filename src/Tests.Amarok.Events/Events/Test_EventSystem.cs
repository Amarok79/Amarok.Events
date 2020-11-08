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
using NCrunch.Framework;
using NFluent;
using NUnit.Framework;


namespace Amarok.Events
{
    [TestFixture]
    public class Test_EventSystem
    {
        [Test]
        public void NotifyUnobservedException_ThrowsForNullException()
        {
            Check.ThatCode(() => EventSystem.NotifyUnobservedException(null))
                 .Throws<ArgumentNullException>()
                 .WithProperty(x => x.ParamName, "exception");
        }

        [Test, Serial]
        public void NotifyUnobservedException_RaisesEvent()
        {
            var       called    = 0;
            Exception exception = null;

            void handler(Exception x)
            {
                exception = x;
                called++;
            }

            using var subscription = EventSystem.UnobservedException.Subscribe(handler);

            Check.That(called).IsEqualTo(0);

            var ex = new ApplicationException();
            EventSystem.NotifyUnobservedException(ex);

            Check.That(called).IsEqualTo(1);
            Check.That(exception).IsSameReferenceAs(ex);
        }
    }
}
