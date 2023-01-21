namespace Users.Api.Models.Entities;

public class UserInformation
{
    public Guid Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string UserName { get; set; }

    public string UserRole { get; set; }

    public DateTime? BirthDay { get; set; }

    public string AvatarUrl { get; set; }

    public bool IsTermsAccepted { get; set; }

    public bool IsAccountPublic { get; set; }

    public string About { get; set; }

    protected bool Equals(UserInformation other)
    {
        return Id.Equals(other.Id) && FirstName == other.FirstName && LastName == other.LastName &&
               UserName == other.UserName && UserRole == other.UserRole && Nullable.Equals(BirthDay, other.BirthDay) &&
               AvatarUrl == other.AvatarUrl && IsTermsAccepted == other.IsTermsAccepted &&
               IsAccountPublic == other.IsAccountPublic && About == other.About;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((UserInformation)obj);
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Id);
        hashCode.Add(FirstName);
        hashCode.Add(LastName);
        hashCode.Add(UserName);
        hashCode.Add(UserRole);
        hashCode.Add(BirthDay);
        hashCode.Add(AvatarUrl);
        hashCode.Add(IsTermsAccepted);
        hashCode.Add(IsAccountPublic);
        hashCode.Add(About);
        return hashCode.ToHashCode();
    }
}