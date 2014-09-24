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
                    site.SetBet(_settings.CurrentBetAmount + amount);
                    break;
                case PercentOrFixed.Percent:
                    decimal percent = amount / 100;
                    site.SetBet(_settings.CurrentBetAmount * percent);
                    break;
                case PercentOrFixed.Reset:
                    site.SetBet(_settings.MinimumBetAmount);
                    break;
                case PercentOrFixed.Multiply:
                    site.SetBet(_settings.CurrentBetAmount * amount);
                    break;
            }
            
        }

        public override bool CanFire()
        {
            return base.CanFire();
        }
    }
}
