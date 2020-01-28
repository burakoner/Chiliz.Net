using System;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Chiliz.Net.Objects
{
    /// <summary>
    /// Price statistics of the last 24 hours
    /// </summary>
    public class ChilizAll24hPrice
    {
        /// <summary>
        /// Time at which this 24 hours closed, Now
        /// </summary>
        [JsonProperty("time"), JsonConverter(typeof(TimestampConverter))]
        public DateTime Time { get; set; }

        /// <summary>
        /// The symbol the price is for
        /// </summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; } = "";

        /// <summary>
        /// The volume traded in the last 24 hours
        /// </summary>
        [JsonProperty("volume")]
        public decimal Volume { get; set; }

        /// <summary>
        /// The quote asset volume traded in the last 24 hours
        /// </summary>
        [JsonProperty("quoteVolume")]
        public decimal QuoteVolume { get; set; }

        /// <summary>
        /// The open price 24 hours ago
        /// </summary>
        [JsonProperty("openPrice")]
        public decimal OpenPrice { get; set; }

        /// <summary>
        /// The highest price in the last 24 hours
        /// </summary>
        [JsonProperty("highPrice")]
        public decimal HighPrice { get; set; }

        /// <summary>
        /// The lowest price in the last 24 hours
        /// </summary>
        [JsonProperty("lowPrice")]
        public decimal LowPrice { get; set; }

        /// <summary>
        /// The last price in the last 24 hours
        /// </summary>
        [JsonProperty("lastPrice")]
        public decimal LastPrice { get; set; }
    }
}
