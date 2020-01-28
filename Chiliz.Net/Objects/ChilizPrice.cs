using Newtonsoft.Json;

namespace Chiliz.Net.Objects
{
    /// <summary>
    /// The price of a symbol
    /// </summary>
    public class ChilizPrice
    {
        /// <summary>
        /// The symbol the price is for
        /// </summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; } = "";
        /// <summary>
        /// The price of the symbol
        /// </summary>
        [JsonProperty("price")]
        public decimal Price { get; set; }
    }
}
