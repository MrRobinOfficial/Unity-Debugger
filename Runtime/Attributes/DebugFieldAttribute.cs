using System;

namespace Debugger
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public sealed class DebugFieldAttribute : Attribute
    {
        public readonly string alias = string.Empty;
        public readonly string description = string.Empty;
        public readonly bool isReadOnly = false;

        public DebugFieldAttribute() { }

        public DebugFieldAttribute(string alias, string description = "", bool isReadOnly = false)
        {
            this.alias = alias;
            this.description = description;
            this.isReadOnly = isReadOnly;
        }
    }
}