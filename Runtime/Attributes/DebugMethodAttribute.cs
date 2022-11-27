using System;
using UnityEngine.Scripting;

namespace Debugger
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class DebugMethodAttribute : PreserveAttribute
    {
        public readonly string alias = string.Empty;
        public readonly string description = string.Empty;

        public DebugMethodAttribute(string alias, string description = "")
        {
            this.alias = alias;
            this.description = description;
        }

        public DebugMethodAttribute([System.Runtime.CompilerServices.CallerMemberName] string alias = "") => this.alias = alias;
    }
}