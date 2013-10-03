using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ninlabs.attachables.Models.Conditions;

namespace ninlabs.attachables.Models
{
    [DataContract]
    [KnownType(typeof(Time))]
    [KnownType(typeof(Session))]
    [KnownType(typeof(Proximity))]
    public class AbstractCondition
    {
        [DataMember]
        public ConditionType Type { get; set; }

        public virtual bool IsApplicable(Reminder reminder)
        {
            return true;
        }
    }
}
