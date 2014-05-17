using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ninlabs.attachables.Models;
using ninlabs.attachables.Util;
using System.Windows.Media;
//using System.Drawing;

namespace ninlabs.attachables.UI
{
    class ViewportNotificationViewModel
    {
        static System.Windows.Media.SolidColorBrush[] colors;
        static Random random;

        // Because Viewports are created so often, 
        // a random assignment each time becomes too distracting        
        static Dictionary<string, SolidColorBrush> TodoColorMapping;

        // Track exposures to reminder, modify opacity accordingly.
        static Dictionary<string, double> ExposureMapping;

        static long Ticks;

        static ViewportNotificationViewModel()
        {
            colors = new System.Windows.Media.SolidColorBrush[]
            {
                new SolidColorBrush(Color.FromArgb(128,222,78,113)),
                new SolidColorBrush(Color.FromArgb(128,0,124,146)),
                new SolidColorBrush(Color.FromArgb(128,255,210,54))
            };
            random = new Random();

            TodoColorMapping = new Dictionary<string, SolidColorBrush>();
            ExposureMapping = new Dictionary<string, double>();

            Ticks = DateTime.Now.Ticks;
        }

        ViewportNote Note;
        public ViewportNotificationViewModel(ViewportNote note, string reminderMessage)
        {
            this.Note = note;
            this.DoneCommand = new RelayCommand(
                () =>
                {
                    this.Reminder.IsCompleted = true;
                    this.Reminder.CompletedOn = DateTime.Now;
                    AttachablesPackage.Manager.SaveReminder(this.Reminder);
                    //note.RemoveAdornment(this.Reminder.Id);
                },
                () => { return true; }
                );

            this.SnoozeCommand = new RelayCommand(() =>
                {
                    AttachablesPackage.Manager.SnoozeReminder(this.Reminder);
                },
                () => { return true; });

            this.GotoCommand = new RelayCommand(() =>
                {
                    AttachablesPackage.Manager.GotoReminder(this.Reminder);
                },
                () => { return true; });


            if (!TodoColorMapping.ContainsKey(reminderMessage))
            {
                TodoColorMapping[reminderMessage] = colors[(int)Math.Floor(random.NextDouble() * colors.Length)];
            }
            this.ColorBrush = TodoColorMapping[reminderMessage];

            if (!ExposureMapping.ContainsKey(reminderMessage))
            {
                ExposureMapping[reminderMessage] = 1.0;
            }

            ExposureMapping[reminderMessage] = Math.Max(.3, ExposureMapping[reminderMessage] * .95);
            Opacity = ExposureMapping[reminderMessage];
        }

        public void ResetExposure()
        {
            ExposureMapping[ReminderMessage] = 1.0;
        }

        public double Opacity
        {
            get;
            set;
        }

        public SolidColorBrush ColorBrush
        {
            get;
            set;
        }

        public string ReminderMessage
        {
            get;
            set;
        }

        public ICommand DoneCommand
        {
            get;
            set;
        }

        public ICommand SnoozeCommand
        {
            get;
            set;
        }

        public ICommand GotoCommand
        {
            get;
            set;
        }


        public Reminder Reminder { get; set; }
    }
}
