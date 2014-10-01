using Gecko;
using Gecko.DOM;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebBot.BetActions.Enums;
using WebBot.BetFunctions;
using WebBot.BetFunctions.Sites;
using WebBot.Data;

namespace WebBot
{
    partial class Main
    {
        BackgroundWorker worker = new BackgroundWorker();
        BackgroundWorker watchWorker = new BackgroundWorker();
        BackgroundWorker bustWorker = new BackgroundWorker();

        BetTasks tasks;

        public delegate WinType isWin();

        public void initBackground()
        {
            worker.WorkerSupportsCancellation = true;
            watchWorker.WorkerSupportsCancellation = true;
            bustWorker.WorkerSupportsCancellation = true;

            worker.RunWorkerCompleted += WorkerCompleted;
            worker.DoWork += DoWork;

            watchWorker.RunWorkerCompleted += watchWorker_RunWorkerCompleted;
            watchWorker.DoWork += watchWorker_DoWork;

            bustTasks = new BetTasks(this, flowLayoutPanel1);
            bustWorker.DoWork += Bust_DoWork;

            buttonStartHigh.Click += StartHigh_Click;
            buttonStartLow.Click += StartLow_Click;
            buttonStop.Click += Stop_Click;

            mainTabControl1.buttonBust.Click += StartBust_Click;
            mainTabControl1.buttonBustStop.Click += StopBust_Click;

            tasks = new BetTasks(this, mainTabControl1.Browser);
            mainTabControl1.BindStatisticsGrid(tasks);
            mainTabControl1.BindBustCheckGrid(bustTasks);
        }

        #region Create Site Events

        public static event EventHandler<SiteChangedEventData> InitializeSite;

        public static void OnInitializeSite(BaseSite site)
        {
            SiteChangedEventData data = new SiteChangedEventData();
            data.site = site;
            if (InitializeSite != null) InitializeSite(null, data);
        }

        #endregion

        #region Main Bet Loop

        void DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker sendWorker = sender as BackgroundWorker;
            Stopwatch timer = new Stopwatch();
            timer.Start();

            this.Invoke(new Action(() =>
                {
                    tasks.Site.RequestStopped += Stop_Click;
                    tasks.Site.OnBetStarting();
                    buttonStop.Enabled = true;
                    buttonStartHigh.Enabled = false;
                    buttonStartLow.Enabled = false;
                    tasks.IsRunning = true;
                }));

            // Going to be a while loop for infinite runs...
            while (!sendWorker.CancellationPending) {
                try
                {
                    // Set the number of Milliseconds to wait
                    var betsPerSecond = WebBot.Properties.Settings.Default.BetsPerSecond;
                    int milliseconds = (int)(1000 / betsPerSecond);

                    if (timer.ElapsedMilliseconds >= milliseconds)
                    {
                        // Do the work that is required
                        this.BeginInvoke(new Action(() =>
                        {
                            tasks.ProcessBet();
                        }));

                        timer.Restart();
                    }
                    else
                    {
                        int sleeptime = milliseconds - (int)timer.ElapsedMilliseconds;
                        if (sleeptime > 0)
                        {
                            Thread.Sleep(sleeptime);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }

            this.Invoke(new Action(() => {
                tasks.Site.RequestStopped -= Stop_Click;
                buttonStop.Enabled = false;
                buttonStartHigh.Enabled = true;
                buttonStartLow.Enabled = true;
                tasks.IsRunning = false;
            }));
        }

        //void ProgressChanged(object sender, ProgressChangedEventArgs e)
        //{
        //    //this.tbProgress.Text = (e.ProgressPercentage.ToString() + "%");
        //}

        void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                //this.tbProgress.Text = "Canceled!";
                Console.WriteLine("Stopping");
            }
            else if (e.Error != null)
            {
                //this.tbProgress.Text = ("Error: " + e.Error.Message);
                Console.WriteLine(e.Error.Message);
            }
            else
            {
                //this.tbProgress.Text = "Done!";
                Console.WriteLine("Stopping this thread...");
            }
        }

        void StartHigh_Click(object sender, EventArgs e)
        {
            WebBot.Properties.Settings.Default.RollHigh = true;
            if (!worker.IsBusy)
            {
                worker.RunWorkerAsync();
            }
        }

        void StartLow_Click(object sender, EventArgs e)
        {
            WebBot.Properties.Settings.Default.RollHigh = false;
            if (!worker.IsBusy)
            {
                worker.RunWorkerAsync();
            }
        }

        void Stop_Click(object sender, EventArgs e)
        {
            if (worker.WorkerSupportsCancellation)
            {
                worker.CancelAsync();
            }
        }
        #endregion

        #region Watch HTML Item loop
        // events to throw when items change
        public static event EventHandler ValueChanged;

        private void OnValueChanged(GeckoHtmlElement element)
        {
            if (ValueChanged != null) ValueChanged(element, EventArgs.Empty);
        }

        public static void AddWatchElement(string name, GeckoHtmlElement element)
        {
            elements.TryAdd(name, element);
        }

        private static ConcurrentDictionary<string, GeckoHtmlElement> elements = new ConcurrentDictionary<string, GeckoHtmlElement>();
        private static ConcurrentDictionary<string, string> elementValues = new ConcurrentDictionary<string, string>();
        
        void watchWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker sendWorker = sender as BackgroundWorker;

            while(!sendWorker.CancellationPending)
            {
                foreach (var element in elements)
                {
                    this.Invoke(new Action(() =>
                            {
                                string el;
                                if (elementValues.TryGetValue(element.Key, out el))
                                {
                                    if (elements[element.Key].InnerHtml != el)
                                    {
                                        elementValues.TryUpdate(element.Key, el, elementValues[element.Key]);

                                        OnValueChanged(element.Value);
                                    }
                                }
                                else
                                {
                                    elementValues.TryAdd(element.Key, element.Value.InnerHtml);
                                }
                            }));
                }

                Thread.Sleep(50);
            }
        }

        void watchWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }

        #endregion

        #region BustCheckWorker Loop
        private NullSite nullSite = new NullSite();
        private BetTasks bustTasks;
        
        void Bust_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker sendWorker = sender as BackgroundWorker;
            this.Invoke(new Action(() =>
                {
                    if (mainTabControl1.radioButton2.Checked)
                    {
                        nullSite.SetBalance(mainTabControl1.numericUpDown2.Value);
                    }
                    else
                    {
                        MessageBox.Show("This option is not Implemented Yet");
                        sendWorker.CancelAsync();
                    }
                }));
            int overCount = 0;
            // While loop, till balance is -(+count roll)
            while ((nullSite.Balance > 0 || overCount < 10) && !sendWorker.CancellationPending)
            {
                this.BeginInvoke(new Action(() =>
                {
                    if (nullSite.Balance <= 0)
                    {
                        overCount++;
                    }

                
                    bustTasks.ProcessBet();
                    //nullSite.SetBalance(nullSite.Balance - .2m);
                }));

                Thread.Sleep(50);
            }
        }

        void StartBust_Click(object sender, EventArgs e)
        {
            
            if (!bustWorker.IsBusy)
            {
                bustTasks.BetsReset();
                bustTasks.Site = nullSite;
                nullSite.Initialize();
                bustWorker.RunWorkerAsync();
            }
        }
        
        void StopBust_Click(object sender, EventArgs e)
        {
            if (bustWorker.WorkerSupportsCancellation)
            {
                bustWorker.CancelAsync();
            }
        }

        #endregion
    }
}
