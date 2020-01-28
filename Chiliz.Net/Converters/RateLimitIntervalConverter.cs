using System.Collections.Generic;
using Chiliz.Net.Objects;
using CryptoExchange.Net.Converters;

namespace Chiliz.Net.Converters
{
    internal class RateLimitIntervalConverter : BaseConverter<RateLimitInterval>
    {
        public RateLimitIntervalConverter() : this(true) { }
        public RateLimitIntervalConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<RateLimitInterval, string>> Mapping => new List<KeyValuePair<RateLimitInterval, string>>
        {
            new KeyValuePair<RateLimitInterval, string>(RateLimitInterval.Second, "SECOND"),
            new KeyValuePair<RateLimitInterval, string>(RateLimitInterval.Minute, "MINUTE"),
            new KeyValuePair<RateLimitInterval, string>(RateLimitInterval.Day, "DAY")
        };
    }
}
