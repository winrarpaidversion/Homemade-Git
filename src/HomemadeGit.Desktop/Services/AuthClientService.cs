using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using HomemadeGit.Core.DTOs;
namespace HomemadeGit.Desktop.Services
{

    public class AuthClientService
    {
        private readonly HttpClient _httpClient;

        public AuthClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5000");
        }
        public async Task<RegisterResponse> Register(string login, string password)
        {
            var request = new RegisterRequest() { Login = login, Password = password };
            var response = await _httpClient.PostAsJsonAsync("/api/auth/register", request);
            if (response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(error);
            }
            return await response.Content.ReadFromJsonAsync<RegisterResponse>();
                   //?? throw new InvalidOperationException("Не удалось десериализовать RegisterResponse.");
        }

        public async Task<LoginResponse> Login(string login, string password)
        {
            var request = new LoginRequest() { Login = login, Password = password };
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(error);
            }
            return await response.Content.ReadFromJsonAsync<LoginResponse>();
                   //?? throw new InvalidOperationException("Не удалось десериализовать LoginResponse.");
        }
    }
}
