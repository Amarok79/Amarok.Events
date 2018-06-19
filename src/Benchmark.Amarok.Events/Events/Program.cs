/* Copyright(c) 2018, Olaf Kober
 * Licensed under GNU Lesser General Public License v3.0 (LPGL-3.0).
 * https://github.com/Amarok79/Events
 */

using BenchmarkDotNet.Running;

namespace Amarok.Events
{
	public static class Program
	{
		public static void Main()
		{
			BenchmarkRunner.Run<Benchmarks>();
		}
	}
}
