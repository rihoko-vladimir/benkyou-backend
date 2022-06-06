using MassTransit;
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
        var userInformation = new UserInformation()
        {
            Id = user.UserId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName,
            IsTermsAccepted = user.IsTermsAccepted
        };
        await _userInformationService.CreateUserAsync(userInformation);
    }
}