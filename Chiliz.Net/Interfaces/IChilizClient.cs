using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Chiliz.Net.Objects;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;

namespace Chiliz.Net.Interfaces
{
    /// <summary>
    /// Interface for the Chiliz client
    /// </summary>
    public interface IChilizClient: IRestClient
    {
        /// <summary>
        /// Set the API key and secret
        /// </summary>
        /// <param name="apiKey">The api key</param>
        /// <param name="apiSecret">The api secret</param>
        void SetApiCredentials(string apiKey, string apiSecret);

        /// <summary>
        /// Pings the Chiliz API
        /// </summary>
        /// <returns>True if successful ping, false if no response</returns>
        new CallResult<long> Ping(CancellationToken ct = default);

        /// <summary>
        /// Pings the Chiliz API
        /// </summary>
        /// <returns>True if successful ping, false if no response</returns>
        new Task<CallResult<long>> PingAsync(CancellationToken ct = default);

        /// <summary>
        /// Requests the server for the local time. This function also determines the offset between server and local time and uses this for subsequent API calls
        /// </summary>
        /// <param name="resetAutoTimestamp">Whether the response should be used for a new auto timestamp calculation</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Server time</returns>
        WebCallResult<DateTime> GetServerTime(bool resetAutoTimestamp = false, CancellationToken ct = default);

        /// <summary>
        /// Requests the server for the local time. This function also determines the offset between server and local time and uses this for subsequent API calls
        /// </summary>
        /// <param name="resetAutoTimestamp">Whether the response should be used for a new auto timestamp calculation</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Server time</returns>
        Task<WebCallResult<DateTime>> GetServerTimeAsync(bool resetAutoTimestamp = false, CancellationToken ct = default);

        /// <summary>
        /// Gets the order book for the provided symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the order book for</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The order book for the symbol</returns>
        WebCallResult<ChilizOrderBook> GetOrderBook(string symbol, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Gets the order book for the provided symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the order book for</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The order book for the symbol</returns>
        Task<WebCallResult<ChilizOrderBook>> GetOrderBookAsync(string symbol, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Gets the recent trades for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get recent trades for</param>
        /// <param name="limit">Result limit</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of recent trades</returns>
        WebCallResult<IEnumerable<ChilizRecentTrade>> GetSymbolTrades(string symbol, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Gets the recent trades for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get recent trades for</param>
        /// <param name="limit">Result limit</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of recent trades</returns>
        Task<WebCallResult<IEnumerable<ChilizRecentTrade>>> GetSymbolTradesAsync(string symbol, int? limit = null, CancellationToken ct = default);

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
        WebCallResult<IEnumerable<ChilizKline>> GetKlines(string symbol, KlineInterval interval, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default);

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
        Task<WebCallResult<IEnumerable<ChilizKline>>> GetKlinesAsync(string symbol, KlineInterval interval, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Get data regarding the last 24 hours for the provided symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the data for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Data over the last 24 hours</returns>
        WebCallResult<Chiliz24HPrice> Get24HPrice(string symbol, CancellationToken ct = default);

        /// <summary>
        /// Get data regarding the last 24 hours for the provided symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the data for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Data over the last 24 hours</returns>
        Task<WebCallResult<Chiliz24HPrice>> Get24HPriceAsync(string symbol, CancellationToken ct = default);

        /// <summary>
        /// Get data regarding the last 24 hours for all symbols
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of data over the last 24 hours</returns>
        WebCallResult<IEnumerable<ChilizAll24hPrice>> GetAll24HPrices(CancellationToken ct = default);

        /// <summary>
        /// Get data regarding the last 24 hours for all symbols
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of data over the last 24 hours</returns>
        Task<WebCallResult<IEnumerable<ChilizAll24hPrice>>> GetAll24HPricesAsync(CancellationToken ct = default);

        /// <summary>
        /// Gets the price of a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the price for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Price of symbol</returns>
        WebCallResult<ChilizPrice> GetPrice(string symbol, CancellationToken ct = default);

        /// <summary>
        /// Gets the price of a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the price for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Price of symbol</returns>
        Task<WebCallResult<ChilizPrice>> GetPriceAsync(string symbol, CancellationToken ct = default);

        /// <summary>
        /// Get a list of the prices of all symbols
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of prices</returns>
        WebCallResult<IEnumerable<ChilizPrice>> GetAllPrices(CancellationToken ct = default);

        /// <summary>
        /// Get a list of the prices of all symbols
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of prices</returns>
        Task<WebCallResult<IEnumerable<ChilizPrice>>> GetAllPricesAsync(CancellationToken ct = default);

        /// <summary>
        /// Gets the best price/quantity on the order book for a symbol.
        /// </summary>
        /// <param name="symbol">Symbol to get book price for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of book prices</returns>
        WebCallResult<ChilizBookPrice> GetBookPrice(string symbol, CancellationToken ct = default);

        /// <summary>
        /// Gets the best price/quantity on the order book for a symbol.
        /// </summary>
        /// <param name="symbol">Symbol to get book price for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of book prices</returns>
        Task<WebCallResult<ChilizBookPrice>> GetBookPriceAsync(string symbol, CancellationToken ct = default);

        /// <summary>
        /// Gets the best price/quantity on the order book for all symbols.
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of book prices</returns>
        WebCallResult<IEnumerable<ChilizBookPrice>> GetAllBookPrices(CancellationToken ct = default);

        /// <summary>
        /// Gets the best price/quantity on the order book for all symbols.
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of book prices</returns>
        Task<WebCallResult<IEnumerable<ChilizBookPrice>>> GetAllBookPricesAsync(CancellationToken ct = default);

        /// <summary>
        /// Gets a list of open orders
        /// </summary>
        /// <param name="symbol">The symbol to get open orders for</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of open orders</returns>
        WebCallResult<IEnumerable<ChilizOrder>> GetOpenOrders(string? symbol = null, long? orderId = null, int? limit = null, int? receiveWindow = null, CancellationToken ct = default);

        /// <summary>
        /// Gets a list of open orders
        /// </summary>
        /// <param name="symbol">The symbol to get open orders for</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of open orders</returns>
        Task<WebCallResult<IEnumerable<ChilizOrder>>> GetOpenOrdersAsync(string? symbol = null, long? orderId = null, int? limit = null, int? receiveWindow = null, CancellationToken ct = default);

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
        WebCallResult<IEnumerable<ChilizOrder>> GetAllOrders(string? symbol = null, long? orderId = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, int? receiveWindow = null, CancellationToken ct = default);

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
        Task<WebCallResult<IEnumerable<ChilizOrder>>> GetAllOrdersAsync(string? symbol = null, long? orderId = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, int? receiveWindow = null, CancellationToken ct = default);

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
        WebCallResult<ChilizPlacedOrder> PlaceOrder(
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
            CancellationToken ct = default);

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
        Task<WebCallResult<ChilizPlacedOrder>> PlaceOrderAsync(
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
            CancellationToken ct = default);

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
        WebCallResult<ChilizPlacedOrder> PlaceTestOrder(
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
            CancellationToken ct = default);

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
        Task<WebCallResult<ChilizPlacedOrder>> PlaceTestOrderAsync(
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
            CancellationToken ct = default);

        /// <summary>
        /// Retrieves data for a specific order. Either orderId or origClientOrderId should be provided.
        /// </summary>
        /// <param name="orderId">The order id of the order</param>
        /// <param name="origClientOrderId">The client order id of the order</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The specific order</returns>
        WebCallResult<ChilizOrder> GetOrder(long? orderId = null, string? origClientOrderId = null, long? receiveWindow = null, CancellationToken ct = default);

        /// <summary>
        /// Retrieves data for a specific order. Either orderId or origClientOrderId should be provided.
        /// </summary>
        /// <param name="orderId">The order id of the order</param>
        /// <param name="origClientOrderId">The client order id of the order</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The specific order</returns>
        Task<WebCallResult<ChilizOrder>> GetOrderAsync(long? orderId = null, string? origClientOrderId = null, long? receiveWindow = null, CancellationToken ct = default);

        /// <summary>
        /// Cancels a pending order
        /// </summary>
        /// <param name="orderId">The order id of the order</param>
        /// <param name="clientOrderId">The client order id of the order</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Id's for canceled order</returns>
        WebCallResult<ChilizCanceledOrder> CancelOrder(long? orderId = null, string? clientOrderId = null, long? receiveWindow = null, CancellationToken ct = default);

        /// <summary>
        /// Cancels a pending order
        /// </summary>
        /// <param name="orderId">The order id of the order</param>
        /// <param name="clientOrderId">The client order id of the order</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Id's for canceled order</returns>
        Task<WebCallResult<ChilizCanceledOrder>> CancelOrderAsync(long? orderId = null, string? clientOrderId = null, long? receiveWindow = null, CancellationToken ct = default);

        /// <summary>
        /// Gets account information, including balances
        /// </summary>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The account information</returns>
        WebCallResult<ChilizAccountInfo> GetAccountInfo(long? receiveWindow = null, CancellationToken ct = default);

        /// <summary>
        /// Gets account information, including balances
        /// </summary>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The account information</returns>
        Task<WebCallResult<ChilizAccountInfo>> GetAccountInfoAsync(long? receiveWindow = null, CancellationToken ct = default);

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
        WebCallResult<IEnumerable<ChilizTrade>> GetMyTrades(DateTime? startTime = null, DateTime? endTime = null, long? fromId = null, long? toId = null, int? limit = null, long? receiveWindow = null, CancellationToken ct = default);

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
        Task<WebCallResult<IEnumerable<ChilizTrade>>> GetMyTradesAsync(DateTime? startTime = null, DateTime? endTime = null, long? fromId = null, long? toId = null, int? limit = null, long? receiveWindow = null, CancellationToken ct = default);

        /// <summary>
        /// Starts a user stream by requesting a listen key. This listen key can be used in subsequent requests to ChilizSocketClient.SubscribeToUserDataUpdates. The stream will close after 60 minutes unless a keep alive is send.
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Listen key</returns>
        WebCallResult<string> StartUserStream(int? receiveWindow = null, CancellationToken ct = default);

        /// <summary>
        /// Starts a user stream by requesting a listen key. This listen key can be used in subsequent requests to ChilizSocketClient.SubscribeToUserDataUpdates. The stream will close after 60 minutes unless a keep alive is send.
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Listen key</returns>
        Task<WebCallResult<string>> StartUserStreamAsync(int? receiveWindow = null, CancellationToken ct = default);

        /// <summary>
        /// Sends a keep alive for the current user stream listen key to keep the stream from closing. Stream auto closes after 60 minutes if no keep alive is send. 30 minute interval for keep alive is recommended.
        /// </summary>
        /// <param name="listenKey">The listen key to keep alive</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        WebCallResult<object> KeepAliveUserStream(string listenKey, int? receiveWindow = null, CancellationToken ct = default);

        /// <summary>
        /// Sends a keep alive for the current user stream listen key to keep the stream from closing. Stream auto closes after 60 minutes if no keep alive is send. 30 minute interval for keep alive is recommended.
        /// </summary>
        /// <param name="listenKey">The listen key to keep alive</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<object>> KeepAliveUserStreamAsync(string listenKey, int? receiveWindow = null, CancellationToken ct = default);

        /// <summary>
        /// Stops the current user stream
        /// </summary>
        /// <param name="listenKey">The listen key to keep alive</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        WebCallResult<object> StopUserStream(string listenKey, int? receiveWindow = null, CancellationToken ct = default);

        /// <summary>
        /// Stops the current user stream
        /// </summary>
        /// <param name="listenKey">The listen key to keep alive</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<object>> StopUserStreamAsync(string listenKey, int? receiveWindow = null, CancellationToken ct = default);

    }
}