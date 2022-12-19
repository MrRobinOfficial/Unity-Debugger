using System;

namespace uDebugger.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class DebugMethodAttribute : UnityEngine.Scripting.PreserveAttribute
    {
        public readonly string alias = string.Empty;
        public readonly string altAlias = string.Empty;
        public readonly string description = string.Empty;

        public DebugMethodAttribute([System.Runtime.CompilerServices.CallerMemberName] string alias = "",
                                    string altAlias = "",
                                    string description = "")
        {
            this.alias= alias;
            this.altAlias= altAlias;
            this.description= description;
        }
    } 
}
