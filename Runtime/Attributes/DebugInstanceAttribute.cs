using System;
using UnityEngine;

namespace Debugger
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public sealed class DebugInstanceAttribute : Attribute { }
}