using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace HomemadeGit.Desktop.Services
{
    public class NewRepositoryClientService
    { 
        private readonly HttpClient _httpClient;
        public NewRepositoryClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5000");
        }
        //public async Task<List<string>> NewRepository(string Title, bool isOpen )
        //{
        //    var request = new HttpRequestMessage(HttpMethod.Post, "api/repository/todo");
       
        //    request.Content = new StringContent(Title, Encoding.UTF8, "application/json");
        //    var request2 = new Content
        //    var response = await _httpClient.SendAsync(request);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        return response.Content.Headers.Add(to;
        //    }
        //    return null;

        //}
    }
}
