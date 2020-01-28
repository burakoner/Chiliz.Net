using Chiliz.Net.Converters;
using Newtonsoft.Json;

namespace Chiliz.Net.Objects
{
    /// <summary>
    /// A filter for order placed on a symbol. Can be either a <see cref="ChilizSymbolPriceFilter"/>, <see cref="ChilizSymbolLotSizeFilter"/>, <see cref="ChilizSymbolMinNotionalFilter"/>, <see cref="ChilizSymbolMaxAlgorithmicOrdersFilter"/> or <see cref="ChilizSymbolIcebergPartsFilter"/>
    /// </summary>
    [JsonConverter(typeof(SymbolFilterConverter))]
    public class ChilizSymbolFilter
    {
        /// <summary>
        /// The type of this filter
        /// </summary>
        public SymbolFilterType FilterType { get; set; }
    }

    /// <summary>
    /// Price filter
    /// </summary>
    public class ChilizSymbolPriceFilter: ChilizSymbolFilter
    {
        /// <summary>
        /// The minimal price the order can be for
        /// </summary>
        public decimal MinPrice { get; set; }
        /// <summary>
        /// The max price the order can be for
        /// </summary>
        public decimal MaxPrice { get; set; }
        /// <summary>
        /// The tick size of the price. The price can not have more precision as this and can only be incremented in steps of this.
        /// </summary>
        public decimal TickSize { get; set; }
    }

    /// <summary>
    /// Lot size filter
    /// </summary>
    public class ChilizSymbolLotSizeFilter : ChilizSymbolFilter
    {
        /// <summary>
        /// The minimal quantity of an order
        /// </summary>
        public decimal MinQuantity { get; set; }
        /// <summary>
        /// The maximum quantity of an order
        /// </summary>
        public decimal MaxQuantity { get; set; }
        /// <summary>
        /// The tick size of the quantity. The quantity can not have more precision as this and can only be incremented in steps of this.
        /// </summary>
        public decimal StepSize { get; set; }
    }

    /// <summary>
    /// Min notional filter
    /// </summary>
    public class ChilizSymbolMinNotionalFilter : ChilizSymbolFilter
    {
        /// <summary>
        /// The minimal total size of an order. This is calculated by Price * Quantity.
        /// </summary>
        public decimal MinNotional { get; set; }
    }

    /// <summary>
    /// Nax orders filter
    /// </summary>
    public class ChilizSymbolMaxOrdersFilter : ChilizSymbolFilter
    {
        /// <summary>
        /// The max number of orders for this symbol
        /// </summary>
        public int MaxNumberOrders { get; set; }
    }

    /// <summary>
    /// Max algo orders filter
    /// </summary>
    public class ChilizSymbolMaxAlgorithmicOrdersFilter : ChilizSymbolFilter
    {
        /// <summary>
        /// The max number of Algorithmic orders for this symbol
        /// </summary>
        public int MaxNumberAlgorithmicOrders { get; set; }
    }

    /// <summary>
    /// Max iceberg parts filter
    /// </summary>
    public class ChilizSymbolIcebergPartsFilter : ChilizSymbolFilter
    {
        /// <summary>
        /// The max parts of an iceberg order for this symbol.
        /// </summary>
        public int Limit { get; set; }
    }
}
