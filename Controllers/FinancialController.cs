using Microsoft.AspNetCore.Mvc;

namespace Amega.BinanceService.Webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FinancialController : ControllerBase
    {
        readonly ILogger Logger;
        public FinancialController(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger(this.GetType());
        }

        private static readonly Dictionary<string, string> Instruments = new Dictionary<string, string>
        {
            { CurrencyType.EURUSD, "EUR/USD" },
            { CurrencyType.USDJPY, "USD/JPY" },
            { CurrencyType.BTCUSD, "BTC/USD" }
        };

        [HttpGet("instruments")]
        public IActionResult GetInstruments()
        {
            return Ok(Instruments.Keys);
        }

        [HttpGet("price/{instrument}")]
        public async Task<IActionResult> GetPrice(string instrument)
        {
            try
            {
                var result = await BinanceDataProvider.Instance.GetCurrentCurrencyByName(instrument);
                return Ok(new { Instrument = result.Currency, Price = result.Price });
            }
            catch (Exception ex)
            {
                Logger.LogError($"controller error {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
