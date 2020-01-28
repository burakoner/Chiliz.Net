using Newtonsoft.Json;
using System;
using CryptoExchange.Net.Converters;

namespace Chiliz.Net.Objects
{
    /// <summary>
    /// Recent trade info
    /// </summary>
    public class ChilizRecentTrade
    {
        /// <summary>
        /// The price of the trade
        /// </summary>
        [JsonProperty("price")]
        public decimal Price { get; set; }

        /// <summary>
        /// The timestamp of the trade
        /// </summary>
        [JsonProperty("time"), JsonConverter(typeof(TimestampConverter))]
        public DateTime Time { get; set; }

        /// <summary>
        /// The quantity of the trade
        /// </summary>
        [JsonProperty("qty")]
        public decimal Quantity { get; set; }

        [JsonProperty("isBuyerMaker")]
        public bool IsBuyerMaker { get; set; }
    }
}
