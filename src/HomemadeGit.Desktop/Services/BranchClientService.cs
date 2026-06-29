using HomemadeGit.Core.DTOs.Branches;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;

namespace HomemadeGit.Desktop.Services;

public class BranchClientService
{
    private readonly HttpClient _httpClient;

    public BranchClientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("http://localhost:5190/");
    }

    public async Task<List<BranchResponse>> GetBranchesAsync(int userId, int repositoryId)
    {
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"api/repositories/{repositoryId}/branches");

        request.Headers.Add("X-User-Id", userId.ToString());

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<BranchResponse>>()
               ?? new List<BranchResponse>();
    }

    public async Task<BranchResponse?> CreateBranchAsync(
        int userId,
        int repositoryId,
        string name,
        int? sourceBranchId)
    {
        Debug.WriteLine($"Create branch request:");
        Debug.WriteLine($"RepositoryId: {repositoryId}");
        Debug.WriteLine($"Name: {name}");
        Debug.WriteLine($"SourceBranchId: {sourceBranchId}");

        var body = new CreateBranchRequest
        {
            Name = name,
            SourceBranchId = sourceBranchId
        };

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"api/repositories/{repositoryId}/branches");

        request.Headers.Add("X-User-Id", userId.ToString());
        request.Content = JsonContent.Create(body);

        var response = await _httpClient.SendAsync(request);
        //response.EnsureSuccessStatusCode();
        var responseText = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Ошибка создания ветки: {(int)response.StatusCode} {response.ReasonPhrase}. Ответ сервера: {responseText}");
        }

        return await response.Content.ReadFromJsonAsync<BranchResponse>();
    }
}