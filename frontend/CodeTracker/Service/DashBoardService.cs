using CodeTracker.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CodeTracker.Service
{
    public class DashBoardService
    {
        HttpClient _httpClient = new HttpClient()
        {
            BaseAddress = new Uri($"{UserSession.BaseUrl}user/")

        };

        public async Task<List<Session>> GetSessionsAsync()
        {
            if (UserSession.CurrentUser == null || string.IsNullOrEmpty(UserSession.CurrentUser.Token))
                throw new InvalidOperationException("User is not logged in.");

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", UserSession.CurrentUser.Token);

            var response = await _httpClient.GetAsync("sessions");

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<SessionResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result?.Data ?? new List<Session>();
        }
    }
}
