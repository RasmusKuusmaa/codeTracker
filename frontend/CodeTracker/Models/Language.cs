using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CodeTracker.Models
{
    public class Language
    {
        [JsonPropertyName("language_id")]
        public int LanguageId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class LanguageResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("data")]
        public List<Language> Data { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }

    public class LanguageStats
    {
        [JsonPropertyName("language_id")]
        public int LanguageId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("session_count")]
        public int SessionCount { get; set; }

        [JsonPropertyName("total_seconds")]
        public int TotalSeconds { get; set; }

        public TimeSpan TotalDuration => TimeSpan.FromSeconds(TotalSeconds);

        public string FormattedTotalTime
        {
            get
            {
                var duration = TotalDuration;
                if (duration.TotalHours >= 1)
                    return $"{(int)duration.TotalHours}h {duration.Minutes}m";
                else if (duration.TotalMinutes >= 1)
                    return $"{duration.Minutes}m";
                else
                    return $"{duration.Seconds}s";
            }
        }
    }

    public class LanguageStatsResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("data")]
        public List<LanguageStats> Data { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
