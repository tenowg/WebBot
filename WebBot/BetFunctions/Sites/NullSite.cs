using Gecko;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBot.BetActions.Enums;
using WebBot.BetFunctions.Data;

namespace WebBot.BetFunctions.Sites
{
    public class NullSite: BaseSite
    {
        public override decimal CurrentBet { get; set; }
        public override int CurrentBets { get; set; }
        public override long AllTimeBets { get; set; }
        public override long TotalBets { get; set; }
        public override int CurrentStreak { get; set; }
        public override decimal CurrentProfit { get; set; }
        public override decimal BaseBet { get { return Settings.MinimumBetAmount; } }
        public override WinType LastResult { get; set; }
        public override long Loses { get; set; }
        public override long AllTimeLoses { get; set; }
        public override long Wins { get; set; }
        public override long AllTimeWins { get; set; }
        public override bool PauseBet { get; set; }
        public override decimal CurrentChance { get; set; }
        public override decimal CurrentWagered { get; set; }
        public override decimal AllTimeWagered { get; set; }

        private decimal _balance;
        private decimal _previousBalance;
        public override decimal Balance { get { return _balance; } }
        public override decimal PreviousBalance { get { return _previousBalance; } }

        private WinType winType = WinType.Lose;

        public NullSite() : base(null) { }

        public override void SetElements() { }

        public void SetBalance(decimal value)
        {
            _balance = value;
        }

        public override void Initialize()
        {
            _previousBalanceValue = "Not Null";
            _balance = 0.0m;
            CurrentBet = BaseBet;
            CurrentBets = 0;
            CurrentStreak = 0;
            CurrentChance = Settings.BaseChance;
            CurrentWagered = 0;
        }

        public override WinType IsWin() 
        {
            return winType;
        }

        public override bool HasBalanceChanged() { return true; }

        public override void Connect() 
        {
            Initialize();
        }

        public override void Roll(bool high)
        {
            SetPreviousBalance();
            if (winType == WinType.Lose)
            {
                _balance -= CurrentBet;
            }
        }

        public override void ClickHigh() { }

        public override void ClickLow() { }

        public override void SetPreviousBalance()
        {
            //_previousBalanceValue = null;
            _previousBalance = Balance;
        }

        public override void SetChance() { }
    }
}
