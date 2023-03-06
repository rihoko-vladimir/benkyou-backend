using System.Text;
using Newtonsoft.Json;
using Xunit.Abstractions;
using Xunit.Priority;

namespace Users.Api.Integration.Users;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class UsersTests
{
    private readonly UsersTestApplication _factory;
    private readonly ITestOutputHelper _testOutputHelper;
    private Guid _userId;

    public UsersTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _factory = new UsersTestApplication();

        Guid.TryParse("7970497D-2359-42CF-E4D3-08DA70A125B6", out _userId);
    }

    [Theory]
    [Priority(1)]
    [InlineData("/api/v1/user/get-info")]
    public async Task Test_GetInfo_IsCorrect(string url)
    {
        //Arrange

        var loginRequest = new
        {
            Login = "test@test.com",
            Password = "testtesttesttest1A"
        };

        var loginSerialized = JsonConvert.SerializeObject(loginRequest);

        var client = _factory.CreateClient();

        var httpClient = new HttpClient();

        var loginResponse = await httpClient.PostAsync("http://host.docker.internal:7080/api/v1/auth/login",
            new StringContent(loginSerialized, Encoding.UTF8, "application/json"));

        var accessToken = loginResponse.Headers.Single(header => header.Key == "Set-Cookie").Value
            .Single(value => value.Contains("access"));

        client.DefaultRequestHeaders.Add("Cookie",
            accessToken);

        //Act

        var response = await client.GetAsync(url);

        //Assert

        _testOutputHelper.WriteLine(response.StatusCode.ToString());

        _testOutputHelper.WriteLine(await new StreamReader(await response.Content.ReadAsStreamAsync())
            .ReadToEndAsync());

        Assert.True(response.IsSuccessStatusCode);
    }

    [Theory]
    [Priority(2)]
    [InlineData("/api/v1/user/update-info")]
    public async Task Test_UpdateInfo_IsCorrect(string url)
    {
        //Arrange

        var loginRequest = new
        {
            Login = "test@test.com",
            Password = "testtesttesttest1A"
        };

        var patchRequest = new[]
        {
            new
            {
                path = "/firstName",
                op = "replace",
                value = "Uladzimir"
            }
        };

        var loginSerialized = JsonConvert.SerializeObject(loginRequest);

        var patchSerialized = JsonConvert.SerializeObject(patchRequest);

        var client = _factory.CreateClient();

        var httpClient = new HttpClient();

        var loginResponse = await httpClient.PostAsync("http://host.docker.internal:7080/api/v1/auth/login",
            new StringContent(loginSerialized, Encoding.UTF8, "application/json"));

        var accessToken = loginResponse.Headers.Single(header => header.Key == "Set-Cookie").Value
            .Single(value => value.Contains("access"));

        client.DefaultRequestHeaders.Add("Cookie",
            accessToken);

        //Act

        var response =
            await client.PatchAsync(url, new StringContent(patchSerialized, Encoding.UTF8, "application/json"));

        //Assert

        _testOutputHelper.WriteLine(response.StatusCode.ToString());

        _testOutputHelper.WriteLine(await new StreamReader(await response.Content.ReadAsStreamAsync())
            .ReadToEndAsync());

        Assert.True(response.IsSuccessStatusCode);
    }

    [Theory]
    [Priority(3)]
    [InlineData("/api/v1/user/upload-avatar")]
    public async Task Test_UploadAvatar_IsCorrect(string url)
    {
        //Arrange

        var loginRequest = new
        {
            Login = "test@test.com",
            Password = "testtesttesttest1A"
        };

        var loginSerialized = JsonConvert.SerializeObject(loginRequest);

        var client = _factory.CreateClient();

        var httpClient = new HttpClient();

        var loginResponse = await httpClient.PostAsync("http://host.docker.internal:7080/api/v1/auth/login",
            new StringContent(loginSerialized, Encoding.UTF8, "application/json"));

        var accessToken = loginResponse.Headers.Single(header => header.Key == "Set-Cookie").Value
            .Single(value => value.Contains("access"));

        client.DefaultRequestHeaders.Add("Cookie",
            accessToken);

        await using var stream = File.OpenRead("../../../Users/maxresdefault.jpg");

        using var request = new HttpRequestMessage(HttpMethod.Put, url);

        using var content = new MultipartFormDataContent
        {
            {
                new StreamContent(stream), "formFile", "maxresdefault.jpg"
            }
        };

        request.Content = content;

        //Act

        var response = await client.SendAsync(request);

        //Assert

        _testOutputHelper.WriteLine(response.StatusCode.ToString());

        _testOutputHelper.WriteLine(await new StreamReader(await response.Content.ReadAsStreamAsync())
            .ReadToEndAsync());

        Assert.True(response.IsSuccessStatusCode);
    }
}