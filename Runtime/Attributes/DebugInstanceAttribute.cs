using System;

namespace uDebugger.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct,
        AllowMultiple = false)]
    public sealed class DebugInstanceAttribute : UnityEngine.Scripting.PreserveAttribute
    {
        public readonly string groupName = string.Empty;

        public DebugInstanceAttribute(string groupName = "") => 
            this.groupName = groupName;
    }
}