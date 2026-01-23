namespace RGAuth.Models
{
    using Newtonsoft.Json;

    public class VeryVanillaStatus
    {
        public VeryVanillaStatus()
        {
        }

        [JsonProperty("Live")]
        public string MQLive
        {
            get;
            set;
        }

        [JsonProperty("Test")]
        public string MQTest
        {
            get;
            set;
        }

        [JsonProperty("LiveLegacy")]
        public string LiveLegacy
        {
            get;
            set;
        }

        [JsonProperty("TestLegacy")]
        public string TestLegacy
        {
            get;
            set;
        }

        [JsonProperty("Emu")]
        public string Emu
        {
            get;
            set;
        }

        [JsonProperty("Beta")]
        public string Beta
        {
            get;
            set;
        }
    }
}