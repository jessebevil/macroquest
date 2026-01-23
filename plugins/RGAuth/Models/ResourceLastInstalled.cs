using Newtonsoft.Json;

namespace RGAuth.Models
{
    class ResourceLastInstalled
    {
        [JsonProperty("resourceId")]
        public long ResourceId { get; set; }

        [JsonProperty("serverType")]
        public RGAuth.Enums.ServerType ServerType { get; set; }

        [JsonProperty("lastUpdated")]
        public long LastUpdated { get; set; }
    }
}
