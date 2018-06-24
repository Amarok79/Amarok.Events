using System;
using System.Threading.Tasks;

namespace Amarok.Events
{
	internal static class TaskUtils
	{
		public static readonly Task<Boolean> TrueTask = Task.FromResult(true);
		public static readonly Task<Boolean> FalseTask = Task.FromResult(false);

		public static readonly ValueTask<Boolean> TrueValueTask = new ValueTask<Boolean>(TrueTask);
		public static readonly ValueTask<Boolean> FalseValueTask = new ValueTask<Boolean>(FalseTask);
	}
}
