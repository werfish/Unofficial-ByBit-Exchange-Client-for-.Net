using System;
using System.Collections.Generic;
using System.Text;

namespace ByBitClientLib.ClientObjectModel
{
    public partial class ConnectionManager
    {
        public enum WalletFundType { Deposit, Withdraw, RealisedPNL, Commission, Refund, Prize, ExchangeOrderWithdraw, ExchangeOrderDeposit }
        public enum WithdrawStatus { ToBeConfirmed, UnderReview, Pending, Success, CancelByUser, Reject , Expire }
        public enum TriggerPriceType { LastPrice, IndexPrice, MarkPrice, NONE}
        public enum OrderStatus { Created, Rejected, New , PartiallyFilled , Filled, Cancelled , PendingCancel, Deactivated, NONE }
        public enum Direction {next, prev}
        public enum StopOrderStatus { Active , Untriggered, Triggered , Cancelled , Rejected , Deactivated , NONE}
        public enum CancelType { CancelByUser, CancelByReduceOnly, CancelByPrepareLiq, CancelAllBeforeLiq, CancelByPrepareAdl, CancelAllBeforeAdl, CancelByAdmin, CancelByTpSlTsClear, CancelByPzSideCh }
        public enum CreateType { CreateByUser, CreateByClosing, CreateByAdminClosing, CreateByStopOrder, CreateByTakeProfit, CreateByStopLoss, CreateByTrailingStop, CreateByLiq, CreateByAdl_PassThrough, CreateByTakeOver_PassThrough }
        public enum ExecType { Trade, AdlTrade, Funding, BustTrade, NONE }
        public enum LiquidityType { AddedLiquidity, RemovedLiquidity }
        public enum Symbol { BTCUSD, ETHUSD, EOSUSD, XRPUSD }
        public enum Currency { BTC, ETH , EOS , XRP , USDT, BCH, LTC, LINK, BNB, ADA, DOGE, DOT, UNI, SOL, MATIC, ETC, FIL, AAVE, XTZ, SUSHI, XEM }
        public enum TimeInForce {GoodTillCancel, ImmediateOrCancel, FillOrKill, PostOnly}
        public enum OrderType { Market, Limit , ConditionalMarket, ConditionalLimit} //This is not ByBit Original Order type ENUM
        public enum Side {Buy, Sell }
        public enum Interval: int {M1 = 1, M3 = 3, M5 = 5, M15 = 15, M30 = 30, M60 = 60, M120 = 120, M240 = 240, M360 = 360, M720 = 720, D,M,W}
        public enum ContractType { InversePerpetual, LinearPerpetual, InverseFutures}
        public enum ContractStatus { Trading, Settling, Closed }

        internal TriggerPriceType getTriggerPriceType(String triggerPriceType)
        {
            return (TriggerPriceType)Enum.Parse(typeof(TriggerPriceType), triggerPriceType);
        }

        internal OrderStatus getOrderStatus(String orderStatus)
        {
            return (OrderStatus)Enum.Parse(typeof(OrderStatus), orderStatus);
        }

        internal TimeInForce getTimeInForce(String timeInForce)
        {
            return (TimeInForce)Enum.Parse(typeof(TimeInForce), timeInForce);
        }

        internal StopOrderStatus getStopOrderStatus(String stopOrderStatus)
        {
            return (StopOrderStatus)Enum.Parse(typeof(StopOrderStatus), stopOrderStatus);
        }

        internal ExecType getExecutionType(String execType) 
        {
            return (ExecType)Enum.Parse(typeof(ExecType), execType);
        }

        internal OrderType getOrderType(String orderType)
        {
            return (OrderType)Enum.Parse(typeof(OrderType), orderType);
        }

        internal Side getSide(String side)
        {
            return (Side)Enum.Parse(typeof(Side), side);
        }

        internal Interval getInterval(String interval) 
        {
            Int32 numInterval;

            if (Int32.TryParse(interval, out numInterval))
            {
                return (Interval)numInterval;
            }
            else 
            {
                return (Interval)Enum.Parse(typeof(Interval), interval);
            }
        }

        internal String parseInterval(Interval interval) 
        {
            if (interval.ToString().Length >= 2)
            {
                return ((Int32)interval).ToString();
            }
            else 
            {
                return interval.ToString();
            }
        }

        internal ContractStatus getContractStatus(String contractStatus) 
        {
            return (ContractStatus)Enum.Parse(typeof(ContractStatus), contractStatus);
        }

        internal Currency getCurrency(String currency)
        {
            return (Currency)Enum.Parse(typeof(Currency), currency);
        }
    }
}
