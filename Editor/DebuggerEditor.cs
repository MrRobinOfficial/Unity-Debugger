using UnityEngine;
using UnityEditor;
using uDebugger.Attributes;
using uDebugger.Commands;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Globalization;
using UnityEngine.Events;

namespace uDebugger.Editor
{
    public static partial class DebuggerEditor
    {
        private const int k_CommandCapacity = 4;

        public static event UnityAction OnForceRefresh;

        public struct DebuggerData
        {
            public IReadOnlyList<DebugFieldCommand> fields;
            public IReadOnlyList<DebugMethodCommand> methods;
        }

        private static List<DebugMethodCommand> m_MethodCommands = new(k_CommandCapacity);

        private static List<DebugFieldCommand> m_FieldCommands = new(k_CommandCapacity);

        public static IReadOnlyList<DebugMethodCommand> MethodCommands => m_MethodCommands;

        public static IReadOnlyList<DebugFieldCommand> FieldCommands => m_FieldCommands;

        private static readonly JsonSerializerSettings m_JsonSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            Converters =
            {
                new RuntimeFieldHandleJsonConverter(),
                new RuntimeMethodHandleJsonConverter(),
            },
        };

        private static System.Diagnostics.Stopwatch m_Stopwatch;

        [MenuItem(
            itemName: "Tools/uDebugger/Find All Commands ^#m",
            isValidateFunction: false,
            priority = 1000)]
        public static void FindAllCommands()
        {
            m_Stopwatch = System.Diagnostics.Stopwatch.StartNew();

            m_FieldCommands.Clear();
            m_MethodCommands.Clear();

            var fields = TypeCache
                .GetFieldsWithAttribute<DebugFieldAttribute>();

            // Debug.Log($"Found [{fields.Count}] fields");

            foreach (var field in fields)
            {
                if (!field.IsStatic)
                {
                    Debug.LogWarning($"Field called '{field.Name}' is not static! Therefore cannot parse into Debugger system! Please remove this 'DebugFieldAttribute' or add 'static' keyword.");
                    continue;
                }

                var command = new DebugFieldCommand
                {
                    name = field.Name,
                    fieldType = field.FieldType,
                    handle = field.FieldHandle,
                };

                m_FieldCommands.Add(command);
            }

            var methods = TypeCache
                .GetMethodsWithAttribute<DebugMethodAttribute>();

            // Debug.Log($"Found [{methods.Count}] methods");

            foreach (var method in methods)
            {
                if (!method.IsStatic)
                {
                    Debug.LogWarning($"Method called '{method.Name}' is not static! Therefore cannot parse into Debugger system! Please remove this 'DebugMethodAttribute' or add 'static' keyword.");
                    continue;
                }        

                var parameters = method.GetParameters();
                var parameterTypes = new Type[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                    parameterTypes[i] = parameters[i].ParameterType;    

                var command = new DebugMethodCommand
                {
                    name = method.Name,
                    handle = method.MethodHandle,
                    parameterTypes = parameterTypes,
                    argumentTypes = method.GetGenericArguments(),
                    returnType = method.ReturnType,
                };

                m_MethodCommands.Add(command);
            }

            var data = new DebuggerData
            {
                fields = m_FieldCommands,
                methods = m_MethodCommands,
            };

            var json = JsonConvert.SerializeObject(data, m_JsonSettings);

            m_Stopwatch.Stop();

            Debug.Log(json);

            var newData = JsonConvert.DeserializeObject<DebuggerData>(json, m_JsonSettings);
            var handle = newData.methods[newData.methods.Count - 1].handle;
            var newMethod = MethodInfo.GetMethodFromHandle(handle);
            newMethod.Invoke(null, new object[] { 69 });

            Debug.Log($"Execution Time: {m_Stopwatch.ElapsedMilliseconds} ms");
        }

        [MenuItem(
            itemName: "Tools/uDebugger/Force Refresh [Editor]",
            isValidateFunction: false,
            priority = 1500)]
        public static void ForceRefresh() => OnForceRefresh?.Invoke();

        private class RuntimeMethodHandleJsonConverter : JsonConverter<RuntimeMethodHandle>
        {
            public override RuntimeMethodHandle ReadJson(JsonReader reader, Type objectType, RuntimeMethodHandle existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var ptr = new IntPtr(Convert.ToInt32(reader.ReadAsString(), fromBase: 16));

                return default;
            }

            public override void WriteJson(JsonWriter writer, RuntimeMethodHandle value, JsonSerializer serializer)
            {
                writer.WriteStartObject();
                writer.WriteValue(value.Value.ToString());
                writer.WriteEndObject();
            }
        }

        private class RuntimeFieldHandleJsonConverter : JsonConverter<RuntimeFieldHandle>
        {
            public override RuntimeFieldHandle ReadJson(JsonReader reader, Type objectType, RuntimeFieldHandle existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                return default;
            }

            public override void WriteJson(JsonWriter writer, RuntimeFieldHandle value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    } 
}
