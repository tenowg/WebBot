using Gecko;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebBot.BetActions;
using WebBot.BetActions.Enums;
using WebBot.BetFunctions.Data;
using WebBot.BetFunctions.Sites;
using WebBot.Controls;
using WebBot.Data;

namespace WebBot.BetFunctions
{
    public class BetTasks
    {
        public ConcurrentQueue<BetData> BetData;
        public BaseSite Site { get; set; }

        private bool _running;

        public bool IsRunning { get { return _running; } set { _running = value; } }

        private int _retries;
        private FlowLayoutPanel _taskPanel;
        private WebBot.Properties.Settings settings;
        private Main _main;
        private GeckoWebBrowser _browser;

        public BetTasks(FlowLayoutPanel taskPanel)
        {
            BetData = new ConcurrentQueue<BetData>();
            _taskPanel = taskPanel;
            settings = WebBot.Properties.Settings.Default;
        }

        public BetTasks(Main main, GeckoWebBrowser browser) : this(main.flowLayoutPanel1)
        {
            _main = main;
            _browser = browser;

            Main.InitializeSite += mainTabControl1_Load;
        }

        void mainTabControl1_Load(object sender, SiteChangedEventData e)
        {
            if (Site != null)
            {
                if (_running)
                {
                    MessageBox.Show("Please stop running the Bot first.");
                    return;
                }
                Site = null;
            }

            Site = e.site;
            Site.OnReset();
            //Site.BetStarting += delegate { _running = true; };
            //Site.RequestStopped += delegate { _running = false; };

            Site.Connect();
        }

        public void ProcessBet()
        {
            if (Site == null)
            {
                return;
            }

            if (!Site.HasBalanceChanged())
            {
                // Completely fail if retries is below threshold.
                if (_retries < WebBot.Properties.Settings.Default.MaxRetry)
                {
                    _retries++;
                    return;
                }
            }

            WinType type = Site.IsWin();

            switch (type)
            {
                case WinType.Win:
                case WinType.Lose:
                    break;
                case WinType.Initial:
                    Site.SetBet(settings.CurrentBetAmount);
                    break;
                case WinType.Unknown:
                    // Show an error of sometype (this shouldnever happen)
                    return;
                default:
                    break;
            }

            if (type != WinType.Initial)
            {
                settings.LastResult = type.ToString();
                var profit = Site.Balance - Site.PreviousBalance;
                settings.CurrentProfit += profit;
                // At this point we should be ready to record data...
                BetData data = new BetData()
                    {
                        CurrentBetNum = settings.CurrentBets,
                        BetAmount = settings.CurrentBetAmount,
                        Result = type,
                        Profit = profit, // TODO make this the currrent bets profit...
                        TotalProfit = settings.CurrentProfit
                    };

                Enqueue(data);
            }

            Site.IncrementStats(type);
            ProcessBetActions();

            Site.Roll(WebBot.Properties.Settings.Default.RollHigh);
        }

        private void ProcessBetActions()
        {
            IEnumerable<BetAction> actions = _taskPanel.Controls.Cast<BetAction>()
                .Where(x => x.BetActionProperties.Action != null)
                .Where(x => x.BetActionProperties.Action.CanFire() == true)
                .Where(x => x.BetActionProperties.Disabled == false)
                .OrderBy(x => x.BetActionProperties.Priority);

            foreach (BetAction action in actions)
            {
                action.BetActionProperties.Action.Execute((BaseSite)Site);
            }
        }

        public event EventHandler QueueChanged;

        protected virtual void OnQueueChanged()
        {
            EventArgs args = new EventArgs();
            if (QueueChanged != null) QueueChanged(this, EventArgs.Empty);
        }

        private void Enqueue(BetData betdata)
        {
            while (BetData.Count > WebBot.Properties.Settings.Default.BetQueueSize)
            {
                BetData result;
                BetData.TryDequeue(out result);
            }
            BetData.Enqueue(betdata);

            // Fire Event
            OnQueueChanged();
        }

        private void BetsReset(object sender, EventArgs args)
        {
            BetData = new ConcurrentQueue<BetData>();
        }
    }
}
