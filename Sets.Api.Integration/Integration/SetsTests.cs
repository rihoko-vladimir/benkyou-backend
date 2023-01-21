using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Sets.Api.Models.DbContext;
using Sets.Api.Models.Requests;
using Xunit.Abstractions;
using Xunit.Priority;

namespace Sets.Api.Integration.Integration;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class SetsTests
{
    private readonly ApplicationContext _dbContext;
    private readonly SetsTestApplication _factory;
    private readonly ITestOutputHelper _testOutputHelper;

    public SetsTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _factory = new SetsTestApplication();
        _dbContext = _factory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider
            .GetRequiredService<ApplicationContext>();
        _dbContext.Database.EnsureCreated();
    }

    [Theory]
    [Priority(1)]
    [InlineData("/api/v1/sets/create")]
    public async Task Test_Create_IsCorrect(string url)
    {
        //Arrange

        var request = new SetRequest
        {
            Name = "test",
            Description = "test test",
            KanjiList = new List<KanjiRequest>
            {
                new()
                {
                    KanjiChar = "会",
                    KunyomiReadings = new List<KunyomiRequest>
                    {
                        new()
                        {
                            Reading = "あ"
                        },
                        new()
                        {
                            Reading = "おな(じ)"
                        }
                    },
                    OnyomiReadings = new List<OnyomiRequest>
                    {
                        new()
                        {
                            Reading = "ドウ"
                        },
                        new()
                        {
                            Reading = "カイ"
                        }
                    }
                },
                new()
                {
                    KanjiChar = "同",
                    KunyomiReadings = new List<KunyomiRequest>
                    {
                        new()
                        {
                            Reading = "あ"
                        },
                        new()
                        {
                            Reading = "おな(じ)"
                        }
                    },
                    OnyomiReadings = new List<OnyomiRequest>
                    {
                        new()
                        {
                            Reading = "ドウ"
                        },
                        new()
                        {
                            Reading = "カイ"
                        }
                    }
                },
                new()
                {
                    KanjiChar = "事",
                    KunyomiReadings = new List<KunyomiRequest>
                    {
                        new()
                        {
                            Reading = "あ"
                        },
                        new()
                        {
                            Reading = "おな(じ)"
                        }
                    },
                    OnyomiReadings = new List<OnyomiRequest>
                    {
                        new()
                        {
                            Reading = "ドウ"
                        },
                        new()
                        {
                            Reading = "カイ"
                        }
                    }
                }
            }
        };

        var loginRequest = new
        {
            Login = "test@test.com",
            Password = "testtesttesttest1A"
        };

        var setSerialized = JsonConvert.SerializeObject(request);

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

        var response = await client.PostAsync(url, new StringContent(setSerialized, Encoding.UTF8, "application/json"));


        //Assert

        _testOutputHelper.WriteLine(response.StatusCode.ToString());

        _testOutputHelper.WriteLine(await new StreamReader(await response.Content.ReadAsStreamAsync())
            .ReadToEndAsync());

        Assert.True(response.IsSuccessStatusCode);
    }

    [Theory]
    [Priority(2)]
    [InlineData("/api/v1/sets/modify")]
    public async Task Test_Modify_IsCorrect(string url)
    {
        //Arrange
        var patchRequest = new[]
        {
            new
            {
                path = "/name",
                op = "replace",
                value = "Test12"
            }
        };

        var loginRequest = new
        {
            Login = "test@test.com",
            Password = "testtesttesttest1A"
        };

        var patchSerialized = JsonConvert.SerializeObject(patchRequest);

        var loginSerialized = JsonConvert.SerializeObject(loginRequest);

        var client = _factory.CreateClient();

        var httpClient = new HttpClient();

        var setId = (await _dbContext.Sets.FirstOrDefaultAsync(set => true))!.Id;

        var loginResponse = await httpClient.PostAsync("http://host.docker.internal:7080/api/v1/auth/login",
            new StringContent(loginSerialized, Encoding.UTF8, "application/json"));

        var accessToken = loginResponse.Headers.Single(header => header.Key == "Set-Cookie").Value
            .Single(value => value.Contains("access"));

        client.DefaultRequestHeaders.Add("Cookie",
            accessToken);

        //Act

        var response = await client.PatchAsync($"{url}?setId={setId}",
            new StringContent(patchSerialized, Encoding.UTF8, "application/json"));

        //Assert

        _testOutputHelper.WriteLine(response.StatusCode.ToString());

        _testOutputHelper.WriteLine(await new StreamReader(await response.Content.ReadAsStreamAsync())
            .ReadToEndAsync());

        Assert.True(response.IsSuccessStatusCode);
    }

    [Theory]
    [Priority(3)]
    [InlineData("/api/v1/sets/my-sets")]
    public async Task Test_MySets_IsCorrect(string url)
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

        var response = await client.GetAsync($"{url}?pageNumber={1}&pageSize={9}");

        //Assert

        _testOutputHelper.WriteLine(response.StatusCode.ToString());

        _testOutputHelper.WriteLine(await new StreamReader(await response.Content.ReadAsStreamAsync())
            .ReadToEndAsync());

        Assert.True(response.IsSuccessStatusCode);
    }

    [Theory]
    [Priority(4)]
    [InlineData("/api/v1/sets/all-sets")]
    public async Task Test_AllSets_IsCorrect(string url)
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

        var response = await client.GetAsync($"{url}?pageNumber={1}&pageSize={9}");

        //Assert

        _testOutputHelper.WriteLine(response.StatusCode.ToString());

        _testOutputHelper.WriteLine(await new StreamReader(await response.Content.ReadAsStreamAsync())
            .ReadToEndAsync());

        Assert.True(response.IsSuccessStatusCode);
    }
}