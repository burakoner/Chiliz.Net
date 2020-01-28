using Newtonsoft.Json;
using System;
using CryptoExchange.Net.Converters;

namespace Chiliz.Net.Objects
{
    /// <summary>
    /// Information about a trade
    /// </summary>
    public class ChilizTrade
    {
        /// <summary>
        /// The id of the trade
        /// </summary>
        [JsonProperty("id")]
        public long Id { get; set; }

        /// <summary>
        /// The symbol the trade is for
        /// </summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; } = "";

        /// <summary>
        /// The order id the trade belongs to
        /// </summary>
        [JsonProperty("orderId")]
        public long OrderId { get; set; }

        [JsonProperty("matchOrderId")]
        public long MatchOrderId { get; set; }

        /// <summary>
        /// The price of the trade
        /// </summary>
        [JsonProperty("price")]
        public decimal Price { get; set; }

        /// <summary>
        /// The quantity of the trade
        /// </summary>
        [JsonProperty("qty")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// The commission paid for the trade
        /// </summary>
        [JsonProperty("commission")]
        public decimal Commission { get; set; }

        /// <summary>
        /// The asset the commission is paid in
        /// </summary>
        [JsonProperty("commissionAsset")]
        public string CommissionAsset { get; set; } = "";

        /// <summary>
        /// The time the trade was made
        /// </summary>
        [JsonConverter(typeof(TimestampConverter)), JsonProperty("time")]
        public DateTime Time { get; set; }

        /// <summary>
        /// Whether account was the buyer in the trade
        /// </summary>
        [JsonProperty("isBuyer")]
        public bool IsBuyer { get; set; }

        /// <summary>
        /// Whether account was the maker in the trade
        /// </summary>
        [JsonProperty("isMaker")]
        public bool IsMaker { get; set; }

        [JsonProperty("fee")]
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public ChilizTradeFee Fee { get; set; }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    }

    public class ChilizTradeFee
    {
        /// <summary>
        /// The asset the commission is paid in
        /// </summary>
        [JsonProperty("feeTokenId")]
        public string FeeTokenId { get; set; } = "";

        /// <summary>
        /// The asset the commission is paid in
        /// </summary>
        [JsonProperty("feeTokenName")]
        public string FeeTokenName { get; set; } = "";

        /// <summary>
        /// The commission paid for the trade
        /// </summary>
        [JsonProperty("fee")]
        public decimal Fee { get; set; }
    }
}
