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
                    note.RemoveAdornment(this.Reminder.Id);
                },
                () => { return true; }
                );
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

        public Reminder Reminder { get; set; }
    }
}
