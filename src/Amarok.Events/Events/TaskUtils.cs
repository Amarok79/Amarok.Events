// Copyright (c) 2024, Olaf Kober <olaf.kober@outlook.com>

using System;
using System.Threading.Tasks;


namespace Amarok.Events;


internal static class TaskUtils
{
    public static readonly Task<Boolean> TrueTask = Task.FromResult(true);
    public static readonly Task<Boolean> FalseTask = Task.FromResult(false);
}
