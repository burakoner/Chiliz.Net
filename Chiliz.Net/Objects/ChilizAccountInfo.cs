using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using CryptoExchange.Net.Converters;

namespace Chiliz.Net.Objects
{
    /// <summary>
    /// Information about an account
    /// </summary>
    public class ChilizAccountInfo
    {
        /// <summary>
        /// List of assets with their current balances
        /// </summary>
        [JsonProperty("balances")]
        public IEnumerable<ChilizBalance> Balances { get; set; } = new List<ChilizBalance>();
    }

    /// <summary>
    /// Information about an asset balance
    /// </summary>
    public class ChilizBalance
    {
        /// <summary>
        /// The asset this balance is for
        /// </summary>
        [JsonProperty("asset")]
        public string Asset { get; set; } = "";
        [JsonProperty("assetId")]
        public string AssetId { get; set; } = "";
        /// <summary>
        /// The total balance of this asset (Free + Locked)
        /// </summary>
        [JsonProperty("total")]
        public decimal Total { get; set; }
        /// <summary>
        /// The amount that isn't locked in a trade
        /// </summary>
        [JsonProperty("free")]
        public decimal Free { get; set; }
        /// <summary>
        /// The amount that is currently locked in a trade
        /// </summary>
        [JsonProperty("locked")]
        public decimal Locked { get; set; }
    }
}
