using System;

namespace Chiliz.Net.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            var sc = new ChilizClient();
            var c01 = sc.GetServerTime();

			var wsc = new ChilizSocketClient();
            wsc.SubscribeToKlineUpdates("BTCUSDT", KlineInterval.FourHours, (data) =>
            {
                Console.WriteLine($"{data.Symbol} O:{data.Data[0].Open} H:{data.Data[0].High} L:{data.Data[0].Low} C:{data.Data[0].Close} V:{data.Data[0].Volume}");
            });

            wsc.SubscribeToMarketTickers(new string[] { "BTCUSDT" }, (data) =>
            {
                Console.WriteLine($"{data.Symbol} O:{data.Data[0].Open} H:{data.Data[0].High} L:{data.Data[0].Low} C:{data.Data[0].Close} V:{data.Data[0].Volume}");
            });

            Console.ReadLine();
            return;
        }
    }
}
