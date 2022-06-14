using System.Data;
using Dapper;

namespace Users.Api.Common.TypeHandlers;

public class TrimmedStringHandler : SqlMapper.TypeHandler<string>
{
    public override void SetValue(IDbDataParameter parameter, string value)
    {
        parameter.Value = value;
    }

    public override string Parse(object value)
    {
        var result = (value as string)?.Trim();
        return result!;
    }
}