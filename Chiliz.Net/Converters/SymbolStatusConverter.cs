using Chiliz.Net.Objects;
using System.Collections.Generic;
using CryptoExchange.Net.Converters;

namespace Chiliz.Net.Converters
{
    public class SymbolStatusConverter : BaseConverter<SymbolStatus>
    {
        public SymbolStatusConverter(): this(true) { }
        public SymbolStatusConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<SymbolStatus, string>> Mapping => new List<KeyValuePair<SymbolStatus, string>>
        {
            new KeyValuePair<SymbolStatus, string>(SymbolStatus.Break, "BREAK"),
            new KeyValuePair<SymbolStatus, string>(SymbolStatus.Halt, "HALT"),
            new KeyValuePair<SymbolStatus, string>(SymbolStatus.Trading, "TRADING")
        };
    }
}
