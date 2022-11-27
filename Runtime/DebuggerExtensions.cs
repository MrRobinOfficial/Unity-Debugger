using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Linq;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UDebug = UnityEngine.Debug;

namespace Debugger
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public static partial class DebuggerExtensions
    {
        public static IReadOnlyCollection<Command> Commands => _commands;
        public static IReadOnlyCollection<IFieldCommand> Fields => _fields;
        public static IReadOnlyCollection<IMethodCommand> Methods => _methods;

        private static List<Command> _commands = new();
        private static List<IFieldCommand> _fields = new();
        private static List<IMethodCommand> _methods = new();

#if UNITY_EDITOR
        static DebuggerExtensions() => CollectCommands();
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void CollectCommands()
        {
            _commands.Clear();
            _fields.Clear();
            _methods.Clear();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var methods = TypeCache.GetMethodsWithAttribute<DebugMethodAttribute>();
            var fields = TypeCache.GetFieldsWithAttribute<DebugFieldAttribute>();

            var list = new List<Command>(capacity: methods.Count + fields.Count);

            _methods = new(GetCommandsFromMethods(methods));
            _fields = new(GetCommandsFromFields(fields));

            list.AddRange(_fields.Cast<Command>());
            list.AddRange(_methods.Cast<Command>());
            _commands = new(list);

            stopwatch.Stop();

            UDebug.Log($"Commands Count: {_commands.Count}");
            UDebug.Log($"Methods Count: {_methods.Count}");
            UDebug.Log($"Fields Count: {_fields.Count}"); 
            UDebug.Log($"Commands Collected: {stopwatch.Elapsed.TotalMilliseconds} ms");
        }

        private static readonly IFieldCommand[] FieldArray_Empty = 
            Array.Empty<IFieldCommand>();

        private static readonly IMethodCommand[] MethodArray_Empty =
            Array.Empty<IMethodCommand>();

        private static IReadOnlyCollection<IFieldCommand> GetCommandsFromFields(TypeCache.FieldInfoCollection fields)
        {
            var cacheFields = new IFieldCommand[fields.Count];

            for (int i = 0; i < cacheFields.Length; i++)
            {
                var field = new Command.Builder()
                    .SetType(fields[i])
                    .BuildAsField();

                cacheFields[i] = field;
            }

            return cacheFields;
        }

        //private static bool HasInstance(Type type, out object instance)
        //{

        //}

        private static IReadOnlyCollection<IMethodCommand> GetCommandsFromMethods(TypeCache.MethodCollection methods)
        {
            var list = new List<IMethodCommand>();

            //var types = TypeCache.GetTypesWithAttribute<DebugInstanceAttribute>();
            object instance = null;

            foreach (var method in methods)
            {
                var type = method.DeclaringType;

                foreach (var attr in method.GetCustomAttributes<DebugMethodAttribute>())
                {
                    var descAttr = method.GetCustomAttribute<DebugDescriptionAttribute>();

                    var methodCommand = new Command.Builder(instance, alias: attr.alias)
                        .SetDescription(GetDescription())
                        .BuildAsMethod();

                    list.Add(methodCommand);

                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                    string GetDescription() => 
                    string.IsNullOrEmpty(attr.description) ? (descAttr != null ? descAttr.description : string.Empty) : attr.description;
                }
            }

            return list;
        }
    }
}