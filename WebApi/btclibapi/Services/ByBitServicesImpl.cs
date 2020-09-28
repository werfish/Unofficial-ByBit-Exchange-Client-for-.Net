using System;
using ByBitClientLib;
using ByBitClientLib.ClientObjectModel;
using Microsoft.Extensions.Logging;

namespace btclibapi.Services
{
    public class ByBitServicesImpl
    {
        private ILogger<ByBitServicesImpl> _logger = null;
        private readonly String apiKey = "tFAO5jGzHwdrSwUOz4";
        private readonly String apiSecurity = "OJGYMUtNDLpo8LjZU61TQBpxkefDrGkWCoX4";
        private readonly String apiUrl = "https://api.bybit.com";

        private ByBitClient client = null;
        private ConnectionManager cm = null;

        public ByBitServicesImpl()
        {
            #region Logger init
            var loggerFactory = (ILoggerFactory)new LoggerFactory();
            _logger = loggerFactory.CreateLogger<ByBitServicesImpl>();
            #endregion

            client = new ByBitClient(apiKey, apiSecurity, apiUrl);
            cm = new ConnectionManager(client);
        }

        public void Init()
        {
            try
            {
                client = new ByBitClient(apiKey, apiSecurity, apiUrl);
                cm = new ConnectionManager(client);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.ToString());
                throw ex;
            }
        }

        public String ChangeLeverage(String cryptoPair, int leverage)
        {
            try
            {
                return cm.ChangeLeverage(cryptoPair, leverage);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// Make order
        /// </summary>
        /// <param name="cryptoPair">Crypto pair eg. ETHUSD</param>
        /// <param name="contracts">Quantity eg. (1-100)</param>
        /// <returns>JSON response</returns>
        public String MarketOrder(string cryptoPair, int contracts)
        {
            // TODO: string to String 
            try
            {
                _logger.LogInformation("cryptoPair={0}, contracts={1}", cryptoPair, contracts);
                return cm.MarketOrder(cryptoPair, contracts).Response;
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.ToString());
                throw ex;
            }
        }

        public String LimitOrder(string cryptoPair, int contracts, double entryPrice)
        {
            try
            {
                Console.WriteLine("cryptoPair={0}, contracts={1}, entryPrice={2}", cryptoPair, contracts, entryPrice);
                return cm.LimitOrder(cryptoPair, contracts, entryPrice).Response;
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.ToString());
                throw ex;
            }
        }

        public String GetPosition(String cryptoPair)
        {
            try
            {
                _logger.LogInformation("cryptoPair={0}", cryptoPair);
                return cm.GetPosition(cryptoPair).response;
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.ToString());
                throw ex;
            }
        }

        public String ClosePosition(string cryptoPair)
        {
            // TODO: string to String 
            try
            {
                cm.LiquidatePosition(cryptoPair); // true false
                return "true";
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.ToString());
                throw ex;
            }
        }

        public String CancelActiveOrder(String cryptoPair, String orderId)
        {
            try
            {
                String ret = cm.CancelActiveOrder(cryptoPair, orderId);
                Console.WriteLine("ret=" + ret);
                return "true";
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.ToString());
                throw ex;
            }
        }
    }
}
