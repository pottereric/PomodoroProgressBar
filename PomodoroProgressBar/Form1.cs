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
        private const int TIMER_INTERVAL = 10_000;
        private const int TIMER_INTERVAL_IN_TICKS = 10_000 * (int)TimeSpan.TicksPerMillisecond;
        private const long WORK_INTERVAL = 25 * TimeSpan.TicksPerMinute;
        private const long NONWARNING_WORK_INTERVAL = 20 * TimeSpan.TicksPerMinute;

        private long _startTime;
        private Timer _pomodoroTimer = new Timer();
        private long _progress = 0;
        private TaskbarProgressBarState _state = TaskbarProgressBarState.NoProgress;
        private int _completed = 0;

        public Form1()
        {
            InitializeComponent();
            _pomodoroTimer.Interval = TIMER_INTERVAL;
            _pomodoroTimer.Tick += _pomodoroTimer_Tick;
        }

        private void _pomodoroTimer_Tick(object sender, EventArgs e)
        {
            var currentTime = DateTime.Now.Ticks;
            var elapsedTime = currentTime - _startTime;

            if(elapsedTime >= WORK_INTERVAL)
            {
                CompletePomodoro();
            }
            else if (elapsedTime >= NONWARNING_WORK_INTERVAL)
            {
                _state = TaskbarProgressBarState.Paused;
            }
            else
            {
                _state = TaskbarProgressBarState.Normal;
            }

            _progress = elapsedTime;
            UpdateTaskbar();
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            StartPomodoro();
            UpdateTaskbar();
        }
        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetPomodoro();
            UpdateTaskbar();
    }
        private void UpdateTaskbar()
        {
            var taskbar = Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance;
            taskbar.SetProgressState(_state);
            taskbar.SetProgressValue((int)(_progress/ TIMER_INTERVAL_IN_TICKS), (int)(WORK_INTERVAL/ TIMER_INTERVAL_IN_TICKS));
        }

        private void StartPomodoro()
        {
            _pomodoroTimer.Start();

            _state = TaskbarProgressBarState.Normal;
            _progress = 0;
            _startTime = DateTime.Now.Ticks;

            lblStatusText.Text = "Running";
        }

        private void CompletePomodoro()
        {
            _pomodoroTimer.Stop();

            _state = TaskbarProgressBarState.Error;
            _completed++;
            
            lblCompletedCount.Text = _completed.ToString();
            lblStatusText.Text = "Finished";
        }

        private void ResetPomodoro()
        {
            _pomodoroTimer.Stop();

            _state = TaskbarProgressBarState.Normal;
            _progress = 0;

            lblStatusText.Text = "Reset";
        }
    }
}
