using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebBot.BetActions;
using WebBot.BetActions.Interfaces;
using WebBot.BetActions.Actions;
using System.Runtime.Serialization;
using WebBot.BetActions.Enums;

namespace WebBot.Controls
{
    [DataContract]
    public partial class BetAction : UserControl, IBetAction
    {
        public BetAction()
        {
            InitializeComponent();
            BetActionProperties = new BetActionProperties(this);
            FormatActionMessage();
        }

        public BetAction(BetActionProperties properties)
        {
            BetActionProperties = properties;
            BetActionProperties.BetAction = this;
            InitializeComponent();
            FormatActionMessage();
        }

        public BetActionProperties BetActionProperties { get; set; }

        public string Label { 
            get { return label1.Text; }
            set { label1.Text = value; }
        }

        public void FormatActionMessage()
        {
            if (BetActionProperties.Action != null)
            {
                Dictionary<string, object> props = BetActionProperties.Action.Properties.Dictionary as Dictionary<string, object>;
                Dictionary<string, object> fireProps = BetActionProperties.Action.FiringParameters.Dictionary as Dictionary<string, object>;
                
                object type;
                fireProps.TryGetValue(CActionType.ROLL_RESULT, out type);
                if (props.ContainsKey(CActionType.COUNT))
                {
                    object count;
                    fireProps.TryGetValue(CActionType.COUNT, out count);
                    label2.Text = String.Format("{0} {1}", type.ToString(), count);
                }
                else
                {
                    label2.Text = String.Format("{0}", type.ToString());
                }
            }

            if (BetActionProperties.Disabled)
            {
                this.BackColor = Color.LightGray;
            }
            else
            {
                this.BackColor = Color.LightGreen;
            }
            
            label1.Text = BetActionProperties.Name;
        }

        public void SetActionType(ActionType? action) {
            CActionType actionType = BetActionProperties.Action;
            switch (action)
            {
                case ActionType.TestAction:
                    actionType = new TestAction();
                    break;
                case ActionType.ChangeBet:
                    actionType = new ChangeBetAction();
                    break;
                case ActionType.StopAction:
                    actionType = new StopAction();
                    break;
                case ActionType.ChangeChance:
                    actionType = new ChangeChanceAction();
                    break;
                default:
                    actionType = null;
                    break;
            }
            BetActionProperties.Action = actionType;
        }
    }
}
