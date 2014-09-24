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

        public BaseSite(GeckoWebBrowser browser)
        {
            Settings = WebBot.Properties.Settings.Default;
            Browser = browser;
            BetStarting += BettingStart;
        }

        public virtual void SetElements() { }
        public virtual void Initialize() { }
        public virtual WinType IsWin() { return WinType.Unknown; }
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

        public void OnReset()
        {
            //var settings = WebBot.Properties.Settings.Default;
            SetBet(Settings.MinimumBetAmount);
            Settings.CurrentBets = 0;
            Settings.CurrentProfit = 0m;
            Settings.CurrentStreak = 0;
            Settings.Loses = 0;
            Settings.Wins = 0;
            _previousBalanceValue = null;
            if (Reset != null) Reset(this, EventArgs.Empty);
        }

        public virtual void SetBet(decimal bet)
        {
            //var settings = WebBot.Properties.Settings.Default;
            Settings.CurrentBetAmount = decimal.Round(bet, 8);
        }

        public abstract void Roll(bool high);

        public virtual void Connect()
        {
            _browser.DocumentCompleted += DocumentCompleted;
            _browser.Navigate(_url);
        }

        protected virtual void DocumentCompleted(object sender, GeckoDocumentCompletedEventArgs e)
        {
            _browser.DocumentCompleted -= DocumentCompleted;
        }

        protected virtual void BettingStart(object sender, EventArgs e)
        {
            Initialize();
        }

        public virtual bool CanLogin() { return false; }
        public abstract void Login();

        public virtual void IncrementStats(WinType winType)
        {
            var settings = WebBot.Properties.Settings.Default;

            switch (winType)
            {
                case WinType.Win:
                    settings.Wins++;
                    settings.AllTimeWins++;

                    if (settings.CurrentStreak < 0)
                    {
                        settings.CurrentStreak = 0;
                    }

                    settings.CurrentStreak++;
                    break;
                case WinType.Lose:
                    settings.Loses++;
                    settings.AllTimeLoses++;

                    if (settings.CurrentStreak > 0)
                    {
                        settings.CurrentStreak = 0;
                    }

                    settings.CurrentStreak--;
                    break;
            }

            settings.AllTimeBets++;
            settings.CurrentBets++;
            settings.TotalBets++;

            settings.LastResult = winType.ToString();
        }
    }
}
