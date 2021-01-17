using Newtonsoft.Json;

namespace Chiliz.Net.Objects
{
    public class ChilizListenKey
    {
        [JsonProperty("listenKey")]
        public string ListenKey { get; set; } = "";
    }
}
