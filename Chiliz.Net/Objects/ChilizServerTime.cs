using System;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Chiliz.Net.Objects
{
    public class ChilizServerTime
    {
        [JsonProperty("serverTime"), JsonConverter(typeof(TimestampConverter))]
        public DateTime ServerTime { get; set; }
    }
}
