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
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;


namespace Amarok.Events
{
    [TestFixture]
    public class Test_Documentation
    {
        public interface IFooService
        {
            Event<Int32> Progress { get; }
        }

        internal sealed class FooServiceImpl : IFooService
        {
            private readonly EventSource<Int32> mProgressEventSource = new EventSource<Int32>();


            public Event<Int32> Progress => mProgressEventSource.Event;


            public void DoSomething()
            {
                // raises the event
                mProgressEventSource.Invoke(50);
            }
        }


        [Test]
        public void Example1()
        {
            var         serviceImpl = new FooServiceImpl();
            IFooService service     = serviceImpl;

            var subscription = service.Progress.Subscribe(x => Console.WriteLine(x + "%"));

            serviceImpl.DoSomething();

            // console output:	50%

            GC.KeepAlive(subscription);
        }

        [Test]
        public void Example2()
        {
            var         serviceImpl = new FooServiceImpl();
            IFooService service     = serviceImpl;

            var subscription = service.Progress.Subscribe(x => Console.WriteLine(x + "%"));

            serviceImpl.DoSomething();

            // console output:	50%

            subscription.Dispose();

            serviceImpl.DoSomething();
        }

        [Test]
        public void Example3()
        {
            using var source = new EventSource<String>();

            source.Event.Subscribe(x => Console.WriteLine(x + "1"));
            source.Event.Subscribe(x => Console.WriteLine(x + "2"));

            Console.WriteLine("A");
            source.Invoke("B");
            Console.WriteLine("C");

            // output:
            //	A
            //	B1
            //	B2
            //	C
        }

        [Test]
        public void Example4()
        {
            using var source = new EventSource<String>();

            source.Event.Subscribe(
                async x => {
                    // async event handler
                    await Task.Delay(10);
                    Console.WriteLine(x + "1");
                }
            );

            source.Event.Subscribe(
                async x => {
                    // async event handler
                    await Task.Delay(50);
                    Console.WriteLine(x + "2");
                }
            );

            Console.WriteLine("A");
            source.Invoke("B");
            Console.WriteLine("C");

            Thread.Sleep(500);

            // output:
            //	A
            //	C
            //	B1
            //	B2
        }

        [Test]
        public async Task Example5()
        {
            using var source = new EventSource<String>();

            source.Event.Subscribe(
                async x => {
                    // async event handler
                    await Task.Delay(10);
                    Console.WriteLine(x + "1");
                }
            );

            source.Event.Subscribe(
                async x => {
                    // async event handler
                    await Task.Delay(50);
                    Console.WriteLine(x + "2");
                }
            );

            Console.WriteLine("A");
            await source.InvokeAsync("B");
            Console.WriteLine("C");

            // output:
            //	A
            //	B1
            //	B2
            //	C
        }

        [Test]
        public async Task Example6()
        {
            using var source = new EventSource<String>();

            source.Event.Subscribe(x => Console.WriteLine(x + "1"));
            source.Event.Subscribe(x => Console.WriteLine(x + "2"));

            Console.WriteLine("A");
            await source.InvokeAsync("B");
            Console.WriteLine("C");

            // output:
            //	A
            //	B1
            //	B2
            //	C
        }

        [Test]
        public void Example7()
        {
            using var source = new EventSource<String>();

            source.Event.Subscribe(x => Console.WriteLine(x + "1"));

            Console.WriteLine("A");
            source.Invoke("B");
            Console.WriteLine("C");
        }
    }
}
