using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

namespace Debugger
{
#nullable enable
    public interface IMethodCommand
    {
        public enum ResultType : byte
        {
            Success,
            Exception,
            TargetParameterCountException = Exception,
        }

        public ParameterInfo[]? Parameters { get; }

        public ResultType TryInvoke(params object[] parameters);
    }

    public interface IFieldCommand
    {
        public enum ResultType : byte
        {
            Success,
            Exception,
            ReadOnlyException = Exception,
        }

        public Type? FieldType { get; }

        public object? GetValue();

        public ResultType TrySetValue(object value);
    }

    public readonly struct Command : IMethodCommand, IFieldCommand
    {
        public readonly object? instance;
        public readonly string alias;
        public readonly string? description;
        public readonly MethodInfo? method;
        public readonly FieldInfo? field;
        public readonly bool? isReadOnly;

        public Type? FieldType => field?.FieldType;

        public ParameterInfo[]? Parameters => method?.GetParameters();

        public Command(Builder builder)
        {
            instance = builder.Instance;
            isReadOnly = builder.IsReadOnly;
            alias = builder.Alias;
            description = builder.Description;
            method = builder.Method;
            field = builder.Field;
        }

        public object? GetValue() => field?.GetValue(instance);

        public IFieldCommand.ResultType TrySetValue(object value)
        {
            if (isReadOnly.HasValue && isReadOnly.Value)
                return IFieldCommand.ResultType.ReadOnlyException;

            try
            {
                value = Convert.ChangeType(value, FieldType);
                field?.SetValue(instance, value);
                return IFieldCommand.ResultType.Success;
            }
            catch (System.Exception ex)
            {
                return ex switch
                {
                    _ => IFieldCommand.ResultType.Exception,
                };
            }
        }

        public IMethodCommand.ResultType TryInvoke(params object[] parameters)
        {
            try
            {
                if (parameters.Length > Parameters?.Length)
                    throw new TargetParameterCountException();

                for (var i = 0; i < parameters.Length; i++)
                    parameters[i] = Convert.ChangeType(parameters[i],
                        Parameters?[i].ParameterType);

                method?.Invoke(instance, parameters);
                return IMethodCommand.ResultType.Success;
            }
            catch (System.Exception ex)
            {
                return ex switch
                {
                    TargetParameterCountException _ => IMethodCommand.ResultType.TargetParameterCountException,
                    _ => IMethodCommand.ResultType.Exception,
                };
            }
        }

        public class Builder
        {
            internal object? Instance { get; private set; }
            internal string Alias { get; private set; }
            internal string? Description { get; private set; }
            internal bool? IsReadOnly { get; private set; }
            internal MethodInfo? Method { get; private set; }
            internal FieldInfo? Field { get; private set; }

            public Builder()
            {
                Instance = null;
                Alias = string.Empty;
            }

            public Builder(object instance, string alias)
            {
                Instance = instance;
                Alias = alias;
            }

            public Builder SetDescription(string description)
            {
                Description = description;
                return this;
            }

            public Builder SetType(MethodInfo method)
            {
                Method = method;

                var attr = method.GetCustomAttribute<DebugMethodAttribute>();

                if (attr != null)
                {
                    Alias = attr.alias;
                    Description = attr.description;
                }

                var descAttr = method.GetCustomAttribute<DebugDescriptionAttribute>();

                if (descAttr != null && string.IsNullOrEmpty(Description))
                    Description = descAttr.description;

                return this;
            }

            public Builder SetType(FieldInfo field)
            {
                Field = field;

                var attr = field.GetCustomAttribute<DebugFieldAttribute>();

                if (attr != null)
                {
                    Alias = attr.alias;
                    Description = attr.description;
                    IsReadOnly = attr.isReadOnly;
                }

                var descAttr = field.GetCustomAttribute<DebugDescriptionAttribute>();

                if (descAttr != null && string.IsNullOrEmpty(Description))
                    Description = descAttr.description;

                return this;
            }

            public IMethodCommand BuildAsMethod() => new Command(this);

            public IFieldCommand BuildAsField() => new Command(this);
        }
    }
#nullable restore
    //public readonly struct DebuggerCommand
    //{
    //    public enum Type : byte
    //    {
    //        Method,
    //        Field,
    //    }

    //    public readonly string alias;
    //    public readonly string description;
    //    public readonly Type type;
    //    public readonly bool isExtension;
    //    public readonly object instance;
    //    public readonly MethodInfo method;
    //    public readonly FieldInfo field;

    //    //public readonly Action<object[]> methodAction;
    //    public readonly System.Type[] expectedTypes;

    //    public DebuggerCommand(object instance, MethodInfo method, string alias, string description)
    //    {
    //        type = Type.Method;

    //        this.instance = instance;
    //        this.method = method;
    //        this.field = null;
    //        this.alias = alias;
    //        this.description = description;

    //        isExtension = method.IsDefined(typeof(ExtensionAttribute), true);
    //        expectedTypes = method.GetParameters().Select(x => x.ParameterType).ToArray();
    //        //methodAction = (Action<object[]>)Delegate.CreateDelegate(typeof(Action<object[]>), instance, method);
    //    }

    //    public DebuggerCommand(object instance, FieldInfo field, string alias, string description)
    //    {
    //        type = Type.Field;

    //        this.instance = instance;
    //        this.method = null;
    //        this.isExtension = false;
    //        this.field = field;
    //        this.alias = alias;
    //        this.description = description;

    //        //fieldAction = (Action<object[]>)Delegate.CreateDelegate(typeof(Action), instance, field);
    //        //methodAction = null;
    //        expectedTypes = new System.Type[1] { field.FieldType };
    //    }

    //    //public void Invoke(object[] parameters = null)
    //    //{
    //    //    try
    //    //    {
    //    //        var infos = method.GetParameters();

    //    //        for (int i = 0; i < parameters.Length; i++)
    //    //        {
    //    //            if (infos[i].ParameterType == parameters[i].GetType())
    //    //                continue;

    //    //            Debug.Log(parameters[i]);
    //    //            parameters[i] = Convert.ChangeType(parameters[i], infos[i].ParameterType);

    //    //            Debug.Log($"{parameters[i]} [{parameters[i].GetType()}]");
    //    //        }

    //    //        var oldSize = parameters.Length;

    //    //        Array.Resize(ref parameters, infos.Length);

    //    //        for (int i = oldSize; i < parameters.Length; i++)
    //    //        {
    //    //            if (!infos[i].HasDefaultValue)
    //    //                continue;

    //    //            parameters[i] = System.Type.Missing;
    //    //        }

    //    //        method.Invoke(instance, parameters);
    //    //    }
    //    //    catch (Exception ex)
    //    //    {
    //    //        if (ex is TargetParameterCountException)
    //    //            Debug.LogError("Parameters weren't correct!");
    //    //        else if (ex is NullReferenceException)
    //    //            Debug.LogError("Couldn't find any commands!");
    //    //        else
    //    //            Debug.LogException(ex);
    //    //    }
    //    //}

    //    //public object GetValue() => field.GetValue(instance);

    //    //public void SetValue(object value)
    //    //{
    //    //    try
    //    //    {
    //    //        var expectedType = field.FieldType;

    //    //        if (expectedType != value.GetType())
    //    //            value = Convert.ChangeType(value, expectedType);

    //    //        field.SetValue(instance, value);
    //    //    }
    //    //    catch (Exception ex)
    //    //    {
    //    //        if (ex is TargetParameterCountException)
    //    //            Debug.LogError("Parameters weren't correct!");
    //    //        else
    //    //            Debug.LogException(ex);
    //    //    }
    //    //}
    //}
}