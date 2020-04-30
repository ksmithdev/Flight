namespace Flight
{
    using System;
    using System.Data.Common;

    internal static class DbParameterCollectionExtensions
    {
        public static DbParameter AddParameter(this DbCommand command, string name, object value)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            command.Parameters.Add(parameter);
            return parameter;
        }
    }
}