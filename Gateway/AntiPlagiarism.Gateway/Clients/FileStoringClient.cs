using System.Net.Http.Headers;
using AntiPlagiarism.Gateway.Models;
using Microsoft.AspNetCore.Http;

namespace AntiPlagiarism.Gateway.Clients;

public sealed class FileStoringClient : IFileStoringClient
{
    private readonly HttpClient _httpClient;

    public FileStoringClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<StoredFileDto> UploadFileAsync(
        IFormFile file,
        CancellationToken cancellationToken = default)
    {
        await using var stream = file.OpenReadStream();

        using var content = new MultipartFormDataContent();
        var streamContent = new StreamContent(stream);

        streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

        content.Add(streamContent, "file", file.FileName);

        using var response = await _httpClient.PostAsync("/files", content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<StoredFileDto>(cancellationToken: cancellationToken);
        if (result is null)
        {
            throw new InvalidOperationException("FileStoring returned empty response.");
        }

        return result;
    }
}