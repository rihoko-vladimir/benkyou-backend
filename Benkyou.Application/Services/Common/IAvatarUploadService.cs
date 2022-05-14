namespace Benkyou.Application.Services.Common;

public interface IAvatarUploadService
{
    public Task<string> UploadFileAsync(MemoryStream fileStream);

    public Task DeleteFileAsync(string fileUrl);
}