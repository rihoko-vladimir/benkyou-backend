using MassTransit;
using Serilog;
using Shared.Models.Messages;
using Users.Api.Interfaces.Services;
using Users.Api.Models.Entities;

namespace Users.Api.Consumers;

public class RegisterUserMessageConsumer : IConsumer<RegisterUserMessage>
{
    private readonly IUserInformationService _userInformationService;

    public RegisterUserMessageConsumer(IUserInformationService userInformationService)
    {
        _userInformationService = userInformationService;
    }

    public async Task Consume(ConsumeContext<RegisterUserMessage> context)
    {
        var user = context.Message;
        
        var userInformation = new UserInformation
        {
            Id = user.UserId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName,
            IsTermsAccepted = user.IsTermsAccepted
        };
        
        Log.Information("Received new user information with id: {UserId}", userInformation.Id);
        
        await _userInformationService.CreateUserAsync(userInformation);
    }
}