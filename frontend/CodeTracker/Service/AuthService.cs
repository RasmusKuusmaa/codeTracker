using CodeTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeTracker.Service
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService()
        {
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri($"{UserSession.BaseUrl}auth/")
            };
        }

        public async Task<bool> RegisterAsync(string username, string password)
        {
            var payload = new
            {
                username = username,
                password = password,
            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync("/register", content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            var payload = new
            {
                username = username,
                password = password,
            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync("login", content);

            if (!response.IsSuccessStatusCode)
                return false;

            var responseBody = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var result = JsonSerializer.Deserialize<LoginResponse>(responseBody, options);

            if (result?.Success == true && result.Data != null)
            {
                UserSession.CurrentUser = new User
                {
                    UserId = result.Data.UserId,
                    UserName = result.Data.Username,
                    Token = result.Data.Token
                };

                return true;
            }

            return false;
        }
    }
}
