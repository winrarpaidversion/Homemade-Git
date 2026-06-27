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
    }
}
