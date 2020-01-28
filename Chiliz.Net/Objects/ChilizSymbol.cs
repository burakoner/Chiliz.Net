using Chiliz.Net.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Chiliz.Net.Objects
{
    /// <summary>
    /// Symbol info
    /// </summary>
    public class ChilizSymbol
    {
        /// <summary>
        /// The symbol
        /// </summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; } = "";

        [JsonProperty("symbolName")]
        public string SymbolName { get; set; } = "";

        [JsonProperty("exchangeId")]
        public int MarketId { get; set; }

        /// <summary>
        /// The status of the symbol
        /// </summary>
        [JsonProperty("status"), JsonConverter(typeof(SymbolStatusConverter))]
        public SymbolStatus Status { get; set; }

        /// <summary>
        /// The base asset
        /// </summary>
        [JsonProperty("baseAsset")]
        public string BaseAsset { get; set; } = "";

        /// <summary>
        /// The precision of the base asset
        /// </summary>
        [JsonProperty("baseAssetPrecision")]
        public decimal BaseAssetPrecision { get; set; }

        /// <summary>
        /// The quote asset
        /// </summary>
        [JsonProperty("quoteAsset")]
        public string QuoteAsset { get; set; } = "";

        /// <summary>
        /// The precision of the quote asset
        /// </summary>
        [JsonProperty("quotePrecision")]
        public decimal QuoteAssetPrecision { get; set; }

        /// <summary>
        /// Ice berg orders allowed
        /// </summary>
        public bool IceBergAllowed { get; set; }
       
        /// <summary>
        /// Filters for order on this symbol
        /// </summary>
        public IEnumerable<ChilizSymbolFilter> Filters { get; set; } = new List<ChilizSymbolFilter>();

        /// <summary>
        /// Filter for max amount of iceberg parts for this symbol
        /// </summary>
        [JsonIgnore]        
        public ChilizSymbolIcebergPartsFilter IceBergPartsFilter => Filters.OfType<ChilizSymbolIcebergPartsFilter>().FirstOrDefault();
        /// <summary>
        /// Filter for max accuracy of the quantity for this symbol
        /// </summary>
        [JsonIgnore]
        public ChilizSymbolLotSizeFilter LotSizeFilter => Filters.OfType<ChilizSymbolLotSizeFilter>().FirstOrDefault();
        /// <summary>
        /// Filter for max number of orders for this symbol
        /// </summary>
        [JsonIgnore]
        public ChilizSymbolMaxOrdersFilter MaxOrdersFilter => Filters.OfType<ChilizSymbolMaxOrdersFilter>().FirstOrDefault();
        /// <summary>
        /// Filter for max algorithmic orders for this symbol
        /// </summary>
        [JsonIgnore]
        public ChilizSymbolMaxAlgorithmicOrdersFilter MaxAlgorithmicOrdersFilter => Filters.OfType<ChilizSymbolMaxAlgorithmicOrdersFilter>().FirstOrDefault();
        /// <summary>
        /// Filter for the minimal size of an order for this symbol
        /// </summary>
        [JsonIgnore]
        public ChilizSymbolMinNotionalFilter MinNotionalFilter => Filters.OfType<ChilizSymbolMinNotionalFilter>().FirstOrDefault();
        /// <summary>
        /// Filter for the max accuracy of the price for this symbol
        /// </summary>
        [JsonIgnore]
        public ChilizSymbolPriceFilter PriceFilter => Filters.OfType<ChilizSymbolPriceFilter>().FirstOrDefault();
    }
}
