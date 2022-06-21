namespace Users.Api.Models.Constants;

public static class SqlCommand
{
    public const string GetUserQuery = "exec getUserById @userId";
    public const string CreateUserQuery = "exec createUser @userId, @userName, @firstName, @lastName";

    public const string UpdateUserQuery =
        "exec updateUser @userId, @firstName, @lastName, @userName, @birthDay, @isAccountPublic, @about";

    public const string UpdateUserAvatarQuery = "exec updateUserAvatar @userId, @avatarUrl";
}