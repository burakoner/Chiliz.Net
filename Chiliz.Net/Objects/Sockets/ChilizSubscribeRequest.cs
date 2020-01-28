using System;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Chiliz.Net.Objects.Sockets
{
    public class ChilizSocketRequest
    {
        [JsonIgnore]
        public string? Id { get; set; }
    }

    public class ChilizSubscribeRequest: ChilizSocketRequest
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; } = "";

        [JsonProperty("topic")]
        public string Topic { get; set; } = "";

        [JsonProperty("event")]
        public string Event { get; set; } = "";

        [JsonProperty("params")]
        public ChilizSubscribeParams Params { get; set; } = new ChilizSubscribeParams();
    }

    public class ChilizSubscribeParams
    {
        //[JsonProperty("limit")]
        //public int Limit { get; set; }

        [JsonProperty("binary")]
        public bool Binary { get; set; } = false;

        [JsonProperty("klineType")]
        public string KlineType { get; set; } = "";
    }
}
