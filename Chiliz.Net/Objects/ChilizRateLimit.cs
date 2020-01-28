using Chiliz.Net.Converters;
using Newtonsoft.Json;

namespace Chiliz.Net.Objects
{
    /// <summary>
    /// Rate limit info
    /// </summary>
    public class ChilizRateLimit
    {
        /// <summary>
        /// The type the rate limit applies to
        /// </summary>
        [JsonProperty("rateLimitType"), JsonConverter(typeof(RateLimitConverter))]
        public RateLimitType Type { get; set; }

        /// <summary>
        /// The interval the rate limit uses to count
        /// </summary>
        [JsonProperty("interval"), JsonConverter(typeof(RateLimitIntervalConverter))]
        public RateLimitInterval Interval { get; set; }

        /// <summary>
        /// The amount of calls the limit is
        /// </summary>
        [JsonProperty("intervalUnit")]
        public int IntervalNumber { get; set; }

        /// <summary>
        /// The amount of calls the limit is
        /// </summary>
        [JsonProperty("limit")]
        public int Limit { get; set; }
    }
}
