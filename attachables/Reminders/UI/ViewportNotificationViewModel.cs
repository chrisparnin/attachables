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
        static ViewportNotificationViewModel()
        {
            colors = new System.Windows.Media.SolidColorBrush[]
            {
                new SolidColorBrush(Color.FromArgb(128,222,78,113)),
                new SolidColorBrush(Color.FromArgb(128,0,124,146)),
                new SolidColorBrush(Color.FromArgb(128,255,210,54))
            };
            random = new Random();
        }

        ViewportNote Note;
        public ViewportNotificationViewModel(ViewportNote note)
        {
            this.Note = note;
            this.DoneCommand = new RelayCommand(
                () =>
                {
                    this.Reminder.IsCompleted = true;
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

            this.ColorBrush = colors[(int)Math.Floor(random.NextDouble() * colors.Length)];
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
