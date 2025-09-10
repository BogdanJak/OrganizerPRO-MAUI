namespace OrganizerPRO.Application.Common.Interfaces;

public interface IUploadService
{
    Task<string> UploadAsync(UploadRequest request);
    Task RemoveAsync(string filename);
}