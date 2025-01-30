using Npgsql;

namespace OrderProcessingService.Infrastructure.Persistence.Extensions;

internal static class NpgsqlCommandExtensions
{
    public static NpgsqlCommand AddParameter<T>(this NpgsqlCommand command, string parameterName, T value)
    {
        var parameter = new NpgsqlParameter<T>(parameterName: parameterName, value: value);

        command.Parameters.Add(parameter);

        return command;
    }
}