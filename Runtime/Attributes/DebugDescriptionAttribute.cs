using System;

namespace Debugger
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class DebugDescriptionAttribute : Attribute
    {
        public readonly string description;

        public DebugDescriptionAttribute(string description) => this.description = description;
    }
}
