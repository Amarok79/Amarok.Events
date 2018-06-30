/* Copyright(c) 2018, Olaf Kober
 * Licensed under GNU Lesser General Public License v3.0 (LPGL-3.0).
 * https://github.com/Amarok79/Events
 */

using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;


namespace Amarok.Events
{
	[ClrJob, /*CoreJob, */MemoryDiagnoser]
	public class Benchmarks
	{
		public class DotNet
		{
			public sealed class FooEventArgs : EventArgs
			{
				public String Payload { get; }

				public FooEventArgs(String payload)
				{
					this.Payload = payload;
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

			private void HandleFooChanged(Object sender, FooEventArgs e)
			{
			}


			public FooServiceImpl ServiceHandlerCount0;
			public FooServiceImpl ServiceHandlerCount1;
			public FooServiceImpl ServiceHandlerCount3;
			public FooServiceImpl ServiceHandlerCount9;

			public DotNet()
			{
				ServiceHandlerCount0 = new FooServiceImpl();

				ServiceHandlerCount1 = new FooServiceImpl();
				ServiceHandlerCount1.FooChanged += HandleFooChanged;

				ServiceHandlerCount3 = new FooServiceImpl();
				ServiceHandlerCount3.FooChanged += HandleFooChanged;
				ServiceHandlerCount3.FooChanged += HandleFooChanged;
				ServiceHandlerCount3.FooChanged += HandleFooChanged;

				ServiceHandlerCount9 = new FooServiceImpl();
				ServiceHandlerCount9.FooChanged += HandleFooChanged;
				ServiceHandlerCount9.FooChanged += HandleFooChanged;
				ServiceHandlerCount9.FooChanged += HandleFooChanged;
				ServiceHandlerCount9.FooChanged += HandleFooChanged;
				ServiceHandlerCount9.FooChanged += HandleFooChanged;
				ServiceHandlerCount9.FooChanged += HandleFooChanged;
				ServiceHandlerCount9.FooChanged += HandleFooChanged;
				ServiceHandlerCount9.FooChanged += HandleFooChanged;
				ServiceHandlerCount9.FooChanged += HandleFooChanged;
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

			private void HandleFooChanged(String args)
			{
			}

			private Task HandleFooChangedAsync(String args)
			{
				return Task.CompletedTask;
			}


			public FooServiceImpl ServiceHandlerCount0;
			public FooServiceImpl ServiceHandlerCount1;
			public FooServiceImpl ServiceHandlerCount3;
			public FooServiceImpl ServiceHandlerCount9;

			public FooServiceImpl ServiceAsyncHandlerCount0;
			public FooServiceImpl ServiceAsyncHandlerCount1;
			public FooServiceImpl ServiceAsyncHandlerCount3;
			public FooServiceImpl ServiceAsyncHandlerCount9;

			public FooServiceImpl ServiceWeakHandlerCount0;
			public FooServiceImpl ServiceWeakHandlerCount1;
			public FooServiceImpl ServiceWeakHandlerCount3;
			public FooServiceImpl ServiceWeakHandlerCount9;


			public Amarok()
			{
				// sync handler
				ServiceHandlerCount0 = new FooServiceImpl();

				ServiceHandlerCount1 = new FooServiceImpl();
				ServiceHandlerCount1.FooChanged.Subscribe(HandleFooChanged);

				ServiceHandlerCount3 = new FooServiceImpl();
				ServiceHandlerCount3.FooChanged.Subscribe(HandleFooChanged);
				ServiceHandlerCount3.FooChanged.Subscribe(HandleFooChanged);
				ServiceHandlerCount3.FooChanged.Subscribe(HandleFooChanged);

				ServiceHandlerCount9 = new FooServiceImpl();
				ServiceHandlerCount9.FooChanged.Subscribe(HandleFooChanged);
				ServiceHandlerCount9.FooChanged.Subscribe(HandleFooChanged);
				ServiceHandlerCount9.FooChanged.Subscribe(HandleFooChanged);
				ServiceHandlerCount9.FooChanged.Subscribe(HandleFooChanged);
				ServiceHandlerCount9.FooChanged.Subscribe(HandleFooChanged);
				ServiceHandlerCount9.FooChanged.Subscribe(HandleFooChanged);
				ServiceHandlerCount9.FooChanged.Subscribe(HandleFooChanged);
				ServiceHandlerCount9.FooChanged.Subscribe(HandleFooChanged);
				ServiceHandlerCount9.FooChanged.Subscribe(HandleFooChanged);

				// async handler
				ServiceAsyncHandlerCount0 = new FooServiceImpl();

				ServiceAsyncHandlerCount1 = new FooServiceImpl();
				ServiceAsyncHandlerCount1.FooChanged.Subscribe(HandleFooChangedAsync);

				ServiceAsyncHandlerCount3 = new FooServiceImpl();
				ServiceAsyncHandlerCount3.FooChanged.Subscribe(HandleFooChangedAsync);
				ServiceAsyncHandlerCount3.FooChanged.Subscribe(HandleFooChangedAsync);
				ServiceAsyncHandlerCount3.FooChanged.Subscribe(HandleFooChangedAsync);

				ServiceAsyncHandlerCount9 = new FooServiceImpl();
				ServiceAsyncHandlerCount9.FooChanged.Subscribe(HandleFooChangedAsync);
				ServiceAsyncHandlerCount9.FooChanged.Subscribe(HandleFooChangedAsync);
				ServiceAsyncHandlerCount9.FooChanged.Subscribe(HandleFooChangedAsync);
				ServiceAsyncHandlerCount9.FooChanged.Subscribe(HandleFooChangedAsync);
				ServiceAsyncHandlerCount9.FooChanged.Subscribe(HandleFooChangedAsync);
				ServiceAsyncHandlerCount9.FooChanged.Subscribe(HandleFooChangedAsync);
				ServiceAsyncHandlerCount9.FooChanged.Subscribe(HandleFooChangedAsync);
				ServiceAsyncHandlerCount9.FooChanged.Subscribe(HandleFooChangedAsync);
				ServiceAsyncHandlerCount9.FooChanged.Subscribe(HandleFooChangedAsync);

				// weak sync handler
				ServiceWeakHandlerCount0 = new FooServiceImpl();

				ServiceWeakHandlerCount1 = new FooServiceImpl();
				ServiceWeakHandlerCount1.FooChanged.SubscribeWeak(HandleFooChanged);

				ServiceWeakHandlerCount3 = new FooServiceImpl();
				ServiceWeakHandlerCount3.FooChanged.SubscribeWeak(HandleFooChanged);
				ServiceWeakHandlerCount3.FooChanged.SubscribeWeak(HandleFooChanged);
				ServiceWeakHandlerCount3.FooChanged.SubscribeWeak(HandleFooChanged);

				ServiceWeakHandlerCount9 = new FooServiceImpl();
				ServiceWeakHandlerCount9.FooChanged.SubscribeWeak(HandleFooChanged);
				ServiceWeakHandlerCount9.FooChanged.SubscribeWeak(HandleFooChanged);
				ServiceWeakHandlerCount9.FooChanged.SubscribeWeak(HandleFooChanged);
				ServiceWeakHandlerCount9.FooChanged.SubscribeWeak(HandleFooChanged);
				ServiceWeakHandlerCount9.FooChanged.SubscribeWeak(HandleFooChanged);
				ServiceWeakHandlerCount9.FooChanged.SubscribeWeak(HandleFooChanged);
				ServiceWeakHandlerCount9.FooChanged.SubscribeWeak(HandleFooChanged);
				ServiceWeakHandlerCount9.FooChanged.SubscribeWeak(HandleFooChanged);
				ServiceWeakHandlerCount9.FooChanged.SubscribeWeak(HandleFooChanged);
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


		// ----- sync handler -----


		[Benchmark]
		public void Amarok_InvokeSync_SyncHandler_0()
		{
			mAmarok.ServiceHandlerCount0.Do();
		}

		[Benchmark]
		public void Amarok_InvokeSync_SyncHandler_1()
		{
			mAmarok.ServiceHandlerCount1.Do();
		}

		[Benchmark]
		public void Amarok_InvokeAsync_SyncHandler_0()
		{
			mAmarok.ServiceHandlerCount0.DoAsync()
				.GetAwaiter().GetResult();
		}

		[Benchmark]
		public void Amarok_InvokeAsync_SyncHandler_1()
		{
			mAmarok.ServiceHandlerCount1.DoAsync()
				.GetAwaiter().GetResult();
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



		//                              Method |       Mean |     Error |    StdDev | Scaled | ScaledSD |  Gen 0 | Allocated |
		//------------------------------------ |-----------:|----------:|----------:|-------:|---------:|-------:|----------:|
		//      DotNet_RaiseSync_SyncHandler_0 |  0.9514 ns | 0.0581 ns | 0.0485 ns |   0.14 |     0.01 |      - |       0 B |
		//      DotNet_RaiseSync_SyncHandler_1 |  6.8132 ns | 0.1753 ns | 0.2781 ns |   1.00 |     0.00 | 0.0076 |      24 B |
		//     Amarok_InvokeSync_SyncHandler_0 | 26.6617 ns | 0.5722 ns | 0.9717 ns |   3.92 |     0.21 |      - |       0 B |
		//     Amarok_InvokeSync_SyncHandler_1 | 34.9019 ns | 0.7049 ns | 0.7542 ns |   5.13 |     0.22 |      - |       0 B |
		//    Amarok_InvokeAsync_SyncHandler_0 | 49.3149 ns | 1.0117 ns | 1.5751 ns |   7.25 |     0.36 |      - |       0 B |
		//    Amarok_InvokeAsync_SyncHandler_1 | 66.4390 ns | 1.7724 ns | 2.0411 ns |   9.77 |     0.47 |      - |       0 B |
		// Amarok_InvokeSync_WeakSyncHandler_0 | 26.5616 ns | 0.8449 ns | 0.9730 ns |   3.90 |     0.20 |      - |       0 B |
		// Amarok_InvokeSync_WeakSyncHandler_1 | 27.9351 ns | 0.5958 ns | 0.9450 ns |   4.11 |     0.21 |      - |       0 B |

	}
}
