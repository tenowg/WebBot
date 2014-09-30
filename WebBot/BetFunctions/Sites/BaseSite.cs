using Gecko;
using Gecko.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBot.BetActions.Enums;
using WebBot.BetFunctions.Data;

namespace WebBot.BetFunctions.Sites
{
    public abstract class BaseSite : ISite
    {
        protected bool _loggedIn = false; 
        protected GeckoWebBrowser _browser;
        protected string _previousBalanceValue;
        protected string _currentBalanceValue;
        protected bool _siteLoaded;

        public WebBot.Properties.Settings Settings { get; set; }
        protected string _url = "";

        public virtual decimal Balance { get { return 0; } }
        public virtual decimal PreviousBalance { get { return 0; } }
        public GeckoWebBrowser Browser { set { _browser = value; } }
        public bool LoggedIn { get { return _loggedIn; } set { _loggedIn = value; } }
        public virtual string Url { get { return _url; } set { _url = value; } }
        public bool SiteLoaded { get { return _siteLoaded; } }

        public virtual decimal CurrentBet { get { return Settings.CurrentBetAmount; } set { Settings.CurrentBetAmount = value; } }
        public virtual int CurrentBets { get { return Settings.CurrentBets; } set { Settings.CurrentBets = value; } }
        public virtual long AllTimeBets { get { return Settings.AllTimeBets; } set { Settings.AllTimeBets = value; } }
        public virtual long TotalBets { get { return Settings.TotalBets; } set { Settings.TotalBets = value; } }
        public virtual int CurrentStreak { get { return Settings.CurrentStreak; } set { Settings.CurrentStreak = value; } }
        public virtual decimal CurrentProfit { get { return Settings.CurrentProfit; } set { Settings.CurrentProfit = value; } }
        public virtual decimal BaseBet { get { return Settings.MinimumBetAmount; } }
        public virtual WinType LastResult { get { return (WinType)Enum.Parse(typeof(WinType), Settings.LastResult); } set { Settings.LastResult = value.ToString(); } }
        public virtual long Loses { get { return Settings.Loses; } set { Settings.Loses = value; } }
        public virtual long AllTimeLoses { get { return Settings.AllTimeLoses; } set { Settings.AllTimeLoses = value; } }
        public virtual long Wins { get { return Settings.Wins; } set { Settings.Wins = value; } }
        public virtual long AllTimeWins { get { return Settings.AllTimeWins; } set { Settings.AllTimeWins = value; } }
        public virtual bool PauseBet { get { return Settings.PauseBet; } set { Settings.PauseBet = value; } }

        public BaseSite(GeckoWebBrowser browser)
        {
            Settings = WebBot.Properties.Settings.Default;
            Browser = browser;
            BetStarting += BettingStart;
        }

        public abstract void SetElements();
        public virtual void Initialize() 
        {
            SetElements();
            SetBet(CurrentBet);
        }
        
        // TODO Change these variables too for completeness
        public virtual WinType IsWin()
        {
            if (HasBalanceChanged())
            {
                if (_previousBalanceValue == "" || _previousBalanceValue == null)
                {
                    SetBet(Settings.MinimumBetAmount);
                    Settings.CurrentChance = Settings.BaseChance;
                    return WinType.Initial;
                }

                if (Balance < PreviousBalance)
                {
                    return WinType.Lose;
                }
                else
                {
                    return WinType.Win;
                }
            }

            return WinType.Unknown;
        }
        
        public abstract bool HasBalanceChanged();

        public event EventHandler RequestStopped;
        public void OnRequestStopped()
        {
            if (RequestStopped != null) RequestStopped(this, EventArgs.Empty);
        }

        public event EventHandler BetStarting;
        public void OnBetStarting()
        {
            if (BetStarting != null) BetStarting(this, EventArgs.Empty);
        }

        public event EventHandler Reset;
        public virtual void OnReset()
        {
            //var settings = WebBot.Properties.Settings.Default;
            SetBet(BaseBet);
            //Settings.CurrentBets = 0;
            CurrentBets = 0;
            //Settings.CurrentProfit = 0m;
            CurrentProfit = 0m;
            //Settings.CurrentStreak = 0;
            CurrentStreak = 0;
            //Settings.Loses = 0;
            Loses = 0;
            //Settings.Wins = 0;
            Wins = 0;
            _previousBalanceValue = null;
            if (Reset != null) Reset(this, EventArgs.Empty);
        }

        public virtual void SetBet(decimal bet)
        {
            //var settings = WebBot.Properties.Settings.Default;
            //Settings.CurrentBetAmount = decimal.Round(bet, 8);
            CurrentBet = decimal.Round(bet, 8);
        }

        public abstract void ClickHigh();
        public abstract void ClickLow();
        public abstract void SetPreviousBalance();
        public abstract void SetChance();

        public virtual void Roll(bool high)
        {
            if (HasBalanceChanged())
            {
                // Do The Roll
                SetChance();
                if (high)
                {
                    if (!PauseBet)
                    {
                        ClickHigh();
                    }
                }
                else
                {
                    if (!PauseBet)
                    {
                        ClickLow();
                    }
                }
                SetPreviousBalance();
            }    
        }

        public virtual void Connect()
        {
            _browser.DocumentCompleted += DocumentCompleted;
            _browser.Navigate(_url);
        }

        protected virtual void DocumentCompleted(object sender, GeckoDocumentCompletedEventArgs e)
        {
            SetElements();
            _browser.DocumentCompleted -= DocumentCompleted;
        }

        protected virtual void BettingStart(object sender, EventArgs e)
        {
            Initialize();
        }

        public virtual bool CanLogin() { return false; }
        //public abstract void Login();

        public virtual void IncrementStats(WinType winType)
        {
            //var settings = WebBot.Properties.Settings.Default;

            switch (winType)
            {
                case WinType.Win:
                    //Settings.Wins++;
                    Wins++;
                    AllTimeWins++;

                    if (CurrentStreak < 0)
                    {
                        CurrentStreak = 0;
                    }

                    CurrentStreak++;
                    break;
                case WinType.Lose:
                    Loses++;
                    AllTimeLoses++;

                    if (CurrentStreak > 0)
                    {
                        CurrentStreak = 0;
                    }

                    CurrentStreak--;
                    break;
            }

            AllTimeBets++;
            CurrentBets++;
            TotalBets++;

            LastResult = winType;
        }
    }
}
