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

        public async Task<List<Session>> GetSessionsAsync()
        {
            EnsureAuthorized();

            var response = await _httpClient.GetAsync("sessions");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<SessionResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result?.Data ?? new List<Session>();
        }

        public async Task<Session?> GetActiveSessionAsync()
        {
            EnsureAuthorized();

            var response = await _httpClient.GetAsync("sessions/active");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<SingleSessionResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result?.Data;
        }

        public async Task<Session?> GetLastSessionAsync()
        {
            EnsureAuthorized();

            var response = await _httpClient.GetAsync("sessions/last");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<SingleSessionResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result?.Data;
        }

        public async Task<Session> CreateSessionAsync(CreateSessionRequest request)
        {
            EnsureAuthorized();

            var content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync("sessions", content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<SingleSessionResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result?.Data ?? throw new Exception("Failed to create session");
        }

        public async Task<Session> PauseSessionAsync(int sessionId, int durationSeconds)
        {
            EnsureAuthorized();

            var content = new StringContent(
                JsonSerializer.Serialize(new { duration_seconds = durationSeconds }),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync($"sessions/{sessionId}/pause", content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<SingleSessionResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result?.Data ?? throw new Exception("Failed to pause session");
        }

        public async Task<Session> ResumeSessionAsync(int sessionId)
        {
            EnsureAuthorized();

            var response = await _httpClient.PostAsync($"sessions/{sessionId}/resume", null);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<SingleSessionResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result?.Data ?? throw new Exception("Failed to resume session");
        }

        public async Task<Session> StopSessionAsync(int sessionId, int durationSeconds)
        {
            EnsureAuthorized();

            var content = new StringContent(
                JsonSerializer.Serialize(new { duration_seconds = durationSeconds }),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync($"sessions/{sessionId}/stop", content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<SingleSessionResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result?.Data ?? throw new Exception("Failed to stop session");
        }

        public async Task<List<Session>> GetSessionsByProjectIdAsync(int projectId)
        {
            EnsureAuthorized();

            var response = await _httpClient.GetAsync($"sessions/project/{projectId}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<SessionResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result?.Data ?? new List<Session>();
        }

        public async Task<List<Session>> GetSessionsByLanguageIdAsync(int languageId)
        {
            EnsureAuthorized();

            var response = await _httpClient.GetAsync($"sessions/language/{languageId}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<SessionResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result?.Data ?? new List<Session>();
        }
    }
}
