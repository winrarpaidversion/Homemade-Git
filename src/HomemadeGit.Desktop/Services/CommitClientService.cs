using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace HomemadeGit.Desktop.Services
{
    public class CommitClientService
    {
        private readonly HttpClient _httpClient;
        public CommitClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5000");
        }
        public async Task<List<string>> NewCommit(string commit)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/commit/todo");
            request.Content = new StringContent(commit, Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                return await System.Text.Json.JsonSerializer.DeserializeAsync<List<string>>(stream);
            }
            else
            {
                throw new Exception($"Error: {response.StatusCode}");
            }
        }
    }
}
