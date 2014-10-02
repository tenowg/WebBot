using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBot.BetActions.Enums;
using WebBot.BetFunctions.Sites;
using WebBot.Converters;

namespace WebBot.BetActions.Actions
{
    public class ChangeBetAction : CActionType
    {
        private WebBot.Properties.Settings _settings;
        public ChangeBetAction() : base()
        {
            _settings = WebBot.Properties.Settings.Default;
            Properties.AddProperty(PERCENT_OR_FIXED, PercentOrFixed.Fixed);
            Properties.AddProperty(AMOUNT, 0.0m);
        }

        public override string GetName()
        {
            return "Open to Edit (ChangeBetAction)";
        }

        public override void Execute(BaseSite site)
        {
            decimal amount;
            Properties.GetProperty(AMOUNT, out amount);

            PercentOrFixed type;
            Properties.GetProperty(PERCENT_OR_FIXED, out type);

            switch(type)
            {
                case PercentOrFixed.Fixed:
                    site.SetBet(site.CurrentBet + amount);
                    break;
                case PercentOrFixed.Percent:
                    decimal percent = amount / 100;
                    site.SetBet(site.CurrentBet * percent);
                    break;
                case PercentOrFixed.Reset:
                    site.SetBet(site.BaseBet);
                    break;
                case PercentOrFixed.Multiply:
                    site.SetBet(site.CurrentBet * amount);
                    break;
            }
            
        }

        public override string GetDescription()
        {
            PercentOrFixed type;
            Properties.GetProperty(PERCENT_OR_FIXED, out type);

            decimal amount;
            Properties.GetProperty(AMOUNT, out amount);

            // Always (Will Always stop betting when executed)
            // EqualTo (Will stop the betting if profit/loss is {0} {1})
            // Others (Will stop the betting if there is a {0} of {1})
            switch (type)
            {
                case PercentOrFixed.Exactly:
                    return string.Format("Chance will change to {0} exactly.", amount);
                case PercentOrFixed.Fixed:
                    return string.Format("Will change bet chance by {0} (addition/substraction)", amount);
                case PercentOrFixed.Multiply:
                    return string.Format("Will multiply bet chance by {0} (not percentage)", amount);
                case PercentOrFixed.Percent:
                    return string.Format("Will multiply bet chance by {0}% (as a percentage)", amount);
                case PercentOrFixed.Reset:
                    return "Reset the Bet back to Base bet.";
            }

            return base.GetDescription();
        }
    }
}
