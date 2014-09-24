using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gecko;
using WebBot.Data;
using System.Threading;
using WebBot.BetFunctions.Sites;
using WebBot.BetFunctions.Data;
using WebBot.BetFunctions;

namespace WebBot.Controls
{
    public partial class MainTabControl : UserControl
    {
        private GeckoWebBrowser _browser;
        //private BaseSite _site;

        //public BaseSite BetSite { get { return _site; } }

        public GeckoWebBrowser Browser
        {
            get { return _browser;  }
        }

        public MainTabControl()
        {
            InitializeComponent();

            _browser = new GeckoWebBrowser()
            {
                Dock = DockStyle.Fill,
            };
            
            //_site = new RPCDice(_browser);
            //_site = new NullSite();
            //BetTasks tasks = new BetTasks((Main)this.Parent, _browser);
            //Main.OnInitializeSite(new RPCDice(_browser));
        }

        protected override void OnLoad(EventArgs e)
        {
            //_browser.DocumentCompleted += _browser_DocumentCompleted;

            //_browser.Navigate(_site.Url);
            //BetSite.Connect();

            tabControl1.TabPages["tabPage1"].Controls.Add(_browser);

            tabControl1.DrawItem += tabControl1_DrawItem;

            BindBustCheckGrid();
            dataGridView2.CellFormatting += dataGridView2_CellFormatting;
        }

        void dataGridView2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Change the color of the cells (and formatting of negative numbers)
            e.CellStyle.BackColor = Color.LightPink;
        }

        void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            //g.DrawString("x", e.Font, Brushes.Black, e.Bounds.Right - 15, e.Bounds.Top + 4);
            g.DrawString(this.tabControl1.TabPages[e.Index].Text, e.Font, Brushes.Black, e.Bounds.Left + 12, e.Bounds.Top + 4);
            e.DrawFocusRectangle();
        }

        public void BindStatisticsGrid(BetTasks betTask)
        {
            dataGridView2.AutoGenerateColumns = false;

            // create the columns programatically
            DataGridViewCell cell = new DataGridViewTextBoxCell();
            DataGridViewTextBoxColumn colBetNum = new DataGridViewTextBoxColumn()
            {
                CellTemplate = cell,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                Name = "Bet",
                HeaderText = "#",
                DataPropertyName = "CurrentBetNum"
            };

            DataGridViewTextBoxColumn colBet = new DataGridViewTextBoxColumn()
            {
                CellTemplate = cell,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                Name = "Wagered",
                HeaderText = "Wagered",
                DataPropertyName = "BetAmount"
            };

            DataGridViewTextBoxColumn colResult = new DataGridViewTextBoxColumn()
            {
                CellTemplate = cell,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                Name = "Result",
                HeaderText = "Result",
                DataPropertyName = "Result"
            };

            DataGridViewTextBoxColumn colProfit = new DataGridViewTextBoxColumn()
            {
                CellTemplate = cell,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                Name = "Reward",
                HeaderText = "Reward",
                DataPropertyName = "Profit"
            };

            DataGridViewTextBoxColumn colTotalResult = new DataGridViewTextBoxColumn()
            {
                CellTemplate = cell,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                Name = "TotalProfit",
                HeaderText = "Total Profit",
                DataPropertyName = "TotalProfit"
            };

            DataGridViewTextBoxColumn colReason = new DataGridViewTextBoxColumn()
            {
                CellTemplate = cell,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                Name = "Reason",
                HeaderText = "Message",
                DataPropertyName = "Reason"
            };

            dataGridView2.Columns.Add(colBetNum);
            dataGridView2.Columns.Add(colBet);
            dataGridView2.Columns.Add(colResult);
            dataGridView2.Columns.Add(colProfit);
            dataGridView2.Columns.Add(colTotalResult);
            dataGridView2.Columns.Add(colReason);

            betTask.QueueChanged += betTask_QueueChanged;

            List<BetData> tasks = betTask.BetData.ToList();
            tasks.Reverse();
            dataGridView2.DataSource = tasks;
        }

        void betTask_QueueChanged(object sender, EventArgs e)
        {
            BetTasks betTask = sender as BetTasks;

            if (betTask != null)
            {
                int index = dataGridView2.FirstDisplayedScrollingRowIndex;
                List<BetData> tasks = betTask.BetData.ToList();
                tasks.Reverse();
                dataGridView2.DataSource = tasks;
                if (index >= 0)
                {
                    dataGridView2.FirstDisplayedScrollingRowIndex = index;
                }
            }
        }

        void BindBustCheckGrid()
        {
            dataGridView1.AutoGenerateColumns = false;

            // create the columns programatically
            DataGridViewCell cell = new DataGridViewTextBoxCell();
            DataGridViewTextBoxColumn colBet = new DataGridViewTextBoxColumn()
                {
                    CellTemplate = cell,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                    Name = "Bet",
                    HeaderText = "Bet Value",
                    DataPropertyName = "Bet"
                };

            dataGridView1.Columns.Add(colBet);

            DataGridViewTextBoxColumn colProfit = new DataGridViewTextBoxColumn()
            {
                CellTemplate = cell,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                Name = "Profit",
                HeaderText = "Profit",
                DataPropertyName = "Profit"
            };

            dataGridView1.Columns.Add(colProfit);

            DataGridViewTextBoxColumn colBalance = new DataGridViewTextBoxColumn()
            {
                CellTemplate = cell,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                Name = "Balance",
                HeaderText = "Balance",
                DataPropertyName = "Balance"
            };

            dataGridView1.Columns.Add(colBalance);

            DataGridViewTextBoxColumn colReason = new DataGridViewTextBoxColumn()
            {
                CellTemplate = cell,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                Name = "Reason",
                HeaderText = "Reason",
                DataPropertyName = "Reason"
            };

            dataGridView1.Columns.Add(colReason);

            var betList = new List<BustCheck>();

            betList.Add(new BustCheck(0.123f, "first", 0.10000001f, 0.01f));
            betList.Add(new BustCheck(0.5323f, "second", 0.10000001f, 0.01f));

            var betsList = new BindingList<BustCheck>(betList);
            dataGridView1.DataSource = betsList;
        }
    }
}
