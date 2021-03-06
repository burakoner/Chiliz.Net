﻿using System;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Chiliz.Net.Objects.Sockets
{
    public class ChilizSocketRequest
    {
        [JsonIgnore]
        public string? Id { get; set; }
    }

    public class ChilizKlineSubscribeRequest : ChilizSocketRequest
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; } = "";

        [JsonProperty("topic")]
        public string Topic { get; set; } = "";

        [JsonProperty("event")]
        public string Event { get; set; } = "";

        [JsonProperty("params")]
        public ChilizKlineSubscribeParams Params { get; set; } = new ChilizKlineSubscribeParams();
    }

    public class ChilizKlineSubscribeParams
    {
        [JsonProperty("binary")]
        public bool Binary { get; set; } = false;

        [JsonProperty("klineType")]
        public string KlineType { get; set; } = "";
    }

    public class ChilizMarketSubscribeRequest : ChilizSocketRequest
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; } = "";

        [JsonProperty("topic")]
        public string Topic { get; set; } = "";

        [JsonProperty("event")]
        public string Event { get; set; } = "";

        [JsonProperty("params")]
        public ChilizMarketSubscribeParams Params { get; set; } = new ChilizMarketSubscribeParams();
    }

    public class ChilizMarketSubscribeParams
    {
        [JsonProperty("binary")]
        public bool Binary { get; set; } = false;
    }
}
