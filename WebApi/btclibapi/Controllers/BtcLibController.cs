using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using btclibapi.Services;
using System.Net;

namespace btclibapi.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/btclib")]
    public class BtcLibControllers : ControllerBase
    {
        #region Private Fields
        private ByBitServicesImpl obj = null;
        private readonly ILogger<BtcLibControllers> _logger = null;
        #endregion

        public BtcLibControllers(ILogger<BtcLibControllers> logger)
        {
            _logger = logger;
            obj = new ByBitServicesImpl();
        }

        [HttpGet]
        [Route("v1/ChangeLeverage")]
        public String ChangeLeverage(String cryptoPair, int leverage)
        {
            // https://localhost:5001/api/BtcLibControllers/FirstTest?cryptoPair=ETHUSD&leverage=25
            try
            {
                Console.WriteLine("cryptoPair={0}, leverage={1}", cryptoPair, leverage);
                return obj.ChangeLeverage(cryptoPair, leverage);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.ToString());
                return ex.ToString();
            }
        }

        [HttpGet]
        [Route("v1/MarketOrder")]
        public String MarketOrder(String cryptoPair, int contracts)
        {
            // https://localhost:5001/api/BtcLibControllers/MarketOrder?cryptoPair=ETHUSD&contracts=10
            // https://localhost:5001/api/BtcLibControllers/MarketOrder?cryptoPair=ETHUSD&contracts=-10
            try
            {
                Console.WriteLine("cryptoPair={0}, contracts={1}", cryptoPair, contracts);
                return obj.MarketOrder(cryptoPair, contracts);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.ToString());
                return ex.ToString();
            }
        }

        [HttpGet]
        [Route("v1/LimitOrder")]
        public String LimitOrder(string cryptoPair, int contracts, double entryPrice)
        {
            // https://localhost:5001/api/btclib/v1/LimitOrder?cryptoPair=ETHUSD&contracts=10&entryPrice=231.9
            try
            {
                Console.WriteLine("cryptoPair={0}, contracts={1}, entryPrice={2}", cryptoPair, contracts, entryPrice);
                return obj.LimitOrder(cryptoPair, contracts, entryPrice);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.ToString());
                return ex.ToString();
            }
        }

        [HttpGet]
        [Route("v1/GetPosition/{cryptoPair}")]
        public ActionResult<String> GetPosition(String cryptoPair)
        {
            // https://localhost:5001/api/btclib/v1/GetPosition/ETHUSD
            try
            {
                _logger.LogInformation("[GetPosition] cryptoPair={0}", cryptoPair);
                String ret = obj.GetPosition(cryptoPair);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return ret;
            }
            catch (Exception ex)
            {
                String responseText = String.Empty;
                _logger.LogError("{0}", ex.ToString());
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return ex.ToString();
            }
        }

        [HttpGet]
        [Route("v1/ClosePosition")]
        public String ClosePosition(String cryptoPair)
        {
            // https://localhost:5001/api/BtcLibControllers/ClosePosition?cryptoPair=ETHUSD
            try
            {
                Console.WriteLine("cryptoPair={0}", cryptoPair);
                return obj.ClosePosition(cryptoPair);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.ToString());
                return ex.ToString();
            }
        }

        [HttpGet]
        [Route("v1/CancelActiveOrder")]
        public String CancelActiveOrder(String cryptoPair, String orderId)
        {
            try
            {
                Console.WriteLine("cryptoPair={0}, orderId={1}", cryptoPair, orderId);
                return obj.CancelActiveOrder(cryptoPair, orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.ToString());
                return ex.ToString();
            }
        }

        [HttpGet]
        [Route("v1/Version")]
        public String Version()
        {
            String version = "1.0.0.0";
            return version;
        }
    }
}