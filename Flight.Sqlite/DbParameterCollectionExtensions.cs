using System.Data.Common;

namespace Flight
{
    internal static class DbParameterCollectionExtensions
    {
        public static DbParameter AddParameter(this DbCommand command, string name, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            command.Parameters.Add(parameter);
            return parameter;
        }
    }
}