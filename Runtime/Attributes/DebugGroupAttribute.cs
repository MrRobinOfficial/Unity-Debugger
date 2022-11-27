using System;

namespace Debugger
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class DebugGroupAttribute : Attribute
    {
        public readonly string groupName;

        public DebugGroupAttribute(string groupName) => this.groupName = groupName;
    }
}
