using Newtonsoft.Json;
using System;
using CryptoExchange.Net.Converters;
using System.Collections.Generic;

namespace Chiliz.Net.Objects
{
    /// <summary>
    /// Exchange info
    /// </summary>
    public class ChilizExchangeInfo
    {
        /// <summary>
        /// The timezone the server uses
        /// </summary>
        [JsonProperty("timezone")]
        public string TimeZone { get; set; } = "";

        /// <summary>
        /// The current server time
        /// </summary>
        [JsonProperty("serverTime"), JsonConverter(typeof(TimestampConverter))]
        public DateTime ServerTime { get; set; }

        /// <summary>
        /// All symbols supported
        /// </summary>
        [JsonProperty("symbols")]
        public IEnumerable<ChilizSymbol> Symbols { get; set; } = new List<ChilizSymbol>();

        /// <summary>
        /// The rate limits used
        /// </summary>
        [JsonProperty("rateLimits")]
        public IEnumerable<ChilizRateLimit> RateLimits { get; set; } = new List<ChilizRateLimit>();
    }
}
