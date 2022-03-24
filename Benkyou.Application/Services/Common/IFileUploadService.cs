namespace Benkyou.Application.Services.Common;

public interface IFileUploadService
{
    public Task<string> UploadFileAsync(MemoryStream fileStream);

    public Task DeleteFileAsync(string fileName);
}