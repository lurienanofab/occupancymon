using Newtonsoft.Json;
using System;

namespace OccupancyMonitor
{
    public class LogEntry
    {
        [JsonProperty("entryDateTime")]
        public DateTime EntryDateTime { get; set; }

        [JsonProperty("areaName")]
        public string AreaName { get; set; }

        [JsonProperty("trigger")]
        public int Trigger { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("state")]
        public bool State { get; set; }

        [JsonProperty("error")]
        public bool Error { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("statusCode")]
        public int StatusCode { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
