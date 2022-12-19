using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using uDebugger.Commands;
using UnityEngine;
using UnityParsers;

namespace uDebugger
{
    public enum MethodResult : byte
    {
        Success,
        Exception,
        TargetParameterCountException = Exception,
    }

    public enum FieldResult : byte
    {
        Success,
        Exception,
        ReadOnlyException = Exception,
    }

    [Icon(path: "Assets/uDebugger/Resources/Debug.png")]
    public static partial class Debugger
	{
        private const int k_CacheCapacity = 16;

        private static List<string> m_Cache = new(capacity: k_CacheCapacity);

        public static IReadOnlyList<string> Cache => m_Cache;

        public static void ClearCache() => m_Cache.Clear();

        public readonly static StringComparison StrComparision = StringComparison.OrdinalIgnoreCase;

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
            else if (action.Equals("=", StrComparision))
                return Operator.Set;
            else if (action.Equals("+=", StrComparision))
                return Operator.Add;
            else if (action.Equals("+=", StrComparision))
                return Operator.Subtract;
            else if (action.Equals("*=", StrComparision))
                return Operator.Multiply;
            else if (action.Equals("/=", StrComparision))
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

        //public static bool SendCommand(string input, out Command command)
        //{
        //    var regexMatches = Regex.Matches(input, REGEX_PATTERN)
        //        .Cast<Match>()
        //        .Select(m => m.Value)
        //        .ToArray();

        //    var alias = regexMatches[0];
        //    var parameters = regexMatches.Skip(count: 1).ToArray();

        //    var results = new object[parameters.Length];

        //    for (int i = 0; i < parameters.Length; i++)
        //        Parser.TryParse(parameters[i], out results[i]);

        //    command = default;

        //    //if (!TryFindCommand(alias, results, out command))
        //    //    return false;

        //    //try
        //    //{
        //    //    switch (command.type)
        //    //    {
        //    //        case DebuggerCommand.Type.Method:
        //    //            var args = command.method.GetParameters();

        //    //            var oldLength = results.Length;

        //    //            Array.Resize(ref results, args.Length);

        //    //            for (int i = oldLength; i < args.Length; i++)
        //    //            {
        //    //                if (args[i].HasDefaultValue)
        //    //                    results[i] = Type.Missing; // Adding missing parameters
        //    //                else
        //    //                    throw new InvalidArgumentFormatException(input, args[i].ParameterType);
        //    //            }

        //    //            for (int i = 0; i < args.Length; i++)
        //    //            {
        //    //                if (args[i].HasDefaultValue)
        //    //                    continue;

        //    //                results[i] = Convert.ChangeType(results[i], args[i].ParameterType); // Casting to correct parameter type
        //    //            }

        //    //            command.method.Invoke(command.instance, results);
        //    //            break;

        //    //        case DebuggerCommand.Type.Field:
        //    //            var op = GetOperator(parameters[0]);

        //    //            var param = regexMatches.Skip(count: 2).FirstOrDefault();

        //    //            if (command.instance == null && !command.field.IsStatic)
        //    //                return false;

        //    //            if (op == Operator.Get)
        //    //                command.field.GetValue(command.instance);
        //    //            else if (op == Operator.Set)
        //    //                command.field.SetValue(command.instance, ConvertParameter(command.field, param));
        //    //            else if (op == Operator.Add)
        //    //            {
        //    //                //dynamic obj = command.field.GetValue(command.instance);
        //    //                //command.field.SetValue(command.instance, obj + ConvertParameters(command, parameters));
        //    //            }
        //    //            else if (op == Operator.Subtract)
        //    //            {
        //    //                //dynamic obj = command.field.GetValue(command.instance);
        //    //                //command.field.SetValue(command.instance, obj - ConvertParameters(command, parameters));
        //    //            }
        //    //            else if (op == Operator.Multiply)
        //    //            {
        //    //                //dynamic obj = command.field.GetValue(command.instance);
        //    //                //command.field.SetValue(command.instance, obj * ConvertParameters(command, parameters));
        //    //            }
        //    //            else if (op == Operator.Divide)
        //    //            {
        //    //                //dynamic obj = command.field.GetValue(command.instance);
        //    //                //command.field.SetValue(command.instance, obj / ConvertParameters(command, parameters));
        //    //            }
        //    //            break;
        //    //    }
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    if (ex is TargetParameterCountException)
        //    //        Debug.LogError("Parameters weren't correct!");
        //    //    else if (ex is NullReferenceException)
        //    //        Debug.LogError("Couldn't find any commands!");
        //    //    else
        //    //        Debug.LogException(ex);
        //    //}

        //    return true;
        //}

        //public static bool _SendCommand(string input)
        //{
        //    string[] args = input.Split(REGEX_PATTERN);

        //    string commandName = args[0].ToLower();

        //    if (!commandName.ContainsKey(commandName))
        //        return;

        //    DebugCommand command = m_Commands[commandName];

        //    MethodInfo method = command.GetType().GetMethod("Execute");
        //    ParameterInfo[] parameters = method.GetParameters();

        //    object[] arguments = new object[parameters.Length];

        //    for (int i = 0; i < parameters.Length; i++)
        //    {
        //        var parameterType = parameters[i].ParameterType;

        //        if (parameterType == typeof(float))
        //            arguments[i] = float.Parse(args[i + 1]);
        //        else if (parameterType == typeof(bool))
        //            arguments[i] = bool.Parse(args[i + 1]);
        //        else if (parameterType == typeof(int))
        //            arguments[i] = int.Parse(args[i + 1]);
        //        else if (parameterType == typeof(double))
        //            arguments[i] = double.Parse(args[i + 1]);
        //        else if (parameterType == typeof(string))
        //            arguments[i] = args[i + 1];
        //    }
        //}
    }
}