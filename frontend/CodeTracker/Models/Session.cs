 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeTracker.Models
{
    public class Session
    {
        [JsonPropertyName("session_id")]
        public int SessionId { get; set; }

        [JsonPropertyName("user_id")]
        public int UserId { get; set; }

        [JsonPropertyName("project_id")]
        public int? ProjectId { get; set; }

        [JsonPropertyName("time_started")]
        public DateTime? TimeStarted { get; set; }

        [JsonPropertyName("time_ended")]
        public DateTime? TimeEnded { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("duration_seconds")]
        public int DurationSeconds { get; set; }

        [JsonPropertyName("languages")]
        public List<Language> Languages { get; set; }

        [JsonPropertyName("project_name")]
        public string? ProjectName { get; set; }

        public TimeSpan Duration => TimeSpan.FromSeconds(DurationSeconds);

        public string FormattedDuration
        {
            get
            {
                var duration = Duration;
                if (duration.TotalHours >= 1)
                    return $"{(int)duration.TotalHours}h {duration.Minutes}m";
                else if (duration.TotalMinutes >= 1)
                    return $"{duration.Minutes}m {duration.Seconds}s";
                else
                    return $"{duration.Seconds}s";
            }
        }

        public string LanguagesString => Languages != null && Languages.Any()
            ? string.Join(", ", Languages.Select(l => l.Name))
            : "None";
    }

    public class SessionResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("data")]
        public List<Session> Data { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }

    public class SingleSessionResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("data")]
        public Session Data { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }

    public class CreateSessionRequest
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("project_id")]
        public int? ProjectId { get; set; }

        [JsonPropertyName("language_ids")]
        public List<int> LanguageIds { get; set; }
    }
}
