namespace OrganizerPRO.Web.Services.JsInterop;

public class BlazorDownloadFileService
{
    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference? _module;

    public BlazorDownloadFileService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task DownloadFileAsync(string fileName, byte[] data, string contentType)
    {
        // Load the JavaScript module from wwwroot/js/downloadFile.js if not already loaded
        if (_module == null)
        {
            _module = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/downloadFile.js");
        }

        // Convert the byte array to a Base64 string
        string base64Data = Convert.ToBase64String(data);

        // Call the JavaScript function to trigger the file download
        await _module.InvokeVoidAsync("downloadFile", fileName, base64Data, contentType);
    }
}

