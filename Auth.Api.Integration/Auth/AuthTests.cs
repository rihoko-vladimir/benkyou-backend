using System.Text;
using Auth.Api.Generators;
using Auth.Api.Models.DbContext;
using Auth.Api.Models.Requests;
using Auth.Api.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Shared.Models.Models.Configurations;
using Xunit.Abstractions;
using Xunit.Priority;

namespace Auth.Api.Integration.Auth;

[CollectionDefinition("Non-Parallel Collection", DisableParallelization = true)]
public class SystemTestCollectionDefinition { }

public static class Configs
{
    public static readonly JwtConfiguration JwtConfiguration = new()
    {
        Audience = "test",
        Issuer = "test",
        AccessSecret = "test123123123123123123123123123123",
        RefreshSecret = "test123123123123123123123123123123",
        ResetSecret = "test123123123123123123123123123123",
        AccessExpiresIn = 200,
        RefreshExpiresIn = 200,
        ResetExpiresIn = 200
    };
}

[Collection("Non-Parallel Collection")]
public class ResetTokenTests
{
    private readonly ResetTokenService _resetTokenService;

    public ResetTokenTests()
    {
        var tokenGenerator = new TokenGenerator();
        _resetTokenService = new ResetTokenService(tokenGenerator, Configs.JwtConfiguration);
    }

    [Fact]
    public void Test_ResetToken_IsCorrect()
    {
        //Arrange

        var userId = Guid.Parse("c6b04f2d-90c9-404a-9210-dcc25aaa0f4c");

        //Act
        var token = _resetTokenService.GetToken(userId);

        var isTokenCorrect = _resetTokenService.VerifyToken(userId, token);

        //Assert

        Assert.True(isTokenCorrect);
    }
}

[Collection("Non-Parallel Collection")]
public class AccessTokenTests
{
    private readonly AccessTokenService _accessTokenService;

    public AccessTokenTests()
    {
        var tokenGenerator = new TokenGenerator();
        _accessTokenService = new AccessTokenService(tokenGenerator, Configs.JwtConfiguration);
    }

    [Fact]
    public void Test_AccessToken_IsCorrect()
    {
        //Arrange

        var userId = Guid.Parse("c6b04f2d-90c9-404a-9210-dcc25aaa0f4c");

        //Act
        var token = _accessTokenService.GetToken(userId);

        var isTokenCorrect = _accessTokenService.GetGuidFromAccessToken(token, out var guid);

        //Assert

        Assert.True(isTokenCorrect);
    }
}

[Collection("Non-Parallel Collection")]
public class RefreshTokenTests
{
    private readonly RefreshTokenService _refreshTokenService;

    public RefreshTokenTests()
    {
        var tokenGenerator = new TokenGenerator();
        _refreshTokenService = new RefreshTokenService(tokenGenerator, Configs.JwtConfiguration);
    }

    [Fact]
    public void Test_AccessToken_IsCorrect()
    {
        //Arrange

        var userId = Guid.Parse("c6b04f2d-90c9-404a-9210-dcc25aaa0f4c");

        //Act
        var token = _refreshTokenService.GetToken(userId);

        var isTokenCorrect = _refreshTokenService.VerifyToken(userId, token);

        //Assert

        Assert.True(isTokenCorrect);
    }
}

[Collection("Non-Parallel Collection")]
[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class UserTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly ApplicationContext _dbContext;
    private readonly AuthTestApplication _factory;

    public UserTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _factory = new AuthTestApplication();
        _dbContext = _factory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider
            .GetRequiredService<ApplicationContext>();
        _dbContext.Database.EnsureCreated();
    }

    [Theory, Priority(1)]
    [InlineData("/api/v1/auth/register")]
    public async Task Test_Registration_IsCorrect(string url)
    {
        //Arrange
        var userCredentials = new RegistrationRequest
        {
            Email = "test@test.com",
            FirstName = "Vladimir_test",
            LastName = "Kozlovsky_test",
            UserName = "rihoko",
            IsTermsAccepted = true,
            Password = "testtetstest1A"
        };

        var credsSerialized = JsonConvert.SerializeObject(userCredentials);

        var client = _factory.CreateClient();

        //Act

        var response = await client.PostAsync(url, new StringContent(credsSerialized, Encoding.UTF8, "application/json"));

        var queriedUser =
            await _dbContext.UserCredentials.FirstOrDefaultAsync(
                credential => credential.Email == userCredentials.Email);

        //Assert

        _testOutputHelper.WriteLine(response.StatusCode.ToString());
        
        _testOutputHelper.WriteLine(await new StreamReader(await response.Content.ReadAsStreamAsync()).ReadToEndAsync());

        Assert.True(response.IsSuccessStatusCode);

        Assert.NotNull(queriedUser);

        Assert.Equal(queriedUser?.Email, userCredentials.Email);
    }

    [Theory, Priority(2)]
    [InlineData("/api/v1/auth/confirm-email")]
    public async Task Test_Confirm_IsCorrect(string url)
    {
        //Arrange
        var confirmationCode = (await _dbContext.UserCredentials.FirstOrDefaultAsync(credential => credential.Email == "test@test.com"))!.EmailConfirmationCode;
        
        var userId = (await _dbContext.UserCredentials.FirstOrDefaultAsync(credential => credential.Email == "test@test.com"))!.Id;

        var confirmationRequest = new ConfirmEmailRequest
        {
            UserId = userId,
            EmailCode = confirmationCode!
        };

        var requestSerialised = JsonConvert.SerializeObject(confirmationRequest);
        
        var client = _factory.CreateClient();
        //Act
        
        var response = await client.PostAsync(url, new StringContent(requestSerialised, Encoding.UTF8, "application/json"));

        //Assert
        
        Assert.True(response.IsSuccessStatusCode);
    }
    
    [Theory, Priority(3)]
    [InlineData("/api/v1/auth/login")]
    public async Task Test_Login_IsCorrect(string url)
    {
        //Arrange
        var loginRequest = new
        {
            Login = "test@test.com",
            Password = "testtetstest1A"
        };
        
        var requestSerialised = JsonConvert.SerializeObject(loginRequest);
        
        var client = _factory.CreateClient();
        
        //Act
        
        var response = await client.PostAsync(url, new StringContent(requestSerialised, Encoding.UTF8, "application/json"));

        var tokensCount = await _dbContext.Tokens.CountAsync();

        //Assert
        
        _testOutputHelper.WriteLine(response.StatusCode.ToString());
        
        _testOutputHelper.WriteLine(await new StreamReader(await response.Content.ReadAsStreamAsync()).ReadToEndAsync());
        
        Assert.True(response.IsSuccessStatusCode);
        
        Assert.True(tokensCount > 0);
    }

    [Theory, Priority(4)]
    [InlineData("/api/v1/auth/reset-password")]
    public async Task Test_ResetPassword_IsCorrect(string url)
    {
        //Arrange
        
        var email = "test@test.com";
        
        var client = _factory.CreateClient();
        
        //Act

        var response = await client.PostAsync($"{url}?email={email}", null);
        
        //Assert
        
        Assert.True(response.IsSuccessStatusCode);
    }

}