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
        public override decimal PreviousBalance
        {
            get
            {
                if (_previousBalanceValue == null) 
                {
                    return 0;
                }
                return decimal.Parse(_previousBalanceValue.Substring(0, _previousBalanceValue.LastIndexOf(" ")));
            }
        }

        public GeckoInputElement Chance { get; set; }
        public GeckoInputElement BetValue { get; set; }
        public GeckoInputElement Payout { get; set; }
        public GeckoAnchorElement BetHigh { get; set; }
        public GeckoAnchorElement BetLow { get; set; }
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
                BetLow = document.GetElementsByClassName("diceLoButton")[0] as GeckoAnchorElement;
                BalanceNode = document.GetElementsByClassName("myBalance")[0] as GeckoHtmlElement;
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
            Chance.Value = Settings.CurrentChance.ToString(); ;
        }
    }
}
