using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ninlabs.attachables.Models;
using ninlabs.attachables.Util;

namespace ninlabs.attachables.UI
{
    class ViewportNotificationViewModel
    {
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
