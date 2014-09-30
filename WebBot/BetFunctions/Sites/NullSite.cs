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
        //private int _betNum = 0;

        //private decimal _previousBalance;
        //private decimal _balance;
        //private decimal _currentBet;
        //private int _currentBets;
        //private int _currentStreak;
        //private WinType _lastResult;
        //public override decimal PreviousBalance
        //{
        //    get
        //    {
        //        return _previousBalance;
        //    }
        //}
        //public override decimal Balance
        //{
        //    get
        //    {
        //        return _balance;
        //    }
        //}

        //private WinType winType = WinType.Lose;
        //public WinType WinType { get { return winType; } set { winType = value; } }
        //public override WinType LastResult
        //{
        //    get
        //    {
        //        return _lastResult;
        //    }

        //    set
        //    {
        //        _lastResult = value;
        //    }
        //}
        //public override decimal CurrentBet
        //{
        //    get
        //    {
        //        return _currentBet;
        //    }
        //    set
        //    {
        //        _currentBet = value;
        //    }
        //}
        //public override int CurrentBets { get { return _currentBets; } }
        //public override int CurrentStreak { get { return _currentStreak; } }

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

        private decimal _balance;
        public override decimal Balance { get { return _balance; } }

        private WinType winType = WinType.Lose;

        public NullSite() : base(null) { }

        public override void SetElements()
        {
            
        }

        public void SetBalance(decimal value)
        {
            _balance = value;
        }

        public override void Initialize()
        {
            _previousBalanceValue = "Not Null";
            //_previousBalance = 0.0m;
            _balance = 0.0m;
            CurrentBet = 0.0m;
            CurrentBets = 0;
            CurrentStreak = 0;
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

        //public override void IncrementStats(WinType winType)
        //{
        //    //_betNum++;
        //    _currentBets++;
        //    if (winType == WinType.Lose)
        //    {
        //        if (_currentStreak < 0)
        //        {
        //            _currentStreak = 0;
        //        }
        //        _currentStreak--;
        //    }
        //    else
        //    {
        //        if (_currentStreak > 0)
        //        {
        //            _currentStreak = 0;
        //        }
        //        _currentStreak++;
        //    }
        //}

        public override void Roll(bool high)
        {
            if (winType == WinType.Lose)
            {
                _balance -= CurrentBet;
            }
        }

        public override void ClickHigh()
        {
            
        }

        public override void ClickLow()
        {
            
        }

        //public override void SetBet(decimal bet)
        //{
        //    _currentBet = decimal.Round(bet, 8);
        //}

        public override void SetPreviousBalance()
        {
            _previousBalanceValue = null;
        }

        public override void SetChance()
        {
            
        }

        //public override void OnReset()
        //{
        //}
    }
}
