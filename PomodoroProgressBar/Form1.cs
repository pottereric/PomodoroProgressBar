using Microsoft.WindowsAPICodePack.Taskbar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PomodoroProgressBar
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            _pomodoroTimer.Interval = TIMER_INTERVAL;
            _pomodoroTimer.Tick += _pomodoroTimer_Tick;
        }


        private const int TIMER_INTERVAL = 10_000;
        private const int TIMER_INTERVAL_IN_TICKS = 10_000 * (int)TimeSpan.TicksPerMillisecond;
        private const long WORK_INTERVAL = 25 * TimeSpan.TicksPerMinute;
        private const long NONWARNING_WORK_INTERVAL = 20 * TimeSpan.TicksPerMinute;
        private long _startTime;
        private Timer _pomodoroTimer = new Timer();
        public long Progress { get; set; } = 0;
        public TaskbarProgressBarState State { get; set; } = TaskbarProgressBarState.NoProgress;
        private int _completed = 0;

        private void _pomodoroTimer_Tick(object sender, EventArgs e)
        {
            var currentTime = DateTime.Now.Ticks;
            var elapsedTime = currentTime - _startTime;

            if(elapsedTime >= WORK_INTERVAL)
            {
                State = TaskbarProgressBarState.Error;
                _pomodoroTimer.Stop();
                _completed++;
                lblCompletedCount.Text = _completed.ToString();
                lblStatusText.Text = "Finished";
            }
            else if (elapsedTime >= NONWARNING_WORK_INTERVAL)
            {
                State = TaskbarProgressBarState.Paused;
            }
            else
            {
                State = TaskbarProgressBarState.Normal;
            }

            Progress = elapsedTime;
            UpdateTaskbar();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            State = TaskbarProgressBarState.Normal;
            Progress = 0;

            _startTime = DateTime.Now.Ticks;
            _pomodoroTimer.Start();

            UpdateTaskbar();

            lblStatusText.Text = "Running";
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            State = TaskbarProgressBarState.Normal;
            UpdateTaskbar();
        }

        private void UpdateTaskbar()
        {
            var taskbar = Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance;
            taskbar.SetProgressState(State);
            taskbar.SetProgressValue((int)(Progress/ TIMER_INTERVAL_IN_TICKS), (int)(WORK_INTERVAL/ TIMER_INTERVAL_IN_TICKS));
        }
    }
}
