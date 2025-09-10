namespace OrganizerPRO.Application.Common.Models;

public class UploadRequest
{
    public UploadRequest(string fileName, UploadType uploadType, byte[] data, bool overwrite = false,string? folder=null)
    {
        FileName = fileName;
        UploadType = uploadType;
        Data = data;
        Overwrite = overwrite;
        Folder = folder;
    }
    public string FileName { get; set; }
    public string? Extension { get; set; }
    public UploadType UploadType { get; set; }
    public bool Overwrite { get; set; }
    public byte[] Data { get; set; }
    public string? Folder { get; set; }
}