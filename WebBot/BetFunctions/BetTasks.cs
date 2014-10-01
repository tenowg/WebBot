using Gecko;
using Gecko.Events;
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
        private bool _paused;

        public BetTasks(FlowLayoutPanel taskPanel)
        {
            BetData = new ConcurrentQueue<BetData>();
            _taskPanel = taskPanel;
            settings = WebBot.Properties.Settings.Default;
        }

        public BetTasks(Main main, FlowLayoutPanel taskPanel)
            : this(taskPanel)
        {
            _main = main;
            _browser = null;
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

            Site.Connect();
        }

        public void ProcessBet()
        {
            if (Site == null || _paused)
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
                else
                {
                    // Do Page Reload
                    // Pause the BetTasks
                    _paused = true;
                    _browser.DocumentCompleted += _browser_DocumentCompleted;
                    // Reload the page
                    Site.Connect();
                    // Wait for reload (Done)
                    // Stop looking for reload (Done)
                    // Unpause the bettings (Done)
                    _retries = 0;
                    return;
                }    
            }
            _retries = 0;
            
            WinType type = Site.IsWin();

            switch (type)
            {
                case WinType.Win:
                case WinType.Lose:
                    break;
                case WinType.Initial:
                    Site.SetBet(Site.CurrentBet);
                    break;
                case WinType.Unknown:
                    // Show an error of sometype (this shouldnever happen)
                    return;
                default:
                    break;
            }

            if (type != WinType.Initial && Site.CurrentBets > 0)
            {
                Site.LastResult = type;

                var wagered = Site.CurrentWagered + Site.CurrentBet;
                Site.CurrentWagered = wagered;

                var profit = Site.Balance - Site.PreviousBalance;
                Site.CurrentProfit += profit;

                BetData data = new BetData()
                    {
                        Balance = Site.Balance,
                        CurrentBetNum = Site.CurrentBets,
                        BetAmount = Site.CurrentBet,
                        Result = type,
                        Profit = profit, // TODO make this the currrent bets profit...
                        TotalProfit = Site.CurrentProfit,
                        Chance = Site.CurrentChance,
                        PossiblePayout = decimal.Round((99 / Site.CurrentChance) * Site.CurrentBet, 8),
                        TotalWagered = Site.CurrentWagered,
                        PossibleProfit = (decimal.Round((99 / Site.CurrentChance) * Site.CurrentBet, 8)) - Site.CurrentWagered 
                    };

                Enqueue(data);
                
                ProcessBetActions();
            }
            
            Site.IncrementStats(type);
            // TODO fix this to site
            Site.Roll(WebBot.Properties.Settings.Default.RollHigh);
        }

        void _browser_DocumentCompleted(object sender, GeckoDocumentCompletedEventArgs e)
        {
            _browser.DocumentCompleted -= _browser_DocumentCompleted;
            ResetPause();
        }

        async void ResetPause()
        {
            await Task.Delay(10000);
            _paused = false;
        }

        private void ProcessBetActions()
        {
            IEnumerable<BetAction> actions = _taskPanel.Controls.Cast<BetAction>()
                .Where(x => x.BetActionProperties.Action != null)
                .Where(x => x.BetActionProperties.Action.CanFire((BaseSite)Site) == true)
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

        public void BetsReset()
        {
            BetData = new ConcurrentQueue<BetData>();
        }
    }
}
