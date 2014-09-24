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
    public class RPCDice : BaseSite
    {
        public override decimal Balance { get { return decimal.Parse(BalanceNode.TextContent.Substring(0, BalanceNode.TextContent.LastIndexOf(" "))); } }
        public override decimal PreviousBalance { get { return decimal.Parse(_previousBalanceValue.Substring(0, _previousBalanceValue.LastIndexOf(" "))); } }

        public GeckoInputElement Chance { get; set; }
        public GeckoInputElement BetValue { get; set; }
        public GeckoInputElement Payout { get; set; }
        public GeckoAnchorElement BetHigh { get; set; }
        public GeckoLinkElement BetLow { get; set; }
        public GeckoHtmlElement BalanceNode { get; set; }

        // Login
        public GeckoLinkElement LoginButton { get; set; }
        public GeckoInputElement UserName { get; set; }
        public GeckoInputElement Password { get; set; }
        public GeckoInputElement SignIn { get; set; }

        public RPCDice(GeckoWebBrowser browser) : base(browser)
        {
            Url = "http://prcdice.eu";
        }

        public override void SetElements()
        {
            try
            {
                GeckoDocument document = _browser.Document;
                Chance = document.GetElementById("diceChance") as GeckoInputElement;
                BetValue = document.GetElementById("diceBetAmount") as GeckoInputElement;
                Payout = document.GetElementById("dicePayout") as GeckoInputElement;
                BetHigh = document.GetElementsByClassName("diceHighButton")[0] as GeckoAnchorElement;
                BetLow = document.GetElementsByClassName("diceLoButton")[0] as GeckoLinkElement;
                BalanceNode = document.GetElementsByClassName("myBalance")[0] as GeckoHtmlElement;
            }
            catch (Exception e)
            {
                Console.WriteLine("Not able to set all Elements");
            }
        }

        public override void Initialize()
        {
            //var settings = WebBot.Properties.Settings.Default;
            SetElements();
            SetBet(Settings.CurrentBetAmount);
        }

        public override WinType IsWin()
        {
            //var settings = WebBot.Properties.Settings.Default;
            if (HasBalanceChanged())
            {
                decimal current = decimal.Parse(_currentBalanceValue.Substring(0, _currentBalanceValue.LastIndexOf(" ")));
                decimal previous;
                if (_previousBalanceValue == "" || _previousBalanceValue == null)
                {
                    previous = 0.0m;
                    //Settings.CurrentBetAmount = Settings.MinimumBetAmount;
                    SetBet(Settings.MinimumBetAmount);
                    return WinType.Initial;
                }
                else
                {
                    previous = decimal.Parse(_previousBalanceValue.Substring(0, _previousBalanceValue.LastIndexOf(" ")));
                }
                    
                if (current < previous)
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

        public override bool HasBalanceChanged()
        {
            // First check if roll has completed by checking of the value of Balance has changed
            _currentBalanceValue = BalanceNode.InnerHtml;
            if (_previousBalanceValue != _currentBalanceValue)
            {
                return true;
            }
            
            return false;
        }

        public override void SetBet(decimal bet)
        {
            //var settings = WebBot.Properties.Settings.Default;
            base.SetBet(bet);
            if (BetValue != null) { BetValue.Value = Settings.CurrentBetAmount.ToString(); }
        }
        public override void Roll(bool high)
        {
            //var settings = WebBot.Properties.Settings.Default;
            
            if (HasBalanceChanged())
            {
                // Do The Roll
                if (high)
                {
                    if (!Settings.PauseBet)
                    {
                        BetHigh.Click();
                    }
                }
                else
                {
                    if (!Settings.PauseBet)
                    {
                        BetLow.Click();
                    }
                }
                _previousBalanceValue = BalanceNode.InnerHtml;
            }            
        }

        public override void Login()
        {
        }

        protected override void DocumentCompleted(object sender, Gecko.Events.GeckoDocumentCompletedEventArgs e)
        {
            SetElements();
            base.DocumentCompleted(sender, e);
        }
    }
}
