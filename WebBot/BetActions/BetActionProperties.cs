using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WebBot.BetActions.Enums;
using WebBot.Controls;

namespace WebBot.BetActions
{
    [DataContract]
    public class BetActionProperties
    {
        private string _name = "Unnamed";
        private CActionType _action;
        private ActionType _actionType;

        private BetAction _betAction;
        private bool _disabled = true;
        private int _priority = 0;

        [Browsable(false)]
        public BetAction BetAction { get { return _betAction; } set { _betAction = value; } }

        [DataMember]
        [Category("General")]
        [Description("The lower the Priority the earilier this action will execute.")]
        public int Priority { get { return _priority; } set { _priority = value; } }

        [DataMember]
        [Category("Action Properties")]
        [Description("The Meat of the Action, open to edit the settings.")]
        public CActionType Action { get { return _action; } set { _action = value; } }

        [DataMember]
        [Category("General")]
        [DisplayName("Bet Action Name")]
        [Description("A human readable name for this action.")]
        public string Name {
            get { return _name; }
            set
            {
                _name = value;
                //_betAction.Label = value;
            }
        }

        [DataMember]
        [Category("General")]
        [Description("If this action is disabled it will not be considered.")]
        public bool Disabled
        {
            get { return _disabled; }
            set { _disabled = value; }
        }

        [DataMember]
        [Category("Action Properties")]
        [DefaultValue("SelectActionType")]
        public ActionType ActionType
        {
            get { return _actionType; }
            set { _actionType = value; }
        }


        public BetActionProperties(BetAction betAction)
        {
            _betAction = betAction;
        }
    }
}
