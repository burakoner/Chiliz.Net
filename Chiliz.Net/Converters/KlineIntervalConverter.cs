using System.Collections.Generic;
using Chiliz.Net.Objects;
using CryptoExchange.Net.Converters;

namespace Chiliz.Net.Converters
{
    internal class KlineIntervalConverter: BaseConverter<KlineInterval>
    {
        public KlineIntervalConverter(): this(true) { }
        public KlineIntervalConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<KlineInterval, string>> Mapping => new List<KeyValuePair<KlineInterval, string>>
        {
            new KeyValuePair<KlineInterval, string>(KlineInterval.OneMinute, "1m"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.ThreeMinutes, "3m"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.FiveMinutes, "5m"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.FifteenMinutes, "15m"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.ThirtyMinutes, "30m"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.OneHour, "1h"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.TwoHours, "2h"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.FourHours, "4h"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.SixHours, "6h"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.EightHours, "8h"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.TwelveHours, "12h"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.OneDay, "1d"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.ThreeDays, "3d"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.OneWeek, "1w"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.OneMonth, "1M")
        };
    }
}
