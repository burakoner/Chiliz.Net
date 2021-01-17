using System.Collections.Generic;
using Chiliz.Net.Objects;
using CryptoExchange.Net.Converters;

namespace Chiliz.Net.Converters
{
    public class TimeInForceConverter : BaseConverter<TimeInForce>
    {
        public TimeInForceConverter(): this(true) { }
        public TimeInForceConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<TimeInForce, string>> Mapping => new List<KeyValuePair<TimeInForce, string>>
        {
            new KeyValuePair<TimeInForce, string>(TimeInForce.GoodTillCancel, "GTC"),
            new KeyValuePair<TimeInForce, string>(TimeInForce.ImmediateOrCancel, "IOC"),
            new KeyValuePair<TimeInForce, string>(TimeInForce.FillOrKill, "FOK")
        };
    }
}
