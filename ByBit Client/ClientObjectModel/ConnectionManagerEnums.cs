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
        public enum OrderStatus { Created, Rejected, New , PartiallyFilled , Filled, Cancelled , PendingCancel }
        public enum StopOrderType { Active , Untriggered, Triggered , Cancelled , Rejected , Deactivated , NONE}
        public enum CancelType { CancelByUser, CancelByReduceOnly, CancelByPrepareLiq, CancelAllBeforeLiq, CancelByPrepareAdl, CancelAllBeforeAdl, CancelByAdmin, CancelByTpSlTsClear, CancelByPzSideCh }
        public enum CreateType { CreateByUser, CreateByClosing, CreateByAdminClosing, CreateByStopOrder, CreateByTakeProfit, CreateByStopLoss, CreateByTrailingStop, CreateByLiq, CreateByAdl_PassThrough, CreateByTakeOver_PassThrough }
        public enum ExecType { Trade, AdlTrade, Funding, BustTrade }
        public enum LiquidityType { AddedLiquidity, RemovedLiquidity }
        public enum Symbol { BTCUSD, ETHUSD, EOSUSD, XRPUSD }
        public enum Currency { BTC, ETH , EOS , XRP , USDT }
        public enum TimeInForce {GoodTillCancel, ImmediateOrCancel, FillOrKill, PostOnly}
        public enum OrderType { Market, Limit , ConditionalMarket, ConditionalLimit} //This is not ByBit Original Order type
        public enum Side {Buy, Sell }
    }
}
