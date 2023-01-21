namespace Users.Api.Models.Requests;

public class UpdateUserInfoRequest
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string UserName { get; set; }

    public DateTime? BirthDay { get; set; }

    public bool IsAccountPublic { get; set; }

    public string About { get; set; }
}