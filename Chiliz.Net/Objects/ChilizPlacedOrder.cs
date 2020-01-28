using Chiliz.Net.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters;

namespace Chiliz.Net.Objects
{
    /// <summary>
    /// The result of placing a new order
    /// </summary>
    public class ChilizPlacedOrder
    {
        /// <summary>
        /// The symbol the order is for
        /// </summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; } = "";

        /// <summary>
        /// The order id as assigned by Chiliz
        /// </summary>
        [JsonProperty("orderId")]
        public long OrderId { get; set; }

        /// <summary>
        /// The order id as assigned by the client
        /// </summary>
        [JsonProperty("clientOrderId")]
        public string ClientOrderId { get; set; } = "";

        /// <summary>
        /// The time the order was placed
        /// </summary>
        [JsonOptionalProperty]
        [JsonProperty("transactTime")]
        [JsonConverter(typeof(TimestampConverter))]
        public DateTime TransactTime { get; set; }

        /// <summary>
        /// The price of the order
        /// </summary>
        [JsonProperty("price")]
        public decimal Price { get; set; }

        /// <summary>
        /// The original quantity of the order
        /// </summary>
        [JsonProperty("origQty")]
        public decimal OriginalQuantity { get; set; }

        /// <summary>
        /// The quantity of the order that is executed
        /// </summary>
        [JsonProperty("executedQty")]
        public decimal ExecutedQuantity { get; set; }

        /// <summary>
        /// The current status of the order
        /// </summary>
        [JsonConverter(typeof(OrderStatusConverter)), JsonProperty("status")]
        public OrderStatus Status { get; set; }

        /// <summary>
        /// For what time the order lasts
        /// </summary>
        [JsonConverter(typeof(TimeInForceConverter)), JsonProperty("timeInForce")]
        public TimeInForce TimeInForce { get; set; }

        /// <summary>
        /// The type of the order
        /// </summary>
        [JsonConverter(typeof(OrderTypeConverter)), JsonProperty("type")]
        public OrderType Type { get; set; }

        /// <summary>
        /// The side of the order
        /// </summary>
        [JsonConverter(typeof(OrderSideConverter)), JsonProperty("side")]
        public OrderSide Side { get; set; }
    }
}
