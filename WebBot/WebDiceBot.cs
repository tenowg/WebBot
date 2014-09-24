using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using WebBot.BetActions;
using WebBot.BetActions.Actions;
using WebBot.BetActions.Enums;
using WebBot.BetFunctions.Sites;
using WebBot.Controls;

namespace WebBot
{
    public partial class Main : Form
    {
        public IList<BetActionProperties> ActionList 
        { 
            get 
            {
                IEnumerable<BetAction> actions = flowLayoutPanel1.Controls.Cast<BetAction>();

                IList<BetActionProperties> properties = new List<BetActionProperties>();

                foreach (BetAction action in actions)
                {
                    properties.Add(action.BetActionProperties);
                    Console.WriteLine("Adding " + action.BetActionProperties.ActionType);
                }

                return properties;
            }
        }

        public Main()
        {
            InitializeComponent();

            initBackground();

            propertyGrid1.PropertyValueChanged += propertyGrid1_PropertyValueChanged;
            saveSettingsToolStripMenuItem.Click += button5_Click;
            loadSettingsToolStripMenuItem.Click += load_Click;

            this.pRCDicehttpprcdiceeuToolStripMenuItem.Click += SiteMenu_Click;
        }


        void AddBetAction()
        {
            BetAction action = new BetAction();
            flowLayoutPanel1.Controls.Add(action);
            action.Click += action_DoubleClick;
        }

        void action_DoubleClick(object sender, EventArgs e)
        {
            BetAction action = sender as BetAction;
            propertyGrid1.SelectedObject = action.BetActionProperties;
        }

        void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            BetAction betAction = ((BetActionProperties)propertyGrid1.SelectedObject).BetAction as BetAction;
            switch (e.ChangedItem.Label)
            {
                case "ActionType":
                    if (MessageBox.Show(String.Format("Confirm change of Action to \"{0}\"?", (ActionType)e.ChangedItem.Value), "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        betAction.SetActionType((ActionType)e.ChangedItem.Value);
                    }
                    else
                    {
                        betAction.BetActionProperties.ActionType = (ActionType)e.OldValue;
                    }
                    propertyGrid1.Refresh();
                    break;
                default:
                    break;
            }
            
            betAction.FormatActionMessage();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            AddBetAction();
        }

        private Type[] KnownTypes()
        {
            return new Type[] 
                { 
                    typeof(TestAction),
                    typeof(StopAction),
                    typeof(ChangeBetAction),
                    typeof(DictionaryPropertyObject),
                    typeof(ConditionalType),
                    typeof(ActionValue),
                    typeof(PercentOrFixed),
                    typeof(ProfitType)
                };
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Create an instance of the open file dialog box.
            SaveFileDialog openFileDialog1 = new SaveFileDialog();

            // Set filter options and filter index.
            openFileDialog1.Filter = "Xml Files (.xml)|*.xml";
            openFileDialog1.InitialDirectory = Directory.GetCurrentDirectory() + "\\bets";
            Console.WriteLine(openFileDialog1.InitialDirectory);
            openFileDialog1.FilterIndex = 1;

            // Process input if the user clicked OK.
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Type[] knownTypes = KnownTypes();
                DataContractSerializer dcs = new DataContractSerializer(typeof(IList<BetActionProperties>), knownTypes);
                FileStream file = new FileStream(openFileDialog1.FileName, FileMode.Create);
                dcs.WriteObject(file, ActionList);
                file.Close();
            }
        }

        private void load_Click(object sender, EventArgs e)
        {
            // Create an instance of the open file dialog box.
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // Set filter options and filter index.
            openFileDialog1.Filter = "Xml Files (.xml)|*.xml";
            openFileDialog1.InitialDirectory = Directory.GetCurrentDirectory() + "\\bets";
            Console.WriteLine(openFileDialog1.InitialDirectory);
            openFileDialog1.FilterIndex = 1;

            // Process input if the user clicked OK.
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                IList<BetActionProperties> actionList;
                Type[] knownTypes = KnownTypes();
                DataContractSerializer dcs = new DataContractSerializer(typeof(IList<BetActionProperties>), knownTypes);
                FileStream stream = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read);
                actionList = dcs.ReadObject(stream) as IList<BetActionProperties>;
                stream.Close();

                if (actionList != null)
                {
                    flowLayoutPanel1.Controls.Clear();

                    foreach (var action in actionList)
                    {
                        BetAction savedAction = new BetAction(action);
                        flowLayoutPanel1.Controls.Add(savedAction);
                        savedAction.Click += action_DoubleClick;
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tasks.Site.OnReset();
        }

        private void SiteMenu_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            Console.WriteLine(item.Tag);

            switch (item.Tag as string)
            {
                case "prc":
                    Main.OnInitializeSite(new RPCDice(mainTabControl1.Browser));
                    break;
            }
        }
    }
}
