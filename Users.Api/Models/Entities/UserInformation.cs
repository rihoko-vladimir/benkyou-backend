namespace Users.Api.Models.Entities;

public class UserInformation
{
    public Guid Id { get; init; }

    public string FirstName { get; init; }

    public string LastName { get; init; }

    public string UserName { get; init; }

    public string UserRole { get; init; }

    public DateTime? BirthDay { get; init; }

    public string AvatarUrl { get; init; }

    public bool IsTermsAccepted { get; init; }

    public bool IsAccountPublic { get; init; }

    public string About { get; init; }

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