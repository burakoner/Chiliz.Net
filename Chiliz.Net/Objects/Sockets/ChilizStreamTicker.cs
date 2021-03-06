﻿using Newtonsoft.Json;
using System;
using CryptoExchange.Net.Converters;
using System.Collections.Generic;

namespace Chiliz.Net.Objects.Sockets
{
    /// <summary>
    /// Wrapper for kline information for a symbol
    /// </summary>
    public class ChilizStreamTicker
    {
        /// <summary>
        /// The symbol the data is for
        /// </summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; } = "";

        [JsonProperty("topic")]
        public string Topic { get; set; } = "";

        [JsonProperty("params")]
        public ChilizKlineSubscribeParams Params { get; set; } = new ChilizKlineSubscribeParams();

        [JsonProperty("data")]
        public List<ChilizStreamTickerData> Data { get; set; } = new List<ChilizStreamTickerData>();

        /// <summary>
        /// Whether this is the first entry
        /// </summary>
        [JsonProperty("f")]
        public bool First { get; set; }

        [JsonProperty("shared")]
        public bool Shared { get; set; }
    }

    /// <summary>
    /// The kline data
    /// </summary>
    public class ChilizStreamTickerData
    {
        /// <summary>
        /// The open time of this candlestick
        /// </summary>
        [JsonProperty("t"), JsonConverter(typeof(TimestampConverter))]
        public DateTime OpenTime { get; set; }

        /// <summary>
        /// The symbol this candlestick is for
        /// </summary>
        [JsonProperty("s")]
        public string Symbol { get; set; } = "";

        /// <summary>
        /// The close price of this candlestick
        /// </summary>
        [JsonProperty("c")]
        public decimal Close { get; set; }

        /// <summary>
        /// The highest price of this candlestick
        /// </summary>
        [JsonProperty("h")]
        public decimal High { get; set; }

        /// <summary>
        /// The lowest price of this candlestick
        /// </summary>
        [JsonProperty("l")]
        public decimal Low { get; set; }

        /// <summary>
        /// The open price of this candlestick
        /// </summary>
        [JsonProperty("o")]
        public decimal Open { get; set; }

        /// <summary>
        /// The volume traded during this candlestick
        /// </summary>
        [JsonProperty("v")]
        public decimal Volume { get; set; }

        [JsonProperty("qv")]
        public decimal QuoteVolume { get; set; }
    }
    
}
