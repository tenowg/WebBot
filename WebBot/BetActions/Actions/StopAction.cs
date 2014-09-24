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
    public class StopAction : CActionType
    {
        public StopAction() : base()
        {
        }
        
        public override void Initialize()
        {
            base.Initialize();
            Properties.AddProperty(PROFIT_TYPE, ProfitType.Always, "This action looks at Profit/Lose, this conditional will determine if this action is needed.");
            Properties.AddProperty(AMOUNT, 0m, "The amount of profit (or lose) this action will fire on, base on the Conditional statement. (this action can get confusing, will work on fixing)");
        }

        public override string GetName()
        {
            return "Open to Edit (StopAction)";
        }

        public override void Execute(BaseSite site)
        {
            var _settings = WebBot.Properties.Settings.Default;

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
                    if (_settings.CurrentProfit != amount) {
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

            site.OnRequestStopped();
        }

        public override bool CanFire()
        {
            return base.CanFire();
        }
    }
}
