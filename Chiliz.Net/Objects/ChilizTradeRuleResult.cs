namespace Chiliz.Net.Objects
{
    public class ChilizTradeRuleResult
    {
        public bool Passed { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
        public string? ErrorMessage { get; set; }

        public static ChilizTradeRuleResult CreatePassed(decimal? quantity, decimal? price)
        {
            return new ChilizTradeRuleResult
            {
                Passed = true,
                Quantity = quantity,
                Price = price
            };
        }

        public static ChilizTradeRuleResult CreateFailed(string message)
        {
            return new ChilizTradeRuleResult
            {
                Passed = false,
                ErrorMessage = message
            };
        }
    }
}
