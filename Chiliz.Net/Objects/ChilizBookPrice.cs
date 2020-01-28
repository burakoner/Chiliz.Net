using Newtonsoft.Json;

namespace Chiliz.Net.Objects
{
    /// <summary>
    /// Information about the best price/quantity available for a symbol
    /// </summary>
    public class ChilizBookPrice
    {
        /// <summary>
        /// The symbol the information is about
        /// </summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; } = "";

        /// <summary>
        /// The highest bid price for the symbol
        /// </summary>
        [JsonProperty("bidPrice")]
        public decimal BidPrice { get; set; }

        /// <summary>
        /// The quantity of the highest bid price currently in the order book
        /// </summary>
        [JsonProperty("bidQty")]
        public decimal BidQuantity { get; set; }

        /// <summary>
        /// The lowest ask price for the symbol
        /// </summary>
        [JsonProperty("askPrice")]
        public decimal AskPrice { get; set; }

        /// <summary>
        /// The quantity of the lowest ask price currently in the order book
        /// </summary>
        [JsonProperty("askQty")]
        public decimal AskQuantity { get; set; }
    }
}
