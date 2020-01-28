using System;
using Chiliz.Net.Objects;
using CryptoExchange.Net.Objects;

namespace Chiliz.Net
{
    /// <summary>
    /// Options for the Chiliz client
    /// </summary>
    public class ChilizClientOptions : RestClientOptions
    {
        /// <summary>
        /// Whether or not to automatically sync the local time with the server time
        /// </summary>
        public bool AutoTimestamp { get; set; } = true;

        /// <summary>
        /// Interval for refreshing the auto timestamp calculation
        /// </summary>
        public TimeSpan AutoTimestampRecalculationInterval { get; set; } = TimeSpan.FromHours(3);

        /// <summary>
        /// A manual offset for the timestamp. Should only be used if AutoTimestamp and regular time synchronization on the OS is not reliable enough
        /// </summary>
        public TimeSpan TimestampOffset { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// Whether to check the trade rules when placing new orders and what to do if the trade isn't valid
        /// </summary>
        public TradeRulesBehaviour TradeRulesBehaviour { get; set; } = TradeRulesBehaviour.None;
        /// <summary>
        /// How often the trade rules should be updated. Only used when TradeRulesBehaviour is not None
        /// </summary>
        public TimeSpan TradeRulesUpdateInterval { get; set; } = TimeSpan.FromMinutes(60);

        /// <summary>
        /// The default receive window for requests
        /// </summary>
        public TimeSpan ReceiveWindow { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// ctor
        /// </summary>
        public ChilizClientOptions(): base("https://api.chiliz.net")
        {
        }

        /// <summary>
        /// Return a copy of these options
        /// </summary>
        /// <returns></returns>
        public ChilizClientOptions Copy()
        {
            var copy = Copy<ChilizClientOptions>();
            copy.AutoTimestamp = AutoTimestamp;
            copy.AutoTimestampRecalculationInterval = AutoTimestampRecalculationInterval;
            copy.TimestampOffset = TimestampOffset;
            copy.TradeRulesBehaviour = TradeRulesBehaviour;
            copy.TradeRulesUpdateInterval = TradeRulesUpdateInterval;
            copy.ReceiveWindow = ReceiveWindow;
            return copy;
        }
    }

}
