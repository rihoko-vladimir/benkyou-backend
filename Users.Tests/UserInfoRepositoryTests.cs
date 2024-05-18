using AutoMapper;
using Dapper;
using Microsoft.Data.SqlClient;
using Moq;
using Users.Api.Common.MapperProfiles;
using Users.Api.Common.TypeHandlers;
using Users.Api.Interfaces.Factories;
using Users.Api.Interfaces.Repositories;
using Users.Api.Models.Entities;
using Users.Api.Models.Requests;
using Users.Api.Repositories;

namespace Users.Tests;

public class RepositoryTest : IDisposable
{
    private readonly IMapper _mapper;
    private readonly IUserInfoRepository _userInfoRepository;

    public RepositoryTest()
    {
        SqlMapper.AddTypeHandler(new TrimmedStringHandler());

        _mapper = new MapperConfiguration(expression => { expression.AddProfile<AutoMappingProfile>(); })
            .CreateMapper();

        var mockedFactory = new Mock<IDbConnectionFactory>();
        mockedFactory.Setup(factory => factory.GetConnection()).Returns(() => new SqlConnection(
            "Server=172.17.0.1; Database=TEST_Benkyou_users; User Id = sa; Password = nandesukaanatawa1A; TrustServerCertificate=True;"));
        _userInfoRepository = new UserInfoRepository(mockedFactory.Object);
    }

    public void Dispose()
    {
        var connection = new SqlConnection(
            "Server=172.17.0.1; Database=TEST_Benkyou_users; User Id = sa; Password = nandesukaanatawa1A; TrustServerCertificate=True;");

        connection.Execute("DELETE FROM UsersInformation");
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

        await _userInfoRepository.CreateUserAsync(expectedInfo);

        //Act

        var user = await _userInfoRepository.GetUserInfoAsync(userId);

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

        await _userInfoRepository.CreateUserAsync(sourceInfo);

        //Act

        await _userInfoRepository.UpdateUserInfoAsync(_mapper.Map<UpdateUserInfoRequest>(expectedInfo), sourceInfo.Id);

        //Assert

        var modifiedUser = await _userInfoRepository.GetUserInfoAsync(sourceInfo.Id);

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

        await _userInfoRepository.CreateUserAsync(sourceInfo);

        //Act

        await _userInfoRepository.UpdateUserAvatarUrl(expectedInfo.AvatarUrl, sourceInfo.Id);

        //Assert

        var updatedUser = await _userInfoRepository.GetUserInfoAsync(sourceInfo.Id);

        Assert.Equal(expectedInfo, updatedUser);
    }
}