using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ninlabs.attachables.Models;

namespace ninlabs.attachables.Storage
{
    class ReminderContract
    {
        public long Id { get; set; }

        public String ConditionAsString { get; set; }

        public NotificationType NotificationType { get; set; }

        public String ReminderMessage { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? SnoozeUntil { get; set; }
    }}
