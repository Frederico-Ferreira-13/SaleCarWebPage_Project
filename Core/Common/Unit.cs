using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Core.Common
{
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public readonly struct Unit
    {
        public static readonly Unit Value = default;
    }
}
