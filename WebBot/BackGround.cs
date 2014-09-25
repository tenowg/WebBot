using Gecko.DOM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebBot.BetActions.Enums;
using WebBot.BetFunctions;
using WebBot.BetFunctions.Sites;
using WebBot.Data;

namespace WebBot
{
    partial class Main
    {
        BackgroundWorker worker = new BackgroundWorker();
        //private BaseSite _site;
        BetTasks tasks;

        public delegate WinType isWin();

        public void initBackground()
        {
            worker.WorkerSupportsCancellation = true;

            worker.RunWorkerCompleted += WorkerCompleted;
            worker.ProgressChanged += ProgressChanged;
            worker.DoWork += DoWork;
            //worker.Disposed += Disposed;

            buttonStartHigh.Click += StartHigh_Click;
            buttonStartLow.Click += StartLow_Click;
            buttonStop.Click += Stop_Click;

            //_site = mainTabControl1.BetSite;

            //tasks = new BetTasks(_site, flowLayoutPanel1);
            tasks = new BetTasks(this, mainTabControl1.Browser);
            mainTabControl1.BindStatisticsGrid(tasks);

            //OnInitializeSite(new RPCDice(mainTabControl1.Browser));
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

        void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //this.tbProgress.Text = (e.ProgressPercentage.ToString() + "%");
        }

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
            if (!worker.IsBusy)
            {
                worker.RunWorkerAsync();
            }
        }

        void StartLow_Click(object sender, EventArgs e)
        {
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
    }
}
