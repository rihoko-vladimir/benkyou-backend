using AutoMapper;
using Sets.Api.Common.MappingProfiles;
using Sets.Api.Interfaces.Services;
using Sets.Api.Models.Entities;
using Sets.Api.Models.Requests;
using Sets.Api.Models.Responses;
using Sets.Api.Services;
using Sets.Tests.Repos;

namespace Sets.Tests;

public class SetsServiceTest
{
    private readonly List<Kanji> _kanjiList = new()
    {
        new Kanji
        {
            Id = Guid.NewGuid(),
            KanjiChar = "A",
            KunyomiReadings = new List<Kunyomi>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Reading = "P"
                }
            },
            OnyomiReadings = new List<Onyomi>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Reading = "P"
                }
            }
        },
        new Kanji
        {
            Id = Guid.NewGuid(),
            KanjiChar = "B",
            KunyomiReadings = new List<Kunyomi>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Reading = "P"
                }
            },
            OnyomiReadings = new List<Onyomi>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Reading = "P"
                }
            }
        },
        new Kanji
        {
            Id = Guid.NewGuid(),
            KanjiChar = "C",
            KunyomiReadings = new List<Kunyomi>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Reading = "P"
                }
            },
            OnyomiReadings = new List<Onyomi>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Reading = "P"
                }
            }
        }
    };

    private readonly IMapper _mapper;
    private readonly ISetsService _service;

    public SetsServiceTest()
    {
        _mapper = new MapperConfiguration(expression => { expression.AddProfile<ApplicationProfile>(); })
            .CreateMapper();

        _service = new SetsService(new MockedSetsRepo(), _mapper, new SenderService(null!));
    }

    [Fact]
    public async Task CreateSetAsyncTest()
    {
        //Arrange

        var authorId = Guid.NewGuid();

        var expectedResult = new SetResponse
        {
            Name = "Test",
            Description = "Test",
            AuthorId = authorId,
            KanjiList = _mapper.Map<List<KanjiResponse>>(_kanjiList)
        };
        var setRequest = new SetRequest
        {
            Name = expectedResult.Name,
            Description = expectedResult.Description,
            KanjiList = _mapper.Map<List<KanjiRequest>>(_kanjiList)
        };

        //Act

        var createdSet = (await _service.CreateSetAsync(authorId, setRequest)).Value;

        //Assert

        Assert.Equal(expectedResult.Name, createdSet.Name);
        Assert.Equal(expectedResult.Description, createdSet.Description);
        Assert.Equal(expectedResult.KanjiList.Count, expectedResult.KanjiList.Count);
    }

    [Fact]
    public async Task PatchSetAsyncTest()
    {
        //Arrange

        var authorId = Guid.NewGuid();

        var expectedResult = new SetResponse
        {
            Name = "Test",
            Description = "Test",
            AuthorId = authorId,
            KanjiList = _mapper.Map<List<KanjiResponse>>(_kanjiList)
        };
        var setRequest = new SetRequest
        {
            Name = expectedResult.Name,
            Description = expectedResult.Description,
            KanjiList = _mapper.Map<List<KanjiRequest>>(_kanjiList)
        };
        var createdSet = (await _service.CreateSetAsync(authorId, setRequest)).Value;

        var mappedSet = _mapper.Map<Set>(createdSet);
        mappedSet.Name = "Test 123";

        //Act

        var patchedSet = (await _service.PatchSetAsync(authorId, createdSet.Id, mappedSet)).Value;

        //Assert

        Assert.Equal(_mapper.Map<SetResponse>(mappedSet).Name, patchedSet.Name);
    }

    [Fact]
    public async Task RemoveSetAsyncTest()
    {
        //Arrange

        var authorId = Guid.NewGuid();

        var expectedResult = new SetResponse
        {
            Name = "Test",
            Description = "Test",
            AuthorId = authorId,
            KanjiList = _mapper.Map<List<KanjiResponse>>(_kanjiList)
        };
        var setRequest = new SetRequest
        {
            Name = expectedResult.Name,
            Description = expectedResult.Description,
            KanjiList = _mapper.Map<List<KanjiRequest>>(_kanjiList)
        };

        var createdSet = (await _service.CreateSetAsync(authorId, setRequest)).Value;

        //Act

        await _service.RemoveSetAsync(authorId, createdSet.Id);

        //Assert

        Assert.Null((await _service.GetSetAsync(createdSet.Id)).Value);
    }
}