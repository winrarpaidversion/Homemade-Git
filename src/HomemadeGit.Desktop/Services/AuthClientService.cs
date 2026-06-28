using HomemadeGit.Core.DTOs;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
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

            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/auth/register", request);

                if (response.IsSuccessStatusCode)
                {
             
                    return await response.Content.ReadFromJsonAsync<RegisterResponse>();
                }
                else
                {
                 
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"API Error: {errorContent}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Ошибка сети или десериализации
                Debug.WriteLine($"Exception: {ex.Message}");
                return null;
            }
        }

        public async Task<LoginResponse> Login(string login, string password)
        {
            try
            {
                var request = new LoginRequest() { Login = login, Password = password };
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<LoginResponse>();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine(error);
                    return null;
                }
            }
            catch (Exception ex)
            {

                return null;


            }
    }
}
}
