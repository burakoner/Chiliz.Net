﻿using Chiliz.Net.Converters;
using Newtonsoft.Json;

namespace Chiliz.Net.Objects
{
    /// <summary>
    /// Information about a canceled order
    /// </summary>
    public class ChilizCanceledOrder
    {
        /// <summary>
        /// The symbol the order was for
        /// </summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; } = "";

        [JsonProperty("exchangeId")]
        public int ExchangeId { get; set; }

        [JsonProperty("clientOrderId")]
        public string ClientOrderId { get; set; } = "";

        /// <summary>
        /// The order id as generated by Chiliz
        /// </summary>
        [JsonProperty("orderId")]
        public long OrderId { get; set; }

        /// <summary>
        /// Status of the order
        /// </summary>
        [JsonConverter(typeof(OrderStatusConverter)), JsonProperty("status")]
        public OrderStatus Status { get; set; }
    }
}
