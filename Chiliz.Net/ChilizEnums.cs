using Chiliz.Net.Converters;
using Newtonsoft.Json;

namespace Chiliz.Net
{
    /// <summary>
    /// The status of an order
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// Order is new
        /// </summary>
        New,
        /// <summary>
        /// Order is partly filled, still has quantity left to fill
        /// </summary>
        PartiallyFilled,
        /// <summary>
        /// The order has been filled and completed
        /// </summary>
        Filled,
        /// <summary>
        /// The order has been canceled
        /// </summary>
        Canceled,
        /// <summary>
        /// The order is in the process of being canceled
        /// </summary>
        PendingCancel,
        /// <summary>
        /// The order has been rejected
        /// </summary>
        Rejected,
    }

    /// <summary>
    /// Status of a symbol
    /// </summary>
    public enum SymbolStatus
    {
        /// <summary>
        /// Trading
        /// </summary>
        Trading,
        /// <summary>
        /// Halted
        /// </summary>
        Halt,
        /// <summary>
        /// 
        /// </summary>
        Break
    }

    /// <summary>
    /// The type of an order
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// Limit orders will be placed at a specific price. If the price isn't available in the order book for that asset the order will be added in the order book for someone to fill.
        /// </summary>
        Limit,
        /// <summary>
        /// Market order will be placed without a price. The order will be executed at the best price available at that time in the order book.
        /// </summary>
        Market,

        LimitMaker,
        StopLoss, // (unavailable now)
        StopLossLimit, // (unavailable now)
        TakeProfit, // (unavailable now)
        TakeProfitLimit, // (unavailable now)
        MarketOfPayout, // (unavailable now)
    }

    /// <summary>
    /// The side of an order
    /// </summary>
    public enum OrderSide
    {
        /// <summary>
        /// Buy
        /// </summary>
        Buy,
        /// <summary>
        /// Sell
        /// </summary>
        Sell
    }

    /// <summary>
    /// The time the order will be active for
    /// </summary>
    public enum TimeInForce
    {
        /// <summary>
        /// GoodTillCancel orders will stay active until they are filled or canceled
        /// </summary>
        GoodTillCancel,
        /// <summary>
        /// ImmediateOrCancel orders have to be at least partially filled upon placing or will be automatically canceled
        /// </summary>
        ImmediateOrCancel,
        /// <summary>
        /// FillOrKill orders have to be entirely filled upon placing or will be automatically canceled
        /// </summary>
        FillOrKill
    }

    /// <summary>
    /// The type of execution
    /// </summary>
    public enum ExecutionType
    {
        /// <summary>
        /// New
        /// </summary>
        New,
        /// <summary>
        /// Canceled
        /// </summary>
        Canceled,
        /// <summary>
        /// Replaced
        /// </summary>
        Replaced,
        /// <summary>
        /// Rejected
        /// </summary>
        Rejected,
        /// <summary>
        /// Trade
        /// </summary>
        Trade,
        /// <summary>
        /// Expired
        /// </summary>
        Expired
    }

    /// <summary>
    /// The reason the order was rejected
    /// </summary>
    public enum OrderRejectReason
    {
        /// <summary>
        /// Not rejected
        /// </summary>
        None,
        /// <summary>
        /// Unknown instrument
        /// </summary>
        UnknownInstrument,
        /// <summary>
        /// Closed market
        /// </summary>
        MarketClosed,
        /// <summary>
        /// Quantity out of bounds
        /// </summary>
        PriceQuantityExceedsHardLimits,
        /// <summary>
        /// Unknown order
        /// </summary>
        UnknownOrder,
        /// <summary>
        /// Duplicate
        /// </summary>
        DuplicateOrder,
        /// <summary>
        /// Unkown account
        /// </summary>
        UnknownAccount,
        /// <summary>
        /// Not enough balance
        /// </summary>
        InsufficientBalance,
        /// <summary>
        /// Account not active
        /// </summary>
        AccountInactive,
        /// <summary>
        /// Cannot settle
        /// </summary>
        AccountCannotSettle
    }

    /// <summary>
    /// The interval for the kline
    /// </summary>
    public enum KlineInterval
    {
        /// <summary>
        /// 1m
        /// </summary>
        OneMinute,
        /// <summary>
        /// 3m
        /// </summary>
        ThreeMinutes,
        /// <summary>
        /// 5m
        /// </summary>
        FiveMinutes,
        /// <summary>
        /// 15m
        /// </summary>
        FifteenMinutes,
        /// <summary>
        /// 30m
        /// </summary>
        ThirtyMinutes,
        /// <summary>
        /// 1h
        /// </summary>
        OneHour,
        /// <summary>
        /// 2h
        /// </summary>
        TwoHours,
        /// <summary>
        /// 4h
        /// </summary>
        FourHours,
        /// <summary>
        /// 6h
        /// </summary>
        SixHours,
        /// <summary>
        /// 8h
        /// </summary>
        EightHours,
        /// <summary>
        /// 12h
        /// </summary>
        TwelveHours,
        /// <summary>
        /// 1d
        /// </summary>
        OneDay,
        /// <summary>
        /// 3d
        /// </summary>
        ThreeDays,
        /// <summary>
        /// 1w
        /// </summary>
        OneWeek,
        /// <summary>
        /// 1M
        /// </summary>
        OneMonth
    }

    /// <summary>
    /// Rate limit on what unit
    /// </summary>
    public enum RateLimitInterval
    {
        /// <summary>
        /// Seconds
        /// </summary>
        Second,
        /// <summary>
        /// Minutes
        /// </summary>
        Minute,
        /// <summary>
        /// Days
        /// </summary>
        Day
    }

    /// <summary>
    /// Type of rate limit
    /// </summary>
    public enum RateLimitType
    {
        /// <summary>
        /// Request weight
        /// </summary>
        RequestWeight,
        /// <summary>
        /// Order amount
        /// </summary>
        Orders,
        /// <summary>
        /// Raw requests
        /// </summary>
        RawRequests
    }

    /// <summary>
    /// Filter type
    /// </summary>
    public enum SymbolFilterType
    {
        /// <summary>
        /// Unknown filter type
        /// </summary>
        Unknown,
        /// <summary>
        /// Price filter
        /// </summary>
        Price,
        /// <summary>
        /// Lot size filter
        /// </summary>
        LotSize,
        /// <summary>
        /// Min notional filter
        /// </summary>
        MinNotional,
        /// <summary>
        /// Max orders filter
        /// </summary>
        MaxNumberOrders,
        /// <summary>
        /// Max algo orders filter
        /// </summary>
        MaxNumberAlgorithmicOrders,
        /// <summary>
        /// Max iceberg parts filter
        /// </summary>
        IcebergParts
    }

    /// <summary>
    /// Response type
    /// </summary>
    public enum OrderResponseType
    {
        /// <summary>
        /// Ack only
        /// </summary>
        Acknowledge,
        /// <summary>
        /// Resulting order
        /// </summary>
        Result,
        /// <summary>
        /// Full order info
        /// </summary>
        Full
    }

    /// <summary>
    /// Trade rules behaviour
    /// </summary>
    public enum TradeRulesBehaviour
    {
        /// <summary>
        /// None
        /// </summary>
        None,
        /// <summary>
        /// Throw an error if not complying
        /// </summary>
        ThrowError,
        /// <summary>
        /// Auto adjust order when not complying
        /// </summary>
        AutoComply
    }

    /// <summary>
    /// Status of a margin action
    /// </summary>
    public enum MarginStatus
    {
        /// <summary>
        /// Pending to execution
        /// </summary>
        Pending,
        /// <summary>
        /// Executed, waiting to be confirmed
        /// </summary>
        Completed,
        /// <summary>
        /// Successfully loaned/repayed
        /// </summary>
        Confirmed,
        /// <summary>
        /// execution failed, nothing happened to your account
        /// </summary>
        Failed
    }

    /// <summary>
    /// List status type
    /// </summary>
    public enum ListStatusType
    {
        /// <summary>
        /// Failed action
        /// </summary>
        Response,
        /// <summary>
        /// Placed
        /// </summary>
        ExecutionStarted,
        /// <summary>
        /// Order list is done
        /// </summary>
        Done
    }

    /// <summary>
    /// List order status
    /// </summary>
    public enum ListOrderStatus
    {
        /// <summary>
        /// Executing
        /// </summary>
        Executing,
        /// <summary>
        /// Executed
        /// </summary>
        Done,
        /// <summary>
        /// Rejected
        /// </summary>
        Rejected
    }
}
