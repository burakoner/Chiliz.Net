using Chiliz.Net.Converters;
using Chiliz.Net.Objects;
using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Logging;
using CryptoExchange.Net.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Chiliz.Net.Interfaces;

namespace Chiliz.Net
{
    /// <summary>
    /// Client providing access to the Chiliz REST Api
    /// </summary>
    public class ChilizClient : RestClient, IChilizClient
    {
        #region Fields 
        private static ChilizClientOptions defaultOptions = new ChilizClientOptions();
        private static ChilizClientOptions DefaultOptions => defaultOptions.Copy();

        private readonly bool autoTimestamp;
        private readonly TimeSpan autoTimestampRecalculationInterval;
        private readonly TimeSpan timestampOffset;
        private readonly TradeRulesBehaviour tradeRulesBehaviour;
        private readonly TimeSpan tradeRulesUpdateInterval;
        private readonly TimeSpan defaultReceiveWindow;

        private double calculatedTimeOffset;
        private bool timeSynced;
        private DateTime lastTimeSync;

        private ChilizExchangeInfo? exchangeInfo;
        private DateTime? lastExchangeInfoUpdate;

        // Addresses
        private const string Api = "openapi";
        private const string ApiQuote = "openapi/quote";

        // Versions
        private const string PublicVersion = "1";
        private const string SignedVersion = "1";
        private const string UserDataStreamVersion = "1";

        // Public
        private const string PingEndpoint = "ping";
        private const string CheckTimeEndpoint = "time";
        private const string ExchangeInfoEndpoint = "brokerInfo";
        private const string OrderBookEndpoint = "depth";
        private const string RecentTradesEndpoint = "trades";
        private const string KlinesEndpoint = "klines";
        private const string Price24HEndpoint = "ticker/24hr";
        private const string AllPricesEndpoint = "ticker/price";
        private const string BookPricesEndpoint = "ticker/bookTicker";

        // Accounts
        private const string AccountInfoEndpoint = "account";

        // Orders
        private const string NewOrderEndpoint = "order";
        private const string NewTestOrderEndpoint = "order/test";
        private const string QueryOrderEndpoint = "order";
        private const string CancelOrderEndpoint = "order";
        private const string OpenOrdersEndpoint = "openOrders";
        private const string HistoryOrdersEndpoint = "historyOrders";
        private const string MyTradesEndpoint = "myTrades";

        // Deposit & Withdrawal
        private const string DepositOrdersEndpoint = "depositOrders";

        // User stream
        private const string GetListenKeyEndpoint = "userDataStream";
        private const string KeepListenKeyAliveEndpoint = "userDataStream";
        private const string CloseListenKeyEndpoint = "userDataStream";

        #endregion

        #region Constructor / Destructor
        /// <summary>
        /// Create a new instance of ChilizClient using the default options
        /// </summary>
        public ChilizClient() : this(DefaultOptions)
        {
        }

        /// <summary>
        /// Create a new instance of ChilizClient using provided options
        /// </summary>
        /// <param name="options">The options to use for this client</param>
        public ChilizClient(ChilizClientOptions options) : base("Chiliz", options, options.ApiCredentials == null ? null : new ChilizAuthenticationProvider (options.ApiCredentials, ArrayParametersSerialization.MultipleValues))
        {
            arraySerialization = ArrayParametersSerialization.MultipleValues;

            autoTimestamp = options.AutoTimestamp;
            tradeRulesBehaviour = options.TradeRulesBehaviour;
            tradeRulesUpdateInterval = options.TradeRulesUpdateInterval;
            autoTimestampRecalculationInterval = options.AutoTimestampRecalculationInterval;
            timestampOffset = options.TimestampOffset;
            defaultReceiveWindow = options.ReceiveWindow;

            postParametersPosition = PostParameters.InUri;
        }
        #endregion

        #region Options & Credentials
        /// <summary>
        /// Set the default options to be used when creating new clients
        /// </summary>
        /// <param name="options"></param>
        public static void SetDefaultOptions(ChilizClientOptions options)
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
        #endregion

        #region Public EndPoints
        /// <summary>
        /// Pings the Chiliz API
        /// </summary>
        /// <returns>True if successful ping, false if no response</returns>
        public override CallResult<long> Ping(CancellationToken ct = default) => PingAsync(ct).Result;

        /// <summary>
        /// Pings the Chiliz API
        /// </summary>
        /// <returns>True if successful ping, false if no response</returns>
        public override async Task<CallResult<long>> PingAsync(CancellationToken ct = default)
        {
            var sw = Stopwatch.StartNew();
            var result = await SendRequest<object>(GetUrl(PingEndpoint, Api, PublicVersion), HttpMethod.Get, ct).ConfigureAwait(false);
            sw.Stop();
            return new CallResult<long>(result.Error == null ? sw.ElapsedMilliseconds : 0, result.Error);
        }

        /// <summary>
        /// Requests the server for the local time. This function also determines the offset between server and local time and uses this for subsequent API calls
        /// </summary>
        /// <param name="resetAutoTimestamp">Whether the response should be used for a new auto timestamp calculation</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Server time</returns>
        public WebCallResult<DateTime> GetServerTime(bool resetAutoTimestamp = false, CancellationToken ct = default) => GetServerTimeAsync(resetAutoTimestamp, ct).Result;

        /// <summary>
        /// Requests the server for the local time. This function also determines the offset between server and local time and uses this for subsequent API calls
        /// </summary>
        /// <param name="resetAutoTimestamp">Whether the response should be used for a new auto timestamp calculation</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Server time</returns>
        public async Task<WebCallResult<DateTime>> GetServerTimeAsync(bool resetAutoTimestamp = false, CancellationToken ct = default)
        {
            var url = GetUrl(CheckTimeEndpoint, Api, PublicVersion);
            if (!autoTimestamp)
            {
                var result = await SendRequest<ChilizServerTime>(url, HttpMethod.Get, ct).ConfigureAwait(false);
                return new WebCallResult<DateTime>(result.ResponseStatusCode, result.ResponseHeaders, result.Data?.ServerTime ?? default, result.Error);
            }
            else
            {
                var localTime = DateTime.UtcNow;
                var result = await SendRequest<ChilizServerTime>(url, HttpMethod.Get, ct).ConfigureAwait(false);
                if (!result)
                    return new WebCallResult<DateTime>(result.ResponseStatusCode, result.ResponseHeaders, default, result.Error);

                if (timeSynced && !resetAutoTimestamp)
                    return new WebCallResult<DateTime>(result.ResponseStatusCode, result.ResponseHeaders, result.Data.ServerTime, result.Error);

                if (TotalRequestsMade == 1)
                {
                    // If this was the first request make another one to calculate the offset since the first one can be slower
                    localTime = DateTime.UtcNow;
                    result = await SendRequest<ChilizServerTime>(url, HttpMethod.Get, ct).ConfigureAwait(false);
                    if (!result)
                        return new WebCallResult<DateTime>(result.ResponseStatusCode, result.ResponseHeaders, default, result.Error);
                }

                // Calculate time offset between local and server
                var offset = (result.Data.ServerTime - localTime).TotalMilliseconds;
                if (offset >= 0 && offset < 500)
                {
                    // Small offset, probably mainly due to ping. Don't adjust time
                    calculatedTimeOffset = 0;
                    timeSynced = true;
                    lastTimeSync = DateTime.UtcNow;
                    log.Write(LogVerbosity.Info, $"Time offset between 0 and 500ms ({offset}ms), no adjustment needed");
                    return new WebCallResult<DateTime>(result.ResponseStatusCode, result.ResponseHeaders, result.Data.ServerTime, result.Error);
                }

                calculatedTimeOffset = (result.Data.ServerTime - localTime).TotalMilliseconds;
                timeSynced = true;
                lastTimeSync = DateTime.UtcNow;
                log.Write(LogVerbosity.Info, $"Time offset set to {calculatedTimeOffset}ms");
                return new WebCallResult<DateTime>(result.ResponseStatusCode, result.ResponseHeaders, result.Data.ServerTime, result.Error);
            }
        }

        /// <summary>
        /// Get's information about the exchange including rate limits and symbol list
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Exchange info</returns>
        public WebCallResult<ChilizExchangeInfo> GetExchangeInfo(CancellationToken ct = default) => GetExchangeInfoAsync(ct).Result;

        /// <summary>
        /// Get's information about the exchange including rate limits and symbol list
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Exchange info</returns>
        public async Task<WebCallResult<ChilizExchangeInfo>> GetExchangeInfoAsync(CancellationToken ct = default)
        {
            var exchangeInfoResult = await SendRequest<ChilizExchangeInfo>(GetUrl(ExchangeInfoEndpoint, Api, PublicVersion), HttpMethod.Get, ct).ConfigureAwait(false);
            if (!exchangeInfoResult)
                return exchangeInfoResult;

            exchangeInfo = exchangeInfoResult.Data;
            lastExchangeInfoUpdate = DateTime.UtcNow;
            log.Write(LogVerbosity.Info, "Trade rules updated");
            return exchangeInfoResult;
        }

        /// <summary>
        /// Gets the order book for the provided symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the order book for</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The order book for the symbol</returns>
        public WebCallResult<ChilizOrderBook> GetOrderBook(string symbol, int? limit = null, CancellationToken ct = default) => GetOrderBookAsync(symbol, limit, ct).Result;

        /// <summary>
        /// Gets the order book for the provided symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the order book for</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The order book for the symbol</returns>
        public async Task<WebCallResult<ChilizOrderBook>> GetOrderBookAsync(string symbol, int? limit = null, CancellationToken ct = default)
        {
            symbol.ValidateChilizSymbol();
            limit?.ValidateIntValues(nameof(limit), 5, 10, 20, 50, 100, 500, 1000);
            var parameters = new Dictionary<string, object> { { "symbol", symbol } };
            parameters.AddOptionalParameter("limit", limit?.ToString());
            var result = await SendRequest<ChilizOrderBook>(GetUrl(OrderBookEndpoint, ApiQuote, PublicVersion), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
            if (result)
                result.Data.Symbol = symbol;
            return result;
        }

        /// <summary>
        /// Gets the recent trades for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get recent trades for</param>
        /// <param name="limit">Result limit</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of recent trades</returns>
        public WebCallResult<IEnumerable<ChilizRecentTrade>> GetSymbolTrades(string symbol, int? limit = null, CancellationToken ct = default) => GetSymbolTradesAsync(symbol, limit, ct).Result;

        /// <summary>
        /// Gets the recent trades for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get recent trades for</param>
        /// <param name="limit">Result limit</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of recent trades</returns>
        public async Task<WebCallResult<IEnumerable<ChilizRecentTrade>>> GetSymbolTradesAsync(string symbol, int? limit = null, CancellationToken ct = default)
        {
            symbol.ValidateChilizSymbol();
            limit?.ValidateIntBetween(nameof(limit), 1, 1000);

            var parameters = new Dictionary<string, object> { { "symbol", symbol } };
            parameters.AddOptionalParameter("limit", limit?.ToString());
            return await SendRequest<IEnumerable<ChilizRecentTrade>>(GetUrl(RecentTradesEndpoint, ApiQuote, PublicVersion), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Get candlestick data for the provided symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the data for</param>
        /// <param name="interval">The candlestick timespan</param>
        /// <param name="startTime">Start time to get candlestick data</param>
        /// <param name="endTime">End time to get candlestick data</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The candlestick data for the provided symbol</returns>
        public WebCallResult<IEnumerable<ChilizKline>> GetKlines(string symbol, KlineInterval interval, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default) => GetKlinesAsync(symbol, interval, startTime, endTime, limit, ct).Result;

        /// <summary>
        /// Get candlestick data for the provided symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the data for</param>
        /// <param name="interval">The candlestick timespan</param>
        /// <param name="startTime">Start time to get candlestick data</param>
        /// <param name="endTime">End time to get candlestick data</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The candlestick data for the provided symbol</returns>
        public async Task<WebCallResult<IEnumerable<ChilizKline>>> GetKlinesAsync(string symbol, KlineInterval interval, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default)
        {
            symbol.ValidateChilizSymbol();
            limit?.ValidateIntBetween(nameof(limit), 1, 2000);
            var parameters = new Dictionary<string, object> {
                { "symbol", symbol },
                { "interval", JsonConvert.SerializeObject(interval, new KlineIntervalConverter(false)) }
            };
            parameters.AddOptionalParameter("startTime", startTime != null ? ToUnixTimestamp(startTime.Value).ToString() : null);
            parameters.AddOptionalParameter("endTime", endTime != null ? ToUnixTimestamp(endTime.Value).ToString() : null);
            parameters.AddOptionalParameter("limit", limit?.ToString());

            return await SendRequest<IEnumerable<ChilizKline>>(GetUrl(KlinesEndpoint, ApiQuote, PublicVersion), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Get data regarding the last 24 hours for the provided symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the data for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Data over the last 24 hours</returns>
        public WebCallResult<Chiliz24HPrice> Get24HPrice(string symbol, CancellationToken ct = default) => Get24HPriceAsync(symbol, ct).Result;

        /// <summary>
        /// Get data regarding the last 24 hours for the provided symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the data for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Data over the last 24 hours</returns>
        public async Task<WebCallResult<Chiliz24HPrice>> Get24HPriceAsync(string symbol, CancellationToken ct = default)
        {
            symbol.ValidateChilizSymbol();
            var parameters = new Dictionary<string, object> { { "symbol", symbol } };

            return await SendRequest<Chiliz24HPrice>(GetUrl(Price24HEndpoint, ApiQuote, PublicVersion), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Get data regarding the last 24 hours for all symbols
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of data over the last 24 hours</returns>
        public WebCallResult<IEnumerable<ChilizAll24hPrice>> GetAll24HPrices(CancellationToken ct = default) => GetAll24HPricesAsync(ct).Result;

        /// <summary>
        /// Get data regarding the last 24 hours for all symbols
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of data over the last 24 hours</returns>
        public async Task<WebCallResult<IEnumerable<ChilizAll24hPrice>>> GetAll24HPricesAsync(CancellationToken ct = default)
        {
            return await SendRequest<IEnumerable<ChilizAll24hPrice>>(GetUrl(Price24HEndpoint, ApiQuote, PublicVersion), HttpMethod.Get, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the price of a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the price for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Price of symbol</returns>
        public WebCallResult<ChilizPrice> GetPrice(string symbol, CancellationToken ct = default) => GetPriceAsync(symbol, ct).Result;

        /// <summary>
        /// Gets the price of a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the price for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Price of symbol</returns>
        public async Task<WebCallResult<ChilizPrice>> GetPriceAsync(string symbol, CancellationToken ct = default)
        {
            symbol.ValidateChilizSymbol();
            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol }
            };

            return await SendRequest<ChilizPrice>(GetUrl(AllPricesEndpoint, ApiQuote, PublicVersion), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Get a list of the prices of all symbols
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of prices</returns>
        public WebCallResult<IEnumerable<ChilizPrice>> GetAllPrices(CancellationToken ct = default) => GetAllPricesAsync(ct).Result;

        /// <summary>
        /// Get a list of the prices of all symbols
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of prices</returns>
        public async Task<WebCallResult<IEnumerable<ChilizPrice>>> GetAllPricesAsync(CancellationToken ct = default)
        {
            return await SendRequest<IEnumerable<ChilizPrice>>(GetUrl(AllPricesEndpoint, ApiQuote, PublicVersion), HttpMethod.Get, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the best price/quantity on the order book for a symbol.
        /// </summary>
        /// <param name="symbol">Symbol to get book price for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of book prices</returns>
        public WebCallResult<ChilizBookPrice> GetBookPrice(string symbol, CancellationToken ct = default) => GetBookPriceAsync(symbol, ct).Result;

        /// <summary>
        /// Gets the best price/quantity on the order book for a symbol.
        /// </summary>
        /// <param name="symbol">Symbol to get book price for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of book prices</returns>
        public async Task<WebCallResult<ChilizBookPrice>> GetBookPriceAsync(string symbol, CancellationToken ct = default)
        {
            symbol.ValidateChilizSymbol();
            var parameters = new Dictionary<string, object> { { "symbol", symbol } };

            return await SendRequest<ChilizBookPrice>(GetUrl(BookPricesEndpoint, ApiQuote, PublicVersion), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the best price/quantity on the order book for all symbols.
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of book prices</returns>
        public WebCallResult<IEnumerable<ChilizBookPrice>> GetAllBookPrices(CancellationToken ct = default) => GetAllBookPricesAsync(ct).Result;

        /// <summary>
        /// Gets the best price/quantity on the order book for all symbols.
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of book prices</returns>
        public async Task<WebCallResult<IEnumerable<ChilizBookPrice>>> GetAllBookPricesAsync(CancellationToken ct = default)
        {
            return await SendRequest<IEnumerable<ChilizBookPrice>>(GetUrl(BookPricesEndpoint, ApiQuote, PublicVersion), HttpMethod.Get, ct).ConfigureAwait(false);
        }
        #endregion

        #region Signed EndPoints

        #region Account Information
        /// <summary>
        /// Gets account information, including balances
        /// </summary>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The account information</returns>
        public WebCallResult<ChilizAccountInfo> GetAccountInfo(long? receiveWindow = null, CancellationToken ct = default) => GetAccountInfoAsync(receiveWindow, ct).Result;

        /// <summary>
        /// Gets account information, including balances
        /// </summary>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The account information</returns>
        public async Task<WebCallResult<ChilizAccountInfo>> GetAccountInfoAsync(long? receiveWindow = null, CancellationToken ct = default)
        {
            var timestampResult = await CheckAutoTimestamp(ct).ConfigureAwait(false);
            if (!timestampResult)
                return new WebCallResult<ChilizAccountInfo>(timestampResult.ResponseStatusCode, timestampResult.ResponseHeaders, null, timestampResult.Error);

            var parameters = new Dictionary<string, object>
            {
                { "timestamp", GetTimestamp() }
            };
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? defaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await SendRequest<ChilizAccountInfo>(GetUrl(AccountInfoEndpoint, Api, SignedVersion), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }
        #endregion

        #region Orders
        /// <summary>
        /// Gets a list of open orders
        /// </summary>
        /// <param name="symbol">The symbol to get open orders for</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of open orders</returns>
        public WebCallResult<IEnumerable<ChilizOrder>> GetOpenOrders(string? symbol = null, long? orderId = null, int? limit = null, int? receiveWindow = null, CancellationToken ct = default) => GetOpenOrdersAsync(symbol, orderId, limit, receiveWindow, ct).Result;

        /// <summary>
        /// Gets a list of open orders
        /// </summary>
        /// <param name="symbol">The symbol to get open orders for</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of open orders</returns>
        public async Task<WebCallResult<IEnumerable<ChilizOrder>>> GetOpenOrdersAsync(string? symbol = null, long? orderId=null, int? limit=null, int? receiveWindow = null, CancellationToken ct = default)
        {
            symbol?.ValidateChilizSymbol();
            limit?.ValidateIntBetween(nameof(limit), 1, 1000);
            var timestampResult = await CheckAutoTimestamp(ct).ConfigureAwait(false);
            if (!timestampResult)
                return new WebCallResult<IEnumerable<ChilizOrder>>(timestampResult.ResponseStatusCode, timestampResult.ResponseHeaders, null, timestampResult.Error);

            var parameters = new Dictionary<string, object>
            {
                { "timestamp", GetTimestamp() }
            };
            parameters.AddOptionalParameter("symbol", symbol);
            parameters.AddOptionalParameter("orderId", orderId?.ToString());
            parameters.AddOptionalParameter("limit", limit?.ToString());
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? defaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await SendRequest<IEnumerable<ChilizOrder>>(GetUrl(OpenOrdersEndpoint, Api, SignedVersion), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets all orders for the provided symbol
        /// </summary>
        /// <param name="symbol">The symbol to get orders for</param>
        /// <param name="orderId">If set, only orders with an order id higher than the provided will be returned</param>
        /// <param name="startTime">If set, only orders placed after this time will be returned</param>
        /// <param name="endTime">If set, only orders placed before this time will be returned</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of orders</returns>
        public WebCallResult<IEnumerable<ChilizOrder>> GetAllOrders(string? symbol = null, long? orderId = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, int? receiveWindow = null, CancellationToken ct = default) => GetAllOrdersAsync(symbol, orderId, startTime, endTime, limit, receiveWindow, ct).Result;

        /// <summary>
        /// Gets all orders for the provided symbol
        /// </summary>
        /// <param name="symbol">The symbol to get orders for</param>
        /// <param name="orderId">If set, only orders with an order id higher than the provided will be returned</param>
        /// <param name="startTime">If set, only orders placed after this time will be returned</param>
        /// <param name="endTime">If set, only orders placed before this time will be returned</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of orders</returns>
        public async Task<WebCallResult<IEnumerable<ChilizOrder>>> GetAllOrdersAsync(string? symbol = null, long? orderId = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, int? receiveWindow = null, CancellationToken ct = default)
        {
            symbol?.ValidateChilizSymbol();
            limit?.ValidateIntBetween(nameof(limit), 1, 1000);
            var timestampResult = await CheckAutoTimestamp(ct).ConfigureAwait(false);
            if (!timestampResult)
                return new WebCallResult<IEnumerable<ChilizOrder>>(timestampResult.ResponseStatusCode, timestampResult.ResponseHeaders, null, timestampResult.Error);

            var parameters = new Dictionary<string, object>
            {
                { "timestamp", GetTimestamp() }
            };
            parameters.AddOptionalParameter("symbol", symbol);
            parameters.AddOptionalParameter("orderId", orderId?.ToString());
            parameters.AddOptionalParameter("startTime", startTime.HasValue ? JsonConvert.SerializeObject(startTime.Value, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("endTime", endTime.HasValue ? JsonConvert.SerializeObject(endTime.Value, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("limit", limit?.ToString());
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? defaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await SendRequest<IEnumerable<ChilizOrder>>(GetUrl(HistoryOrdersEndpoint, Api, SignedVersion), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Places a new order
        /// </summary>
        /// <param name="symbol">The symbol the order is for</param>
        /// <param name="side">The order side (buy/sell)</param>
        /// <param name="type">The order type</param>
        /// <param name="timeInForce">Lifetime of the order (GoodTillCancel/ImmediateOrCancel/FillOrKill)</param>
        /// <param name="quantity">The amount of the base symbol</param>
        /// <param name="price">The price to use</param>
        /// <param name="newClientOrderId">Unique id for order</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Id's for the placed order</returns>
        public WebCallResult<ChilizPlacedOrder> PlaceOrder(
            string symbol,
            OrderSide side,
            OrderType type,
            TimeInForce? timeInForce = null,
            decimal? quantity = null,
            decimal? price = null,
            string? newClientOrderId = null,
            decimal? stopPrice = null,
            decimal? icebergQty = null,
            string? assetType=null,
            int? receiveWindow = null,
            CancellationToken ct = default) => PlaceOrderAsync(symbol, side, type, timeInForce, quantity, price, newClientOrderId, stopPrice, icebergQty, assetType, receiveWindow, ct).Result;

        /// <summary>
        /// Places a new order
        /// </summary>
        /// <param name="symbol">The symbol the order is for</param>
        /// <param name="side">The order side (buy/sell)</param>
        /// <param name="type">The order type</param>
        /// <param name="timeInForce">Lifetime of the order (GoodTillCancel/ImmediateOrCancel/FillOrKill)</param>
        /// <param name="quantity">The amount of the symbol</param>
        /// <param name="price">The price to use</param>
        /// <param name="newClientOrderId">Unique id for order</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Id's for the placed order</returns>
        public async Task<WebCallResult<ChilizPlacedOrder>> PlaceOrderAsync(
            string symbol,
            OrderSide side,
            OrderType type,
            TimeInForce? timeInForce = null,
            decimal? quantity = null,
            decimal? price = null,
            string? newClientOrderId = null,
            decimal? stopPrice = null,
            decimal? icebergQty = null,
            string? assetType = null,
            int? receiveWindow = null,
            CancellationToken ct = default)
        {
            return await PlaceOrderInternal(GetUrl(NewOrderEndpoint, Api, SignedVersion),
                symbol, side, type, timeInForce, quantity, price, newClientOrderId, stopPrice, icebergQty, assetType, receiveWindow, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Places a new test order. Test orders are not actually being executed and just test the functionality.
        /// </summary>
        /// <param name="symbol">The symbol the order is for</param>
        /// <param name="side">The order side (buy/sell)</param>
        /// <param name="type">The order type (limit/market)</param>
        /// <param name="timeInForce">Lifetime of the order (GoodTillCancel/ImmediateOrCancel)</param>
        /// <param name="quantity">The amount of the symbol</param>
        /// <param name="price">The price to use</param>
        /// <param name="newClientOrderId">Unique id for order</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Id's for the placed test order</returns>
        public WebCallResult<ChilizPlacedOrder> PlaceTestOrder(
            string symbol,
            OrderSide side,
            OrderType type,
            TimeInForce? timeInForce = null,
            decimal? quantity = null,
            decimal? price = null,
            string? newClientOrderId = null,
            decimal? stopPrice = null,
            decimal? icebergQty = null,
            string? assetType = null,
            int? receiveWindow = null,
            CancellationToken ct = default) => PlaceTestOrderAsync(symbol, side, type, timeInForce, quantity, price, newClientOrderId, stopPrice, icebergQty, assetType, receiveWindow, ct).Result;

        /// <summary>
        /// Places a new test order. Test orders are not actually being executed and just test the functionality.
        /// </summary>
        /// <param name="symbol">The symbol the order is for</param>
        /// <param name="side">The order side (buy/sell)</param>
        /// <param name="type">The order type (limit/market)</param>
        /// <param name="timeInForce">Lifetime of the order (GoodTillCancel/ImmediateOrCancel)</param>
        /// <param name="quantity">The amount of the symbol</param>
        /// <param name="price">The price to use</param>
        /// <param name="newClientOrderId">Unique id for order</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Id's for the placed test order</returns>
        public async Task<WebCallResult<ChilizPlacedOrder>> PlaceTestOrderAsync(
            string symbol,
            OrderSide side,
            OrderType type,
            TimeInForce? timeInForce = null,
            decimal? quantity = null,
            decimal? price = null,
            string? newClientOrderId = null,
            decimal? stopPrice = null,
            decimal? icebergQty = null,
            string? assetType = null,
            int? receiveWindow = null,
            CancellationToken ct = default)
        {
            return await PlaceOrderInternal(GetUrl(NewTestOrderEndpoint, Api, SignedVersion),
                symbol, side, type, timeInForce, quantity, price, newClientOrderId, stopPrice, icebergQty, assetType, receiveWindow, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves data for a specific order. Either orderId or origClientOrderId should be provided.
        /// </summary>
        /// <param name="symbol">The symbol the order is for</param>
        /// <param name="orderId">The order id of the order</param>
        /// <param name="origClientOrderId">The client order id of the order</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The specific order</returns>
        public WebCallResult<ChilizOrder> GetOrder(long? orderId = null, string? origClientOrderId = null, long? receiveWindow = null, CancellationToken ct = default) => GetOrderAsync(orderId, origClientOrderId, receiveWindow, ct).Result;

        /// <summary>
        /// Retrieves data for a specific order. Either orderId or origClientOrderId should be provided.
        /// </summary>
        /// <param name="symbol">The symbol the order is for</param>
        /// <param name="orderId">The order id of the order</param>
        /// <param name="origClientOrderId">The client order id of the order</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The specific order</returns>
        public async Task<WebCallResult<ChilizOrder>> GetOrderAsync(long? orderId = null, string? origClientOrderId = null, long? receiveWindow = null, CancellationToken ct = default)
        {
            if (orderId == null && origClientOrderId == null)
                throw new ArgumentException("Either orderId or origClientOrderId must be sent");

            var timestampResult = await CheckAutoTimestamp(ct).ConfigureAwait(false);
            if (!timestampResult)
                return new WebCallResult<ChilizOrder>(timestampResult.ResponseStatusCode, timestampResult.ResponseHeaders, null, timestampResult.Error);

            var parameters = new Dictionary<string, object>
            {
                { "timestamp", GetTimestamp() }
            };
            parameters.AddOptionalParameter("orderId", orderId?.ToString());
            parameters.AddOptionalParameter("origClientOrderId", origClientOrderId);
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? defaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await SendRequest<ChilizOrder>(GetUrl(QueryOrderEndpoint, Api, SignedVersion), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Cancels a pending order
        /// </summary>
        /// <param name="orderId">The order id of the order</param>
        /// <param name="clientOrderId">The client order id of the order</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Id's for canceled order</returns>
        public WebCallResult<ChilizCanceledOrder> CancelOrder(long? orderId = null, string? clientOrderId = null, long? receiveWindow = null, CancellationToken ct = default) => CancelOrderAsync(orderId, clientOrderId, receiveWindow, ct).Result;

        /// <summary>
        /// Cancels a pending order
        /// </summary>
        /// <param name="orderId">The order id of the order</param>
        /// <param name="clientOrderId">The client order id of the order</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Id's for canceled order</returns>
        public async Task<WebCallResult<ChilizCanceledOrder>> CancelOrderAsync(long? orderId = null, string? clientOrderId = null, long? receiveWindow = null, CancellationToken ct = default)
        {
            var timestampResult = await CheckAutoTimestamp(ct).ConfigureAwait(false);
            if (!timestampResult)
                return new WebCallResult<ChilizCanceledOrder>(timestampResult.ResponseStatusCode, timestampResult.ResponseHeaders, null, timestampResult.Error);

            if (!orderId.HasValue && string.IsNullOrEmpty(clientOrderId))
                throw new ArgumentException("Either orderId or origClientOrderId must be sent");

            var parameters = new Dictionary<string, object>
            {
                { "timestamp", GetTimestamp() }
            };
            parameters.AddOptionalParameter("orderId", orderId?.ToString());
            parameters.AddOptionalParameter("clientOrderId", clientOrderId);
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? defaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await SendRequest<ChilizCanceledOrder>(GetUrl(CancelOrderEndpoint, Api, SignedVersion), HttpMethod.Delete, ct, parameters, true).ConfigureAwait(false);
        }
        #endregion

        #region Trades
        /// <summary>
        /// Gets all user trades for provided symbol
        /// </summary>
        /// <param name="startTime">Orders newer than this date will be retrieved</param>
        /// <param name="endTime">Orders older than this date will be retrieved</param>
        /// <param name="fromId">TradeId to fetch from. Default gets most recent trades</param>
        /// <param name="toId">TradeId to fetch to. Default gets most recent trades</param>
        /// <param name="limit">The max number of results</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of trades</returns>
        public WebCallResult<IEnumerable<ChilizTrade>> GetMyTrades(DateTime? startTime = null, DateTime? endTime = null, long? fromId = null, long? toId = null, int? limit = null, long? receiveWindow = null, CancellationToken ct = default) => GetMyTradesAsync(startTime, endTime, fromId, toId, limit, receiveWindow, ct).Result;

        /// <summary>
        /// Gets all user trades for provided symbol
        /// </summary>
        /// <param name="startTime">Orders newer than this date will be retrieved</param>
        /// <param name="endTime">Orders older than this date will be retrieved</param>
        /// <param name="fromId">TradeId to fetch from. Default gets most recent trades</param>
        /// <param name="toId">TradeId to fetch to. Default gets most recent trades</param>
        /// <param name="limit">The max number of results</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of trades</returns>
        public async Task<WebCallResult<IEnumerable<ChilizTrade>>> GetMyTradesAsync(DateTime? startTime = null, DateTime? endTime = null, long? fromId = null, long? toId = null, int? limit = null, long? receiveWindow = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 1000);

            var timestampResult = await CheckAutoTimestamp(ct).ConfigureAwait(false);
            if (!timestampResult)
                return new WebCallResult<IEnumerable<ChilizTrade>>(timestampResult.ResponseStatusCode, timestampResult.ResponseHeaders, null, timestampResult.Error);

            var parameters = new Dictionary<string, object>
            {
                { "timestamp", GetTimestamp() }
            };
            parameters.AddOptionalParameter("startTime", startTime.HasValue ? JsonConvert.SerializeObject(startTime.Value, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("endTime", endTime.HasValue ? JsonConvert.SerializeObject(endTime.Value, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("fromId", fromId?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("toId", toId?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? defaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await SendRequest<IEnumerable<ChilizTrade>>(GetUrl(MyTradesEndpoint, Api, SignedVersion), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }
        #endregion

        #region Deposit & Wtihdrawal
        /// <summary>
        /// GET deposit orders for a specific account.
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="fromId">Deposit OrderId to fetch from. Default gets most recent deposit orders.</param>
        /// <param name="limit">Default 500; max 1000.</param>
        /// <param name="receiveWindow"></param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public WebCallResult<IEnumerable<ChilizDeposit>> GetDepositHistory(DateTime? startTime = null, DateTime? endTime = null, long? fromId = null, int limit = 500, int? receiveWindow = null, CancellationToken ct = default) => GetDepositHistoryAsync(startTime, endTime, fromId, limit, receiveWindow, ct).Result;
        /// <summary>
        /// GET deposit orders for a specific account.
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="fromId">Deposit OrderId to fetch from. Default gets most recent deposit orders.</param>
        /// <param name="limit">Default 500; max 1000.</param>
        /// <param name="receiveWindow"></param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<IEnumerable<ChilizDeposit>>> GetDepositHistoryAsync(DateTime? startTime = null, DateTime? endTime = null, long? fromId = null, int limit=500, int? receiveWindow = null, CancellationToken ct = default)
        {
            limit.ValidateIntBetween(nameof(limit), 1, 1000);

            var timestampResult = await CheckAutoTimestamp(ct).ConfigureAwait(false);
            if (!timestampResult)
                return new WebCallResult<IEnumerable<ChilizDeposit>>(timestampResult.ResponseStatusCode, timestampResult.ResponseHeaders, null, timestampResult.Error);

            var parameters = new Dictionary<string, object>
            {
                { "timestamp", GetTimestamp() }
            };
            parameters.AddOptionalParameter("startTime", startTime != null ? ToUnixTimestamp(startTime.Value).ToString(CultureInfo.InvariantCulture) : null);
            parameters.AddOptionalParameter("endTime", endTime != null ? ToUnixTimestamp(endTime.Value).ToString(CultureInfo.InvariantCulture) : null);
            parameters.AddOptionalParameter("fromId", fromId);
            parameters.AddOptionalParameter("limit", limit);
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? defaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await SendRequest< IEnumerable<ChilizDeposit>>(GetUrl(DepositOrdersEndpoint, Api, SignedVersion), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }
        #endregion

        #region User Data Stream
        /// <summary>
        /// Starts a user stream by requesting a listen key. This listen key can be used in subsequent requests to ChilizSocketClient.SubscribeToUserDataUpdates. The stream will close after 60 minutes unless a keep alive is send.
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Listen key</returns>
        public WebCallResult<string> StartUserStream(int? receiveWindow = null, CancellationToken ct = default) => StartUserStreamAsync(receiveWindow,ct).Result;

        /// <summary>
        /// Starts a user stream by requesting a listen key. This listen key can be used in subsequent requests to ChilizSocketClient.SubscribeToUserDataUpdates. The stream will close after 60 minutes unless a keep alive is send.
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Listen key</returns>
        public async Task<WebCallResult<string>> StartUserStreamAsync(int? receiveWindow = null, CancellationToken ct = default)
        {
            var timestampResult = await CheckAutoTimestamp(ct).ConfigureAwait(false);
            if (!timestampResult)
                return new WebCallResult<string>(timestampResult.ResponseStatusCode, timestampResult.ResponseHeaders, null, timestampResult.Error);

            var parameters = new Dictionary<string, object>
            {
                { "timestamp", GetTimestamp() }
            };
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? defaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            var result = await SendRequest<ChilizListenKey>(GetUrl(GetListenKeyEndpoint, Api, UserDataStreamVersion), HttpMethod.Post, ct, parameters, signed:true).ConfigureAwait(false);
            return new WebCallResult<string>(result.ResponseStatusCode, result.ResponseHeaders, result.Data?.ListenKey, result.Error);
        }

        /// <summary>
        /// Sends a keep alive for the current user stream listen key to keep the stream from closing. Stream auto closes after 60 minutes if no keep alive is send. 30 minute interval for keep alive is recommended.
        /// </summary>
        /// <param name="listenKey">The listen key to keep alive</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public WebCallResult<object> KeepAliveUserStream(string listenKey, int? receiveWindow = null, CancellationToken ct = default) => KeepAliveUserStreamAsync(listenKey, receiveWindow, ct).Result;

        /// <summary>
        /// Sends a keep alive for the current user stream listen key to keep the stream from closing. Stream auto closes after 60 minutes if no keep alive is send. 30 minute interval for keep alive is recommended.
        /// </summary>
        /// <param name="listenKey">The listen key to keep alive</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<object>> KeepAliveUserStreamAsync(string listenKey, int? receiveWindow = null, CancellationToken ct = default)
        {
            listenKey.ValidateNotNull(nameof(listenKey));
            var timestampResult = await CheckAutoTimestamp(ct).ConfigureAwait(false);
            if (!timestampResult)
                return new WebCallResult<object>(timestampResult.ResponseStatusCode, timestampResult.ResponseHeaders, null, timestampResult.Error);

            var parameters = new Dictionary<string, object>
            {
                { "timestamp", GetTimestamp() },
                { "listenKey", listenKey }
            };
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? defaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await SendRequest<object>(GetUrl(KeepListenKeyAliveEndpoint, Api, UserDataStreamVersion), HttpMethod.Put, ct, parameters, signed: true).ConfigureAwait(false);
        }

        /// <summary>
        /// Stops the current user stream
        /// </summary>
        /// <param name="listenKey">The listen key to keep alive</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public WebCallResult<object> StopUserStream(string listenKey, int? receiveWindow = null, CancellationToken ct = default) => StopUserStreamAsync(listenKey, receiveWindow, ct).Result;

        /// <summary>
        /// Stops the current user stream
        /// </summary>
        /// <param name="listenKey">The listen key to keep alive</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<object>> StopUserStreamAsync(string listenKey, int? receiveWindow = null, CancellationToken ct = default)
        {
            listenKey.ValidateNotNull(nameof(listenKey));
            var timestampResult = await CheckAutoTimestamp(ct).ConfigureAwait(false);
            if (!timestampResult)
                return new WebCallResult<object>(timestampResult.ResponseStatusCode, timestampResult.ResponseHeaders, null, timestampResult.Error);

            var parameters = new Dictionary<string, object>
            {
                { "timestamp", GetTimestamp() },
                { "listenKey", listenKey }
            };
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? defaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await SendRequest<object>(GetUrl(CloseListenKeyEndpoint, Api, UserDataStreamVersion), HttpMethod.Delete, ct, parameters, signed: true).ConfigureAwait(false);
        }
        #endregion

        #endregion

        #region Private & Protected Methods

        private async Task<WebCallResult<ChilizPlacedOrder>> PlaceOrderInternal(Uri uri,
            string symbol,
            OrderSide side,
            OrderType type,
            TimeInForce? timeInForce = null,
            decimal? quantity = null,
            decimal? price = null,
            string? newClientOrderId = null,
            decimal? stopPrice = null,
            decimal? icebergQty = null,
            string? assetType = null,
            int? receiveWindow = null,
            CancellationToken ct = default)
        {
            symbol.ValidateChilizSymbol();

            var timestampResult = await CheckAutoTimestamp(ct).ConfigureAwait(false);
            if (!timestampResult)
                return new WebCallResult<ChilizPlacedOrder>(timestampResult.ResponseStatusCode, timestampResult.ResponseHeaders, null, timestampResult.Error);

            var rulesCheck = await CheckTradeRules(symbol, quantity, price, type, ct).ConfigureAwait(false);
            if (!rulesCheck.Passed)
            {
                log.Write(LogVerbosity.Warning, rulesCheck.ErrorMessage!);
                return new WebCallResult<ChilizPlacedOrder>(null, null, null, new ArgumentError(rulesCheck.ErrorMessage!));
            }

            quantity = rulesCheck.Quantity;
            price = rulesCheck.Price;

            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol },
                { "side", JsonConvert.SerializeObject(side, new OrderSideConverter(false)) },
                { "type", JsonConvert.SerializeObject(type, new OrderTypeConverter(false)) },
                { "timestamp", GetTimestamp() }
            };
            parameters.AddOptionalParameter("timeInForce", timeInForce == null ? null : JsonConvert.SerializeObject(timeInForce, new TimeInForceConverter(false)));
            parameters.AddOptionalParameter("quantity", quantity?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("price", price?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("newClientOrderId", newClientOrderId);
            parameters.AddOptionalParameter("stopPrice", stopPrice?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("icebergQty", icebergQty?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("assetType", assetType);
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? defaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await SendRequest<ChilizPlacedOrder>(uri, HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        protected override Error ParseErrorResponse(JToken error)
        {
            if (!error.HasValues)
                return new ServerError(error.ToString());

            if (error["msg"] == null && error["code"] == null)
                return new ServerError(error.ToString());

            if (error["msg"] != null && error["code"] == null)
                return new ServerError((string)error["msg"]);

            return new ServerError((int)error["code"], (string)error["msg"]);
        }

        private Uri GetUrl(string endpoint, string api, string version)
        {
            return new Uri($"{BaseAddress.TrimEnd('/')}/{api}/v{version}/{endpoint}");
        }

        private static long ToUnixTimestamp(DateTime time)
        {
            return (long)(time - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        private string GetTimestamp()
        {
            var offset = autoTimestamp ? calculatedTimeOffset : 0;
            offset += timestampOffset.TotalMilliseconds;
            return ToUnixTimestamp(DateTime.UtcNow.AddMilliseconds(offset)).ToString();
        }

        private async Task<WebCallResult<DateTime>> CheckAutoTimestamp(CancellationToken ct)
        {
            if (autoTimestamp && (!timeSynced || DateTime.UtcNow - lastTimeSync > autoTimestampRecalculationInterval))
                return await GetServerTimeAsync(timeSynced, ct).ConfigureAwait(false);

            return new WebCallResult<DateTime>(null, null, default, null);
        }

        private async Task<ChilizTradeRuleResult> CheckTradeRules(string symbol, decimal? quantity, decimal? price, OrderType type, CancellationToken ct)
        {
            var outputQuantity = quantity;
            var outputPrice = price;

            if (tradeRulesBehaviour == TradeRulesBehaviour.None)
                return ChilizTradeRuleResult.CreatePassed(outputQuantity, outputPrice);

            if (exchangeInfo == null || lastExchangeInfoUpdate == null || (DateTime.UtcNow - lastExchangeInfoUpdate.Value).TotalMinutes > tradeRulesUpdateInterval.TotalMinutes)
                await GetExchangeInfoAsync(ct).ConfigureAwait(false);

            if (exchangeInfo == null)
                return ChilizTradeRuleResult.CreateFailed("Unable to retrieve trading rules, validation failed");

            var symbolData = exchangeInfo.Symbols.SingleOrDefault(s => string.Equals(s.Symbol, symbol, StringComparison.CurrentCultureIgnoreCase));
            if (symbolData == null)
                return ChilizTradeRuleResult.CreateFailed($"Trade rules check failed: Symbol {symbol} not found");

            if (symbolData.LotSizeFilter != null)
            {
                var minQty = symbolData.LotSizeFilter?.MinQuantity;
                var maxQty = symbolData.LotSizeFilter?.MaxQuantity;
                var stepSize = symbolData.LotSizeFilter?.StepSize;

                if (minQty.HasValue && quantity.HasValue)
                {
                    outputQuantity = ChilizHelpers.ClampQuantity(minQty.Value, maxQty!.Value, stepSize!.Value, quantity.Value);
                    if (outputQuantity != quantity.Value)
                    {
                        if (tradeRulesBehaviour == TradeRulesBehaviour.ThrowError)
                        {
                            return ChilizTradeRuleResult.CreateFailed($"Trade rules check failed: LotSize filter failed. Original quantity: {quantity}, Closest allowed: {outputQuantity}");
                        }

                        log.Write(LogVerbosity.Info, $"Quantity clamped from {quantity} to {outputQuantity}");
                    }
                }
            }

            if (price == null)
                return ChilizTradeRuleResult.CreatePassed(outputQuantity, null);

            if (symbolData.PriceFilter != null)
            {
                if (symbolData.PriceFilter.MaxPrice != 0 && symbolData.PriceFilter.MinPrice != 0)
                {
                    outputPrice = ChilizHelpers.ClampPrice(symbolData.PriceFilter.MinPrice, symbolData.PriceFilter.MaxPrice, price.Value);
                    if (outputPrice != price)
                    {
                        if (tradeRulesBehaviour == TradeRulesBehaviour.ThrowError)
                            return ChilizTradeRuleResult.CreateFailed($"Trade rules check failed: Price filter max/min failed. Original price: {price}, Closest allowed: {outputPrice}");

                        log.Write(LogVerbosity.Info, $"price clamped from {price} to {outputPrice}");
                    }
                }

                if (symbolData.PriceFilter.TickSize != 0)
                {
                    var beforePrice = outputPrice;
                    outputPrice = ChilizHelpers.FloorPrice(symbolData.PriceFilter.TickSize, price.Value);
                    if (outputPrice != beforePrice)
                    {
                        if (tradeRulesBehaviour == TradeRulesBehaviour.ThrowError)
                            return ChilizTradeRuleResult.CreateFailed($"Trade rules check failed: Price filter tick failed. Original price: {price}, Closest allowed: {outputPrice}");

                        log.Write(LogVerbosity.Info, $"price rounded from {beforePrice} to {outputPrice}");
                    }
                }
            }

            if (symbolData.MinNotionalFilter == null || quantity == null)
                return ChilizTradeRuleResult.CreatePassed(outputQuantity, outputPrice);

            var notional = quantity * price.Value;
            if (notional < symbolData.MinNotionalFilter.MinNotional)
                return ChilizTradeRuleResult.CreateFailed($"Trade rules check failed: MinNotional filter failed. Order size: {notional}, minimal order size: {symbolData.MinNotionalFilter.MinNotional}");

            return ChilizTradeRuleResult.CreatePassed(outputQuantity, outputPrice);
        }
        #endregion

    }
}
