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
        public enum OrderStatus { Created, Rejected, New , PartiallyFilled , Filled, Cancelled , PendingCancel, NONE }
        public enum StopOrderStatus { Active , Untriggered, Triggered , Cancelled , Rejected , Deactivated , NONE}
        public enum CancelType { CancelByUser, CancelByReduceOnly, CancelByPrepareLiq, CancelAllBeforeLiq, CancelByPrepareAdl, CancelAllBeforeAdl, CancelByAdmin, CancelByTpSlTsClear, CancelByPzSideCh }
        public enum CreateType { CreateByUser, CreateByClosing, CreateByAdminClosing, CreateByStopOrder, CreateByTakeProfit, CreateByStopLoss, CreateByTrailingStop, CreateByLiq, CreateByAdl_PassThrough, CreateByTakeOver_PassThrough }
        public enum ExecType { Trade, AdlTrade, Funding, BustTrade }
        public enum LiquidityType { AddedLiquidity, RemovedLiquidity }
        public enum Symbol { BTCUSD, ETHUSD, EOSUSD, XRPUSD }
        public enum Currency { BTC, ETH , EOS , XRP , USDT }
        public enum TimeInForce {GoodTillCancel, ImmediateOrCancel, FillOrKill, PostOnly}
        public enum OrderType { Market, Limit , ConditionalMarket, ConditionalLimit} //This is not ByBit Original Order type ENUM
        public enum Side {Buy, Sell }

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

        internal Side getSide(String side)
        {
            return (Side)Enum.Parse(typeof(Side), side);
        }

    }
}
