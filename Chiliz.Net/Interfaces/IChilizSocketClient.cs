using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chiliz.Net.Objects;
using Chiliz.Net.Objects.Sockets;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;

namespace Chiliz.Net.Interfaces
{
    /// <summary>
    /// Interface for the Chiliz socket client
    /// </summary>
    public interface IChilizSocketClient : ISocketClient
    {
        /// <summary>
        /// Set the API key and secret
        /// </summary>
        /// <param name="apiKey">The api key</param>
        /// <param name="apiSecret">The api secret</param>
        void SetApiCredentials(string apiKey, string apiSecret);

        /// <summary>
        /// Subscribes to the candlestick update stream for the provided symbol
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="interval">The interval of the candlesticks</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        CallResult<UpdateSubscription> SubscribeToKlineUpdates(string symbol, KlineInterval interval, Action<ChilizStreamKline> onMessage);

        /// <summary>
        /// Subscribes to the candlestick update stream for the provided symbol
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="interval">The interval of the candlesticks</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(string symbol, KlineInterval interval, Action<ChilizStreamKline> onMessage);

        /// <summary>
        /// Subscribes to the candlestick update stream for the provided symbols
        /// </summary>
        /// <param name="symbols">The symbols</param>
        /// <param name="interval">The interval of the candlesticks</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        CallResult<UpdateSubscription> SubscribeToKlineUpdates(IEnumerable<string> symbols, KlineInterval interval, Action<ChilizStreamKline> onMessage);

        /// <summary>
        /// Subscribes to the candlestick update stream for the provided symbols
        /// </summary>
        /// <param name="symbols">The symbols</param>
        /// <param name="interval">The interval of the candlesticks</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(IEnumerable<string> symbols, KlineInterval interval, Action<ChilizStreamKline> onMessage);
    }
}