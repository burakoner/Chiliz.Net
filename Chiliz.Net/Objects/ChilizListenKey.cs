using Newtonsoft.Json;

namespace Chiliz.Net.Objects
{
    internal class ChilizListenKey
    {
        [JsonProperty("listenKey")]
        public string ListenKey { get; set; } = "";
    }
}
