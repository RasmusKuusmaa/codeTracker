using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CodeTracker.Models
{
    public class Project
    {
        [JsonPropertyName("project_id")]
        public int ProjectId { get; set; }

        [JsonPropertyName("user_id")]
        public int UserId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("total_time_minutes")]
        public int TotalTimeMinutes { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        public string FormattedTotalTime
        {
            get
            {
                var hours = TotalTimeMinutes / 60;
                var minutes = TotalTimeMinutes % 60;
                if (hours > 0)
                    return $"{hours}h {minutes}m";
                else
                    return $"{minutes}m";
            }
        }
    }

    public class ProjectResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("data")]
        public List<Project> Data { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }

    public class SingleProjectResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("data")]
        public Project Data { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }

    public class CreateProjectRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }
}
