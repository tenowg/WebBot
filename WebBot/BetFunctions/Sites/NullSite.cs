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
        //private GeckoWebBrowser _browser;
        //private string _url = "";
        //private bool _loggedIn = false;
        //public bool LoggedIn { get { return _loggedIn; } }
        //public float Balance { get { return 0; } }
        //public string Url { get { return _url; } }
        //public GeckoWebBrowser Browser { set { _browser = value; } }
        //bool LoggedIn { get { return _loggedIn; } set { _loggedIn = value; } }

        public NullSite() : base(null) { }

        //public void SetElements()
        //{
            
        //}

        public override void Initialize()
        {
            _previousBalanceValue = "Not Null";
        }

        public override WinType IsWin() 
        {
            var settings = WebBot.Properties.Settings.Default;

            if (_previousBalanceValue == null)
            {
                SetBet(settings.MinimumBetAmount);
                _previousBalanceValue = "Not Null"; // Create a Null stimulator settings
                return WinType.Initial;
            }
            return WinType.Lose; 
        }
        public override bool HasBalanceChanged() { return true; }

        //public override void Roll(bool high)
        //{
        //}

        public override void Connect() 
        {
            Initialize();
        }

        public override void ClickHigh()
        {
            
        }

        public override void ClickLow()
        {
            
        }

        public override void SetPreviousBalance()
        {
            
        }

        public override void SetChance()
        {
            
        }

        //public override void Login()
        //{
        //    _loggedIn = true;
        //}


        //public override void IncrementStats(WinType winType) { }
    }
}
