using Gecko;
using Gecko.DOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBot.BetActions.Enums;
using WebBot.BetFunctions.Data;

namespace WebBot.BetFunctions.Sites
{
    public interface ISite
    {
        bool SiteLoaded { get; }
        string Url { get; }
        GeckoWebBrowser Browser { set; }
        decimal Balance { get; }
        decimal PreviousBalance { get; }
        bool LoggedIn { get; set; }
        void SetElements();
        void Initialize();
        WinType IsWin();
        bool HasBalanceChanged();
        void SetBet(decimal amount);
        void Roll(bool high);
        void Connect();
        bool CanLogin();
        //void Login();
        void IncrementStats(WinType winType);
    }
}
