using System.Data;
using Dapper;
using Users.Api.Common.Helpers;
using Users.Api.Interfaces.Repositories;
using Users.Api.Models;
using Users.Api.Models.Entities;
using Users.Api.Models.Requests;

namespace Users.Api.Repositories;

public class UserInfoRepository : IUserInfoRepository
{
    private const string GetUserQuery = "exec getUserById @userId";
    private const string CreateUserQuery = "exec createUser @userId @userName @firstName @lastName";
    private const string UpdateUserQuery =
        "exec updateUser @userId @firstName @lastName @userName @birthDay @isAccountPublic @about";
    private const string UpdateUserAvatarQuery = "exec updateUserAvatar @userId @avatarUrl";
    private readonly DapperContext _context;

    public UserInfoRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task UpdateUserInfoAsync(UpdateUserInfoRequest userInformation, Guid id)
    {
        using var connection = _context.CreateDbConnection();

        var queryParams = new DynamicParameters();
        queryParams.Add("userId", id, DbType.Guid);
        queryParams.Add("firstName", userInformation.FirstName, DbType.StringFixedLength);
        queryParams.Add("lastName", userInformation.LastName, DbType.StringFixedLength);
        queryParams.Add("userName", userInformation.UserName, DbType.StringFixedLength);
        queryParams.Add("birthDay", userInformation.BirthDay, DbType.DateTime2);
        queryParams.Add("isAccountPublic", userInformation.IsAccountPublic, DbType.Byte);
        queryParams.Add("about", userInformation.About, DbType.StringFixedLength);

        await connection.ExecuteAsync(UpdateUserQuery, queryParams);
    }

    public async Task<UserInformation> GetUserInfoAsync(Guid userId)
    {
        using var connection = _context.CreateDbConnection();
        
        var queryParams = new DynamicParameters();
        queryParams.Add("userId", userId, DbType.Guid);

        var userInfo = await connection.QueryFirstOrDefaultAsync<UserInformation>(GetUserQuery, userId);

        return userInfo;
    }

    public async Task CreateUserAsync(UserInformation userInformation)
    {
        using var connection = _context.CreateDbConnection();

        var queryParams = new DynamicParameters();
        queryParams.Add("userId", userInformation.Id, DbType.Guid);
        queryParams.Add("userName", userInformation.UserName, DbType.StringFixedLength);
        queryParams.Add("firstName", userInformation.FirstName, DbType.StringFixedLength);
        queryParams.Add("lastName", userInformation.LastName, DbType.StringFixedLength);

        await connection.ExecuteAsync(CreateUserQuery, queryParams);
    }

    public async Task UpdateUserAvatarUrl(string avatarUrl, Guid userId)
    {
        using var connection = _context.CreateDbConnection();

        var queryParams = new DynamicParameters();
        queryParams.Add("userId", userId, DbType.Guid);
        queryParams.Add("avatarUrl", avatarUrl, DbType.String);

        await connection.ExecuteAsync(UpdateUserAvatarQuery, queryParams);
    }
}