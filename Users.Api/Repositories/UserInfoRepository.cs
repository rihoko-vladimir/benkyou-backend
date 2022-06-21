using System.Data;
using Dapper;
using Serilog;
using Users.Api.Interfaces.Factories;
using Users.Api.Interfaces.Repositories;
using Users.Api.Models.Entities;
using Users.Api.Models.Requests;
using SqlCommand = Users.Api.Models.Constants.SqlCommand;

namespace Users.Api.Repositories;

public class UserInfoRepository : IUserInfoRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserInfoRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task UpdateUserInfoAsync(UpdateUserInfoRequest userInformation, Guid id)
    {
        await using var connection = _connectionFactory.GetConnection();
        var queryParams = new DynamicParameters();
        
        queryParams.Add("userId", id, DbType.Guid);
        queryParams.Add("firstName", userInformation.FirstName, DbType.StringFixedLength);
        queryParams.Add("lastName", userInformation.LastName, DbType.StringFixedLength);
        queryParams.Add("userName", userInformation.UserName, DbType.StringFixedLength);
        queryParams.Add("birthDay", userInformation.BirthDay, DbType.DateTime2);
        queryParams.Add("isAccountPublic", userInformation.IsAccountPublic, DbType.Byte);
        queryParams.Add("about", userInformation.About, DbType.StringFixedLength);
        
        Log.Information("Updating user info: {FirstName}, {LastName}, {UserName}, {BirthDay}, {IsPublic}, {About}", 
            userInformation.FirstName,
            userInformation.LastName,
            userInformation.UserName,
            userInformation.BirthDay ?? new DateTime(),
            userInformation.IsAccountPublic,
            userInformation.About);

        await connection.ExecuteAsync(SqlCommand.UpdateUserQuery, queryParams);
    }

    public async Task<UserInformation> GetUserInfoAsync(Guid userId)
    {
        await using var connection = _connectionFactory.GetConnection();
        
        var queryParams = new DynamicParameters();
        queryParams.Add("userId", userId, DbType.Guid);

        Log.Information("Querying for user info with id {UserId}", userId);
        
        var userInfo = await connection.QueryFirstOrDefaultAsync<UserInformation>(SqlCommand.GetUserQuery, queryParams);

        return userInfo;
    }

    public async Task CreateUserAsync(UserInformation userInformation)
    {
        await using var connection = _connectionFactory.GetConnection();

        var queryParams = new DynamicParameters();
        queryParams.Add("userId", userInformation.Id, DbType.Guid);
        queryParams.Add("userName", userInformation.UserName, DbType.StringFixedLength);
        queryParams.Add("firstName", userInformation.FirstName, DbType.StringFixedLength);
        queryParams.Add("lastName", userInformation.LastName, DbType.StringFixedLength);
        
        Log.Information("Creating new user: {Id}, {UserName}, {FirstName}, {LastName}", 
            userInformation.Id,
            userInformation.UserName,
            userInformation.FirstName,
            userInformation.LastName);
        
        await connection.ExecuteAsync(SqlCommand.CreateUserQuery, queryParams);
    }

    public async Task UpdateUserAvatarUrl(string avatarUrl, Guid userId)
    {
        await using var connection = _connectionFactory.GetConnection();

        var queryParams = new DynamicParameters();
        queryParams.Add("userId", userId, DbType.Guid);
        queryParams.Add("avatarUrl", avatarUrl, DbType.StringFixedLength);
        
        Log.Information("Changing user's {Id} avatar url with new value : {AvatarUrl}", 
            userId,
            avatarUrl);
        
        await connection.ExecuteAsync(SqlCommand.UpdateUserAvatarQuery, queryParams);
    }
}