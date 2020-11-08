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
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;


namespace Amarok.Events
{
    [SimpleJob(RuntimeMoniker.Net471), SimpleJob(RuntimeMoniker.Net48), SimpleJob(RuntimeMoniker.NetCoreApp21),
     SimpleJob(RuntimeMoniker.NetCoreApp31), MemoryDiagnoser]
    public class Benchmarks
    {
        public class DotNet
        {
            public sealed class FooEventArgs : EventArgs
            {
                public String Payload { get; }

                public FooEventArgs(String payload)
                {
                    Payload = payload;
                }
            }

            public interface IFooService
            {
                event EventHandler<FooEventArgs> FooChanged;
            }

            public sealed class FooServiceImpl : IFooService
            {
                public event EventHandler<FooEventArgs> FooChanged;

                public void Do()
                {
                    FooChanged?.Invoke(this, new FooEventArgs("A"));
                }
            }

            private void _HandleFooChanged(Object sender, FooEventArgs e)
            {
                // Method intentionally left empty.
            }


            public FooServiceImpl ServiceHandlerCount0;
            public FooServiceImpl ServiceHandlerCount1;
            public FooServiceImpl ServiceHandlerCount3;
            public FooServiceImpl ServiceHandlerCount9;


            public DotNet()
            {
                ServiceHandlerCount0 = new FooServiceImpl();

                ServiceHandlerCount1            =  new FooServiceImpl();
                ServiceHandlerCount1.FooChanged += _HandleFooChanged;

                ServiceHandlerCount3            =  new FooServiceImpl();
                ServiceHandlerCount3.FooChanged += _HandleFooChanged;
                ServiceHandlerCount3.FooChanged += _HandleFooChanged;
                ServiceHandlerCount3.FooChanged += _HandleFooChanged;

                ServiceHandlerCount9            =  new FooServiceImpl();
                ServiceHandlerCount9.FooChanged += _HandleFooChanged;
                ServiceHandlerCount9.FooChanged += _HandleFooChanged;
                ServiceHandlerCount9.FooChanged += _HandleFooChanged;
                ServiceHandlerCount9.FooChanged += _HandleFooChanged;
                ServiceHandlerCount9.FooChanged += _HandleFooChanged;
                ServiceHandlerCount9.FooChanged += _HandleFooChanged;
                ServiceHandlerCount9.FooChanged += _HandleFooChanged;
                ServiceHandlerCount9.FooChanged += _HandleFooChanged;
                ServiceHandlerCount9.FooChanged += _HandleFooChanged;
            }
        }

        public class Amarok
        {
            public interface IFooService
            {
                Event<String> FooChanged { get; }
            }

            public sealed class FooServiceImpl : IFooService
            {
                private readonly EventSource<String> mFooChanged = new EventSource<String>();

                public Event<String> FooChanged => mFooChanged.Event;

                public void Do()
                {
                    mFooChanged.Invoke("A");
                }

                public ValueTask<Boolean> DoAsync()
                {
                    return mFooChanged.InvokeAsync("A");
                }
            }

            private void _HandleFooChanged(String args)
            {
                // Method intentionally left empty.
            }

            private Task _HandleFooChangedAsync(String args)
            {
                return Task.CompletedTask;
            }


            public FooServiceImpl ServiceSyncHandlerCount0;
            public FooServiceImpl ServiceSyncHandlerCount1;
            public FooServiceImpl ServiceSyncHandlerCount3;
            public FooServiceImpl ServiceSyncHandlerCount9;

            public FooServiceImpl ServiceAsyncHandlerCount0;
            public FooServiceImpl ServiceAsyncHandlerCount1;
            public FooServiceImpl ServiceAsyncHandlerCount3;
            public FooServiceImpl ServiceAsyncHandlerCount9;

            public FooServiceImpl ServiceWeakHandlerCount0;
            public FooServiceImpl ServiceWeakHandlerCount1;
            public FooServiceImpl ServiceWeakHandlerCount3;
            public FooServiceImpl ServiceWeakHandlerCount9;

            private readonly List<IDisposable> mSubscriptions = new List<IDisposable>();


            public Amarok()
            {
                // sync handler
                ServiceSyncHandlerCount0 = new FooServiceImpl();

                ServiceSyncHandlerCount1 = new FooServiceImpl();
                ServiceSyncHandlerCount1.FooChanged.Subscribe(_HandleFooChanged);

                ServiceSyncHandlerCount3 = new FooServiceImpl();
                ServiceSyncHandlerCount3.FooChanged.Subscribe(_HandleFooChanged);
                ServiceSyncHandlerCount3.FooChanged.Subscribe(_HandleFooChanged);
                ServiceSyncHandlerCount3.FooChanged.Subscribe(_HandleFooChanged);

                ServiceSyncHandlerCount9 = new FooServiceImpl();
                ServiceSyncHandlerCount9.FooChanged.Subscribe(_HandleFooChanged);
                ServiceSyncHandlerCount9.FooChanged.Subscribe(_HandleFooChanged);
                ServiceSyncHandlerCount9.FooChanged.Subscribe(_HandleFooChanged);
                ServiceSyncHandlerCount9.FooChanged.Subscribe(_HandleFooChanged);
                ServiceSyncHandlerCount9.FooChanged.Subscribe(_HandleFooChanged);
                ServiceSyncHandlerCount9.FooChanged.Subscribe(_HandleFooChanged);
                ServiceSyncHandlerCount9.FooChanged.Subscribe(_HandleFooChanged);
                ServiceSyncHandlerCount9.FooChanged.Subscribe(_HandleFooChanged);
                ServiceSyncHandlerCount9.FooChanged.Subscribe(_HandleFooChanged);

                // async handler
                ServiceAsyncHandlerCount0 = new FooServiceImpl();

                ServiceAsyncHandlerCount1 = new FooServiceImpl();
                ServiceAsyncHandlerCount1.FooChanged.Subscribe(_HandleFooChangedAsync);

                ServiceAsyncHandlerCount3 = new FooServiceImpl();
                ServiceAsyncHandlerCount3.FooChanged.Subscribe(_HandleFooChangedAsync);
                ServiceAsyncHandlerCount3.FooChanged.Subscribe(_HandleFooChangedAsync);
                ServiceAsyncHandlerCount3.FooChanged.Subscribe(_HandleFooChangedAsync);

                ServiceAsyncHandlerCount9 = new FooServiceImpl();
                ServiceAsyncHandlerCount9.FooChanged.Subscribe(_HandleFooChangedAsync);
                ServiceAsyncHandlerCount9.FooChanged.Subscribe(_HandleFooChangedAsync);
                ServiceAsyncHandlerCount9.FooChanged.Subscribe(_HandleFooChangedAsync);
                ServiceAsyncHandlerCount9.FooChanged.Subscribe(_HandleFooChangedAsync);
                ServiceAsyncHandlerCount9.FooChanged.Subscribe(_HandleFooChangedAsync);
                ServiceAsyncHandlerCount9.FooChanged.Subscribe(_HandleFooChangedAsync);
                ServiceAsyncHandlerCount9.FooChanged.Subscribe(_HandleFooChangedAsync);
                ServiceAsyncHandlerCount9.FooChanged.Subscribe(_HandleFooChangedAsync);
                ServiceAsyncHandlerCount9.FooChanged.Subscribe(_HandleFooChangedAsync);

                // weak sync handler
                ServiceWeakHandlerCount0 = new FooServiceImpl();

                ServiceWeakHandlerCount1 = new FooServiceImpl();
                mSubscriptions.Add(ServiceWeakHandlerCount1.FooChanged.SubscribeWeak(_HandleFooChanged));

                ServiceWeakHandlerCount3 = new FooServiceImpl();
                mSubscriptions.Add(ServiceWeakHandlerCount3.FooChanged.SubscribeWeak(_HandleFooChanged));
                mSubscriptions.Add(ServiceWeakHandlerCount3.FooChanged.SubscribeWeak(_HandleFooChanged));
                mSubscriptions.Add(ServiceWeakHandlerCount3.FooChanged.SubscribeWeak(_HandleFooChanged));

                ServiceWeakHandlerCount9 = new FooServiceImpl();
                mSubscriptions.Add(ServiceWeakHandlerCount9.FooChanged.SubscribeWeak(_HandleFooChanged));
                mSubscriptions.Add(ServiceWeakHandlerCount9.FooChanged.SubscribeWeak(_HandleFooChanged));
                mSubscriptions.Add(ServiceWeakHandlerCount9.FooChanged.SubscribeWeak(_HandleFooChanged));
                mSubscriptions.Add(ServiceWeakHandlerCount9.FooChanged.SubscribeWeak(_HandleFooChanged));
                mSubscriptions.Add(ServiceWeakHandlerCount9.FooChanged.SubscribeWeak(_HandleFooChanged));
                mSubscriptions.Add(ServiceWeakHandlerCount9.FooChanged.SubscribeWeak(_HandleFooChanged));
                mSubscriptions.Add(ServiceWeakHandlerCount9.FooChanged.SubscribeWeak(_HandleFooChanged));
                mSubscriptions.Add(ServiceWeakHandlerCount9.FooChanged.SubscribeWeak(_HandleFooChanged));
                mSubscriptions.Add(ServiceWeakHandlerCount9.FooChanged.SubscribeWeak(_HandleFooChanged));
            }
        }


        private readonly DotNet mDotNet;
        private readonly Amarok mAmarok;


        public Benchmarks()
        {
            mDotNet = new DotNet();
            mAmarok = new Amarok();
        }


        // ----- .NET Events -----


        [Benchmark]
        public void DotNet_RaiseSync_SyncHandler_0()
        {
            mDotNet.ServiceHandlerCount0.Do();
        }

        [Benchmark(Baseline = true)]
        public void DotNet_RaiseSync_SyncHandler_1()
        {
            mDotNet.ServiceHandlerCount1.Do();
        }

        [Benchmark]
        public void DotNet_RaiseSync_SyncHandler_3()
        {
            mDotNet.ServiceHandlerCount3.Do();
        }

        [Benchmark]
        public void DotNet_RaiseSync_SyncHandler_9()
        {
            mDotNet.ServiceHandlerCount9.Do();
        }


        // ----- sync handler -----


        [Benchmark]
        public void Amarok_InvokeSync_SyncHandler_0()
        {
            mAmarok.ServiceSyncHandlerCount0.Do();
        }

        [Benchmark]
        public void Amarok_InvokeSync_SyncHandler_1()
        {
            mAmarok.ServiceSyncHandlerCount1.Do();
        }

        [Benchmark]
        public void Amarok_InvokeSync_SyncHandler_3()
        {
            mAmarok.ServiceSyncHandlerCount3.Do();
        }

        [Benchmark]
        public void Amarok_InvokeSync_SyncHandler_9()
        {
            mAmarok.ServiceSyncHandlerCount9.Do();
        }


        [Benchmark]
        public void Amarok_InvokeAsync_SyncHandler_0()
        {
            mAmarok.ServiceSyncHandlerCount0.DoAsync().GetAwaiter().GetResult();
        }

        [Benchmark]
        public void Amarok_InvokeAsync_SyncHandler_1()
        {
            mAmarok.ServiceSyncHandlerCount1.DoAsync().GetAwaiter().GetResult();
        }

        [Benchmark]
        public void Amarok_InvokeAsync_SyncHandler_3()
        {
            mAmarok.ServiceSyncHandlerCount3.DoAsync().GetAwaiter().GetResult();
        }

        [Benchmark]
        public void Amarok_InvokeAsync_SyncHandler_9()
        {
            mAmarok.ServiceSyncHandlerCount9.DoAsync().GetAwaiter().GetResult();
        }


        // ----- weak sync handler -----


        [Benchmark]
        public void Amarok_InvokeSync_WeakSyncHandler_0()
        {
            mAmarok.ServiceWeakHandlerCount0.Do();
        }

        [Benchmark]
        public void Amarok_InvokeSync_WeakSyncHandler_1()
        {
            mAmarok.ServiceWeakHandlerCount1.Do();
        }

        [Benchmark]
        public void Amarok_InvokeSync_WeakSyncHandler_3()
        {
            mAmarok.ServiceWeakHandlerCount3.Do();
        }

        [Benchmark]
        public void Amarok_InvokeSync_WeakSyncHandler_9()
        {
            mAmarok.ServiceWeakHandlerCount9.Do();
        }


        //                              Method |  Job | Runtime |       Mean |     Error |     StdDev |     Median | Scaled | ScaledSD |  Gen 0 | Allocated |
        //------------------------------------ |----- |-------- |-----------:|----------:|-----------:|-----------:|-------:|---------:|-------:|----------:|
        //      DotNet_RaiseSync_SyncHandler_0 |  Clr |     Clr |   1.267 ns | 0.0625 ns |  0.1305 ns |   1.202 ns |   0.19 |     0.02 |      - |       0 B |
        //      DotNet_RaiseSync_SyncHandler_1 |  Clr |     Clr |   6.768 ns | 0.0935 ns |  0.0730 ns |   6.754 ns |   1.00 |     0.00 | 0.0076 |      24 B |
        //      DotNet_RaiseSync_SyncHandler_3 |  Clr |     Clr |  15.693 ns | 0.3604 ns |  1.0224 ns |  15.214 ns |   2.32 |     0.15 | 0.0076 |      24 B |
        //      DotNet_RaiseSync_SyncHandler_9 |  Clr |     Clr |  28.419 ns | 0.3977 ns |  0.3720 ns |  28.453 ns |   4.20 |     0.07 | 0.0076 |      24 B |
        //     Amarok_InvokeSync_SyncHandler_0 |  Clr |     Clr |  26.191 ns | 0.8128 ns |  1.0851 ns |  25.720 ns |   3.87 |     0.16 |      - |       0 B |
        //     Amarok_InvokeSync_SyncHandler_1 |  Clr |     Clr |  38.007 ns | 1.1104 ns |  3.2565 ns |  38.274 ns |   5.62 |     0.48 |      - |       0 B |
        //     Amarok_InvokeSync_SyncHandler_3 |  Clr |     Clr |  44.542 ns | 0.9195 ns |  1.5861 ns |  44.172 ns |   6.58 |     0.24 |      - |       0 B |
        //     Amarok_InvokeSync_SyncHandler_9 |  Clr |     Clr |  67.300 ns | 1.3681 ns |  2.0477 ns |  66.427 ns |   9.95 |     0.31 |      - |       0 B |
        //    Amarok_InvokeAsync_SyncHandler_0 |  Clr |     Clr |  49.116 ns | 0.9556 ns |  0.8471 ns |  49.301 ns |   7.26 |     0.14 |      - |       0 B |
        //    Amarok_InvokeAsync_SyncHandler_1 |  Clr |     Clr |  71.772 ns | 1.4588 ns |  2.6305 ns |  71.664 ns |  10.61 |     0.40 |      - |       0 B |
        //    Amarok_InvokeAsync_SyncHandler_3 |  Clr |     Clr | 109.438 ns | 1.9861 ns |  1.8578 ns | 108.476 ns |  16.17 |     0.31 |      - |       0 B |
        //    Amarok_InvokeAsync_SyncHandler_9 |  Clr |     Clr | 234.884 ns | 6.6757 ns | 13.1772 ns | 231.006 ns |  34.71 |     1.96 |      - |       0 B |
        // Amarok_InvokeSync_WeakSyncHandler_0 |  Clr |     Clr |  28.870 ns | 0.6146 ns |  1.0098 ns |  28.891 ns |   4.27 |     0.15 |      - |       0 B |
        // Amarok_InvokeSync_WeakSyncHandler_1 |  Clr |     Clr |  43.219 ns | 0.5658 ns |  0.4091 ns |  43.130 ns |   6.39 |     0.09 |      - |       0 B |
        // Amarok_InvokeSync_WeakSyncHandler_3 |  Clr |     Clr |  60.113 ns | 1.1852 ns |  1.4989 ns |  59.814 ns |   8.88 |     0.24 |      - |       0 B |
        // Amarok_InvokeSync_WeakSyncHandler_9 |  Clr |     Clr | 124.998 ns | 2.4008 ns |  2.8579 ns | 125.333 ns |  18.47 |     0.45 |      - |       0 B |
        //                                     |      |         |            |           |            |            |        |          |        |           |
        //      DotNet_RaiseSync_SyncHandler_0 | Core |    Core |   1.192 ns | 0.0663 ns |  0.1954 ns |   1.156 ns |   0.16 |     0.03 |      - |       0 B |
        //      DotNet_RaiseSync_SyncHandler_1 | Core |    Core |   7.607 ns | 0.4404 ns |  0.4325 ns |   7.445 ns |   1.00 |     0.00 | 0.0076 |      24 B |
        //      DotNet_RaiseSync_SyncHandler_3 | Core |    Core |  17.818 ns | 0.5724 ns |  0.5354 ns |  17.701 ns |   2.35 |     0.14 | 0.0076 |      24 B |
        //      DotNet_RaiseSync_SyncHandler_9 | Core |    Core |  40.972 ns | 0.8560 ns |  2.0673 ns |  40.427 ns |   5.40 |     0.39 | 0.0076 |      24 B |
        //     Amarok_InvokeSync_SyncHandler_0 | Core |    Core |  20.088 ns | 0.4307 ns |  0.8298 ns |  19.847 ns |   2.65 |     0.17 |      - |       0 B |
        //     Amarok_InvokeSync_SyncHandler_1 | Core |    Core |  30.858 ns | 0.6576 ns |  0.9843 ns |  31.036 ns |   4.07 |     0.24 |      - |       0 B |
        //     Amarok_InvokeSync_SyncHandler_3 | Core |    Core |  40.385 ns | 1.0559 ns |  1.0843 ns |  40.113 ns |   5.32 |     0.31 |      - |       0 B |
        //     Amarok_InvokeSync_SyncHandler_9 | Core |    Core |  64.362 ns | 1.3204 ns |  1.2351 ns |  64.688 ns |   8.48 |     0.46 |      - |       0 B |
        //    Amarok_InvokeAsync_SyncHandler_0 | Core |    Core |  46.022 ns | 0.8198 ns |  1.2519 ns |  46.214 ns |   6.07 |     0.35 |      - |       0 B |
        //    Amarok_InvokeAsync_SyncHandler_1 | Core |    Core |  62.260 ns | 1.2659 ns |  1.9709 ns |  61.935 ns |   8.21 |     0.49 |      - |       0 B |
        //    Amarok_InvokeAsync_SyncHandler_3 | Core |    Core |  85.385 ns | 2.3386 ns |  2.5994 ns |  84.594 ns |  11.26 |     0.66 |      - |       0 B |
        //    Amarok_InvokeAsync_SyncHandler_9 | Core |    Core | 180.270 ns | 3.5585 ns |  6.3253 ns | 179.774 ns |  23.76 |     1.47 |      - |       0 B |
        // Amarok_InvokeSync_WeakSyncHandler_0 | Core |    Core |  21.953 ns | 0.4531 ns |  0.4653 ns |  21.984 ns |   2.89 |     0.16 |      - |       0 B |
        // Amarok_InvokeSync_WeakSyncHandler_1 | Core |    Core |  36.503 ns | 0.7521 ns |  1.3172 ns |  36.455 ns |   4.81 |     0.30 |      - |       0 B |
        // Amarok_InvokeSync_WeakSyncHandler_3 | Core |    Core |  56.001 ns | 1.2472 ns |  1.5773 ns |  55.486 ns |   7.38 |     0.43 |      - |       0 B |
        // Amarok_InvokeSync_WeakSyncHandler_9 | Core |    Core | 102.407 ns | 2.0547 ns |  2.8124 ns | 101.716 ns |  13.50 |     0.78 |      - |       0 B |
    }
}
