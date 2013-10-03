using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ninlabs.attachables.Models.Conditions
{
    [DataContract]
    public enum ConditionType : int
    {
        [EnumMember]
        Session = 0x0001,
        [EnumMember]
        Tasking = 0x0010,
        [EnumMember]
        Proximity = 0x0100
    }
}
