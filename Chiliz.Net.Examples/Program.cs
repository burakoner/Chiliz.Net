using System;

namespace Chiliz.Net.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            ChilizClient cc = new ChilizClient(new ChilizClientOptions { LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug });
            cc.SetApiCredentials("GvhsxgQLtGeUqD6uI48za3bHaGyIp9j0jQY1qBdMDvHipJo8He1FWKXKpEDhID7c", "oBcKNqd4tHB7Gmdtv6ZmdmbteOw933iNXxdYI2LmeuYbdRjAEssLGpRLkttzN47x");
            var cc01 = cc.GetDepositHistory();












            Console.ReadLine();
            return;
        }
    }
}
