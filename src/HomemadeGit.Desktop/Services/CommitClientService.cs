using HomemadeGit.Core.DTOs;
using HomemadeGit.Core.DTOs.Commits;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace HomemadeGit.Desktop.Services
{
    public class CommitClientService
    {
        private readonly HttpClient _httpClient;

        public CommitClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5190/");
        }

        public async Task<List<CommitListItemResponse>> GetRepositoryCommitsAsync(int userId, int repositoryId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/repositories/{repositoryId}/commits");

            request.Headers.Add("X-User-Id", userId.ToString());

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<CommitListItemResponse>>() ?? new List<CommitListItemResponse>();
        }

        public async Task<CommitResponse?> GetCommitByIdAsync(int userId, int commitId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/commits/{commitId}");

            request.Headers.Add("X-User-Id", userId.ToString());

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<CommitResponse>();
        }

        public async Task<CommitResponse?> CreateCommitFormFolderAsync(int userId, int repositoryId, string folderPath, int branchId, string title, string? description)
        {
            var files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories).Where(file => !ShouldSkipFile(folderPath, file))
                .Select(file => new CreateCommitFileRequest
                {
                    Path = Path.GetRelativePath(folderPath, file).Replace('\\', '/'),
                    Data = File.ReadAllBytes(file)
                }).ToList();

            var request = new CreateCommitRequest
            {
                Title = title,
                Description = description,
                Files = files
            };

            var httpRequest = new HttpRequestMessage(
                HttpMethod.Post,
                $"api/repositories/{repositoryId}/branches/{branchId}/commits");

            httpRequest.Headers.Add("X-User-Id", userId.ToString());
            httpRequest.Content = JsonContent.Create(request);

            var response = await _httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<CommitResponse>();
        }

        public async Task<RepositorySnapshotResponse?> CloneRepositoryAsync(int userId, int repositoryId, int? branchId = null)
        {
            var url = branchId.HasValue ? $"api/repositories/{repositoryId}/clone?branchId={branchId.Value}" :
                                          $"api/repositories/{repositoryId}/clone";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("X-User-Id", userId.ToString());

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<RepositorySnapshotResponse>();
        }

        public async Task ResetBranchToCommitAsync(int userId, int repositoryId, int branchId, int commitId)
        {
            var requestBody = new ResetBranchRequest { CommitId = commitId };

            var request = new HttpRequestMessage(HttpMethod.Post, $"api/repositories/{repositoryId}/branches/{branchId}/reset");
            request.Headers.Add("X-User-Id", userId.ToString());
            request.Content = JsonContent.Create(requestBody);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task<RepositorySnapshotResponse?> GetCommitSnapshotAsync(int userId, int commitId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/commits/{commitId}/snapshot");

            request.Headers.Add("X-User-Id", userId.ToString());

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<RepositorySnapshotResponse>();
        }

        private bool ShouldSkipFile(string rootFolder, string filePath)
        {
            var relativePath = Path.GetRelativePath(rootFolder, filePath)
                .Replace('\\', '/')
                .ToLowerInvariant();

            return relativePath.StartsWith(".git/") ||
                   relativePath.StartsWith("bin/") ||
                   relativePath.StartsWith("obj/") ||
                   relativePath.StartsWith("/bin/") ||
                   relativePath.StartsWith("/obj/") ||
                   relativePath.StartsWith(".vs/");
        }
    }
}
