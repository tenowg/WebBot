using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WebBot.BetActions.Enums;
using WebBot.BetActions.Interfaces;
using WebBot.BetFunctions.Sites;
using WebBot.Converters;

namespace WebBot.BetActions
{
    [DataContract]
    [TypeConverter(typeof(CActionTypeConverter))]
    public class CActionType : IActionType
    {
        private WebBot.Properties.Settings _settings;

        public const string ROLL_RESULT = "RollResult";
        public const string CONDITIONAL = "Conditional";
        public const string PRIORITY = "Priority";
        public const string COUNT = "Count";
        public const string PERCENT_OR_FIXED = "Percent or Fixed";
        public const string AMOUNT = "Amount";
        public const string PROFIT_TYPE = "Profit Type";
        public const string CHANGE_AMOUNT = "Change Amount";

        [DataMember]
        [Description("Properties decide what happens once a Action is fired, they do not determine if the action is attempted like Firing Parameters do.")]
        public virtual DictionaryPropertyObject Properties { get; set; }

        [DataMember]
        [Description("Firing Parameters are used to decide when an action is going to be attempted, Then once fired properties are used to decide what happens.")]
        public virtual DictionaryPropertyObject FiringParameters { get; set; }

        public CActionType()
        {
            Properties = new DictionaryPropertyObject();
            FiringParameters = new DictionaryPropertyObject();
            Initialize();
        }
        
        [OnDeserialized]
        public void Deserialized(StreamingContext stream)
        {
            Initialize();
        }

        public virtual void Initialize()
        {
            FiringParameters.AddProperty(CONDITIONAL, ConditionalType.EqualTo, "Works with Result type and Count to determine win this action is considered");
            FiringParameters.AddProperty(ROLL_RESULT, ActionValue.Bet, "When this Action is considered based on Wins, Losses, or just Bets");
            FiringParameters.AddProperty(COUNT, 0, "Works with the Conditional Value and Result type to determine how often this action fires.");

            _settings = WebBot.Properties.Settings.Default;
        }

        public virtual string GetName()
        {
            return "Type (Unnamed)";
        }


        public virtual void Execute(BaseSite site)
        {
        }

        public virtual bool CanFire()
        {
            WinType type = (WinType)Enum.Parse(typeof(WinType), _settings.LastResult);

            ActionValue actionType;
            FiringParameters.GetProperty(ROLL_RESULT, out actionType);

            int checkCount;

            if (actionType == ActionValue.Bet)
            {
                checkCount = Math.Abs(_settings.CurrentBets);
            }
            else
            {
                switch (type)
                {
                    case WinType.Win:
                        // It was a Win...
                        if (actionType != ActionValue.Win)
                        {
                            return false;
                        }
                        checkCount = Math.Abs(_settings.CurrentStreak);
                        break;
                    case WinType.Lose:
                        // It was a Loss...
                        if (actionType != ActionValue.Lose)
                        {
                            return false;
                        }
                        checkCount = Math.Abs(_settings.CurrentStreak);
                        break;
                    default:
                        return false;
                }
            }

            int count;
            FiringParameters.GetProperty(COUNT, out count);

            ConditionalType conditionalType;
            FiringParameters.GetProperty(CONDITIONAL, out conditionalType);

            switch(conditionalType)
            {
                case ConditionalType.Always:
                    break;
                case ConditionalType.EqualTo:
                    if (count != checkCount)
                    {
                        return false;
                    }
                    break;
                case ConditionalType.GreaterThan:
                    if (checkCount <= count)
                    {
                        return false;
                    }
                    break;
                case ConditionalType.LessThan:
                    if (checkCount >= count)
                    {
                        return false;
                    }
                    break;
                case ConditionalType.GreaterThanOrEqualTo:
                    if (checkCount < count)
                    {
                        return false;
                    }
                    break;
                case ConditionalType.LessThanOrEqualTo:
                    if (checkCount > count)
                    {
                        return false;
                    }
                    break;
                case ConditionalType.MultipleOf:
                    if (checkCount % count != 0)
                    {
                        return false;
                    }
                    break;
            }
            return true;
        }

        public virtual string GetDescription()
        {
            return "No Description Set";
        }
    }

    internal class CActionTypeConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context,
                             System.Globalization.CultureInfo culture,
                             object value, Type destType)
        {
            if (destType == typeof(string) && value is CActionType)
            {
                CActionType type = (CActionType)value;
                return type.GetName();
            }
            else
            {
                return "Select ActionType Below";
            }
            //return base.ConvertTo(context, culture, value, destType);
        }
    }
}
