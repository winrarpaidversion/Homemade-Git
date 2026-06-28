using HomemadeGit.Core.DTOs.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace HomemadeGit.Desktop.Services
{
    public class RepositoryClientService
    {
        private readonly HttpClient _httpClient;

        public RepositoryClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5000");
        }

        // Получение списка репозиториев
        public async Task<List<RepositoryListItemResponse>?> GetRepositoriesAsync(int userId, string? query = null)
        {
            try
            {
                string url = "/api/repositories";
                if (!string.IsNullOrWhiteSpace(query))
                {
                    url += $"?query={Uri.EscapeDataString(query)}";
                }

                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("X-User-Id", userId.ToString());

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<RepositoryListItemResponse>>();
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"API Error: {response.StatusCode} - {errorContent}");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetRepositories Exception: {ex.Message}");
                return null;
            }
        }

        // Получение одного репозитория по ID
        public async Task<RepositoryResponse?> GetRepositoryAsync(int repositoryId, int userId)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"/api/repositories/{repositoryId}");
                request.Headers.Add("X-User-Id", userId.ToString());

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<RepositoryResponse>();
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"API Error: {response.StatusCode} - {errorContent}");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetRepository Exception: {ex.Message}");
                return null;
            }
        }

        // Создание репозитория
        public async Task<RepositoryResponse?> CreateRepositoryAsync(int userId, string title, string? description, bool isPublic)
        {
            try
            {
                var requestBody = new CreateRepositoryRequest
                {
                    Name = title,
                    isPublic = isPublic,
                    Description = description
                };

                var request = new HttpRequestMessage(HttpMethod.Post, "/api/repositories");
                request.Headers.Add("X-User-Id", userId.ToString());
                request.Content = JsonContent.Create(requestBody);

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<RepositoryResponse>();
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"API Error: {response.StatusCode} - {errorContent}");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CreateRepository Exception: {ex.Message}");
                return null;
            }
        }

        // Обновление репозитория
        public async Task<RepositoryResponse?> UpdateRepositoryAsync(int repositoryId, int userId, string title, string? description, bool isPublic)
        {
            try
            {
                var requestBody = new UpdateRepositoryRequest
                {
                    Name = title,
                    isPublic = isPublic,
                    Description = description
                };

                var request = new HttpRequestMessage(HttpMethod.Put, $"/api/repositories/{repositoryId}");
                request.Headers.Add("X-User-Id", userId.ToString());
                request.Content = JsonContent.Create(requestBody);

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<RepositoryResponse>();
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"API Error: {response.StatusCode} - {errorContent}");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"UpdateRepository Exception: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DeleteRepositoryAsync(int repositoryId, int userId)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/repositories/{repositoryId}");
                request.Headers.Add("X-User-Id", userId.ToString());

                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DeleteRepository Exception: {ex.Message}");
                return false;
            }
        }
    }
}