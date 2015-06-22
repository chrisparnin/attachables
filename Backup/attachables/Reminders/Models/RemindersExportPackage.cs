using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ninlabs.attachables.Models
{
    [DataContract]
    public class ReminderExportPackage
    {
        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public List<Reminder> Reminders { get; set; }
    }
}
