using Newtonsoft.Json;

namespace ExchangeSharp.API.Exchanges.Ndax.Models
{
    public class GenericResponse
    {
        [JsonProperty("result")]
        public bool Result { get; set; }
        [JsonProperty("errormsg")]
        public string ErrorMsg { get; set; }
        [JsonProperty("errorcode")]
        public int ErrorCode { get; set; }
        [JsonProperty("detail")]
        public int Detail { get; set; }
    }
}