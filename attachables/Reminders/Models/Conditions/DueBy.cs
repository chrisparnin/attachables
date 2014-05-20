using Microsoft.VisualStudio.Text.Editor;
using ninlabs.attachables.Models;
using ninlabs.attachables.Models.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ninlabs.attachables.Reminders.Models.Conditions
{
    [DataContract]
    public class DueBy : AbstractCondition
    {
        public DueBy()
        {
            Type = ConditionType.Time;
        }
        [DataMember]
        public DateTime DueByDate { get; set; }
        [DataMember]
        public string FriendlyDueDate { get; set; }

        public override string ToString()
        {
            return "Due by " + DueByDate.ToShortDateString();
        }

        public override bool IsApplicable(Reminder reminder, IWpfTextView view)
        {
            // If there is a snooze applied and time has past, then applicable.
            if (reminder.SnoozeUntil.HasValue && DateTime.Now >= reminder.SnoozeUntil.Value && DateTime.Today >= DueByDate)
            {
                return true;
            }
            else if (reminder.SnoozeUntil.HasValue)
            {
                return false;
            }
            return DateTime.Today >= DueByDate;
        }
    }
}
