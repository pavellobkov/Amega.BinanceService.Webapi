using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;

namespace Amega.BinanceService.Webapi
{
    public class BinanceDataProvider
    {
        private static volatile BinanceDataProvider instance;

        private static object syncRoot = new Object();
        private BinanceDataProvider()
        {
            Task.Run(Subscribe);
            Task.Delay(1000);
        }
        public static BinanceDataProvider Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new BinanceDataProvider();
                        }
                    }
                }

                return instance;
            }
        }

        public string CurrentReceivedMessage { get; private set; }
        public ReceivedCurrency CurrentEURUSD { get; private set; }
        public ReceivedCurrency CurrentUSDJPY { get; private set; }
        public ReceivedCurrency CurrentBTCUSD { get; private set; }

        private void initCurrentCurrencies(string receivedMessage)
        {
            CurrentReceivedMessage = receivedMessage;

            var currency = System.Text.Json.JsonSerializer.Deserialize<ReceivedCurrency>(receivedMessage);
            if (string.IsNullOrEmpty(currency.Price)) { Task.Delay(1000); return; }
            currency.SourceMessage = receivedMessage;
            var currencyName = currency.Currency.ToUpper();

            switch (currency.Currency.ToUpper())
            {
                case "BTCUSD":
                case "BTCUSDT":
                    CurrentBTCUSD = currency;
                    break;

                case "EURUSD":
                    CurrentEURUSD = currency;
                    break;

                case "USDJPY":
                    CurrentUSDJPY = currency;
                    break;

                default:
                    throw new Exception($"Unknown currency{currencyName}");
            }
        }

        public async Task<ReceivedCurrency> GetCurrencyByName(string name)
        {
            return await Task.Run(() =>
            {
                return name switch
                {
                    "BTCUSD" => CurrentBTCUSD,
                    "EURUSD" => CurrentBTCUSD,
                    "USDJPY" => CurrentBTCUSD,
                    _ => throw new Exception($"Unknown currency{name}"),
                };
            });
        }

        async Task Subscribe()
        {
            string uri = "wss://stream.binance.com:443/ws";

            using (ClientWebSocket webSocket = new ClientWebSocket())
            {
                await webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
                Console.WriteLine("Connected to WebSocket.");

                var subscribeMessage = new
                {
                    method = "SUBSCRIBE",
                    @params = new[] { "btcusdt@aggTrade" },
                    id = 1
                };

                string jsonMessage = JsonConvert.SerializeObject(subscribeMessage);
                ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(jsonMessage));
                await webSocket.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
                Console.WriteLine("Sent subscription message.");

                var buffer = new byte[1024 * 4];
                while (webSocket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    if (string.IsNullOrEmpty(receivedMessage)) continue;
                    initCurrentCurrencies(receivedMessage);           
                    
                    Console.WriteLine("Received message: " + receivedMessage);
                }
            }
        }
    }
}
