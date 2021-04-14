using Newtonsoft.Json;

namespace src.Models.ResponseModel
{
    public class OneResponse<TValue>
    {
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "data")]
        public TValue Data { get; set; }
    }
}
