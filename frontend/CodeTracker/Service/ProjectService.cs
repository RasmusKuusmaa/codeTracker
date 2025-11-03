using CodeTracker.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CodeTracker.Service
{
    public class ProjectService
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

        public async Task<List<Project>> GetProjectsAsync()
        {
            EnsureAuthorized();

            var response = await _httpClient.GetAsync("projects");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ProjectResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result?.Data ?? new List<Project>();
        }

        public async Task<Project> GetProjectByIdAsync(int projectId)
        {
            EnsureAuthorized();

            var response = await _httpClient.GetAsync($"projects/{projectId}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<SingleProjectResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result?.Data ?? throw new Exception("Project not found");
        }

        public async Task<Project> CreateProjectAsync(CreateProjectRequest request)
        {
            EnsureAuthorized();

            var content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync("projects", content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<SingleProjectResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result?.Data ?? throw new Exception("Failed to create project");
        }

        public async Task<Project> UpdateProjectAsync(int projectId, CreateProjectRequest request)
        {
            EnsureAuthorized();

            var content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PutAsync($"projects/{projectId}", content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<SingleProjectResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result?.Data ?? throw new Exception("Failed to update project");
        }

        public async Task DeleteProjectAsync(int projectId)
        {
            EnsureAuthorized();

            var response = await _httpClient.DeleteAsync($"projects/{projectId}");
            response.EnsureSuccessStatusCode();
        }
    }
}
