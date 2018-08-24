using Newtonsoft.Json;

namespace OccupancyMonitor
{
    public class Profile
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("areaName")]
        public string AreaName { get; set; }

        /// <summary>
        /// The point state will be set when the current number of occupants is less than or equal to this number.
        /// </summary>
        [JsonProperty("trigger")]
        public int Trigger { get; set; }

        [JsonProperty("state")]
        public bool State { get; set; }

        [JsonProperty("pointId")]
        public int PointID { get; set; }
    }
}
