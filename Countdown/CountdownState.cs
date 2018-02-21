using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Countdown.Annotations;
using Countdown.Properties;

namespace Countdown
{
    public class CountdownState : INotifyPropertyChanged
    {
        private DateTime dateFrom;
        private DateTime dateTo;
        private double passedTimeAngle;

        public event PropertyChangedEventHandler PropertyChanged;
        public DateTime DateFrom
        {
            get => dateFrom;
            set
            {
                if (value.Equals(dateFrom))
                    return;
                dateFrom = value;
                Settings.Default.DateFrom = DateFrom;
                Settings.Default.Save();
                OnPropertyChanged();
            }
        }
        public DateTime DateTo
        {
            get => dateTo;
            set
            {
                if (value.Equals(dateTo))
                    return;
                dateTo = value;
                OnPropertyChanged();
                Settings.Default.DateTo = DateTo;
                Settings.Default.Save();
            }
        }

        public string Ellapsed
        {
            get
            {
                var nl = Environment.NewLine;
                var ellapsed = DateTime.Now - DateFrom;
                if (ellapsed.TotalMilliseconds < 0)
                    ellapsed = TimeSpan.Zero;

                return $"{ellapsed.Days:000} Tage{nl}{ellapsed.Hours:00} Stunden{nl}{ellapsed.Minutes:00} Minuten{nl}{ellapsed.Seconds:00} Sekunden{nl}{ellapsed.Milliseconds:0000} Millisekunden";
            }
        }
        public string Remaining
        {
            get
            {
                var nl = Environment.NewLine;
                var remaining = DateTo - DateTime.Now;
                if (remaining.TotalMilliseconds < 0)
                    remaining = TimeSpan.Zero;

                return $"{remaining.Days:000} Tage{nl}{remaining.Hours:00} Stunden{nl}{remaining.Minutes:00} Minuten{nl}{remaining.Seconds:00} Sekunden{nl}{remaining.Milliseconds:0000} Millisekunden";
            }
        }

        public double Progress
        {
            get
            {
                var totalMilliseconds = TotalTimeSpan.TotalMilliseconds;
                var passedMilliseconds = (DateTime.Now - DateFrom).TotalMilliseconds;
                if (passedMilliseconds < 0)
                    return 0;
                if (passedMilliseconds > totalMilliseconds)
                    return 1;
                return passedMilliseconds * 1.0 / totalMilliseconds;
            }
        }

        public double PassedTimeAngle
        {
            get => passedTimeAngle;
            private set
            {
                if (value.Equals(passedTimeAngle))
                    return;
                passedTimeAngle = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan TotalTimeSpan => DateTo - DateFrom;

        public CountdownState()
        {
            DateFrom = Settings.Default.DateFrom;
            DateTo = Settings.Default.DateTo;
            Task.Factory.StartNew(Update, TaskCreationOptions.LongRunning);
        }

        private void Update()
        {
            while (true)
            {
                OnPropertyChanged(nameof(Progress));
                OnPropertyChanged(nameof(Remaining));
                OnPropertyChanged(nameof(Ellapsed));
                PassedTimeAngle = -90 + Progress * 360;
                if (Progress >= 1)
                    break;
            }
        }

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}