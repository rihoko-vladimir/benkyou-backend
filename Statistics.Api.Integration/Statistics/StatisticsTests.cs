using System.Text;
using MongoDB.Driver;
using Newtonsoft.Json;
using Statistics.Api.Models.Configurations;
using Statistics.Api.Models.Entities;
using Statistics.Api.Repositories;
using Xunit.Priority;

namespace Statistics.Api.Integration.Statistics;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class StatisticsTests
{
    private readonly StatisticsTestApplication _factory;
    private readonly MongoClient _mongoClient;
    private readonly StatisticsRepository _statisticsRepo;
    private readonly Guid _userId;

    public StatisticsTests()
    {
        Guid.TryParse("7970497D-2359-42CF-E4D3-08DA70A125B6", out _userId);

        const string mongoConnectionString = "mongodb://test_rihoko:testtesttesttest1A@host.docker.internal:30341";

        _factory = new StatisticsTestApplication();

        _mongoClient = new MongoClient(mongoConnectionString);

        _statisticsRepo = new StatisticsRepository(new MongoConfig
        {
            ConnectionString = mongoConnectionString
        });

        _statisticsRepo.SetRegistrationDate(_userId, DateTime.Now).Wait();
    }

    [Theory]
    [Priority(1)]
    [InlineData("/api/v1/statistics/last-online")]
    public async Task Test_LastOnline_IsCorrect(string url)
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

        var response = await client.PostAsync(url, null);

        //Assert

        Assert.True(response.IsSuccessStatusCode);

        var statistics = await (await _mongoClient.GetDatabase("benkyou_statistics")
            .GetCollection<GeneralStatistics>("generalStatistics")
            .FindAsync(_ => true)).FirstOrDefaultAsync();

        Assert.NotNull(statistics);
    }

    [Theory]
    [Priority(2)]
    [InlineData("/api/v1/statistics/user-stats")]
    public async Task Test_UserStats_IsCorrect(string url)
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

        Assert.True(response.IsSuccessStatusCode);
    }


    [Theory]
    [Priority(3)]
    [InlineData("/api/v1/statistics/set-results")]
    public async Task Test_SetResults_IsCorrect(string url)
    {
        //Arrange

        var setId = Guid.NewGuid();

        var loginRequest = new
        {
            Login = "test@test.com",
            Password = "testtesttesttest1A"
        };

        var studyResults = new StudyResult[]
        {
            new()
            {
                Kanji = 'A',
                CorrectKunyomi = new[] { "A" },
                CorrectOnyomi = new[] { "A" },
                SelectedKunyomi = new[] { "A" },
                SelectedOnyomi = new[] { "A" }
            }
        };

        await _statisticsRepo.AddSetLearnResult(_userId, setId, DateTime.Now, DateTime.Now, studyResults);

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

        var response = await client.GetAsync($"{url}?setId={setId}");

        //Assert

        Assert.True(response.IsSuccessStatusCode);
    }
}