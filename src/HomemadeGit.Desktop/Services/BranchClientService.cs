using HomemadeGit.Core.DTOs.Branches;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;

namespace HomemadeGit.Desktop.Services
{

    public class BranchClientService
    {    private readonly HttpClient _httpClient;
        public BranchClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5190/");
        }
        public async Task<List<BranchResponse>> GetBranchResponses(int userId, int repositoryId)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"api/repositories/{repositoryId}/branches");
                request.Headers.Add("X-User-Id", userId.ToString());
                var response = _httpClient.SendAsync(request).Result;
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<BranchResponse>>() ?? new List<BranchResponse>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error GetBranchResponses: {ex.Message}");
                return null;
            }
        }
        public async Task<List<BranchResponse>> CreateBranchInRepository(int userId, int repositoryId, string title)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"api/repositories/{repositoryId}/branches");
            var requestBody = new CreateBranchRequest()
            {
                Name = title,

            };
            try
            {
                request.Headers.Add("X-User-Id", userId.ToString());
                request.Content = JsonContent.Create(requestBody);
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<BranchResponse>>() ?? new List<BranchResponse>();

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating branch: {ex.Message}");
                return null;
            }
        }
    }
}
