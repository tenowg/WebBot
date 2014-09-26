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
    public class ChangeChanceAction : CActionType
    {
        private Properties.Settings _settings;
        public ChangeChanceAction() : base()
        {
            _settings = WebBot.Properties.Settings.Default;
            Properties.AddProperty(PROFIT_TYPE, ProfitType.Always, "Determines what type of profit this action is waiting for (If just checking for Bet/Win/Lose set to always)");
            Properties.AddProperty(AMOUNT, 0m, "The profit this action is looking for (abs values, based on ProfitType)");
            Properties.AddProperty(PERCENT_OR_FIXED, PercentOrFixed.Fixed, "The type of change to apply");
            Properties.AddProperty(CHANGE_AMOUNT, 0.0m, "The amount the Chance is going to be changed by");
        }

        public override string GetName()
        {
            return "Open to Edit (ChangeChanceAction)";
        }

        public override void Execute(BaseSite site)
        {
            ProfitType type;
            Properties.GetProperty(PROFIT_TYPE, out type);

            decimal amount;
            Properties.GetProperty(AMOUNT, out amount);

            switch (type)
            {
                case ProfitType.Always:
                    break;
                case ProfitType.EqualTo:
                    // Probably not ever going to fire do to exact numbers will likely never happen here.
                    if (_settings.CurrentProfit != amount)
                    {
                        return;
                    }
                    break;
                case ProfitType.Loss:
                    if (Math.Abs(_settings.CurrentProfit) < amount)
                    {
                        return;
                    }
                    break;
                case ProfitType.Profit:
                    if (Math.Abs(_settings.CurrentProfit) < amount)
                    {
                        return;
                    }
                    break;
            }

            decimal changeAmount;
            Properties.GetProperty(CHANGE_AMOUNT, out changeAmount);

            PercentOrFixed percentType;
            Properties.GetProperty(PERCENT_OR_FIXED, out percentType);

            switch (percentType)
            {
                case PercentOrFixed.Fixed:
                    _settings.CurrentChance += changeAmount;
                    break;
                case PercentOrFixed.Percent:
                    decimal percent = amount / 100;
                    _settings.CurrentChance *= percent;
                    break;
                case PercentOrFixed.Reset:
                    _settings.CurrentChance = _settings.BaseChance;
                    break;
                case PercentOrFixed.Multiply:
                    //site.SetBet(_settings.CurrentBetAmount * amount);
                    _settings.CurrentChance *= amount;
                    break;
            }

            site.SetChance();
        }
    }
}
