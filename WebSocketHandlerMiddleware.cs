using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json.Serialization;

namespace Amega.BinanceService.Webapi
{
    public class WebSocketHandlerMiddleware
    {
        private static readonly ConcurrentDictionary<string, WebSocket> WebSockets = new ConcurrentDictionary<string, WebSocket>();

        private readonly RequestDelegate _next;
        private readonly ILogger<WebSocketHandlerMiddleware> _logger;

        public WebSocketHandlerMiddleware(RequestDelegate next, ILogger<WebSocketHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    var receivedBytes = new ArraySegment<byte>(new byte[1024]);
                    var result = await webSocket.ReceiveAsync(receivedBytes, CancellationToken.None);
                    var instrument = System.Text.Json.JsonSerializer.Deserialize<SubscriptionRequest>(Encoding.UTF8.GetString(receivedBytes.Array, 0, result.Count));

                    var socketId = Guid.NewGuid().ToString();
                    WebSockets[socketId] = webSocket;

                    await ListenWebSocketAsync(socketId, webSocket, instrument);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Middleware error: '{ex.Message}'.");
            }
            finally
            {
                await _next.Invoke(context);
            }
        }
   
        private async Task ListenWebSocketAsync(string socketId, WebSocket webSocket, SubscriptionRequest instrument)
        {
            var cancellationToken = new CancellationToken();

            while (webSocket.State == WebSocketState.Open)
            {
                if (BinanceDataProvider.Instance.CurrentReceivedMessage is null) continue;

                await Task.Delay(800);

                var updateMessage = Encoding.UTF8.GetBytes(BinanceDataProvider.Instance.CurrentReceivedMessage);
                await webSocket.SendAsync(new ArraySegment<byte>(updateMessage), WebSocketMessageType.Text, true, cancellationToken);            
            }
        }    

        private class SubscriptionRequest
        {
            [JsonPropertyName("instrument")]
            public string Instrument { get; set; }
        }
    }

    public static class WebSocketHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseWebSocketHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<WebSocketHandlerMiddleware>();
        }
    }
}
