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
        private const long BREAK_INTERVAL = 5 * TimeSpan.TicksPerMinute;

        private long _startTime;
        private Timer _pomodoroTimer = new Timer();
        private long _progress = 0;
        private TaskbarProgressBarState _state = TaskbarProgressBarState.NoProgress;
        private int _completed = 0;
        private bool _isWorkSession = true;
        private long _currentInterval;

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

            if(elapsedTime >= _currentInterval)
            {
                if (_isWorkSession)
                    CompletePomodoro();
                else
                    CompleteBreak();
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
            if (_isWorkSession)
                StartPomodoro();
            else
                StartBreak();

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
            taskbar.SetProgressValue((int)(_progress/ TIMER_INTERVAL_IN_TICKS), (int)(_currentInterval/ TIMER_INTERVAL_IN_TICKS));
        }

        private void StartPomodoro()
        {
            _currentInterval = WORK_INTERVAL;
            _pomodoroTimer.Start();

            _state = TaskbarProgressBarState.Normal;
            _progress = 0;
            _startTime = DateTime.Now.Ticks;

            lblStatusText.Text = "Running Work";
        }

        private void StartBreak()
        {
            _currentInterval = BREAK_INTERVAL;
            _pomodoroTimer.Start();

            _state = TaskbarProgressBarState.Normal;
            _progress = 0;
            _startTime = DateTime.Now.Ticks;

            lblStatusText.Text = "Taking a break";
        }
        private void CompletePomodoro()
        {
            _pomodoroTimer.Stop();

            _state = TaskbarProgressBarState.Error;
            _completed++;
            
            lblCompletedCount.Text = _completed.ToString();
            lblStatusText.Text = "Finished";

            _isWorkSession = false;
        }

        private void CompleteBreak()
        {
            _pomodoroTimer.Stop();

            _state = TaskbarProgressBarState.Error;

            lblStatusText.Text = "Finished Break";

            _isWorkSession = true;
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
