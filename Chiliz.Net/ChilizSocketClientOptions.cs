using System;
using CryptoExchange.Net.Objects;

namespace Chiliz.Net
{
    /// <summary>
    /// Chiliz socket client options
    /// </summary>
    public class ChilizSocketClientOptions : SocketClientOptions
    {
        /// <summary>
        /// ctor
        /// </summary>
        public ChilizSocketClientOptions(): base("wss://wsapi.chiliz.net/openapi/quote/ws/v1")
        {
        }        

        /// <summary>
        /// Return a copy of these options
        /// </summary>
        /// <returns></returns>
        public ChilizSocketClientOptions Copy()
        {
            var copy = Copy<ChilizSocketClientOptions>();
            return copy;
        }
    }

}
