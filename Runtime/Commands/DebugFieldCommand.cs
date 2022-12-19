namespace uDebugger.Commands
{
    public struct DebugFieldCommand
    {
        public string name;
        public System.RuntimeFieldHandle handle;
        public System.Type fieldType;

        public FieldResult GetValue(out object value)
        {
            value = default;
            return FieldResult.Exception;
        }

        public FieldResult GetValue<T>(out T value)
        {
            value = default;
            return FieldResult.Exception;

            //try
            //{
            //    value = Convert.ChangeType(value, FieldType);
            //    field?.SetValue(instance, value);
            //    return IFieldCommand.ResultType.Success;
            //}
            //catch (System.Exception ex)
            //{
            //    return ex switch
            //    {
            //        _ => IFieldCommand.ResultType.Exception,
            //    };
            //}
        }

        public FieldResult SetValue(object value)
        {
            return FieldResult.Exception;
        }

        public FieldResult SetValue<T>(T value)
        {
            return FieldResult.Exception;
        }
    }
}
