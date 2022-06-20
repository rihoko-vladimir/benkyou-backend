using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Moq;
using Users.Api.Common.TypeHandlers;
using Users.Api.Interfaces.Factories;
using Users.Api.Models.Entities;
using SqlCommands = Users.Api.Models.Constants.SqlCommand;

namespace Users.Tests;

public class RepositoryTest : IDisposable
{
    private readonly Mock<IDbConnectionFactory> _mockedFactory;

    public RepositoryTest()
    {
        var testConnection =
            new SqlConnection(
                "Server=172.17.0.1; Database=TEST_Benkyou_users; User Id = sa; Password = nandesukaanatawa1A; TrustServerCertificate=True;");
        _mockedFactory = new Mock<IDbConnectionFactory>();
        _mockedFactory.Setup(factory => factory.GetConnection()).Returns(testConnection);
        SqlMapper.AddTypeHandler(new TrimmedStringHandler());
    }

    [Fact]
    public async Task Create_And_Get_UserInfo_Test_Returns_Correct_User_Information()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var expectedInfo = new UserInformation
        {
            Id = userId,
            FirstName = "Uladzimir",
            LastName = "Kazlouski",
            UserName = "rihoko",
            UserRole = "User",
            IsTermsAccepted = true,
            IsAccountPublic = false
        };
        var connection = _mockedFactory.Object.GetConnection();
        var queryParams = new DynamicParameters();
        queryParams.Add("userId", expectedInfo.Id, DbType.Guid);
        queryParams.Add("userName", expectedInfo.UserName, DbType.StringFixedLength);
        queryParams.Add("firstName", expectedInfo.FirstName, DbType.StringFixedLength);
        queryParams.Add("lastName", expectedInfo.LastName, DbType.StringFixedLength);
        await connection.ExecuteAsync(SqlCommands.CreateUserQuery,queryParams);
        //Act
        var user = await connection.QueryFirstOrDefaultAsync<UserInformation>("exec getUserById @id", new
        {
            id = userId
        });
        //Assert
        Assert.Equal(expectedInfo, user);
    }

    [Fact]
    public async Task UpdateUserInfo_Test()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var sourceInfo = new UserInformation
        {
            Id = userId,
            FirstName = "Uladzimir",
            LastName = "Kazlouski",
            UserName = "rihoko",
            UserRole = "User",
            IsTermsAccepted = true,
            IsAccountPublic = false
        };
        
        var expectedInfo = new UserInformation
        {
            Id = userId,
            FirstName = "Uladzimir1",
            LastName = "Kazlouski",
            UserName = "rihoko",
            UserRole = "User",
            IsTermsAccepted = true,
            IsAccountPublic = false
        };
        
        var connection = _mockedFactory.Object.GetConnection();
        
        var queryParams = new DynamicParameters();
        queryParams.Add("userId", sourceInfo.Id, DbType.Guid);
        queryParams.Add("userName", sourceInfo.UserName, DbType.StringFixedLength);
        queryParams.Add("firstName", sourceInfo.FirstName, DbType.StringFixedLength);
        queryParams.Add("lastName", sourceInfo.LastName, DbType.StringFixedLength);
        
        await connection.ExecuteAsync(SqlCommands.CreateUserQuery,queryParams);
        
        //Act
        var queryParamsAct = new DynamicParameters();
        
        queryParamsAct.Add("userId", expectedInfo.Id, DbType.Guid);
        queryParamsAct.Add("firstName", expectedInfo.FirstName, DbType.StringFixedLength);
        queryParamsAct.Add("lastName", expectedInfo.LastName, DbType.StringFixedLength);
        queryParamsAct.Add("userName", expectedInfo.UserName, DbType.StringFixedLength);
        queryParamsAct.Add("birthDay", expectedInfo.BirthDay, DbType.DateTime2);
        queryParamsAct.Add("isAccountPublic", expectedInfo.IsAccountPublic, DbType.Byte);
        queryParamsAct.Add("about", expectedInfo.About, DbType.StringFixedLength);
        
        await connection.ExecuteAsync(SqlCommands.UpdateUserQuery, queryParamsAct);
        //Assert
        var modifiedUser = await connection.QueryFirstOrDefaultAsync<UserInformation>("exec getUserById @id", new
        {
            id = userId
        });
        Assert.Equal(expectedInfo, modifiedUser);
    }

    [Fact]
    public async Task Update_User_Avatar_Test()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var sourceInfo = new UserInformation
        {
            Id = userId,
            FirstName = "Uladzimir",
            LastName = "Kazlouski",
            UserName = "rihoko",
            UserRole = "User",
            IsTermsAccepted = true,
            IsAccountPublic = false
        };
        var expectedInfo = new UserInformation
        {
            Id = userId,
            FirstName = "Uladzimir",
            LastName = "Kazlouski",
            UserName = "rihoko",
            UserRole = "User",
            IsTermsAccepted = true,
            IsAccountPublic = false,
            AvatarUrl = "okokok"
        };
        var connection = _mockedFactory.Object.GetConnection();
        var queryParams = new DynamicParameters();
        queryParams.Add("userId", sourceInfo.Id, DbType.Guid);
        queryParams.Add("userName", sourceInfo.UserName, DbType.StringFixedLength);
        queryParams.Add("firstName", sourceInfo.FirstName, DbType.StringFixedLength);
        queryParams.Add("lastName", sourceInfo.LastName, DbType.StringFixedLength);
        await connection.ExecuteAsync(SqlCommands.CreateUserQuery,queryParams);
        //Act
        var queryParamsAct = new DynamicParameters();
        queryParamsAct.Add("userId", userId, DbType.Guid);
        queryParamsAct.Add("avatarUrl", expectedInfo.AvatarUrl, DbType.StringFixedLength);
        
        await connection.ExecuteAsync(SqlCommands.UpdateUserAvatarQuery, queryParamsAct);
        //Assert
        var updatedUser = await connection.QueryFirstOrDefaultAsync<UserInformation>("exec getUserById @id", new
        {
            id = userId
        });
        
        Assert.Equal(expectedInfo, updatedUser);
    }
    
    public void Dispose()
    {
        var connection = _mockedFactory.Object.GetConnection();
        connection.Execute("DELETE FROM UsersInformation");
    }
}