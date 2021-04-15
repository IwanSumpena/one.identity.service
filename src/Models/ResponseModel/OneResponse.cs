using Newtonsoft.Json;
using src.Utilities;

namespace src.Models.ResponseModel
{
    public class OneResponse<TValue>
    {
        public OneResponse()
        {
            Status = AppConstans.Response_Status_Failed;
        }
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; } 

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "data")]
        public TValue Data { get; set; }
    }
}
