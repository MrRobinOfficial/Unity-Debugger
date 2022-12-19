using System;

namespace uDebugger.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class DebugFieldAttribute : UnityEngine.Scripting.PreserveAttribute
    {
        public readonly string description = string.Empty;

        public DebugFieldAttribute(string description = "") => 
            this.description = description;
    }
}
