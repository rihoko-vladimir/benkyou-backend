using System.Text.RegularExpressions;
using Benkyou.Application.Services.Common;
using Firebase.Auth;
using Firebase.Storage;
using Microsoft.Extensions.Configuration;

namespace Benkyou.Infrastructure.Services;

public class AvatarUploadService : IAvatarUploadService
{
    private readonly string _apiKey;
    private readonly string _email;
    private readonly string _password;

    public AvatarUploadService(IConfiguration configuration)
    {
        _apiKey = configuration.GetSection("Firebase")["Key"] ?? string.Empty;
        _email = configuration.GetSection("Firebase")["Email"] ?? string.Empty;
        _password = configuration.GetSection("Firebase")["Password"] ?? string.Empty;
    }

    public async Task<string> UploadFileAsync(MemoryStream fileStream)
    {
        var authInfo = new FirebaseAuthProvider(new FirebaseConfig(_apiKey));
        var a = await authInfo.SignInWithEmailAndPasswordAsync(_email, _password);
        return await new FirebaseStorage("benkyou-18d11.appspot.com", new FirebaseStorageOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                ThrowOnCancel = true
            })
            .Child("avatars")
            .Child(Guid.NewGuid().ToString())
            .PutAsync(fileStream);
    }

    public async Task DeleteFileAsync(string fileUrl)
    {
        var fileName = new Regex(@"%2F([^?]+)").Match(fileUrl).Groups[1].Value;
        var authInfo = new FirebaseAuthProvider(new FirebaseConfig(_apiKey));
        var a = await authInfo.SignInWithEmailAndPasswordAsync(_email, _password);
        await new FirebaseStorage("benkyou-18d11.appspot.com", new FirebaseStorageOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                ThrowOnCancel = true
            })
            .Child("avatars")
            .Child(fileName)
            .DeleteAsync();
    }
}