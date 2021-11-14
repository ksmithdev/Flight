namespace Flight
{
    using System;
    using System.Data.Common;

    /// <summary>
    /// Represents the collection of extension methods for <see cref="DbCommand"/> instances.
    /// </summary>
    internal static class DbParameterCollectionExtensions
    {
        /// <summary>
        /// Add a new instance of <see cref="DbParameter"/> with the suppied name and value to the <see cref="DbCommand"/>.
        /// </summary>
        /// <param name="command">The command to add the parameter to.</param>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The parameter value.</param>
        /// <returns>The newly created <see cref="DbParameter"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when a parameter is null.</exception>
        public static DbParameter AddParameter(this DbCommand command, string name, object value)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            command.Parameters.Add(parameter);
            return parameter;
        }
    }
}