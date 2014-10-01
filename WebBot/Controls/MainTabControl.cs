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
using System.Threading;
using WebBot.BetFunctions.Sites;
using WebBot.BetFunctions.Data;
using WebBot.BetFunctions;
using WebBot.BetActions.Enums;

namespace WebBot.Controls
{
    public partial class MainTabControl : UserControl
    {
        private GeckoWebBrowser _browser;
        public GeckoWebBrowser Browser { get { return _browser; } }

        public MainTabControl()
        {
            InitializeComponent();

            _browser = new GeckoWebBrowser()
            {
                Dock = DockStyle.Fill,
            };
        }

        protected override void OnLoad(EventArgs e)
        {
            tabControl1.TabPages["tabPage1"].Controls.Add(_browser);

            tabControl1.DrawItem += tabControl1_DrawItem;

            //BindBustCheckGrid();
            dataGridView2.CellFormatting += dataGridView2_CellFormatting;
            dataGridView1.CellFormatting += dataGridView1_CellFormatting;
        }

        void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            g.DrawString(this.tabControl1.TabPages[e.Index].Text, e.Font, Brushes.Black, e.Bounds.Left + 12, e.Bounds.Top + 4);
            e.DrawFocusRectangle();
        }

        private const string BET_COLUMN = "Bet";
        private const string BALANCE = "Balance";
        private const string WAGERED_COLUMN = "Wagered";
        private const string RESULT_COLUMN = "Result";
        private const string REWARD_COLUMN = "Reward";
        private const string TOTAL_PROFIT_COLUMN = "TotalProfit";
        private const string REASON_COLUMN = "Reason";
        private const string POSSIBLE_PROFIT = "PossibleProfit";
        private const string TOTAL_POSSIBLE_PROFIT = "TotalPossibleProfit";
        private const string TOTAL_WAGERED = "TotalWagered";

        #region Statistics grid view
        private void dataGridView2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView send = sender as DataGridView;

            var Column = send.Columns[e.ColumnIndex];

            //if (Column.Name == REWARD_COLUMN)
            switch(Column.Name)
            {
                case TOTAL_PROFIT_COLUMN:
                case REWARD_COLUMN:
                    decimal test = (decimal)e.Value;
                    if (test < 0)
                    {
                        e.CellStyle.ForeColor = Color.Red;
                    }
                    else if (test > 0)
                    {
                        e.CellStyle.ForeColor = Color.Green;
                    }
                    break;
                case RESULT_COLUMN:
                    WinType type = (WinType)e.Value;
                    if (type == WinType.Win)
                    {
                        e.CellStyle.ForeColor = Color.Green;
                    }
                    else
                    {
                        e.CellStyle.ForeColor = Color.Red;
                    }
                    break;
            }
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
                Name = BET_COLUMN,
                HeaderText = "#",
                DataPropertyName = "CurrentBetNum"
            };

            DataGridViewTextBoxColumn colBet = new DataGridViewTextBoxColumn()
            {
                CellTemplate = cell,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                Name = WAGERED_COLUMN,
                HeaderText = "Wagered",
                DataPropertyName = "BetAmount"
            };

            DataGridViewTextBoxColumn colResult = new DataGridViewTextBoxColumn()
            {
                CellTemplate = cell,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                Name = RESULT_COLUMN,
                HeaderText = "Result",
                DataPropertyName = "Result"
            };

            DataGridViewTextBoxColumn colProfit = new DataGridViewTextBoxColumn()
            {
                CellTemplate = cell,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                Name = REWARD_COLUMN,
                HeaderText = "Reward",
                DataPropertyName = "Profit"
            };

            DataGridViewTextBoxColumn colTotalResult = new DataGridViewTextBoxColumn()
            {
                CellTemplate = cell,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                Name = TOTAL_PROFIT_COLUMN,
                HeaderText = "Total Profit",
                DataPropertyName = "TotalProfit"
            };

            DataGridViewTextBoxColumn colReason = new DataGridViewTextBoxColumn()
            {
                CellTemplate = cell,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                Name = REASON_COLUMN,
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
        #endregion

        #region Bust Check Code
        public void BindBustCheckGrid(BetTasks betTask)
        {
            dataGridView1.AutoGenerateColumns = false;

            // create the columns programatically
            DataGridViewCell cell = new DataGridViewTextBoxCell();
            DataGridViewTextBoxColumn colBet = new DataGridViewTextBoxColumn()
            {
                CellTemplate = cell,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                Name = BET_COLUMN,
                HeaderText = "#",
                DataPropertyName = "CurrentBetNum"
            };

            dataGridView1.Columns.Add(colBet);

            DataGridViewTextBoxColumn colWagered = new DataGridViewTextBoxColumn()
            {
                CellTemplate = cell,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                Name = WAGERED_COLUMN,
                HeaderText = "Wagered",
                DataPropertyName = "BetAmount"
            };

            dataGridView1.Columns.Add(colWagered);

            DataGridViewTextBoxColumn colBalance = new DataGridViewTextBoxColumn()
            {
                CellTemplate = cell,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                Name = BALANCE,
                HeaderText = "Balance",
                DataPropertyName = "Balance"
            };

            dataGridView1.Columns.Add(colBalance);

            DataGridViewTextBoxColumn colPossibleProfit = new DataGridViewTextBoxColumn()
            {
                CellTemplate = cell,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                Name = POSSIBLE_PROFIT,
                HeaderText = "Payout",
                DataPropertyName = "PossiblePayout"
            };

            dataGridView1.Columns.Add(colPossibleProfit);

            DataGridViewTextBoxColumn colTotalWagered = new DataGridViewTextBoxColumn()
            {
                CellTemplate = cell,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                Name = TOTAL_WAGERED,
                HeaderText = "TotalWagered",
                DataPropertyName = "TotalWagered"
            };

            dataGridView1.Columns.Add(colTotalWagered);

            DataGridViewTextBoxColumn colProfit = new DataGridViewTextBoxColumn()
            {
                CellTemplate = cell,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                Name = TOTAL_POSSIBLE_PROFIT,
                HeaderText = "Profitable",
                DataPropertyName = "PossibleProfit"
            };

            dataGridView1.Columns.Add(colProfit);

            betTask.QueueChanged += bustTask_QueueChanged;

            List<BetData> tasks = betTask.BetData.ToList();
            tasks.Reverse();
            dataGridView1.DataSource = tasks;
        }

        void bustTask_QueueChanged(object sender, EventArgs e)
        {
            BetTasks betTask = sender as BetTasks;

            if (betTask != null)
            {
                int index = dataGridView1.FirstDisplayedScrollingRowIndex;
                List<BetData> tasks = betTask.BetData.ToList();
                tasks.Reverse();
                dataGridView1.DataSource = tasks;
                if (index >= 0)
                {
                    try
                    {
                        dataGridView1.FirstDisplayedScrollingRowIndex = index;
                    }
                    catch (Exception) { }
                }
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView send = sender as DataGridView;

            var Column = send.Columns[e.ColumnIndex];

            switch (Column.Name)
            {
                case TOTAL_POSSIBLE_PROFIT:
                    decimal test = (decimal)e.Value;
                    if (test < 0)
                    {
                        e.CellStyle.ForeColor = Color.Red;
                    }
                    else if (test > 0)
                    {
                        e.CellStyle.ForeColor = Color.Green;
                    }
                    break;
                case BALANCE:
                    //var betCol = send.Columns[BET_COLUMN];
                    decimal wagered = (decimal)send.Rows[e.RowIndex].Cells[WAGERED_COLUMN].Value;
                    decimal balance = (decimal)e.Value;

                    if (wagered > balance)
                    {
                        e.CellStyle.ForeColor = Color.Red;
                    }
                    break;
            }
        }
        #endregion

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown2.Enabled = radioButton2.Checked;
        }

    }
}
