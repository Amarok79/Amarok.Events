// Copyright (c) 2024, Olaf Kober <olaf.kober@outlook.com>

using System;


namespace Amarok.Events;


internal static class ThrowHelper
{
    public static void ThrowArgumentNullException(String? name)
    {
        throw new ArgumentNullException(name);
    }
}
