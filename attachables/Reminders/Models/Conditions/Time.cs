using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ninlabs.attachables.Models.Conditions
{
    [DataContract]
    public class Time : AbstractCondition
    {
        public Time()
        {
            Type = ConditionType.Session;
        }
        [DataMember]
        public DateTime TriggerBy { get; set; }

        public override string ToString()
        {
            return "1 hour before " + TriggerBy;
        }

        public override bool IsApplicable(Reminder reminder, IWpfTextView view)
        {
            return DateTime.Now.AddHours(-1) >= TriggerBy;
        }
    }

}
