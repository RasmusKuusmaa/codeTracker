using CodeTracker.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace CodeTracker.Service
{
    public class LanguageService
    {
        private readonly HttpClient _httpClient = new HttpClient()
        {
            BaseAddress = new Uri($"{UserSession.BaseUrl}user/")
        };

        private void EnsureAuthorized()
        {
            if (UserSession.CurrentUser == null || string.IsNullOrEmpty(UserSession.CurrentUser.Token))
                throw new InvalidOperationException("User is not logged in.");

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", UserSession.CurrentUser.Token);
        }

        public async Task<List<Language>> GetLanguagesAsync()
        {
            var response = await _httpClient.GetAsync("languages");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<LanguageResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result?.Data ?? new List<Language>();
        }

        public async Task<List<LanguageStats>> GetLanguageStatsAsync()
        {
            EnsureAuthorized();

            var response = await _httpClient.GetAsync("languages/stats");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<LanguageStatsResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result?.Data ?? new List<LanguageStats>();
        }
    }
}
