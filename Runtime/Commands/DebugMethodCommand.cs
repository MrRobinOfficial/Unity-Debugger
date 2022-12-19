namespace uDebugger.Commands
{
    public struct DebugMethodCommand
    {
        public string name;
        public System.RuntimeMethodHandle handle;
        public System.Type[] parameterTypes;
        public System.Type[] argumentTypes;
        public System.Type returnType;

        public FieldResult TryInvoke(params object[] parameters)
        {
            return FieldResult.Exception;

            //try
            //{
            //    if (parameters.Length > Parameters?.Length)
            //        throw new TargetParameterCountException();

            //    for (var i = 0; i < parameters.Length; i++)
            //        parameters[i] = Convert.ChangeType(parameters[i],
            //            Parameters?[i].ParameterType);

            //    method?.Invoke(instance, parameters);
            //    return IMethodCommand.ResultType.Success;
            //}
            //catch (System.Exception ex)
            //{
            //    return ex switch
            //    {
            //        TargetParameterCountException _ => IMethodCommand.ResultType.TargetParameterCountException,
            //        _ => IMethodCommand.ResultType.Exception,
            //    };
            //}
        }

        public FieldResult TryInvoke<T>(params T[] parameters)
        {
            return FieldResult.Exception;
        }

        public FieldResult TryInvoke<T1>(T1 parameter_1)
        {
            return FieldResult.Exception;
        }

        public FieldResult TryInvoke<T1, T2>(T1 parameter_1,
                                             T2 parameter_2)
        {
            return FieldResult.Exception;
        }

        public FieldResult TryInvoke<T1, T2, T3>(T1 parameter_1,
                                                 T2 parameter_2,
                                                 T3 parameter_3)
        {
            return FieldResult.Exception;
        }

        public FieldResult TryInvoke<T1, T2, T3, T4>(T1 parameter_1,
                                                     T2 parameter_2,
                                                     T3 parameter_3,
                                                     T4 parameter_4)
        {
            return FieldResult.Exception;
        }

        public FieldResult TryInvoke<T1, T2, T3, T4, T5>(T1 parameter_1,
                                                         T2 parameter_2,
                                                         T3 parameter_3,
                                                         T4 parameter_4,
                                                         T5 parameter_5)
        {
            return FieldResult.Exception;
        }

        public FieldResult TryInvoke<T1, T2, T3, T4, T5, T6>(T1 parameter_1,
                                                             T2 parameter_2,
                                                             T3 parameter_3,
                                                             T4 parameter_4,
                                                             T5 parameter_5,
                                                             T6 parameter_6)
        {
            return FieldResult.Exception;
        }

        public FieldResult TryInvoke<T1, T2, T3, T4, T5, T6, T7>(T1 parameter_1,
                                                                 T2 parameter_2,
                                                                 T3 parameter_3,
                                                                 T4 parameter_4,
                                                                 T5 parameter_5,
                                                                 T6 parameter_6,
                                                                 T7 parameter_7)
        {
            return FieldResult.Exception;
        }

        public FieldResult TryInvoke<T1, T2, T3, T4, T5, T6, T7, T8>(T1 parameter_1,
                                                                     T2 parameter_2,
                                                                     T3 parameter_3,
                                                                     T4 parameter_4,
                                                                     T5 parameter_5,
                                                                     T5 parameter_6,
                                                                     T5 parameter_7,
                                                                     T5 parameter_8)
        {
            return FieldResult.Exception;
        }

        public FieldResult TryInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 parameter_1,
                                                                         T2 parameter_2,
                                                                         T3 parameter_3,
                                                                         T4 parameter_4,
                                                                         T5 parameter_5,
                                                                         T5 parameter_6,
                                                                         T5 parameter_7,
                                                                         T5 parameter_8,
                                                                         T5 parameter_9)
        {
            return FieldResult.Exception;
        }
    }
}