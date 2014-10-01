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
    public class DiceNinja : BaseSite
    {
        public override decimal Balance { get { return decimal.Parse(BalanceNode.TextContent); } }//.Substring(0, BalanceNode.TextContent.LastIndexOf(" "))); } }
        public override decimal PreviousBalance
        {
            get
            {
                if (_previousBalanceValue == null) 
                {
                    return 0;
                }
                return decimal.Parse(_previousBalanceValue); //.Substring(0, _previousBalanceValue.LastIndexOf(" ")));
            }
        }

        public GeckoInputElement Chance { get; set; }
        public GeckoInputElement BetValue { get; set; }
        public GeckoHtmlElement Payout { get; set; }
        public GeckoDivElement BetHigh { get; set; }
        public GeckoDivElement BetLow { get; set; }
        public GeckoDivElement BalanceNode { get; set; }

        // Login
        public GeckoLinkElement LoginButton { get; set; }
        public GeckoInputElement UserName { get; set; }
        public GeckoInputElement Password { get; set; }
        public GeckoInputElement SignIn { get; set; }

        public DiceNinja(GeckoWebBrowser browser) : base(browser)
        {
            Url = "https://dice.ninja";
        }

        public override void SetElements()
        {
            try
            {
                GeckoDocument document = _browser.Document;
                Chance = document.GetElementById("win_chance") as GeckoInputElement;
                BetValue = document.GetElementById("bet") as GeckoInputElement;
                Payout = document.GetElementById("payout") as GeckoHtmlElement;
                BetHigh = document.GetElementsByName(">")[0] as GeckoDivElement;
                BetLow = document.GetElementsByName("<")[0] as GeckoDivElement;
                BalanceNode = document.GetElementsByClassName("mybalance")[0] as GeckoDivElement;
            }
            catch (Exception)
            {
                Console.WriteLine("Not able to set all Elements");
            }
        }

        // Uses local buttons to check values... might change this behaviour
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

        // Uses local buttons to set the value...
        public override void SetBet(decimal bet)
        {
            //var settings = WebBot.Properties.Settings.Default;
            base.SetBet(bet);
            if (BetValue != null) { BetValue.Value = Settings.CurrentBetAmount.ToString(); }
        }

        public override void ClickHigh()
        {
            BetHigh.Click();
        }

        public override void ClickLow()
        {
            BetLow.Click();
        }

        public override void SetPreviousBalance()
        {
            _previousBalanceValue = BalanceNode.InnerHtml;
        }

        public override void SetChance()
        {
            // Add checks for trying to set over max/min amounts
            Chance.Value = CurrentChance.ToString();
        }
    }
}
