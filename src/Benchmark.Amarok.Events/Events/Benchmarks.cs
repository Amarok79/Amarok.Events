// Copyright (c) 2022, Olaf Kober <olaf.kober@outlook.com>

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CA2012  // Use ValueTasks correctly

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;


namespace Amarok.Events;


[SimpleJob(RuntimeMoniker.Net471)]
[SimpleJob(RuntimeMoniker.Net48)]
[SimpleJob(RuntimeMoniker.NetCoreApp31)]
[SimpleJob(RuntimeMoniker.Net50)]
[SimpleJob(RuntimeMoniker.Net60)]
[MemoryDiagnoser]
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
            private readonly EventSource<String> mFooChanged = new();

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

        private readonly List<IDisposable> mSubscriptions = new();


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

            mSubscriptions.Add(
                ServiceWeakHandlerCount1.FooChanged.SubscribeWeak(_HandleFooChanged)
            );

            ServiceWeakHandlerCount3 = new FooServiceImpl();

            mSubscriptions.Add(
                ServiceWeakHandlerCount3.FooChanged.SubscribeWeak(_HandleFooChanged)
            );

            mSubscriptions.Add(
                ServiceWeakHandlerCount3.FooChanged.SubscribeWeak(_HandleFooChanged)
            );

            mSubscriptions.Add(
                ServiceWeakHandlerCount3.FooChanged.SubscribeWeak(_HandleFooChanged)
            );

            ServiceWeakHandlerCount9 = new FooServiceImpl();

            mSubscriptions.Add(
                ServiceWeakHandlerCount9.FooChanged.SubscribeWeak(_HandleFooChanged)
            );

            mSubscriptions.Add(
                ServiceWeakHandlerCount9.FooChanged.SubscribeWeak(_HandleFooChanged)
            );

            mSubscriptions.Add(
                ServiceWeakHandlerCount9.FooChanged.SubscribeWeak(_HandleFooChanged)
            );

            mSubscriptions.Add(
                ServiceWeakHandlerCount9.FooChanged.SubscribeWeak(_HandleFooChanged)
            );

            mSubscriptions.Add(
                ServiceWeakHandlerCount9.FooChanged.SubscribeWeak(_HandleFooChanged)
            );

            mSubscriptions.Add(
                ServiceWeakHandlerCount9.FooChanged.SubscribeWeak(_HandleFooChanged)
            );

            mSubscriptions.Add(
                ServiceWeakHandlerCount9.FooChanged.SubscribeWeak(_HandleFooChanged)
            );

            mSubscriptions.Add(
                ServiceWeakHandlerCount9.FooChanged.SubscribeWeak(_HandleFooChanged)
            );

            mSubscriptions.Add(
                ServiceWeakHandlerCount9.FooChanged.SubscribeWeak(_HandleFooChanged)
            );
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
        mAmarok.ServiceSyncHandlerCount0.DoAsync()
           .GetAwaiter()
           .GetResult();
    }

    [Benchmark]
    public void Amarok_InvokeAsync_SyncHandler_1()
    {
        mAmarok.ServiceSyncHandlerCount1.DoAsync()
           .GetAwaiter()
           .GetResult();
    }

    [Benchmark]
    public void Amarok_InvokeAsync_SyncHandler_3()
    {
        mAmarok.ServiceSyncHandlerCount3.DoAsync()
           .GetAwaiter()
           .GetResult();
    }

    [Benchmark]
    public void Amarok_InvokeAsync_SyncHandler_9()
    {
        mAmarok.ServiceSyncHandlerCount9.DoAsync()
           .GetAwaiter()
           .GetResult();
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
}

//BenchmarkDotNet=v0.13.1, OS=Windows 10.0.22000
//Intel Core i7-10875H CPU 2.30GHz, 1 CPU, 16 logical and 8 physical cores
//.NET SDK=6.0.101
//  [Host]             : .NET Core 3.1.22 (CoreCLR 4.700.21.56803, CoreFX 4.700.21.57101), X64 RyuJIT
//  .NET 5.0           : .NET 5.0.13 (5.0.1321.56516), X64 RyuJIT
//  .NET 6.0           : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
//  .NET Core 3.1      : .NET Core 3.1.22 (CoreCLR 4.700.21.56803, CoreFX 4.700.21.57101), X64 RyuJIT
//  .NET Framework 4.8 : .NET Framework 4.8 (4.8.4420.0), X64 RyuJIT


//|                              Method |                  Job |              Runtime |        Mean |     Error |    StdDev |      Median | Ratio | RatioSD |  Gen 0 | Allocated |
//|------------------------------------ |--------------------- |--------------------- |------------:|----------:|----------:|------------:|------:|--------:|-------:|----------:|
//|      DotNet_RaiseSync_SyncHandler_0 |             .NET 5.0 |             .NET 5.0 |   0.5805 ns | 0.0365 ns | 0.0657 ns |   0.6109 ns |  0.10 |    0.01 |      - |         - |
//|      DotNet_RaiseSync_SyncHandler_1 |             .NET 5.0 |             .NET 5.0 |   5.9689 ns | 0.1408 ns | 0.2149 ns |   6.0762 ns |  1.00 |    0.00 | 0.0029 |      24 B |
//|      DotNet_RaiseSync_SyncHandler_3 |             .NET 5.0 |             .NET 5.0 |  14.8251 ns | 0.3148 ns | 0.4902 ns |  15.0715 ns |  2.49 |    0.12 | 0.0029 |      24 B |
//|      DotNet_RaiseSync_SyncHandler_9 |             .NET 5.0 |             .NET 5.0 |  29.1730 ns | 0.6055 ns | 1.1665 ns |  29.9215 ns |  4.90 |    0.29 | 0.0029 |      24 B |
//|     Amarok_InvokeSync_SyncHandler_0 |             .NET 5.0 |             .NET 5.0 |  11.3021 ns | 0.2046 ns | 0.1914 ns |  11.3751 ns |  1.89 |    0.09 |      - |         - |
//|     Amarok_InvokeSync_SyncHandler_1 |             .NET 5.0 |             .NET 5.0 |  20.7443 ns | 0.7483 ns | 1.9844 ns |  20.8400 ns |  3.58 |    0.47 |      - |         - |
//|     Amarok_InvokeSync_SyncHandler_3 |             .NET 5.0 |             .NET 5.0 |  22.6107 ns | 0.4634 ns | 0.5516 ns |  22.9351 ns |  3.81 |    0.12 |      - |         - |
//|     Amarok_InvokeSync_SyncHandler_9 |             .NET 5.0 |             .NET 5.0 |  37.5741 ns | 0.7644 ns | 1.1673 ns |  37.8835 ns |  6.30 |    0.25 |      - |         - |
//|    Amarok_InvokeAsync_SyncHandler_0 |             .NET 5.0 |             .NET 5.0 |  20.4555 ns | 0.3425 ns | 0.3204 ns |  20.4379 ns |  3.41 |    0.14 |      - |         - |
//|    Amarok_InvokeAsync_SyncHandler_1 |             .NET 5.0 |             .NET 5.0 |  32.7925 ns | 0.4566 ns | 0.4271 ns |  32.8775 ns |  5.47 |    0.16 |      - |         - |
//|    Amarok_InvokeAsync_SyncHandler_3 |             .NET 5.0 |             .NET 5.0 |  56.6026 ns | 0.5388 ns | 0.4499 ns |  56.6902 ns |  9.39 |    0.31 |      - |         - |
//|    Amarok_InvokeAsync_SyncHandler_9 |             .NET 5.0 |             .NET 5.0 | 137.2136 ns | 2.1744 ns | 2.0340 ns | 138.0566 ns | 22.89 |    0.97 |      - |         - |
//| Amarok_InvokeSync_WeakSyncHandler_0 |             .NET 5.0 |             .NET 5.0 |  11.1051 ns | 0.0698 ns | 0.0653 ns |  11.1181 ns |  1.85 |    0.07 |      - |         - |
//| Amarok_InvokeSync_WeakSyncHandler_1 |             .NET 5.0 |             .NET 5.0 |  20.1987 ns | 0.1052 ns | 0.0984 ns |  20.2128 ns |  3.37 |    0.13 |      - |         - |
//| Amarok_InvokeSync_WeakSyncHandler_3 |             .NET 5.0 |             .NET 5.0 |  30.1409 ns | 0.2534 ns | 0.2371 ns |  30.0739 ns |  5.03 |    0.18 |      - |         - |
//| Amarok_InvokeSync_WeakSyncHandler_9 |             .NET 5.0 |             .NET 5.0 |  58.4995 ns | 1.1708 ns | 1.0952 ns |  59.0376 ns |  9.76 |    0.38 |      - |         - |
//|                                     |                      |                      |             |           |           |             |       |         |        |           |
//|      DotNet_RaiseSync_SyncHandler_0 |             .NET 6.0 |             .NET 6.0 |   0.8701 ns | 0.0290 ns | 0.0271 ns |   0.8850 ns |  0.15 |    0.01 |      - |         - |
//|      DotNet_RaiseSync_SyncHandler_1 |             .NET 6.0 |             .NET 6.0 |   5.9545 ns | 0.1359 ns | 0.1566 ns |   6.0161 ns |  1.00 |    0.00 | 0.0029 |      24 B |
//|      DotNet_RaiseSync_SyncHandler_3 |             .NET 6.0 |             .NET 6.0 |  15.3852 ns | 0.3246 ns | 0.3864 ns |  15.5578 ns |  2.58 |    0.08 | 0.0029 |      24 B |
//|      DotNet_RaiseSync_SyncHandler_9 |             .NET 6.0 |             .NET 6.0 |  30.8361 ns | 0.6325 ns | 0.6212 ns |  31.1148 ns |  5.20 |    0.19 | 0.0029 |      24 B |
//|     Amarok_InvokeSync_SyncHandler_0 |             .NET 6.0 |             .NET 6.0 |   9.3046 ns | 0.0410 ns | 0.0383 ns |   9.3112 ns |  1.57 |    0.05 |      - |         - |
//|     Amarok_InvokeSync_SyncHandler_1 |             .NET 6.0 |             .NET 6.0 |  13.2512 ns | 0.0877 ns | 0.0820 ns |  13.2328 ns |  2.24 |    0.06 |      - |         - |
//|     Amarok_InvokeSync_SyncHandler_3 |             .NET 6.0 |             .NET 6.0 |  16.8545 ns | 0.1075 ns | 0.1006 ns |  16.8561 ns |  2.85 |    0.08 |      - |         - |
//|     Amarok_InvokeSync_SyncHandler_9 |             .NET 6.0 |             .NET 6.0 |  27.8527 ns | 0.1341 ns | 0.1188 ns |  27.8248 ns |  4.71 |    0.15 |      - |         - |
//|    Amarok_InvokeAsync_SyncHandler_0 |             .NET 6.0 |             .NET 6.0 |  20.9351 ns | 0.1075 ns | 0.1006 ns |  20.9140 ns |  3.53 |    0.12 |      - |         - |
//|    Amarok_InvokeAsync_SyncHandler_1 |             .NET 6.0 |             .NET 6.0 |  31.8995 ns | 0.1572 ns | 0.1393 ns |  31.8977 ns |  5.39 |    0.16 |      - |         - |
//|    Amarok_InvokeAsync_SyncHandler_3 |             .NET 6.0 |             .NET 6.0 |  53.0268 ns | 0.3997 ns | 0.3739 ns |  53.1587 ns |  8.95 |    0.26 |      - |         - |
//|    Amarok_InvokeAsync_SyncHandler_9 |             .NET 6.0 |             .NET 6.0 | 125.9396 ns | 0.6973 ns | 0.6522 ns | 126.2177 ns | 21.26 |    0.63 |      - |         - |
//| Amarok_InvokeSync_WeakSyncHandler_0 |             .NET 6.0 |             .NET 6.0 |   9.3111 ns | 0.0493 ns | 0.0412 ns |   9.3214 ns |  1.57 |    0.05 |      - |         - |
//| Amarok_InvokeSync_WeakSyncHandler_1 |             .NET 6.0 |             .NET 6.0 |  19.3097 ns | 0.6462 ns | 1.7472 ns |  20.0657 ns |  3.41 |    0.25 |      - |         - |
//| Amarok_InvokeSync_WeakSyncHandler_3 |             .NET 6.0 |             .NET 6.0 |  25.6130 ns | 0.2427 ns | 0.2152 ns |  25.6713 ns |  4.33 |    0.13 |      - |         - |
//| Amarok_InvokeSync_WeakSyncHandler_9 |             .NET 6.0 |             .NET 6.0 |  53.9319 ns | 0.8024 ns | 0.7506 ns |  54.3100 ns |  9.10 |    0.30 |      - |         - |
//|                                     |                      |                      |             |           |           |             |       |         |        |           |
//|      DotNet_RaiseSync_SyncHandler_0 |        .NET Core 3.1 |        .NET Core 3.1 |   0.9289 ns | 0.0073 ns | 0.0065 ns |   0.9298 ns |  0.16 |    0.01 |      - |         - |
//|      DotNet_RaiseSync_SyncHandler_1 |        .NET Core 3.1 |        .NET Core 3.1 |   5.6879 ns | 0.1380 ns | 0.1694 ns |   5.7416 ns |  1.00 |    0.00 | 0.0029 |      24 B |
//|      DotNet_RaiseSync_SyncHandler_3 |        .NET Core 3.1 |        .NET Core 3.1 |  15.0819 ns | 0.2641 ns | 0.2470 ns |  15.1296 ns |  2.65 |    0.10 | 0.0029 |      24 B |
//|      DotNet_RaiseSync_SyncHandler_9 |        .NET Core 3.1 |        .NET Core 3.1 |  32.5337 ns | 0.5088 ns | 0.4759 ns |  32.7034 ns |  5.71 |    0.17 | 0.0029 |      24 B |
//|     Amarok_InvokeSync_SyncHandler_0 |        .NET Core 3.1 |        .NET Core 3.1 |  10.9485 ns | 0.0624 ns | 0.0553 ns |  10.9489 ns |  1.92 |    0.07 |      - |         - |
//|     Amarok_InvokeSync_SyncHandler_1 |        .NET Core 3.1 |        .NET Core 3.1 |  15.9715 ns | 0.0950 ns | 0.0842 ns |  15.9580 ns |  2.81 |    0.10 |      - |         - |
//|     Amarok_InvokeSync_SyncHandler_3 |        .NET Core 3.1 |        .NET Core 3.1 |  21.2405 ns | 0.4419 ns | 0.5088 ns |  20.9567 ns |  3.74 |    0.17 |      - |         - |
//|     Amarok_InvokeSync_SyncHandler_9 |        .NET Core 3.1 |        .NET Core 3.1 |  35.9346 ns | 0.0898 ns | 0.0750 ns |  35.9491 ns |  6.32 |    0.23 |      - |         - |
//|    Amarok_InvokeAsync_SyncHandler_0 |        .NET Core 3.1 |        .NET Core 3.1 |  24.4840 ns | 0.0915 ns | 0.0811 ns |  24.4726 ns |  4.30 |    0.15 |      - |         - |
//|    Amarok_InvokeAsync_SyncHandler_1 |        .NET Core 3.1 |        .NET Core 3.1 |  35.0774 ns | 0.6370 ns | 0.5959 ns |  35.3832 ns |  6.16 |    0.28 |      - |         - |
//|    Amarok_InvokeAsync_SyncHandler_3 |        .NET Core 3.1 |        .NET Core 3.1 |  55.4701 ns | 1.0123 ns | 0.9469 ns |  55.9555 ns |  9.74 |    0.38 |      - |         - |
//|    Amarok_InvokeAsync_SyncHandler_9 |        .NET Core 3.1 |        .NET Core 3.1 | 118.4249 ns | 2.3811 ns | 2.2273 ns | 119.4892 ns | 20.80 |    0.92 |      - |         - |
//| Amarok_InvokeSync_WeakSyncHandler_0 |        .NET Core 3.1 |        .NET Core 3.1 |  10.9761 ns | 0.0565 ns | 0.0529 ns |  10.9733 ns |  1.93 |    0.07 |      - |         - |
//| Amarok_InvokeSync_WeakSyncHandler_1 |        .NET Core 3.1 |        .NET Core 3.1 |  19.8956 ns | 0.1192 ns | 0.1115 ns |  19.9224 ns |  3.49 |    0.11 |      - |         - |
//| Amarok_InvokeSync_WeakSyncHandler_3 |        .NET Core 3.1 |        .NET Core 3.1 |  30.9489 ns | 0.2153 ns | 0.2014 ns |  30.9931 ns |  5.43 |    0.20 |      - |         - |
//| Amarok_InvokeSync_WeakSyncHandler_9 |        .NET Core 3.1 |        .NET Core 3.1 |  60.5469 ns | 0.6724 ns | 0.6290 ns |  60.5451 ns | 10.63 |    0.39 |      - |         - |
//|                                     |                      |                      |             |           |           |             |       |         |        |           |
//|      DotNet_RaiseSync_SyncHandler_0 | .NET Framework 4.7.1 | .NET Framework 4.7.1 |          NA |        NA |        NA |          NA |     ? |       ? |      - |         - |
//|      DotNet_RaiseSync_SyncHandler_1 | .NET Framework 4.7.1 | .NET Framework 4.7.1 |          NA |        NA |        NA |          NA |     ? |       ? |      - |         - |
//|      DotNet_RaiseSync_SyncHandler_3 | .NET Framework 4.7.1 | .NET Framework 4.7.1 |          NA |        NA |        NA |          NA |     ? |       ? |      - |         - |
//|      DotNet_RaiseSync_SyncHandler_9 | .NET Framework 4.7.1 | .NET Framework 4.7.1 |          NA |        NA |        NA |          NA |     ? |       ? |      - |         - |
//|     Amarok_InvokeSync_SyncHandler_0 | .NET Framework 4.7.1 | .NET Framework 4.7.1 |          NA |        NA |        NA |          NA |     ? |       ? |      - |         - |
//|     Amarok_InvokeSync_SyncHandler_1 | .NET Framework 4.7.1 | .NET Framework 4.7.1 |          NA |        NA |        NA |          NA |     ? |       ? |      - |         - |
//|     Amarok_InvokeSync_SyncHandler_3 | .NET Framework 4.7.1 | .NET Framework 4.7.1 |          NA |        NA |        NA |          NA |     ? |       ? |      - |         - |
//|     Amarok_InvokeSync_SyncHandler_9 | .NET Framework 4.7.1 | .NET Framework 4.7.1 |          NA |        NA |        NA |          NA |     ? |       ? |      - |         - |
//|    Amarok_InvokeAsync_SyncHandler_0 | .NET Framework 4.7.1 | .NET Framework 4.7.1 |          NA |        NA |        NA |          NA |     ? |       ? |      - |         - |
//|    Amarok_InvokeAsync_SyncHandler_1 | .NET Framework 4.7.1 | .NET Framework 4.7.1 |          NA |        NA |        NA |          NA |     ? |       ? |      - |         - |
//|    Amarok_InvokeAsync_SyncHandler_3 | .NET Framework 4.7.1 | .NET Framework 4.7.1 |          NA |        NA |        NA |          NA |     ? |       ? |      - |         - |
//|    Amarok_InvokeAsync_SyncHandler_9 | .NET Framework 4.7.1 | .NET Framework 4.7.1 |          NA |        NA |        NA |          NA |     ? |       ? |      - |         - |
//| Amarok_InvokeSync_WeakSyncHandler_0 | .NET Framework 4.7.1 | .NET Framework 4.7.1 |          NA |        NA |        NA |          NA |     ? |       ? |      - |         - |
//| Amarok_InvokeSync_WeakSyncHandler_1 | .NET Framework 4.7.1 | .NET Framework 4.7.1 |          NA |        NA |        NA |          NA |     ? |       ? |      - |         - |
//| Amarok_InvokeSync_WeakSyncHandler_3 | .NET Framework 4.7.1 | .NET Framework 4.7.1 |          NA |        NA |        NA |          NA |     ? |       ? |      - |         - |
//| Amarok_InvokeSync_WeakSyncHandler_9 | .NET Framework 4.7.1 | .NET Framework 4.7.1 |          NA |        NA |        NA |          NA |     ? |       ? |      - |         - |
//|                                     |                      |                      |             |           |           |             |       |         |        |           |
//|      DotNet_RaiseSync_SyncHandler_0 |   .NET Framework 4.8 |   .NET Framework 4.8 |   0.9130 ns | 0.0031 ns | 0.0029 ns |   0.9134 ns |  0.15 |    0.00 |      - |         - |
//|      DotNet_RaiseSync_SyncHandler_1 |   .NET Framework 4.8 |   .NET Framework 4.8 |   5.9532 ns | 0.0845 ns | 0.0790 ns |   5.9478 ns |  1.00 |    0.00 | 0.0038 |      24 B |
//|      DotNet_RaiseSync_SyncHandler_3 |   .NET Framework 4.8 |   .NET Framework 4.8 |  12.1494 ns | 0.0868 ns | 0.0812 ns |  12.1754 ns |  2.04 |    0.03 | 0.0038 |      24 B |
//|      DotNet_RaiseSync_SyncHandler_9 |   .NET Framework 4.8 |   .NET Framework 4.8 |  19.5324 ns | 0.3949 ns | 0.4226 ns |  19.7139 ns |  3.29 |    0.07 | 0.0038 |      24 B |
//|     Amarok_InvokeSync_SyncHandler_0 |   .NET Framework 4.8 |   .NET Framework 4.8 |  17.8298 ns | 0.3734 ns | 0.3492 ns |  17.9679 ns |  3.00 |    0.07 |      - |         - |
//|     Amarok_InvokeSync_SyncHandler_1 |   .NET Framework 4.8 |   .NET Framework 4.8 |  23.0582 ns | 0.2831 ns | 0.2510 ns |  23.1678 ns |  3.87 |    0.08 |      - |         - |
//|     Amarok_InvokeSync_SyncHandler_3 |   .NET Framework 4.8 |   .NET Framework 4.8 |  29.6417 ns | 0.3230 ns | 0.3022 ns |  29.7629 ns |  4.98 |    0.08 |      - |         - |
//|     Amarok_InvokeSync_SyncHandler_9 |   .NET Framework 4.8 |   .NET Framework 4.8 |  44.6920 ns | 0.6069 ns | 0.5677 ns |  44.7503 ns |  7.51 |    0.15 |      - |         - |
//|    Amarok_InvokeAsync_SyncHandler_0 |   .NET Framework 4.8 |   .NET Framework 4.8 |  32.0351 ns | 0.3977 ns | 0.3720 ns |  32.1181 ns |  5.38 |    0.09 |      - |         - |
//|    Amarok_InvokeAsync_SyncHandler_1 |   .NET Framework 4.8 |   .NET Framework 4.8 |  49.2810 ns | 0.6584 ns | 0.6159 ns |  49.6994 ns |  8.28 |    0.15 |      - |         - |
//|    Amarok_InvokeAsync_SyncHandler_3 |   .NET Framework 4.8 |   .NET Framework 4.8 |  74.5677 ns | 0.5560 ns | 0.5201 ns |  74.7829 ns | 12.53 |    0.21 |      - |         - |
//|    Amarok_InvokeAsync_SyncHandler_9 |   .NET Framework 4.8 |   .NET Framework 4.8 | 162.4250 ns | 0.7876 ns | 0.6982 ns | 162.5351 ns | 27.29 |    0.40 |      - |         - |
//| Amarok_InvokeSync_WeakSyncHandler_0 |   .NET Framework 4.8 |   .NET Framework 4.8 |  17.8263 ns | 0.2674 ns | 0.2370 ns |  17.8783 ns |  2.99 |    0.05 |      - |         - |
//| Amarok_InvokeSync_WeakSyncHandler_1 |   .NET Framework 4.8 |   .NET Framework 4.8 |  26.9525 ns | 0.2506 ns | 0.2345 ns |  27.0585 ns |  4.53 |    0.08 |      - |         - |
//| Amarok_InvokeSync_WeakSyncHandler_3 |   .NET Framework 4.8 |   .NET Framework 4.8 |  37.5088 ns | 0.6341 ns | 0.5932 ns |  37.8220 ns |  6.30 |    0.12 |      - |         - |
//| Amarok_InvokeSync_WeakSyncHandler_9 |   .NET Framework 4.8 |   .NET Framework 4.8 |  77.4570 ns | 0.7385 ns | 0.6546 ns |  77.6895 ns | 13.01 |    0.18 |      - |         - |
