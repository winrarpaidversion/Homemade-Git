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
    }
}
