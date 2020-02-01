using System;
using Chiliz.Net.Converters;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Chiliz.Net.Objects
{
    public class ChilizDeposit
    {
        [JsonProperty("orderId")]
        public long OrderId { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; } = "";

        [JsonProperty("address")]
        public string Address { get; set; } = "";

        [JsonProperty("addressTag")]
        public string AddressTag { get; set; } = "";

        [JsonProperty("fromAddress")]
        public string FromAddress { get; set; } = "";

        [JsonProperty("fromAddressTag")]
        public string FromAddressTag { get; set; } = "";

        [JsonProperty("time"), JsonConverter(typeof(TimestampConverter))]
        public DateTime Time { get; set; }

        [JsonProperty("quantity")]
        public decimal Quantity { get; set; }
    }
}
