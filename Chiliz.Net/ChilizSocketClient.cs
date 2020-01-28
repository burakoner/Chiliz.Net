using Chiliz.Net.Converters;
using Chiliz.Net.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chiliz.Net.Interfaces;
using Chiliz.Net.Objects.Sockets;
using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Logging;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using Newtonsoft.Json.Linq;

namespace Chiliz.Net
{
    /// <summary>
    /// Client providing access to the Chiliz websocket Api
    /// </summary>
    public class ChilizSocketClient : SocketClient, IChilizSocketClient
    {
        #region fields
        private static ChilizSocketClientOptions defaultOptions = new ChilizSocketClientOptions();
        private static ChilizSocketClientOptions DefaultOptions => defaultOptions.Copy();

        private const string DepthStreamEndpoint = "@depth";
        private const string BookTickerStreamEndpoint = "@bookTicker";
        private const string KlineStreamEndpoint = "@kline";
        private const string TradesStreamEndpoint = "@trade";
        private const string AggregatedTradesStreamEndpoint = "@aggTrade";
        private const string SymbolTickerStreamEndpoint = "@ticker";
        private const string AllSymbolTickerStreamEndpoint = "!ticker@arr";
        private const string PartialBookDepthStreamEndpoint = "@depth";

        private const string AccountUpdateEvent = "outboundAccountInfo";
        private const string ExecutionUpdateEvent = "executionReport";
        private const string OcoOrderUpdateEvent = "listStatus";
        private const string AccountPositionUpdateEvent = "outboundAccountPosition";
        private const string BalanceUpdateEvent = "balanceUpdate";
        #endregion

        #region constructor/destructor

        /// <summary>
        /// Create a new instance of ChilizSocketClient with default options
        /// </summary>
        public ChilizSocketClient() : this(DefaultOptions)
        {
        }

        /// <summary>
        /// Create a new instance of ChilizSocketClient using provided options
        /// </summary>
        /// <param name="options">The options to use for this client</param>
        public ChilizSocketClient(ChilizSocketClientOptions options) : base(options, options.ApiCredentials == null ? null : new ChilizAuthenticationProvider (options.ApiCredentials, ArrayParametersSerialization.MultipleValues))
        {
        }
        #endregion 

        #region methods
        /// <summary>
        /// Set the default options to be used when creating new socket clients
        /// </summary>
        /// <param name="options"></param>
        public static void SetDefaultOptions(ChilizSocketClientOptions options)
        {
            defaultOptions = options;
        }

        /// <summary>
        /// Set the API key and secret
        /// </summary>
        /// <param name="apiKey">The api key</param>
        /// <param name="apiSecret">The api secret</param>
        public void SetApiCredentials(string apiKey, string apiSecret)
        {
            SetAuthenticationProvider(new ChilizAuthenticationProvider (new ApiCredentials(apiKey, apiSecret), ArrayParametersSerialization.MultipleValues));
        }

        /// <summary>
        /// Subscribes to the candlestick update stream for the provided symbol
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="interval">The interval of the candlesticks</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public CallResult<UpdateSubscription> SubscribeToKlineUpdates(string symbol, KlineInterval interval, Action<ChilizStreamKline> onMessage) => SubscribeToKlineUpdatesAsync(symbol, interval, onMessage).Result;

        /// <summary>
        /// Subscribes to the candlestick update stream for the provided symbol
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="interval">The interval of the candlesticks</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public async Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(string symbol, KlineInterval interval, Action<ChilizStreamKline> onMessage) => await SubscribeToKlineUpdatesAsync(new [] { symbol }, interval, onMessage).ConfigureAwait(false);


        /// <summary>
        /// Subscribes to the candlestick update stream for the provided symbols
        /// </summary>
        /// <param name="symbols">The symbols</param>
        /// <param name="interval">The interval of the candlesticks</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public CallResult<UpdateSubscription> SubscribeToKlineUpdates(IEnumerable<string> symbols, KlineInterval interval, Action<ChilizStreamKline> onMessage) => SubscribeToKlineUpdatesAsync(symbols, interval, onMessage).Result;

        /// <summary>
        /// Subscribes to the candlestick update stream for the provided symbols
        /// </summary>
        /// <param name="symbols">The symbols</param>
        /// <param name="interval">The interval of the candlesticks</param>
        /// <param name="onMessage">The event handler for the received data</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public async Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(IEnumerable<string> symbols, KlineInterval interval, Action<ChilizStreamKline> onMessage)
        {
            /*
            symbols.ValidateNotNull(nameof(symbols));
            foreach(var symbol in symbols)
                symbol.ValidateChilizSymbol();
            */


            var request = new ChilizSubscribeRequest
            {
                Id = NextId().ToString(),
                Symbol = string.Join(", ", symbols),
                Topic = "kline_"+JsonConvert.SerializeObject(interval, new KlineIntervalConverter(false)),
                Event ="sub",
                Params= new ChilizSubscribeParams
                {
                    // Limit=1,
                    Binary = false,
                    KlineType = JsonConvert.SerializeObject(interval, new KlineIntervalConverter(false))
                }
            };
            var internalHandler = new Action<ChilizStreamKline>(data => onMessage(data));
            return await Subscribe(request, null, false, internalHandler).ConfigureAwait(false);
        }

        private async Task<CallResult<UpdateSubscription>> Subscribe<T>(string url, bool combined, Action<T> onData)
        {
            url = BaseAddress + url;
            return await Subscribe(url, null, url + NextId(), false, onData).ConfigureAwait(false);
        }

        /// <inheritdoc />
        protected override bool HandleQueryResponse<T>(SocketConnection s, object request, JToken data, out CallResult<T> callResult)
        {
            throw new NotImplementedException();
        }

        protected override bool HandleSubscriptionResponse(SocketConnection s, SocketSubscription subscription, object request, JToken message, out CallResult<object>? callResult)
        {
            callResult = null;
            /** /
            var isError = message["status"] != null && (string)message["status"] == "error";
            if (isError)
            {
                if (request is HuobiSubscribeRequest hRequest)
                {
                    var subResponse = Deserialize<HuobiSubscribeResponse>(message, false);
                    if (!subResponse)
                    {
                        log.Write(LogVerbosity.Warning, "Subscription failed: " + subResponse.Error);
                        return false;
                    }

                    var id = subResponse.Data.Id;
                    if (id != hRequest.Id)
                        return false; // Not for this request

                    log.Write(LogVerbosity.Warning, "Subscription failed: " + subResponse.Data.ErrorMessage);
                    callResult = new CallResult<object>(null, new ServerError($"{subResponse.Data.ErrorCode}, {subResponse.Data.ErrorMessage}"));
                    return true;
                }

                if (request is HuobiAuthenticatedRequest haRequest)
                {
                    var subResponse = Deserialize<HuobiSocketAuthResponse>(message, false);
                    if (!subResponse)
                    {
                        log.Write(LogVerbosity.Warning, "Subscription failed: " + subResponse.Error);
                        callResult = new CallResult<object>(null, subResponse.Error);
                        return false;
                    }

                    var id = subResponse.Data.Id;
                    if (id != haRequest.Id)
                        return false; // Not for this request

                    log.Write(LogVerbosity.Warning, "Subscription failed: " + subResponse.Data.ErrorMessage);
                    callResult = new CallResult<object>(null, new ServerError($"{subResponse.Data.ErrorCode}, {subResponse.Data.ErrorMessage}"));
                    return true;
                }
            }
            */

            var v1Sub = message["subbed"] != null;
            if (true)
            {
                var subResponse = Deserialize<ChilizStreamKline>(message, false);
                if (!subResponse)
                {
                    log.Write(LogVerbosity.Warning, "Subscription failed: " + subResponse.Error);
                    return false;
                }
                /*
                var hRequest = (ChilizStreamKlineData)request;
                if (subResponse.Data.Id != hRequest.Id)
                    return false;
                */
                /*
                if (!subResponse.Data.IsSuccessful)
                {
                    log.Write(LogVerbosity.Warning, "Subscription failed: " + subResponse.Data.ErrorMessage);
                    callResult = new CallResult<object>(null, new ServerError($"{subResponse.Data.ErrorCode}, {subResponse.Data.ErrorMessage}"));
                    return true;
                }
                */

                log.Write(LogVerbosity.Debug, "Subscription completed");
                callResult = new CallResult<object>(subResponse.Data, null);
                return true;
            }
            /**/

            return false;
        }

        /// <inheritdoc />
        protected override bool MessageMatchesHandler(JToken message, object request)
        {
            //var a = (string)message["topic"] + "_" + (string)message["topic"]["params"]["klineType"];
            if (request is ChilizSubscribeRequest hRequest)
            {
                if (message.HasValues)
                {
                    if (message["pong"] != null)
                    {
                        var pong = (long)message["pong"];
                        return false;
                    }

                    if (message["topic"] != null && message["params"] != null && message["params"]["klineType"] != null)
                    {
                        var msgtopic = (string)message["topic"] + "_" + (string)message["params"]["klineType"];
                        return hRequest.Topic == msgtopic;
                    }
                }
            }

            /*
            if (request is HuobiAuthenticatedRequest haRequest)
                return haRequest.Topic == (string)message["topic"];
            */

            return false;
        }

        /// <inheritdoc />
        protected override bool MessageMatchesHandler(JToken message, string identifier)
        {
            if (message.Type != JTokenType.Object)
                return false;

            if (identifier == "PingV1" && message["ping"] != null)
                return true;

            if (identifier == "PingV2" && (string)message["op"] == "ping")
                return true;

            return false;
        }

        /// <inheritdoc />
        protected override Task<CallResult<bool>> AuthenticateSocket(SocketConnection s)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        protected override Task<bool> Unsubscribe(SocketConnection connection, SocketSubscription s)
        {
            return Task.FromResult(true);
        }
        #endregion
    }
}
