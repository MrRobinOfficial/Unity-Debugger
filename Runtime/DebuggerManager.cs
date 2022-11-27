using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
using UnityParsers;
using System.Reflection;
using System;
//using UnityEngine.Windows;
//using System.Security.Cryptography;
//using System.Windows.Input;
//using System.Dynamic;

#if UNITY_EDITOR
using UnityEditor;
#endif

using static Debugger.DebuggerExtensions;

namespace Debugger
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public static class DebuggerManager
    {
        public const int COMMAND_EXECUTED_CAPAICTY = 100;
        public const string REGEX_PATTERN = "[\\\"\"].+?[\\\"\"]|\\(.*\\)|[^ ]+";

        public static List<string> Cache = new(COMMAND_EXECUTED_CAPAICTY);

        [DebugMethod("clearCache")]
        public static void ClearCache() => Cache.Clear();

        public static int ExecutionIndex
        {
            get => _executionIndex;
            set
            {
                _executionIndex = value;

                if (_executionIndex < 0)
                    _executionIndex = 0;

                if (_executionIndex > Cache.Count - 1)
                    _executionIndex = Cache.Count - 1;
            }
        }

        private static int _executionIndex;
        private static string password = string.Empty;

        //public static bool IsTypingPassword => DebuggerSettings.Instance.enablePassword && string.IsNullOrEmpty(password);

        public static StringComparison Comparision { get; private set; } = StringComparison.OrdinalIgnoreCase;

        public static event Action OnInit;

        static DebuggerManager() => Init();

#if !UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
#endif
        public static void Init()
        {
            password = string.Empty;
            OnInit?.Invoke();
        }

        private enum Operator
        {
            Get,
            Set,
            Add,
            Subtract,
            Multiply,
            Divide,
        }

        private static Operator GetOperator(string action)
        {
            if (string.IsNullOrEmpty(action))
                return Operator.Get;
            else if (action.Equals("=", Comparision))
                return Operator.Set;
            else if (action.Equals("+=", Comparision))
                return Operator.Add;
            else if (action.Equals("+=", Comparision))
                return Operator.Subtract;
            else if (action.Equals("*=", Comparision))
                return Operator.Multiply;
            else if (action.Equals("/=", Comparision))
                return Operator.Divide;
            else
                return Operator.Get;
        }

        private static object[] ConvertParameters(MethodInfo info, object[] parameters)
        {
            var targetParameters = info.GetParameters();
            var results = new object[targetParameters.Length];

            for (int i = results.Length; i < targetParameters.Length; i++)
            {
                if (!targetParameters[i].HasDefaultValue)
                    continue;

                results[i] = Type.Missing;
            }

            for (int i = 0; i < parameters.Length; i++)
            {
                Debug.Log(results[i].GetType());
                results[i] = Convert.ChangeType(results[i], targetParameters[i].ParameterType);
                Debug.Log(results[i].GetType());

                //if (results[i].GetType() != targetParameters[i].ParameterType)
                //    throw new InvalidArgumentFormatException($"{results[i]}", targetParameters[i].ParameterType);
            }

            return results;
        }

        private static object ConvertParameter(FieldInfo info, string parameter)
        {
            if (string.IsNullOrEmpty(parameter))
                return Type.Missing;

            if (!Parser.TryParse(parameter, out object result))
                result = Type.Missing;

            return Convert.ChangeType(result, info.FieldType);
        }


        public static bool SendCommand(string input, out Command command)
        {
            var regexMatches = Regex.Matches(input, REGEX_PATTERN)
                .Cast<Match>()
                .Select(m => m.Value)
                .ToArray();

            var alias = regexMatches[0];
            var parameters = regexMatches.Skip(count: 1).ToArray();

            var results = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
                Parser.TryParse(parameters[i], out results[i]);

            command = default;

            //if (!TryFindCommand(alias, results, out command))
            //    return false;

            //try
            //{
            //    switch (command.type)
            //    {
            //        case DebuggerCommand.Type.Method:
            //            var args = command.method.GetParameters();

            //            var oldLength = results.Length;

            //            Array.Resize(ref results, args.Length);

            //            for (int i = oldLength; i < args.Length; i++)
            //            {
            //                if (args[i].HasDefaultValue)
            //                    results[i] = Type.Missing; // Adding missing parameters
            //                else
            //                    throw new InvalidArgumentFormatException(input, args[i].ParameterType);
            //            }

            //            for (int i = 0; i < args.Length; i++)
            //            {
            //                if (args[i].HasDefaultValue)
            //                    continue;

            //                results[i] = Convert.ChangeType(results[i], args[i].ParameterType); // Casting to correct parameter type
            //            }

            //            command.method.Invoke(command.instance, results);
            //            break;

            //        case DebuggerCommand.Type.Field:
            //            var op = GetOperator(parameters[0]);

            //            var param = regexMatches.Skip(count: 2).FirstOrDefault();

            //            if (command.instance == null && !command.field.IsStatic)
            //                return false;

            //            if (op == Operator.Get)
            //                command.field.GetValue(command.instance);
            //            else if (op == Operator.Set)
            //                command.field.SetValue(command.instance, ConvertParameter(command.field, param));
            //            else if (op == Operator.Add)
            //            {
            //                //dynamic obj = command.field.GetValue(command.instance);
            //                //command.field.SetValue(command.instance, obj + ConvertParameters(command, parameters));
            //            }
            //            else if (op == Operator.Subtract)
            //            {
            //                //dynamic obj = command.field.GetValue(command.instance);
            //                //command.field.SetValue(command.instance, obj - ConvertParameters(command, parameters));
            //            }
            //            else if (op == Operator.Multiply)
            //            {
            //                //dynamic obj = command.field.GetValue(command.instance);
            //                //command.field.SetValue(command.instance, obj * ConvertParameters(command, parameters));
            //            }
            //            else if (op == Operator.Divide)
            //            {
            //                //dynamic obj = command.field.GetValue(command.instance);
            //                //command.field.SetValue(command.instance, obj / ConvertParameters(command, parameters));
            //            }
            //            break;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    if (ex is TargetParameterCountException)
            //        Debug.LogError("Parameters weren't correct!");
            //    else if (ex is NullReferenceException)
            //        Debug.LogError("Couldn't find any commands!");
            //    else
            //        Debug.LogException(ex);
            //}

            return true;
        }
    }
}