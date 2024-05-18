namespace Users.Api.Models.Requests;

public class UpdateUserInfoRequest
{
    public string FirstName { get; init; }

    public string LastName { get; init; }

    public string UserName { get; init; }

    public DateTime? BirthDay { get; init; }

    public bool IsAccountPublic { get; init; }

    public string About { get; init; }
}