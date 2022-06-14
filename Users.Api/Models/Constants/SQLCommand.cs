namespace Users.Api.Models.Constants;

public static class SqlCommand
{
    internal const string GetUserQuery = "exec getUserById @userId";
    internal const string CreateUserQuery = "exec createUser @userId, @userName, @firstName, @lastName";

    internal const string UpdateUserQuery =
        "exec updateUser @userId, @firstName, @lastName, @userName, @birthDay, @isAccountPublic, @about";

    internal const string UpdateUserAvatarQuery = "exec updateUserAvatar @userId, @avatarUrl";
}