namespace Users.Api.Models.Entities;

public class UserInformation
{
    public Guid Id { get; set; }
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string UserName { get; set; }
    
    public string UserRole { get; set; }

    public DateTime? BirthDay { get; set; } = null;
    
    public string AvatarUrl { get; set; }
    
    public bool IsTermsAccepted { get; set; }
    
    public bool IsAccountPublic { get; set; }
    
    public string About { get; set; }
}