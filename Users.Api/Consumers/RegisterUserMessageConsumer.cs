using AutoMapper;
using MassTransit;
using Serilog;
using Shared.Models.Messages;
using Users.Api.Interfaces.Services;
using Users.Api.Models.Entities;

namespace Users.Api.Consumers;

public class RegisterUserMessageConsumer : IConsumer<RegisterUserMessage>
{
    private readonly IUserInformationService _userInformationService;
    private readonly IMapper _mapper;

    public RegisterUserMessageConsumer(IUserInformationService userInformationService, IMapper mapper)
    {
        _userInformationService = userInformationService;
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<RegisterUserMessage> context)
    {
        var user = context.Message;

        var userInformation = _mapper.Map<UserInformation>(user);
        
        Log.Information("Received new user information with id: {UserId}", userInformation.Id);
        
        await _userInformationService.CreateUserAsync(userInformation);
    }
}